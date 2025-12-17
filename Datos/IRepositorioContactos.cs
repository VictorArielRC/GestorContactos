using Dominio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Datos
{
    public interface IRepositorioContactos
    {
        Task<IEnumerable<Contacto>> ObtenerTodosAsync();
        Task<Contacto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Contacto>> ObtenerFiltradosAsync(string? nombre, string? rut);
        Task AgregarAsync(Contacto contacto);
        Task ActualizarAsync(Contacto contacto);
        Task EliminarAsync(int id);
        Task EliminarLogicoAsync(int id);
    }
}

