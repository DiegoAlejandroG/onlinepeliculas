using api.Entidades;

namespace api.DTOs
{
    public class ProductoraDTO : IId
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public DateOnly FechaFundacion { get; set; }
        public required string SitioWeb { get; set; }
        public string? Logo { get; set; }
    }
}
