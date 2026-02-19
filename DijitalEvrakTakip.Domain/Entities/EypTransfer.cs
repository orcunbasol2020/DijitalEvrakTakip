using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class EypTransfer : Entity
    {
        public Guid? EypPackageId { get; set; }

        public int? Status { get; set; }

        public int? TryCount { get; set; }

        public string? Error { get; set; }

        // Navigation
        public EypPackage? EypPackage { get; set; }
    }
}
