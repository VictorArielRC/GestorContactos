using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Datos
{
    public class GestorContactosDbContextFactory : IDesignTimeDbContextFactory<GestorContactosDbContext>
    {
        public GestorContactosDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GestorContactosDbContext>();

            // Conexión a SQL Server en localhost con autenticación de Windows
            optionsBuilder.UseSqlServer("Server=localhost;Database=GestorContactosDB;Trusted_Connection=True;Encrypt=False;");


            return new GestorContactosDbContext(optionsBuilder.Options);
        }
    }
}
