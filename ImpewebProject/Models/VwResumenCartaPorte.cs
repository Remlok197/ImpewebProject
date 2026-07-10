using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("VwResumenCartaPorte")]
    public class VwResumenCartaPorte
    {
        [Key]
        public string CartaPorte { get; set; }
        public string? Serie { get; set; }
        public int? Folio { get; set; }
        public int? Cliente { get; set; }
        public DateTime? Fecha { get; set; }
        public string? TipoDeComprobante { get; set; }
        public string? Uuid { get; set; }
        public string? Factura { get; set; }
        public string? Destinatario { get; set; }
        public string? Rfc { get; set; }
        public string? Calle { get; set; }
        public string? TranspInternac { get; set; }
        public string? EntradaSalidaMerc { get; set; }
        public string? ViaEntradaSalida { get; set; }
        public decimal? TotalDistRec { get; set; }
        public decimal? PesoBrutoTotal { get; set; }
        public string? UnidadPeso { get; set; }
        public decimal? PesoNetoTotal { get; set; }
        public int? NumTotalMercancias { get; set; }
        public string? Obs { get; set; }
        public string? Status { get; set; }
        public string? MotivoBaja { get; set; }
        public string? Equipo { get; set; }
        public string? Usuario { get; set; }
        public short? Periodo { get; set; }
    }
}