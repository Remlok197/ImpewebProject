using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwDetallesPago")]
    [Keyless]
    public class VwDetallePago
    {
        public string? Pago { get; set; }
        public string? Factura { get; set; }
        public string? UuidDocumento { get; set; }
        public DateTime? FechaFactura { get; set; }
        public short? Plazo { get; set; }
        public DateTime? VenceFactura { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? TotalFactura { get; set; }

        public int? NumParcialidad { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ImpSaldoAnt { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? ImpPagado { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? ImpSaldoInsoluto { get; set; }

        public string? ObjetoImpDR { get; set; }
    }
}