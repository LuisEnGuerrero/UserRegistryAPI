using Microsoft.EntityFrameworkCore;
using UserRegistryAPI.Models;

namespace UserRegistryAPI.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Country
            modelBuilder.Entity<Country>()
                .ToTable("pais") // Mapea el nombre de la tabla en la base de datos
                .HasKey(c => c.Id); // Define la clave primaria

            modelBuilder.Entity<Country>()
                .Property(c => c.Id)
                .HasColumnName("id"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<Country>()
                .Property(c => c.Name)
                .HasColumnName("nombre"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<Country>()
                .HasMany(c => c.Departments)
                .WithOne(d => d.Country)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la entidad Department
            modelBuilder.Entity<Department>()
                .ToTable("departamento") // Mapea el nombre de la tabla en la base de datos
                .HasKey(d => d.Id); // Define la clave primaria

            modelBuilder.Entity<Department>()
                .Property(d => d.Id)
                .HasColumnName("id");

            modelBuilder.Entity<Department>()
                .Property(d => d.Name)
                .HasColumnName("nombre"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Municipalities)
                .WithOne(m => m.Department)
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la entidad Municipality
            modelBuilder.Entity<Municipality>()
                .ToTable("municipio") // Mapea el nombre de la tabla en la base de datos
                .HasKey(m => m.Id); // Define la clave primaria

            modelBuilder.Entity<Municipality>()
                .Property(m => m.Id)
                .HasColumnName("id");

            modelBuilder.Entity<Municipality>()
                .Property(m => m.Name)
                .HasColumnName("nombre"); // Especifica el nombre de la columna en la base de datos

            // Configuración de la entidad User
            modelBuilder.Entity<User>()
                .ToTable("usuario") // Mapea el nombre de la tabla en la base de datos
                .HasKey(u => u.Id); // Define la clave primaria

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnName("id");

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasColumnName("nombre"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<User>()
                .Property(u => u.Phone)
                .HasColumnName("telefono"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<User>()
                .Property(u => u.Address)
                .HasColumnName("direccion"); // Especifica el nombre de la columna en la base de datos

            modelBuilder.Entity<User>()
                .HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Municipality)
                .WithMany()
                .HasForeignKey(u => u.MunicipalityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
