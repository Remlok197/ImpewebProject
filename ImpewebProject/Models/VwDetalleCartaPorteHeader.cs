using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwDetallesCartaPorteHeader")]
    [Keyless]
    public class VwDetalleCartaPorteHeader
    {
        public string? CartaPorte { get; set; }
        public string? UUID { get; set; }
        public string? IdCCP { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? PesoBrutoTotal { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? PesoNetoTotal { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? CargoPorTasacion { get; set; }

        public string? UnidadPeso { get; set; }
        public int? NumTotalMercancias { get; set; }
        public string? LogisticaInversaRecoleccionDevolucion { get; set; }
        public string? ClienteRfc { get; set; }
    }
}