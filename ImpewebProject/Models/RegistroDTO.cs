using System.ComponentModel.DataAnnotations;

namespace ImpewebProject.Models
{
    public class RegistroDTO
    {
        [Required(ErrorMessage = "El RFC es obligatorio.")]
        public string RFC { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Debe confirmar su contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = "";
    }
}
