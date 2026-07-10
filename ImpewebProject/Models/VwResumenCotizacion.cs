using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("VwResumenCotizacion")]
    public class VwResumenCotizacion
    {
        [Key]
        public string Cotizacion { get; set; }

        public string? Serie { get; set; }
        public int? Folio { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Hora { get; set; }

        public int? Cliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? Rfc { get; set; } 

        public string? Tipo { get; set; }
        public string? Referencia { get; set; }
        public short? Plazo { get; set; }

        public decimal? SubTotal { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? Iva { get; set; }
        public decimal? Total { get; set; }

        public string? Moneda { get; set; }
        public string? Status { get; set; }
        public string? MotivoBaja { get; set; }
        public string? Obs { get; set; }

        public string? Usuario { get; set; }
        public string? Equipo { get; set; }
        public short? Periodo { get; set; }
    }
}