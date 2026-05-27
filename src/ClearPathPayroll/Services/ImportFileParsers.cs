using ClearPathPayroll.Domain;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace ClearPathPayroll.Services;

public interface IImportFileParser
{
    ImportFileFormat FileFormat { get; }

    bool CanParse(string fileName);

    Task<ImportFileParseResult> ParseAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
}

public class ImportFileParseResult
{
    public ImportFileFormat FileFormat { get; set; }

    public List<string> Columns { get; set; } = new();

    public List<Dictionary<string, string>> Rows { get; set; } = new();

    public List<string> Warnings { get; set; } = new();
}

public class CsvImportFileParser : DelimitedImportFileParser
{
    public override ImportFileFormat FileFormat => ImportFileFormat.Csv;

    public override bool CanParse(string fileName)
    {
        return string.Equals(Path.GetExtension(fileName), ".csv", StringComparison.OrdinalIgnoreCase);
    }

    protected override char Delimiter => ',';
}

public class TabDelimitedImportFileParser : DelimitedImportFileParser
{
    public override ImportFileFormat FileFormat => ImportFileFormat.TabDelimited;

    public override bool CanParse(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".tsv", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".tab", StringComparison.OrdinalIgnoreCase);
    }

    protected override char Delimiter => '\t';
}

public abstract class DelimitedImportFileParser : IImportFileParser
{
    public abstract ImportFileFormat FileFormat { get; }

    protected abstract char Delimiter { get; }

    public abstract bool CanParse(string fileName);

    public async Task<ImportFileParseResult> ParseAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var content = await reader.ReadToEndAsync(cancellationToken);
        var records = ParseRecords(content);

        if (records.Count == 0)
        {
            return new ImportFileParseResult
            {
                FileFormat = FileFormat,
                Warnings = { "The selected file did not contain any rows." }
            };
        }

        var columns = DeduplicateColumns(records[0]);
        var result = new ImportFileParseResult
        {
            FileFormat = FileFormat,
            Columns = columns
        };

        for (var index = 1; index < records.Count; index++)
        {
            var values = records[index];
            if (values.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                row[columns[columnIndex]] = columnIndex < values.Count ? values[columnIndex].Trim() : string.Empty;
            }

            result.Rows.Add(row);
        }

        return result;
    }

    private List<List<string>> ParseRecords(string content)
    {
        var records = new List<List<string>>();
        var currentRecord = new List<string>();
        var currentField = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < content.Length; index++)
        {
            var character = content[index];

            if (character == '"')
            {
                if (inQuotes && index + 1 < content.Length && content[index + 1] == '"')
                {
                    currentField.Append('"');
                    index++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }

                continue;
            }

            if (!inQuotes && character == Delimiter)
            {
                currentRecord.Add(currentField.ToString());
                currentField.Clear();
                continue;
            }

            if (!inQuotes && (character == '\r' || character == '\n'))
            {
                if (character == '\r' && index + 1 < content.Length && content[index + 1] == '\n')
                {
                    index++;
                }

                currentRecord.Add(currentField.ToString());
                currentField.Clear();
                records.Add(currentRecord);
                currentRecord = new List<string>();
                continue;
            }

            currentField.Append(character);
        }

        if (currentField.Length > 0 || currentRecord.Count > 0)
        {
            currentRecord.Add(currentField.ToString());
            records.Add(currentRecord);
        }

        return records;
    }

    private static List<string> DeduplicateColumns(IReadOnlyList<string> rawColumns)
    {
        var names = new List<string>();
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < rawColumns.Count; index++)
        {
            var name = string.IsNullOrWhiteSpace(rawColumns[index])
                ? $"Column{index + 1}"
                : rawColumns[index].Trim();

            if (counts.TryGetValue(name, out var count))
            {
                count++;
                counts[name] = count;
                name = $"{name}_{count}";
            }
            else
            {
                counts[name] = 1;
            }

            names.Add(name);
        }

        return names;
    }
}

public class ExcelImportFileParser : IImportFileParser
{
    public ImportFileFormat FileFormat => ImportFileFormat.Excel;

    public bool CanParse(string fileName)
    {
        return string.Equals(Path.GetExtension(fileName), ".xlsx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Path.GetExtension(fileName), ".xls", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<ImportFileParseResult> ParseAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        if (string.Equals(Path.GetExtension(fileName), ".xls", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Binary .xls import is not supported by the local parser. Save the workbook as .xlsx, CSV, or tab-delimited text.");
        }

        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        var sharedStrings = await ReadSharedStringsAsync(archive, cancellationToken);
        var sheetPath = await GetFirstWorksheetPathAsync(archive, cancellationToken);
        var sheetEntry = archive.GetEntry(sheetPath)
            ?? throw new InvalidOperationException("The workbook does not contain a readable worksheet.");

