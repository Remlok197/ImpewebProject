using System.ComponentModel.DataAnnotations;

namespace ImpewebProject.Models
{
    public class BitacoraAuditoria
    {
        [Key]
        public int Id { get; set; }
        public string RFC { get; set; } = "";
        public string Accion { get; set; } = "";
        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}