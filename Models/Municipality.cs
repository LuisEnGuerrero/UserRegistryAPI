namespace UserRegistryAPI.Models
{
    public class Municipality
    {
        /// <summary>
        /// ID del municipio, clave primaria en la base de datos.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del municipio.
        /// </summary>
        public string Name { get; set; } = string.Empty; // Inicialización segura para evitar nulos

        /// <summary>
        /// ID del departamento al que pertenece el municipio (clave foránea).
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Propiedad de navegación para la relación con Department.
        /// Representa el departamento al que pertenece el municipio.
        /// </summary>
        public Department? Department { get; set; } // Relación opcional con Department
    }
}
