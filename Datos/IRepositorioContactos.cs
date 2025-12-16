using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public interface IRepositorioContactos
    {
        Task<IEnumerable<Contacto>> ObtenerTodosAsync();
        Task<Contacto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Contacto contacto);
        Task ActualizarAsync(Contacto contacto);
        Task EliminarAsync(int id);
    }
}
