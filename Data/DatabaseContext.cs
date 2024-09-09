using Microsoft.EntityFrameworkCore;
using UserRegistryAPI.Models;

namespace UserRegistryAPI.Data
{
    public class DatabaseContext : DbContext
    {
        // Constructor que recibe las opciones del contexto
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // Definición de los DbSets para cada entidad (tablas en la base de datos)
        public DbSet<Country> Countries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Configuración de los modelos al crear la base de datos (mapeo entre las clases y las tablas de la BD).
        /// </summary>
        /// <param name="modelBuilder">Instancia del ModelBuilder para configurar las entidades</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Country (mapeo de la tabla 'pais')
            modelBuilder.Entity<Country>()
                .ToTable("pais") // Mapea la clase Country a la tabla 'pais'
                .HasKey(c => c.Id); // Define la clave primaria

            modelBuilder.Entity<Country>()
                .Property(c => c.Id)
                .HasColumnName("id"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<Country>()
                .Property(c => c.Name)
                .HasColumnName("nombre"); // Mapea la propiedad Name a la columna 'nombre'

            // Relación 1 a muchos (un país tiene muchos departamentos)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Departments)
                .WithOne(d => d.Country)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminación en cascada: si se elimina un país, se eliminan sus departamentos

            // Configuración de la entidad Department (mapeo de la tabla 'departamento')
            modelBuilder.Entity<Department>()
                .ToTable("departamento") // Mapea la clase Department a la tabla 'departamento'
                .HasKey(d => d.Id); // Define la clave primaria

            modelBuilder.Entity<Department>()
                .Property(d => d.Id)
                .HasColumnName("id");

            modelBuilder.Entity<Department>()
                .Property(d => d.Name)
                .HasColumnName("nombre"); // Mapea la propiedad Name a la columna 'nombre'

            modelBuilder.Entity<Department>()
                .Property(d => d.CountryId)
                .HasColumnName("pais_id"); // Mapea la relación con Country a la columna 'pais_id'

            // Relación 1 a muchos (un departamento tiene muchos municipios)
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Municipalities)
                .WithOne(m => m.Department)
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminación en cascada: si se elimina un departamento, se eliminan sus municipios

            // Configuración de la entidad Municipality (mapeo de la tabla 'municipio')
            modelBuilder.Entity<Municipality>()
                .ToTable("municipio") // Mapea la clase Municipality a la tabla 'municipio'
                .HasKey(m => m.Id); // Define la clave primaria

            modelBuilder.Entity<Municipality>()
                .Property(m => m.Id)
                .HasColumnName("id");

            modelBuilder.Entity<Municipality>()
                .Property(m => m.Name)
                .HasColumnName("nombre"); // Mapea la propiedad Name a la columna 'nombre'

            modelBuilder.Entity<Municipality>()
                .Property(m => m.DepartmentId)
                .HasColumnName("departamento_id"); // Mapea la relación con Department a la columna 'departamento_id'

            // Configuración de la entidad User (mapeo de la tabla 'usuario')
            modelBuilder.Entity<User>()
                .ToTable("usuario") // Mapea la clase User a la tabla 'usuario'
                .HasKey(u => u.Id); // Define la clave primaria

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnName("id");

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasColumnName("nombre"); // Mapea la propiedad Name a la columna 'nombre'

            modelBuilder.Entity<User>()
                .Property(u => u.Phone)
                .HasColumnName("telefono"); // Mapea la propiedad Phone a la columna 'telefono'

            modelBuilder.Entity<User>()
                .Property(u => u.Address)
                .HasColumnName("direccion"); // Mapea la propiedad Address a la columna 'direccion'
            
            modelBuilder.Entity<User>()
                .Property(u => u.CountryId)
                .HasColumnName("pais_id"); // Mapea la relación con Country a la columna 'pais_id'

            modelBuilder.Entity<User>()
                .Property(u => u.DepartmentId)
                .HasColumnName("departamento_id"); // Mapea la relación con Department a la columna 'departamento_id'

            modelBuilder.Entity<User>()
                .Property(u => u.MunicipalityId)
                .HasColumnName("municipio_id"); // Mapea la relación con Municipality a la columna 'municipio_id'

            // Configuración de las relaciones de la entidad User

            // Un usuario tiene un país asociado (restricción de eliminación)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.CountryId)
                .OnDelete(DeleteBehavior.Restrict); // No se permite eliminar el país si está asignado a un usuario

            // Un usuario tiene un departamento asociado (restricción de eliminación)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // No se permite eliminar el departamento si está asignado a un usuario

            // Un usuario tiene un municipio asociado (restricción de eliminación)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Municipality)
                .WithMany()
                .HasForeignKey(u => u.MunicipalityId)
                .OnDelete(DeleteBehavior.Restrict); // No se permite eliminar el municipio si está asignado a un usuario
        }
    }
}
