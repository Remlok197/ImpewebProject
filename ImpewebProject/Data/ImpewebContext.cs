using System;
using System.Collections.Generic;
using ImpewebProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ImpewebProject.Data;

public partial class ImpewebContext : DbContext
{
    public ImpewebContext(DbContextOptions<ImpewebContext> options)
        : base(options)
    {

    }
    public virtual DbSet<VwResumenCotizacion> VwResumenCotizaciones { get; set; }

    public virtual DbSet<VwResumenFactura> VwResumenFacturas { get; set; }
    public virtual DbSet<FacturasScan> FacturasScans { get; set; }
    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<VwResumenOrdenCompra> VwResumenOrdenesCompra { get; set; }
    public virtual DbSet<VwResumenCartaPorte> VwResumenCartaPorte { get; set; }
    public virtual DbSet<VwResumenComplementosPago> VwResumenComplementosPago { get; set; }
    public virtual DbSet<UsuarioWeb> UsuarioWeb { get; set; }
    public virtual DbSet<BitacoraAuditoria> BitacoraAuditoria { get; set; }
    public virtual DbSet<VwDetalleFactura> VwDetallesFactura { get; set; }
    public virtual DbSet<VwDetalleCotizacion> VwDetallesCotizaciones { get; set; }
    public virtual DbSet<VwDetalleCartaPorteHeader> VwDetallesCartaPorteHeader { get; set; }
    public virtual DbSet<VwDetalleCartaPorteMercancia> VwDetallesCartaPorteMercancias { get; set; }
    public virtual DbSet<VwDetallePago> VwDetallesPagos { get; set; }
    public virtual DbSet<VwFacturaPdfHeader> VwFacturaPdfHeaders { get; set; }
    public virtual DbSet<VwCotizacionPdfHeader> VwCotizacionPdfHeaders { get; set; }
    public virtual DbSet<VwPagoPdfHeader> VwPagoPdfHeaders { get; set; }
    public virtual DbSet<VwCartaPortePdfHeader> VwCartaPortePdfHeader { get; set; }
    public virtual DbSet<VwCartaPortePdfUbicacion> VwCartaPortePdfUbicaciones { get; set; }
    public virtual DbSet<VwCartaPortePdfMercancia> VwCartaPortePdfMercancias { get; set; }
    public virtual DbSet<VwCartaPortePdfRemolque> VwCartaPortePdfRemolques { get; set; }
    public virtual DbSet<VwCartaPortePdfFigura> VwCartaPortePdfFiguras { get; set; }
    public virtual DbSet<VwResumenPedido> VwResumenPedidos { get; set; }
    public virtual DbSet<VwDetallePedido> VwPedidosDetalles { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //   => optionsBuilder.UseSqlServer("Server=SERVERMAX;Database=CompuMax;User Id=sa;Password=rrmvolvo_siv;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}