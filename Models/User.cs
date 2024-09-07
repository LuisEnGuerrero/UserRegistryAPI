namespace UserRegistryAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Asegurando que no sea nulo
        public string Phone { get; set; } = string.Empty; // Asegurando que no sea nulo
        public string? Address { get; set; } // Puede ser nulo

        // Claves foráneas
        public int CountryId { get; set; }
        public int DepartmentId { get; set; }
        public int MunicipalityId { get; set; }

        // Propiedades de navegación
        public Country? Country { get; set; } // Puede ser nulo
        public Department? Department { get; set; } // Puede ser nulo
        public Municipality? Municipality { get; set; } // Puede ser nulo
    }
}
