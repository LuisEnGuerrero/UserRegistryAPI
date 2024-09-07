using System.Collections.Generic;

namespace UserRegistryAPI.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        // Propiedad de navegación para la relación uno a muchos con Department
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