        await using var sheetStream = sheetEntry.Open();
        var sheet = await XDocument.LoadAsync(sheetStream, LoadOptions.None, cancellationToken);
        XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        var rows = sheet.Descendants(main + "row")
            .Select(row => row.Elements(main + "c").Select(cell => ReadCellValue(cell, sharedStrings, main)).ToList())
            .Where(row => row.Any(value => !string.IsNullOrWhiteSpace(value)))
            .ToList();

        if (rows.Count == 0)
        {
            return new ImportFileParseResult
            {
                FileFormat = FileFormat,
                Warnings = { "The selected workbook did not contain any rows in the first worksheet." }
            };
        }

        var columns = rows[0].Select((column, index) => string.IsNullOrWhiteSpace(column) ? $"Column{index + 1}" : column.Trim()).ToList();
        var result = new ImportFileParseResult
        {
            FileFormat = FileFormat,
            Columns = columns
        };

        foreach (var values in rows.Skip(1))
        {
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                row[columns[columnIndex]] = columnIndex < values.Count ? values[columnIndex].Trim() : string.Empty;
            }

            result.Rows.Add(row);
        }

        result.Warnings.Add("The local .xlsx parser reads the first worksheet only.");
        return result;
    }

    private static async Task<List<string>> ReadSharedStringsAsync(ZipArchive archive, CancellationToken cancellationToken)
    {
        var entry = archive.GetEntry("xl/sharedStrings.xml");
        if (entry == null)
        {
            return new List<string>();
        }

        await using var stream = entry.Open();
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
        XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        return document.Descendants(main + "si")
            .Select(item => string.Concat(item.Descendants(main + "t").Select(text => text.Value)))
            .ToList();
    }

    private static async Task<string> GetFirstWorksheetPathAsync(ZipArchive archive, CancellationToken cancellationToken)
    {
        var workbookEntry = archive.GetEntry("xl/workbook.xml")
            ?? throw new InvalidOperationException("The workbook metadata could not be read.");
        var relationshipsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels")
            ?? throw new InvalidOperationException("The workbook relationships could not be read.");

        await using var workbookStream = workbookEntry.Open();
        await using var relationshipsStream = relationshipsEntry.Open();
        var workbook = await XDocument.LoadAsync(workbookStream, LoadOptions.None, cancellationToken);
        var relationships = await XDocument.LoadAsync(relationshipsStream, LoadOptions.None, cancellationToken);
        XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        XNamespace rel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        XNamespace packageRel = "http://schemas.openxmlformats.org/package/2006/relationships";

        var firstSheetRelationshipId = workbook.Descendants(main + "sheet")
            .FirstOrDefault()
            ?.Attribute(rel + "id")
            ?.Value;

        if (string.IsNullOrWhiteSpace(firstSheetRelationshipId))
        {
            throw new InvalidOperationException("The workbook does not contain a worksheet reference.");
        }

        var target = relationships.Descendants(packageRel + "Relationship")
            .FirstOrDefault(relationship => string.Equals(
                relationship.Attribute("Id")?.Value,
                firstSheetRelationshipId,
                StringComparison.OrdinalIgnoreCase))
            ?.Attribute("Target")
            ?.Value;

        if (string.IsNullOrWhiteSpace(target))
        {
            throw new InvalidOperationException("The first worksheet relationship could not be resolved.");
        }

        return target.StartsWith("xl/", StringComparison.OrdinalIgnoreCase)
            ? target
            : $"xl/{target.TrimStart('/')}";
    }

    private static string ReadCellValue(XElement cell, IReadOnlyList<string> sharedStrings, XNamespace main)
    {
        var value = cell.Element(main + "v")?.Value ?? string.Empty;
        var type = cell.Attribute("t")?.Value;

        if (type == "s"
            && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sharedStringIndex)
            && sharedStringIndex >= 0
            && sharedStringIndex < sharedStrings.Count)
        {
            return sharedStrings[sharedStringIndex];
        }

        if (type == "inlineStr")
        {
            return string.Concat(cell.Descendants(main + "t").Select(text => text.Value));
        }

        return value;
    }
}
