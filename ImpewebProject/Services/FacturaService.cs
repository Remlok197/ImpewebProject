using ImpewebProject.Data;
using ImpewebProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ImpewebProject.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;
        public FacturaService(IDbContextFactory<ImpewebContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VwResumenFactura>> ObtenerFacturasAsync(int anio, string rfc, bool esAdmin)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.VwResumenFacturas.Where(f => f.Periodo == anio);
            if (!esAdmin)
            {
                query = query.Where(f => f.Rfc == rfc);
            }
            return await query
                .OrderByDescending(f => f.Factura)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VwFacturaPdfHeader?> ObtenerPdfAsync(string factura)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.VwFacturaPdfHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Factura == factura);
        }

        public async Task<List<VwDetalleFactura>> ObtenerDetallesPorFacturaAsync(string factura)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.VwDetallesFactura
                .Where(d => d.Factura == factura)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
