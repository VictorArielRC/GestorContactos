using Dominio;
using Datos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicios
{
    public class ServiciodeContacto
    {
        private readonly IRepositorioContactos _repositorio;

        public ServiciodeContacto(IRepositorioContactos repositorio)
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

        public async Task<IEnumerable<Contacto>> ObtenerFiltradosAsync(string? nombre, string? rut)
        {
            return await _repositorio.ObtenerFiltradosAsync(nombre, rut);
        }

        public async Task<bool> AgregarAsync(Contacto contacto)
        {
            if (!RutValidador.EsValido(contacto.Rut))
                return false;

            await _repositorio.AgregarAsync(contacto);
            return true;
        }

        public async Task ActualizarAsync(Contacto contacto)
        {
            if (!RutValidador.EsValido(contacto.Rut))
                throw new Exception("RUT inválido");

            await _repositorio.ActualizarAsync(contacto);
        }

        public async Task EliminarLogicoAsync(int id)
        {
            await _repositorio.EliminarLogicoAsync(id);
        }
    }
}
