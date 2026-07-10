using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public interface ICartaPorteService
    {
        Task<List<VwResumenCartaPorte>> ObtenerCartasPorteAsync(int anio, string rfc, bool esAdmin);

        Task<VwDetalleCartaPorteHeader?> ObtenerDetalleHeaderAsync(string cartaPorte);
        Task<List<VwDetalleCartaPorteMercancia>> ObtenerDetalleMercanciasAsync(string cartaPorte);
    }
}