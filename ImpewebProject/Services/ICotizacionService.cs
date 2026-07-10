using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public interface ICotizacionService
    {
        Task<List<VwResumenCotizacion>> ObtenerCotizacionesAsync(int anio, string rfc, bool esAdmin);
        Task<List<VwDetalleCotizacion>> ObtenerDetallesPorCotizacionAsync(string cotizacion);
        Task<VwCotizacionPdfHeader> ObtenerPdfAsync(string cotizacion);
    }
}
