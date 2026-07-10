using System.ComponentModel.DataAnnotations;

namespace ImpewebProject.Models
{
    public class VwResumenOrdenCompra
    {
        [Key]
        public int Orden {  get; set; }
        public string? Serie { get; set; }
        public string? Folio { get; set; }
        public int? NumeroProveedor { get; set; }
        public string? NombreProveedor { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Tipo { get; set; }
        public string? Referencia { get; set; }
        public string? Almacen { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Iva { get; set; }
        public decimal? Total { get; set; }
        public string? Status { get; set; }
        public int? Periodo { get; set; }
    }
}
