namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class IncomingDocumentPendingScanStatsDto
{
    public int PendingScanCount { get; set; }
    public double ChangePercent { get; set; }
}