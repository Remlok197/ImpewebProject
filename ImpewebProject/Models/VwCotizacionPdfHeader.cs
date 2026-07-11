using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwCotizacionPdfHeader")]
    [Keyless]
    public class VwCotizacionPdfHeader
    {
        public string? Cotizacion { get; set; }
        public DateTime? Fecha { get; set; }
        public short? Vigencia { get; set; } 
        public DateTime? FechaVencimiento { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public string? Elaboro { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Descuento { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? SubTotal { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? Iva { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? Total { get; set; }

        public int? ClienteNumero { get; set; }
        public string? ClienteNombre { get; set; }
        public int? AgenteNumero { get; set; }
        public string? AgenteNombre { get; set; }
        public string? ClienteRfc { get; set; }
    }
}