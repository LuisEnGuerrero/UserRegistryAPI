using System.Collections.Generic;

namespace UserRegistryAPI.Models
{
    public class Department
    {
        /// <summary>
        /// ID del departamento, clave primaria en la base de datos.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del departamento.
        /// </summary>
        public string Name { get; set; } = string.Empty; // Inicialización segura para evitar nulos

        /// <summary>
        /// ID del país al que pertenece el departamento (clave foránea).
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Propiedad de navegación para la relación con Country.
        /// Representa el país al que pertenece el departamento.
        /// </summary>
        public Country? Country { get; set; } // Relación opcional

        /// <summary>
        /// Propiedad de navegación para la relación uno a muchos con Municipality.
        /// Un departamento puede tener varios municipios.
        /// </summary>
        public ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>(); // Inicialización para evitar nulos
    }
}
