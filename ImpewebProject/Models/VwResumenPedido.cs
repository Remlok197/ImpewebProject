using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models
{
    [Table("vw_ResumenPedidos")]
    public class VwResumenPedido
    {
        [Key]
        public string Pedido { get; set; }
        public string Serie { get; set; }
        public int? Folio { get; set; }
        public int? NumeroCliente { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteRFC { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Hora { get; set; }
        public short? Plazo { get; set; }
        public string Referencia { get; set; }
        public string Tipo { get; set; }
        public decimal? Importe { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Iva { get; set; }
        public decimal? Total { get; set; }
        public string Obs { get; set; }
        public string Status { get; set; }
        public string MotivoBaja { get; set; }
        public string? FacturaRelacionada { get; set; }
    }
}
