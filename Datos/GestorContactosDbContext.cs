using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Datos
{
    public class GestorContactosDbContext : DbContext
    {
        public GestorContactosDbContext(DbContextOptions<GestorContactosDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contacto>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Direccion>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Contacto>()
                .HasOne(c => c.Direccion)
                .WithOne()
                .HasForeignKey<Direccion>(d => d.ContactoId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

