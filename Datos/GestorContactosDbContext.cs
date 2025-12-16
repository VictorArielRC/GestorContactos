using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;    

namespace Datos
{
    public class GestorContactosDbContext : DbContext
    {
        public GestorContactosDbContext(DbContextOptions<GestorContactosDbContext> options)
                : base(options)
        {
        }

        // Tablas que se van a mapear
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración básica de Contacto
            modelBuilder.Entity<Contacto>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Contacto>()
                .HasOne(c => c.Direccion);

            // Configuración básica de Direccion
            modelBuilder.Entity<Direccion>()
                .HasKey(d => d.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}