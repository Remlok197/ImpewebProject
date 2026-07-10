using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [Column("Numero")]
        public int Numero { get; set; }
        [Column("Nombre")]
        public string? Nombre { get; set; }
        [Column("Rfc")]
        public string? Rfc { get; set; }
    }
}
