using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("Facturas Scan")]
    public class FacturasScan
    {
        [Key]
        public String Factura { get; set; } = null!;
        public byte[]? Scan1 { get; set; }
        public byte[]? Scan2 { get; set; }
        public byte[]? Scan3 { get; set; }
    }
}
