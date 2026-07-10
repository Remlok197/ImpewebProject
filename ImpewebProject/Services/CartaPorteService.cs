using ImpewebProject.Data;
using ImpewebProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Services
{
    public class CartaPorteService : ICartaPorteService
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;

        public CartaPorteService(IDbContextFactory<ImpewebContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VwResumenCartaPorte>> ObtenerCartasPorteAsync(int anio, string rfc, bool esAdmin)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.VwResumenCartaPorte.Where(cp => cp.Periodo == anio);

            if (!esAdmin)
            {
                query = query.Where(cp => cp.Rfc == rfc);
            }

            return await query
                .OrderByDescending(cp => cp.Fecha)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VwDetalleCartaPorteHeader?> ObtenerDetalleHeaderAsync(string cartaPorte)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.VwDetallesCartaPorteHeader
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CartaPorte == cartaPorte);
        }

        public async Task<List<VwDetalleCartaPorteMercancia>> ObtenerDetalleMercanciasAsync(string cartaPorte)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.VwDetallesCartaPorteMercancias
                .Where(x => x.CartaPorte == cartaPorte)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
