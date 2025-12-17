using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Datos
{
    public class GestorContactosDbContextFactory : IDesignTimeDbContextFactory<GestorContactosDbContext>
    {
        public GestorContactosDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GestorContactosDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=GestorContactosDB;Trusted_Connection=True;Encrypt=False;");
            return new GestorContactosDbContext(optionsBuilder.Options);
        }
    }
}
