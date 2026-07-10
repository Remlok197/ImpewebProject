using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwPagoPdfHeader")]
    [Keyless]
    public class VwPagoPdfHeader
    {
        public string? Pago { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? FormaPago { get; set; }
        public string? Anotacion { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MontoTotalPago { get; set; }
        public string? Moneda { get; set; }

        public string? EmisorRfc { get; set; }
        public string? ReceptorRfc { get; set; }
        public string? RegimenFiscalReceptor { get; set; }

        public int? ClienteNumero { get; set; }
        public string? ClienteNombre { get; set; }

        // Dirección
        public string? Calle { get; set; }
        public string? noExterior { get; set; }
        public string? noInterior { get; set; }
        public string? Colonia { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Municipio { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }

        // Timbre Fiscal SAT
        public string? UUID { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string? CertificadoEmisor { get; set; }
        public string? CertificadoSAT { get; set; }
        public string? SelloDigitalCFDI { get; set; }
        public string? SelloDigitalSAT { get; set; }
        public string? CadenaOriginal { get; set; }
    }
}