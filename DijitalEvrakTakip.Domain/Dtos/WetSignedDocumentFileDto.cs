namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class WetSignedDocumentFileDto
{
    public byte[] Content { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
}
