using System.Windows;
using Datos;                  // Proyecto de capa de datos
using Microsoft.EntityFrameworkCore;
using Servicios;              // Proyecto de capa de servicios
using GestionaContactos.ViewModels; // Ajusta al namespace real de tu ViewModel
using GestionaContactos.Views;     // Ajusta al namespace real de tu MainWindow

namespace GestionaContactos
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuración de conexión a SQL Server
            var options = new DbContextOptionsBuilder<GestorContactosDbContext>()
                .UseSqlServer("Server=localhost;Database=GestorContactosDB;Trusted_Connection=True;Encrypt=False;")
                .Options;

            // Inicializar DbContext
            var db = new GestorContactosDbContext(options);
            await db.Database.EnsureCreatedAsync();

            // Inyección de dependencias manual
            var repo = new RepositorioContactos(db);
            var servicio = new ServiciodeContacto(repo);
            var vm = new MainViewModel(servicio);
            await vm.CargarAsync();

            // Crear ventana principal y asignar DataContext
            var ventana = new MainWindow { DataContext = vm };
            ventana.Show();
        }
    }
}
