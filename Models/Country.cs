using System.Collections.Generic;

namespace UserRegistryAPI.Models
{
    public class Country
    {
        /// <summary>
        /// ID del país, clave primaria en la base de datos.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del país.
        /// </summary>
        public string Name { get; set; } = string.Empty; // Inicialización para evitar nulos

        /// <summary>
        /// Propiedad de navegación para la relación uno a muchos con Department.
        /// Un país puede tener varios departamentos.
        /// </summary>
        public ICollection<Department> Departments { get; set; } = new List<Department>(); // Inicialización para evitar nulos
    }
}
