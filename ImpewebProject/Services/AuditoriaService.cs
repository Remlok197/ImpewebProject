using Microsoft.EntityFrameworkCore;
using ImpewebProject.Data;
using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;

        public AuditoriaService(IDbContextFactory<ImpewebContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task RegistrarAccionAsync(string rfc, string accion)
        {
            using var context = _contextFactory.CreateDbContext();

            var registro = new BitacoraAuditoria
            {
                RFC = string.IsNullOrEmpty(rfc) ? "Desconocido" : rfc,
                Accion = accion,
                FechaHora = DateTime.Now
            };

            context.BitacoraAuditoria.Add(registro);
            await context.SaveChangesAsync();
        }

        public async Task<List<BitacoraAuditoria>> ObtenerBitacoraAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.BitacoraAuditoria
                .OrderByDescending(b => b.FechaHora)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}