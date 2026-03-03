using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class ScannedDocument : Entity
    {
        public string? DocumentNumber { get; set; }
        public string? FileName { get; set; }
        public string? OriginalPath { get; set; }
        public string? NewPath { get; set; }

    }
}