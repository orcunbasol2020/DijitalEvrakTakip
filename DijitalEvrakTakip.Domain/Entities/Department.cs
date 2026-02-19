using DijitalEvrakTakip.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.Domain.Entities
{
    public sealed class Department : Entity
    {
        public int? ParentId { get; set; }
        public int? DisnetId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? Type { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
