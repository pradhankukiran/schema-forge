using System.Text;
using SchemaForge.Core.Models;

namespace SchemaForge.Core.Services;

public static class CsvExporter
{
    public static string ToCsv(QueryResult result)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", result.Columns.Select(EscapeCsv)));
        foreach (var row in result.Rows)
        {
            sb.AppendLine(string.Join(",", row.Select(cell => EscapeCsv(cell?.ToString() ?? ""))));
        }
        return sb.ToString();
    }

    public static string ToInsertStatements(QueryResult result, string tableName)
    {
        var sb = new StringBuilder();
        var columns = string.Join(", ", result.Columns.Select(c => $"\"{c}\""));
        foreach (var row in result.Rows)
        {
            var values = string.Join(", ", row.Select(FormatSqlValue));
            sb.AppendLine($"INSERT INTO \"{tableName}\" ({columns}) VALUES ({values});");
        }
        return sb.ToString();
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static string FormatSqlValue(object? value) => value switch
    {
        null => "NULL",
        bool b => b ? "1" : "0",
        double or float or int or long or decimal or short or byte => ((IFormattable)value).ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => $"'{value.ToString()!.Replace("'", "''")}'",
    };
}
