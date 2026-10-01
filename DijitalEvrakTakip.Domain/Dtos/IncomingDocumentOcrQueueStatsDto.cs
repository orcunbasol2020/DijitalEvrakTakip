namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class IncomingDocumentOcrQueueStatsDto
{
    public int OcrQueueCount { get; set; }
    public double ChangePercent { get; set; }
}