using System;
using System.Collections.Generic;
using System.Windows;
using Dominio;
using Servicios;
using Datos;

namespace GestionaContactos.Views
{
    public partial class MainWindow : Window
    {
        private readonly ServiciodeContacto _servicio;

        // Constructor sin parámetros requerido por WPF
        public MainWindow() : this(
            new ServiciodeContacto(
                new RepositorioContactos(
                    new GestorContactosDbContextFactory().CreateDbContext(Array.Empty<string>())
                )
            )
        )
        {
            // Deja el constructor vacío, el otro inicializa componentes
        }

        // Constructor principal con inyección de servicio
        public MainWindow(ServiciodeContacto servicio)
        {
            InitializeComponent(); // Carga XAML y elementos visuales
            _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
            Loaded += OnLoaded; // cargar datos cuando ventana esté lista
        }

        private async void OnLoaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            await CargarContactosAsync();
        }

        // Carga todos los contactos al iniciar
        private async System.Threading.Tasks.Task CargarContactosAsync()
        {
            var lista = await _servicio.ObtenerTodosAsync();
            GridContactos.ItemsSource = lista;
            return;
        }

        // Filtra contactos por nombre o RUT
        private async void Buscar_Click(object sender, RoutedEventArgs e)
        {
            var nombre = FiltroNombre.Text;
            var rut = FiltroRut.Text;
            var lista = await _servicio.ObtenerFiltradosAsync(nombre, rut);
            GridContactos.ItemsSource = lista;
        }

        // Crea un nuevo contacto de ejemplo
        private async void Crear_Click(object sender, RoutedEventArgs e)
        {
            var nuevo = new Contacto
            {
                Rut = "11.111.111-1",
                Nombres = "Nuevo",
                Apellidos = "Contacto",
                Email = "nuevo@correo.cl",
                Telefono = "+56 9 1234 5678",
                Direccion = new Direccion
                {
                    Calle = "Calle Falsa 123",
                    Ciudad = "Valparaíso",
                    Region = "Valparaíso"
                }
            };

            await _servicio.AgregarAsync(nuevo);
            await CargarContactosAsync();
        }

        // Actualiza el contacto seleccionado
        private async void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            if (GridContactos.SelectedItem is Contacto seleccionado)
            {
                seleccionado.Email = "actualizado@correo.cl";
                await _servicio.ActualizarAsync(seleccionado);
                await CargarContactosAsync();
            }
        }

        // Elimina lógicamente el contacto seleccionado
        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GridContactos.SelectedItem is Contacto seleccionado)
            {
                await _servicio.EliminarLogicoAsync(seleccionado.Id);
                await CargarContactosAsync();
            }
        }
    }
}
