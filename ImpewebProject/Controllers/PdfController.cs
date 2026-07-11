using ImpewebProject.Data;
using ImpewebProject.Pdfs;
using ImpewebProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QuestPDF.Fluent;
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Logging;

namespace ImpewebProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly ILogger <PdfController> _logger;
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;
        private readonly IFacturaService _facturaService;
        private readonly ICotizacionService _cotizacionService;
        private readonly IComplementosPagoService _complementosPagoService;


        public PdfController(IFacturaService facturaService, ICotizacionService cotizacionService, 
            IComplementosPagoService complementosPagoService, IDbContextFactory<ImpewebContext> contextFactory,
            ILogger<PdfController> logger)
        {
            _facturaService = facturaService;
            _cotizacionService = cotizacionService;
            _complementosPagoService = complementosPagoService;
            _contextFactory = contextFactory;
            _logger = logger;
        }

        [HttpGet("factura/{folio}")]
        public async Task<IActionResult> DescargarFacturaPdf(string folio)
        {
            try
            {
                var header = await _facturaService.ObtenerPdfAsync(folio);
                var detalles = await _facturaService.ObtenerDetallesPorFacturaAsync(folio);

                if (header == null) return NotFound($"No se encontró la factura con folio: {folio}");

                bool esAdmin = User.IsInRole("Administrador");
                string rfcActual = User.Identity?.Name ?? "";

                if (!esAdmin && !string.Equals(header.ReceptorRfc, rfcActual, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                string sello8 = header.SelloDigitalCFDI?.Length >= 8
                    ? header.SelloDigitalCFDI.Substring(header.SelloDigitalCFDI.Length - 8)
                    : "";
                string totalFmt = header.Total?.ToString("0.000000") ?? "0.000000";
                string urlSat = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id={header.UUID}&re={header.EmisorRfc}&rr={header.ReceptorRfc}&tt={totalFmt}&fe={sello8}";
                using var qrGenerator = new QRCoder.QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(urlSat, QRCoder.QRCodeGenerator.ECCLevel.M);
                using var qrCode = new QRCoder.PngByteQRCode(qrData);
                byte[] qrImagenBytes = qrCode.GetGraphic(20);
                var documento = new FacturaDocument(header, detalles, qrImagenBytes);

                byte[] pdfBytes = documento.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Factura_{folio}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al generar PDF de Factura {folio}");
                return StatusCode(500, "Error interno al procesar el documento.");
            }
        }

        [HttpGet("cotizacion/{folio}")]
        public async Task<IActionResult> DescargarCotizacionPdf(string folio)
        {
            try
            {
                var header = await _cotizacionService.ObtenerPdfAsync(folio);
                var detalles = await _cotizacionService.ObtenerDetallesPorCotizacionAsync(folio);

                if (header == null) return NotFound($"No se encontró la cotización: {folio}");

                bool esAdmin = User.IsInRole("Administrador");
                string rfcActual = User.Identity?.Name ?? "";

                if (!esAdmin && !string.Equals(header.ClienteRfc, rfcActual, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
                var documento = new CotizacionDocument(header, detalles);
                byte[] pdfBytes = documento.GeneratePdf();

                return File(pdfBytes, "application/pdf", $"Cotizacion_{folio}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al generar PDF de la Cotizacion {folio}");
                return StatusCode(500, "Error interno al procesar el documento."); ;
            }
        }

        [HttpGet("pago/{folio}")]
        public async Task<IActionResult> DescargarPagoPdf(string folio)
        {
            try
            {
                var header = await _complementosPagoService.ObtenerPdfAsync(folio);
                var detalles = await _complementosPagoService.ObtenerDetallesPorPagoAsync(folio);

                if (header == null) return NotFound($"No se encontró el registro de pago: {folio}");

                bool esAdmin = User.IsInRole("Administrador");
                string rfcActual = User.Identity?.Name ?? "";

                if (!esAdmin && !string.Equals(header.ReceptorRfc, rfcActual, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                byte[]? qrBytes = null;

                // Si está timbrado, generamos el QR oficial del SAT
                if (!string.IsNullOrEmpty(header.UUID))
                {
                    string sello8 = header.SelloDigitalCFDI?.Length >= 8 ? header.SelloDigitalCFDI.Substring(header.SelloDigitalCFDI.Length - 8) : "";
                    string totalFmt = header.MontoTotalPago?.ToString("0.000000") ?? "0.000000";
                    string urlSat = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id={header.UUID}&re={header.EmisorRfc}&rr={header.ReceptorRfc}&tt={totalFmt}&fe={sello8}";

                    using var qrGenerator = new QRCoder.QRCodeGenerator();
                    using var qrData = qrGenerator.CreateQrCode(urlSat, QRCoder.QRCodeGenerator.ECCLevel.M);
                    using var qrCode = new QRCoder.PngByteQRCode(qrData);
                    qrBytes = qrCode.GetGraphic(20);
                }

                // Le pasamos el header, detalles y el QR 
                var documento = new PagoDocument(header, detalles, qrBytes);
                byte[] pdfBytes = documento.GeneratePdf();

                return File(pdfBytes, "application/pdf", $"Pago_{folio}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al generar PDF del Pago {folio}");
                return StatusCode(500, "Error interno al procesar el documento.");
            }
        }

        [HttpGet("cartaporte/{folio}")]
        public async Task<IActionResult> DescargarCartaPortePdf(string folio)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var header = await context.VwCartaPortePdfHeader.AsNoTracking().FirstOrDefaultAsync(x => x.CartaPorte == folio);
                if (header == null) return NotFound("No se encontró la Carta Porte");

                bool esAdmin = User.IsInRole("Administrador");
                string rfcActual = User.Identity?.Name ?? "";


                if (!esAdmin && !string.Equals(header.ClienteRfc, rfcActual, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                var ubicaciones = await context.VwCartaPortePdfUbicaciones.AsNoTracking().Where(x => x.CartaPorte == folio).ToListAsync();
                var mercancias = await context.VwCartaPortePdfMercancias.AsNoTracking().Where(x => x.CartaPorte == folio).ToListAsync();
                var remolques = await context.VwCartaPortePdfRemolques.AsNoTracking().Where(x => x.CartaPorte == folio).ToListAsync();
                var figuras = await context.VwCartaPortePdfFiguras.AsNoTracking().Where(x => x.CartaPorte == folio).ToListAsync();

                byte[]? qrBytes = null;
                if (!string.IsNullOrEmpty(header.UUID))
                {
                    string sello8 = header.SelloDigitalCFDI?.Length >= 8 ? header.SelloDigitalCFDI.Substring(header.SelloDigitalCFDI.Length - 8) : "";
                    string urlSat = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id={header.UUID}&re=ISO150212C50&rr=ISO150212C50&tt=0.000000&fe={sello8}"; // Traslado lleva ceros y mismo RFC
                    using var qrGenerator = new QRCoder.QRCodeGenerator();
                    using var qrData = qrGenerator.CreateQrCode(urlSat, QRCoder.QRCodeGenerator.ECCLevel.M);
                    using var qrCode = new QRCoder.PngByteQRCode(qrData);
                    qrBytes = qrCode.GetGraphic(20);
                }

                var documento = new CartaPorteDocument(header, ubicaciones, mercancias, remolques, figuras, qrBytes);
                return File(documento.GeneratePdf(), "application/pdf", $"CartaPorte_{folio}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al generar PDF de Carta Porte {folio}");
                return StatusCode(500, "Error interno al procesar el documento.");
            }
        }

        [HttpGet("acuse/{factura}")]
        public async Task<IActionResult> DescargarAcuse(string factura)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var acuseData = await (from s in context.FacturasScans
                                       join f in context.VwResumenFacturas on s.Factura equals f.Factura
                                       join c in context.Clientes on f.Cliente equals c.Numero
                                       where s.Factura == factura
                                       select new
                                       {
                                           Scan1 = s.Scan1,
                                           RfcReceptor = c.Rfc
                                       }).FirstOrDefaultAsync();

                if (acuseData == null || acuseData.Scan1 == null)
                    return NotFound("El acuse no está disponible.");

                bool esAdmin = User.IsInRole("Administrador");
                string rfcActual = User.Identity?.Name ?? "";

                if (!esAdmin && !string.Equals(acuseData.RfcReceptor, rfcActual, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                return File(acuseData.Scan1, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al encontrar el acuse {factura}");
                return StatusCode(500, "Error interno al procesar el documento.");
            }
        }

        [HttpGet("pedido/{folio}")]
        public async Task<IActionResult> DescargarPedidoPdf(string folio)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var header = await context.VwResumenPedidos.FirstOrDefaultAsync(p => p.Pedido == folio);

                var detalles = await context.VwPedidosDetalles.Where(d => d.Pedido == folio).ToListAsync();

                if (header == null) return NotFound("Pedido no encontrado.");

                bool esAdmin = User.IsInRole("Administrador");
                string rfcActual = User.Identity?.Name ?? "";
                if (!esAdmin && !string.Equals(header.ClienteRFC, rfcActual, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                var document = new PedidoDocument(header, detalles);
                 
                byte[] pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Pedido_{folio}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al generar PDF del Pedido {folio}");
                return StatusCode(500, "Error interno al procesar el documento.");
            }
        }
    }
}