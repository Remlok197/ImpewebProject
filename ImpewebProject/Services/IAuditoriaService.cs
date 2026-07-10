using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public interface IAuditoriaService
    {
        Task RegistrarAccionAsync(string rfc, string accion);
        Task<List<BitacoraAuditoria>> ObtenerBitacoraAsync();
    }
}