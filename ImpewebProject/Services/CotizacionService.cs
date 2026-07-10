using ImpewebProject.Data;
using ImpewebProject.Models;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ImpewebProject.Services
{
    public class CotizacionService : ICotizacionService
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;

        public CotizacionService(IDbContextFactory<ImpewebContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VwResumenCotizacion>> ObtenerCotizacionesAsync(int año, string rfc, bool esAdmin)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.VwResumenCotizaciones.Where(c => c.Periodo == año);

            if (!esAdmin)
            {
                query = query.Where(c => c.Rfc == rfc);
            }

            return await query
                .OrderByDescending(c => c.Cotizacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<VwDetalleCotizacion>> ObtenerDetallesPorCotizacionAsync(string cotizacion)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.VwDetallesCotizaciones
                .Where(d => d.Cotizacion == cotizacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VwCotizacionPdfHeader?> ObtenerPdfAsync(string cotizacion)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.VwCotizacionPdfHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Cotizacion == cotizacion);
        }
    }
}
