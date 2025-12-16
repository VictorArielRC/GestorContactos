using Dominio;
using Datos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicios
{
    public class ServiciodeContacto
    {
        private readonly IRepositorioContactos _repositorio;

        public ServiciodeContacto(RepositorioContactos repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<IEnumerable<Contacto>> ObtenerTodosAsync()
        {
            return await _repositorio.ObtenerTodosAsync();
        }
        public async Task<Contacto?> ObtenerPorIdAsync(int id)
        {
            return await _repositorio.ObtenerPorIdAsync(id);
        }
        public async Task<bool> AgregarAsync(Contacto contacto)
        {
            if (!RutValidador.EsValido(contacto.Rut))
                return false; // o lanzar una excepción si prefieres

            await _repositorio.AgregarAsync(contacto);
            return true;
        }
        public async Task ActualizarAsync(Contacto contacto)
        {
            if (!RutValidador.EsValido(contacto.Rut))
                throw new Exception("RUT inválido");

            await _repositorio.ActualizarAsync(contacto);
        }


    }
}
