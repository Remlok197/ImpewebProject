using ImpewebProject.Data;
using ImpewebProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Services
{
    public class OrdenesCompraService : IOrdenesCompraService
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;
        public OrdenesCompraService(IDbContextFactory<ImpewebContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<VwResumenOrdenCompra>> ObtenerOrdenesCompraAsync(int anio)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.VwResumenOrdenesCompra
                .Where(o=> o.Periodo == anio)
                .OrderByDescending(f => f.Fecha)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
