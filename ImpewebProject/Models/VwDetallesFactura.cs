using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwDetallesFactura")]
    [Keyless]
    public class VwDetalleFactura
    {
        public string? Factura { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public string? Unidad { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Cantidad { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Precio { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Importe { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Dcto { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Descuento { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ImporteDescuento { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Iva { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Total { get; set; }
    }
}