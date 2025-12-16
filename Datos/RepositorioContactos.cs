using Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Datos
{
    public class ContactoRepository : IRepositorioContactos
    {
        private readonly GestorContactosDbContext _context;

        public ContactoRepository(GestorContactosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contacto>> ObtenerTodosAsync()
        {
            return await _context.Contactos.Include(c => c.Direccion).ToListAsync();
        }

        public async Task<Contacto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Contactos.Include(c => c.Direccion)
                                           .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AgregarAsync(Contacto contacto)
        {
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Contacto contacto)
        {
            _context.Contactos.Update(contacto);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto != null)
            {
                _context.Contactos.Remove(contacto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
