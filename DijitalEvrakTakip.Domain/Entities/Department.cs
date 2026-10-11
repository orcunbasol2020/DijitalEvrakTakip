namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Birim; yalnızca okunur. vw_Departments view'ından gelir (ComDepartment). Merkez birimlerin yanında
    /// yurt dışı temsilciliklerimizi de içerir; ayrım Type (ComDepartment.DepartmentTypeId) ile yapılır,
    /// bkz. DepartmentTypes.
    /// </summary>
    public sealed class Department
    {
        /// <summary>ComDepartment.DepartmentId</summary>
        public int Id { get; set; }

        public int? ParentId { get; set; }
        public Department? Parent { get; set; }
        public ICollection<Department> Children { get; set; } = new List<Department>();

        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? Type { get; set; }

        /// <summary>
        /// Makam seviyesi (ComDepartment.DepartmentLevel): 1 Bakan, 2 Bakan Yardımcılığı / makam birimleri,
        /// 4 Genel Müdürlük, 5 Genel Müdür Yardımcılığı, 6 Daire Başkanlığı, 7+ alt birimler. Bazı birimlerde boş.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>Aynı seviyedeki birimlerin sırası (ComDepartment.DomisticOrganizationSortOrder); çoğunda boş.</summary>
        public int? SortOrder { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
