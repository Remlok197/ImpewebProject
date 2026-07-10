using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImpewebProject.Models;

[Table("vw_ResumenFacturas")]
public partial class VwResumenFactura
{
    [Key]
    
    public string Factura { get; set; } = null!;

    public string? Serie { get; set; }

    public int? Folio { get; set; }

    public string? FormaPago { get; set; }

    public string? MetodoPago { get; set; }

    public int? TipoPa { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Status { get; set; }

    public int? DiasUltimoPago { get; set; }

    public string? Uuid { get; set; }

    public string? Estado { get; set; }

    public string? Esquema { get; set; }

    public string? Depto { get; set; }

    public int? Cliente { get; set; }

    public string? Nombre { get; set; }

    public string? Rfc { get; set; }

    public string? Remision { get; set; }

    public string? Orden { get; set; }

    public string? Economico { get; set; }

    public string? Pedido { get; set; }

    public int? Agente { get; set; }

    public string? NombreAgente { get; set; }

    public string? Tipo { get; set; }

    public short? Plazo { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? IvaRetencion { get; set; }

    public decimal? IsrRetencion { get; set; }

    public decimal? IepsRetencion { get; set; }

    public decimal? Total { get; set; }

    public decimal? Abonos { get; set; }

    public decimal? Descuentos { get; set; }

    public decimal? Saldo { get; set; }

    public string? Equipo { get; set; }

    public string? Usuario { get; set; }

    public string? MotivoBaja { get; set; }

    public string? Scan { get; set; }

    public short? Periodo { get; set; }
}
