using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("VwResumenComplementosPago")]
    public class VwResumenComplementosPago
    {
        [Key]
        public string Pago { get; set; } 

        public string? Serie { get; set; }

        public int? Folio { get; set; }

        public DateTime? Fecha { get; set; }
        public string? Uuid { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? Abono { get; set; }
        public int? Cliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? Rfc { get; set; }
        public string? FormaPago { get; set; }
        public string? NombreFormaPago { get; set; }
        public string? Esquema { get; set; }
        public string? NomBancoOrdExt { get; set; }
        public string? CtaOrdenante { get; set; }
        public string? Anotacion { get; set; }
        public string? Status { get; set; }
        public string? MotivoBaja { get; set; }
        public string? Usuario { get; set; }
        public string? Equipo { get; set; }
        public short? Periodo { get; set; }
    }
}