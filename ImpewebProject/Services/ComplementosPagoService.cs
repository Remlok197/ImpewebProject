using ImpewebProject.Data;
using ImpewebProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Services
{
    public class ComplementosPagoService : IComplementosPagoService
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;

        public ComplementosPagoService(IDbContextFactory<ImpewebContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<VwResumenComplementosPago>> ObtenerComplemetosPagoAsync(int anio, string rfc, bool esAdmin)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.VwResumenComplementosPago.Where(c=> c.Periodo == anio);
            if (!esAdmin)
            {
                query = query.Where(c=> c.Rfc == rfc);
            }
            return await query
                .OrderByDescending (c => c.Pago)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<VwDetallePago>> ObtenerDetallesPorPagoAsync(string pago)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.VwDetallesPagos
                .Where(d => d.Pago == pago)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VwPagoPdfHeader?> ObtenerPdfAsync(string pago)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.VwPagoPdfHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Pago == pago);
        }
    }
}
