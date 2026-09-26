using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public sealed class Department : Entity
    {
        public Guid? ParentId { get; set; }
        public Department? Parent { get; set; }
        public ICollection<Department> Children { get; set; } = new List<Department>();

        public int? DisnetId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? Type { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
