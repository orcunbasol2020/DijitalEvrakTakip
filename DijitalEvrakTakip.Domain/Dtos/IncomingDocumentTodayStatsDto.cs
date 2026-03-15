namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class IncomingDocumentTodayStatsDto
{
    public int TodayCount { get; set; }
    public double ChangePercent { get; set; }
}