namespace Dominio
{
    public class Contacto
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Rut {  get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        public Direccion Direccion { get; set; } = new Direccion ();


    }
}
