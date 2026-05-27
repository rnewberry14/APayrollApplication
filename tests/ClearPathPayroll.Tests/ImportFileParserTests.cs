using ClearPathPayroll.Services;
using System.Text;
using Xunit;

namespace ClearPathPayroll.Tests;

public class ImportFileParserTests
{
    [Fact]
    public async Task CsvParser_ParsesQuotedValuesAndDetectedColumns()
    {
        var parser = new CsvImportFileParser();
        await using var stream = ToStream("FirstName,LastName,GrossPay\r\n\"Ada, A.\",Lovelace,1200.50\r\nGrace,Hopper,1300");

        var result = await parser.ParseAsync(stream, "employees.csv");

        Assert.Equal(new[] { "FirstName", "LastName", "GrossPay" }, result.Columns);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal("Ada, A.", result.Rows[0]["FirstName"]);
        Assert.Equal("1200.50", result.Rows[0]["GrossPay"]);
    }

    [Fact]
    public async Task CsvParser_DeduplicatesBlankAndRepeatedColumns()
    {
        var parser = new CsvImportFileParser();
        await using var stream = ToStream("Name,Name,\nOne,Two,Three");

        var result = await parser.ParseAsync(stream, "employees.csv");

        Assert.Equal(new[] { "Name", "Name_2", "Column3" }, result.Columns);
        Assert.Equal("Two", result.Rows[0]["Name_2"]);
        Assert.Equal("Three", result.Rows[0]["Column3"]);
    }

    [Fact]
    public async Task TabDelimitedParser_ParsesRows()
    {
        var parser = new TabDelimitedImportFileParser();
        await using var stream = ToStream("EmployeeNumber\tRegularHours\tGrossPay\nE001\t40\t1000");

        var result = await parser.ParseAsync(stream, "checks.tsv");

        Assert.Equal(new[] { "EmployeeNumber", "RegularHours", "GrossPay" }, result.Columns);
        Assert.Single(result.Rows);
        Assert.Equal("40", result.Rows[0]["RegularHours"]);
    }

    private static MemoryStream ToStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}
