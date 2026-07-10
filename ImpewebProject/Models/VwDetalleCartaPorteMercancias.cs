using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwDetallesCartaPorteMercancias")]
    [Keyless]
    public class VwDetalleCartaPorteMercancia
    {
        public string? CartaPorte { get; set; }
        public string? BienesTransp { get; set; }
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Cantidad { get; set; }
        public string? ClaveUnidad { get; set; }
    }
}