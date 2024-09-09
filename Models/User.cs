using System.ComponentModel.DataAnnotations.Schema;

namespace UserRegistryAPI.Models
{
    public class User
{
    public int Id { get; set; }
    
    [Column("nombre")]  // Asegura de que el nombre de la columna coincide con el de la base de datos
    public required string Name { get; set; }
    
    [Column("telefono")]
    public required string Phone { get; set; }
    
    [Column("direccion")]
    public string? Address { get; set; }
    
    [Column("pais_id")]
    public int CountryId { get; set; }
    
    [Column("departamento_id")]
    public int DepartmentId { get; set; }
    
    [Column("municipio_id")]
    public int MunicipalityId { get; set; }
    
    // Relaciones con otras tablas, como País, Departamento o Municipio
    public Country? Country { get; set; }
    public Department? Department { get; set; }
    public Municipality? Municipality { get; set; }
}

}
