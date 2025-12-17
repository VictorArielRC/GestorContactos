namespace Dominio
{
    public class Contacto
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        public Direccion Direccion { get; set; } = new Direccion();
    }
}
