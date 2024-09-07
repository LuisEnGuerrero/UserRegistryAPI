using System.Collections.Generic;

namespace UserRegistryAPI.Models
{
    public class Municipality
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int DepartmentId { get; set; }
        
        // Propiedad de navegación para la relación con Department
        public Department? Department { get; set; }
    }
}
