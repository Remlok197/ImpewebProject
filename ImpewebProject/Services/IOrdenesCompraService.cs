using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public interface IOrdenesCompraService
    {
        Task<List<VwResumenOrdenCompra>> ObtenerOrdenesCompraAsync(int anio);
    }
}
