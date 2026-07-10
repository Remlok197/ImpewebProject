using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("Usuarios Web")]
    public class UsuarioWeb
    {
        [Key]
        public int Id { get; set; }
        public string RFC { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Rol { get; set; } = "Cliente";
        public bool Activo { get; set; } = true;
    }
}
