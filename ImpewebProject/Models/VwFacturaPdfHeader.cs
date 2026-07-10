using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwFacturaPdfHeader")]
    [Keyless]
    public class VwFacturaPdfHeader
    {
        public string? Factura { get; set; }
        public string? Serie { get; set; }
        public int? Folio { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Pedido { get; set; }
        public short? Plazo { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? SubTotal { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? Iva { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? Total { get; set; }

        public string? Moneda { get; set; }
        public string? EmisorRfc { get; set; }
        public string? ReceptorRfc { get; set; }
        public string? ReceptorNombre { get; set; }

        // Dirección
        public string? Calle { get; set; }
        public string? noExterior { get; set; }
        public string? noInterior { get; set; }
        public string? Colonia { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Municipio { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? Telefonos { get; set; }

        // Comercial y SAT
        public string? NombreAgente { get; set; }
        public string? FormaPago { get; set; }
        public string? MetodoPago { get; set; }
        public string? UsoCfdi { get; set; }
        public string? RegimenFiscal { get; set; }

        // Timbre Fiscal
        public string? UUID { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string? CertificadoEmisor { get; set; }
        public string? CertificadoSAT { get; set; }
        public string? SelloDigitalCFDI { get; set; }
        public string? SelloDigitalSAT { get; set; }
        public string? CadenaOriginal { get; set; }
    }
}