using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ImpewebProject.Data;
using ImpewebProject.Models;

namespace ImpewebProject.Services
{
    public class PedidoService : IPedidosService
    {
        private readonly ImpewebContext _context;
        public PedidoService(ImpewebContext context) 
        { 
            _context = context;
        }

        public async Task<List<VwResumenPedido>> ObtenerPedidosPorRfcAsync(int anio, string rfc, bool esAdmin)
        {
            try
            {
                var query = _context.VwResumenPedidos
                                    .Where(p => p.Fecha.HasValue && p.Fecha.Value.Year == anio);
                if (!esAdmin)
                {
                    if (string.IsNullOrEmpty(rfc)) return new List<VwResumenPedido>();

                    var rfcLimpio = rfc.Trim().ToUpper();
                    query = query.Where(p => p.ClienteRFC == rfcLimpio);
                }

                return await query.OrderByDescending(p => p.Fecha).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener pedidos: {ex.Message}");
                return new List<VwResumenPedido>();
            }
        }

        public async Task<List<VwDetallePedido>> ObtenerDetallesPorPedidoAsync(string pedidoId)
        {
            try
            {
                if (string.IsNullOrEmpty(pedidoId)) return new List<VwDetallePedido>();

                return await _context.VwPedidosDetalles
                                     .Where(d => d.Pedido == pedidoId)
                                     .OrderBy(d => d.Item)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles del pedido {pedidoId}: {ex.Message}");
                return new List<VwDetallePedido>();
            }
        }
    }
}
