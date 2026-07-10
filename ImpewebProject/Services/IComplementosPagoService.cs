using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public interface IComplementosPagoService
    {
        Task<List<VwResumenComplementosPago>> ObtenerComplemetosPagoAsync(int anio, string rfc, bool esAdmin);
        Task<List<VwDetallePago>> ObtenerDetallesPorPagoAsync(string pago);
        Task<VwPagoPdfHeader?> ObtenerPdfAsync(string pago);
    }
}
