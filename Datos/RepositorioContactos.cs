using Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Datos
{
    public class RepositorioContactos : IRepositorioContactos
    {
        private readonly GestorContactosDbContext _contexto;

        public RepositorioContactos(GestorContactosDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Contacto>> ObtenerTodosAsync()
        {
            return await _contexto.Contactos.Include(c => c.Direccion).ToListAsync();
        }

        public async Task<Contacto?> ObtenerPorIdAsync(int id)
        {
            return await _contexto.Contactos.Include(c => c.Direccion)
                                            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AgregarAsync(Contacto contacto)
        {
            _contexto.Contactos.Add(contacto);
            await _contexto.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Contacto contacto)
        {
            _contexto.Contactos.Update(contacto);
            await _contexto.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var contacto = await _contexto.Contactos.FindAsync(id);
            if (contacto != null)
            {
                _contexto.Contactos.Remove(contacto);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}
