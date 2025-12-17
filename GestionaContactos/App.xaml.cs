using System.Windows;
using Datos;
using Microsoft.EntityFrameworkCore;
using Servicios;
using GestionaContactos.Views;

namespace GestionaContactos
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var options = new DbContextOptionsBuilder<GestorContactosDbContext>()
                .UseSqlServer("Server=localhost;Database=GestorContactosDB;Trusted_Connection=True;Encrypt=False;")
                .Options;

            var db = new GestorContactosDbContext(options);
            await db.Database.EnsureCreatedAsync();

            var repo = new RepositorioContactos(db);
            var servicio = new ServiciodeContacto(repo);

            var ventana = new MainWindow(servicio);
            ventana.Show();
        }
    }
}

