using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public interface  IFacturaService
    {
        Task<List<VwResumenFactura>> ObtenerFacturasAsync(int anio, string rfc, bool esAdmin);
        Task<VwFacturaPdfHeader?> ObtenerPdfAsync(string factura);
        Task<List<VwDetalleFactura>> ObtenerDetallesPorFacturaAsync(string factura);
    }
}
