namespace ClearPathPayroll.Services;

public enum ImportProgressStage
{
    WaitingForFile,
    ReadingFile,
    ParsingRows,
    DetectingColumns,
    MappingColumns,
    ValidatingRows,
    PreviewReady,
    SavingImport,
    Complete,
    Failed
}

public sealed class ImportProgressModel
{
    public ImportProgressStage Stage { get; set; } = ImportProgressStage.WaitingForFile;

    public int? RowCount { get; set; }

    public int WarningCount { get; set; }

    public int ErrorCount { get; set; }

    public string Message { get; set; } = "Not started.";

    public bool IsWorking => Stage is ImportProgressStage.ReadingFile
        or ImportProgressStage.ParsingRows
        or ImportProgressStage.DetectingColumns
        or ImportProgressStage.ValidatingRows
        or ImportProgressStage.SavingImport;

    public string StatusLabel => Stage switch
    {
        ImportProgressStage.WaitingForFile => "Not started",
        ImportProgressStage.MappingColumns or ImportProgressStage.PreviewReady => "Needs review",
        ImportProgressStage.Complete => "Complete",
        ImportProgressStage.Failed => "Failed",
        _ => "Working"
    };

    public int PercentComplete => Stage switch
    {
        ImportProgressStage.WaitingForFile => 0,
        ImportProgressStage.ReadingFile => 12,
        ImportProgressStage.ParsingRows => 25,
        ImportProgressStage.DetectingColumns => 38,
        ImportProgressStage.MappingColumns => 50,
        ImportProgressStage.ValidatingRows => 65,
        ImportProgressStage.PreviewReady => 75,
        ImportProgressStage.SavingImport => 88,
        ImportProgressStage.Complete => 100,
        ImportProgressStage.Failed => 100,
        _ => 0
    };

    public void MoveTo(ImportProgressStage stage, string message, int? rowCount = null, int? warningCount = null, int? errorCount = null)
    {
        Stage = stage;
        Message = message;
        RowCount = rowCount ?? RowCount;
        WarningCount = warningCount ?? WarningCount;
        ErrorCount = errorCount ?? ErrorCount;
    }
}
