namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures;

/// <summary>Nota raporu uç noktalarının ortak dateField değerleri.</summary>
public static class NotaReportPeriodRules
{
    public const string CreatedDateField = "created";
    public const string DocumentDateField = "document";

    public static bool IsValidDateField(string? dateField) =>
        string.IsNullOrWhiteSpace(dateField) ||
        string.Equals(dateField, CreatedDateField, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(dateField, DocumentDateField, StringComparison.OrdinalIgnoreCase);

    public static bool UseDocumentDate(string? dateField) =>
        string.Equals(dateField, DocumentDateField, StringComparison.OrdinalIgnoreCase);
}
