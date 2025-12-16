namespace Dominio
{
    public class Contacto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rut {  get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        public Direccion Direccion { get; set; } = new Direccion ();


    }
}
