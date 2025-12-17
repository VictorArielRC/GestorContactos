namespace Dominio
{
    public class Direccion
    {
        public int Id { get; set; }
        public string Calle { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;

        public int ContactoId { get; set; } // FK explícita
    }
}
