using System.ComponentModel.DataAnnotations.Schema;

namespace UserRegistryAPI.Models
{
    public class User
    {
        /// <summary>
        /// ID del usuario, clave primaria en la base de datos.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del usuario. Es un campo requerido.
        /// </summary>
        [Column("nombre")] // Mapea la propiedad 'Name' a la columna 'nombre' en la base de datos
        public required string Name { get; set; } // El nombre es requerido

        /// <summary>
        /// Teléfono del usuario. Es un campo requerido.
        /// </summary>
        [Column("telefono")] // Mapea la propiedad 'Phone' a la columna 'telefono' en la base de datos
        public required string Phone { get; set; } // El teléfono es requerido

        /// <summary>
        /// Dirección del usuario. Este campo es opcional.
        /// </summary>
        [Column("direccion")] // Mapea la propiedad 'Address' a la columna 'direccion' en la base de datos
        public string? Address { get; set; } // La dirección es opcional

        /// <summary>
        /// ID del país al que pertenece el usuario (clave foránea).
        /// </summary>
        [Column("pais_id")] // Mapea la propiedad 'CountryId' a la columna 'pais_id' en la base de datos
        public int CountryId { get; set; }

        /// <summary>
        /// ID del departamento al que pertenece el usuario (clave foránea).
        /// </summary>
        [Column("departamento_id")] // Mapea la propiedad 'DepartmentId' a la columna 'departamento_id' en la base de datos
        public int DepartmentId { get; set; }

        /// <summary>
        /// ID del municipio al que pertenece el usuario (clave foránea).
        /// </summary>
        [Column("municipio_id")] // Mapea la propiedad 'MunicipalityId' a la columna 'municipio_id' en la base de datos
        public int MunicipalityId { get; set; }

        /// <summary>
        /// Propiedad de navegación para la relación con el país.
        /// </summary>
        public Country? Country { get; set; } // Relación opcional con Country

        /// <summary>
        /// Propiedad de navegación para la relación con el departamento.
        /// </summary>
        public Department? Department { get; set; } // Relación opcional con Department

        /// <summary>
        /// Propiedad de navegación para la relación con el municipio.
        /// </summary>
        public Municipality? Municipality { get; set; } // Relación opcional con Municipality
    }
}
