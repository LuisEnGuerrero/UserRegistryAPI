using System.Collections.Generic;

namespace UserRegistryAPI.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CountryId { get; set; }
        
        // Propiedad de navegación para la relación con Country
        public Country? Country { get; set; }
        
        // Propiedad de navegación para la relación con Municipality
        public ICollection<Municipality>? Municipalities { get; set; }
    }
}
