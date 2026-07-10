using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Keyless]
    [Table("vw_PedidosDetalles")]
    public class VwDetallePedido
    {
        public string Pedido { get; set; }
        public short? Item { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Iva { get; set; }
        public decimal? Total { get; set; }
        public decimal? Surtir { get; set; }
        public string Relaciona { get; set; }
    }
}
