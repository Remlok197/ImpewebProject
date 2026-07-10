using System.Collections.Generic;
using System.Threading.Tasks;
using ImpewebProject.Models;

namespace ImpewebProject.Services

{
    public interface IPedidosService
    {
        Task<List<VwResumenPedido>> ObtenerPedidosPorRfcAsync(int anio, string rfc, bool esAdmin);
        Task<List<VwDetallePedido>> ObtenerDetallesPorPedidoAsync(string pedidoId);
    }
}
