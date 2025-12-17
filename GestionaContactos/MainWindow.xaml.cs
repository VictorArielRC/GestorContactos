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
        }

        // Constructor principal con inyección de servicio
        public MainWindow(ServiciodeContacto servicio)
        {
            InitializeComponent();
            _servicio = servicio;
            CargarContactos();
        }

        private async void CargarContactos()
        {
            var lista = await _servicio.ObtenerTodosAsync();
            GridContactos.ItemsSource = lista;
        }

        private async void Buscar_Click(object sender, RoutedEventArgs e)
        {
            var nombre = FiltroNombre.Text;
            var rut = FiltroRut.Text;
            var lista = await _servicio.ObtenerFiltradosAsync(nombre, rut);
            GridContactos.ItemsSource = lista;
        }

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
            CargarContactos();
        }

        private async void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            if (GridContactos.SelectedItem is Contacto seleccionado)
            {
                seleccionado.Email = "actualizado@correo.cl";
                await _servicio.ActualizarAsync(seleccionado);
                CargarContactos();
            }
        }

        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GridContactos.SelectedItem is Contacto seleccionado)
            {
                await _servicio.EliminarLogicoAsync(seleccionado.Id);
                CargarContactos();
            }
        }
    }
}
