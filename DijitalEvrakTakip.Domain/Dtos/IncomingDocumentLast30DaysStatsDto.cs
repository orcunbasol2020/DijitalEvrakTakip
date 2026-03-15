namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class IncomingDocumentLast30DaysStatsDto
{
    public int Last30DaysCount { get; set; }
    public double ChangePercent { get; set; }
}