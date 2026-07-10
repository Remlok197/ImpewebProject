using ImpewebProject.Data;
using ImpewebProject.Pdfs;
using ImpewebProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QuestPDF.Fluent;

namespace ImpewebProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IDbContextFactory<ImpewebContext> _contextFactory;
        private readonly IFacturaService _facturaService;
        private readonly ICotizacionService _cotizacionService;
        private readonly IComplementosPagoService _complementosPagoService;

        public PdfController(IFacturaService facturaService, ICotizacionService cotizacionService, IComplementosPagoService complementosPagoService, IDbContextFactory<ImpewebContext> contextFactory)
        {
            _facturaService = facturaService;
            _cotizacionService = cotizacionService;
            _complementosPagoService = complementosPagoService;
            _contextFactory = contextFactory;
        }

        [HttpGet("factura/{folio}")]
        public async Task<IActionResult> DescargarFacturaPdf(string folio)
        {
            var header = await _facturaService.ObtenerPdfAsync(folio);
            var detalles = await _facturaService.ObtenerDetallesPorFacturaAsync(folio);

            if (header == null) return NotFound($"No se encontró la factura con folio: {folio}");

            // 1. Armar la URL oficial del SAT para el QR
            string sello8 = header.SelloDigitalCFDI?.Length >= 8
                ? header.SelloDigitalCFDI.Substring(header.SelloDigitalCFDI.Length - 8)
                : "";
            string totalFmt = header.Total?.ToString("0.000000") ?? "0.000000";
            string urlSat = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id={header.UUID}&re={header.EmisorRfc}&rr={header.ReceptorRfc}&tt={totalFmt}&fe={sello8}";

            // 2. Generar el código QR como Imagen (Arreglo de Bytes)
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(urlSat, QRCoder.QRCodeGenerator.ECCLevel.M);
            using var qrCode = new QRCoder.PngByteQRCode(qrData);
            byte[] qrImagenBytes = qrCode.GetGraphic(20);

            // 3. Le pasamos TODO al documento (incluyendo la imagen del QR)
            var documento = new FacturaDocument(header, detalles, qrImagenBytes);

            byte[] pdfBytes = documento.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Factura_{folio}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("cotizacion/{folio}")]
        public async Task<IActionResult> DescargarCotizacionPdf(string folio)
        {
            var header = await _cotizacionService.ObtenerPdfAsync(folio);
            var detalles = await _cotizacionService.ObtenerDetallesPorCotizacionAsync(folio);

            if (header == null) return NotFound($"No se encontró la cotización: {folio}");

            var documento = new CotizacionDocument(header, detalles);
            byte[] pdfBytes = documento.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Cotizacion_{folio}.pdf");
        }

        [HttpGet("pago/{folio}")]
        public async Task<IActionResult> DescargarPagoPdf(string folio)
        {
            QuestPDF.Settings.EnableDebugging = true;
            var header = await _complementosPagoService.ObtenerPdfAsync(folio);
            var detalles = await _complementosPagoService.ObtenerDetallesPorPagoAsync(folio);

            if (header == null) return NotFound($"No se encontró el registro de pago: {folio}");

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

        [HttpGet("cartaporte/{folio}")]
        public async Task<IActionResult> DescargarCartaPortePdf(string folio)
        {
            using var context = _contextFactory.CreateDbContext(); 

            var header = await context.VwCartaPortePdfHeader.AsNoTracking().FirstOrDefaultAsync(x => x.CartaPorte == folio);
            if (header == null) return NotFound("No se encontró la Carta Porte");

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

        [HttpGet("acuse/{factura}")]
        public async Task<IActionResult> DescargarAcuse(string factura)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var acuse = await context.FacturasScans
                                          .FirstOrDefaultAsync(s => s.Factura == factura);

                if (acuse == null || acuse.Scan1 == null)
                {
                    return NotFound("El acuse de recibo no está disponible para esta factura.");
                }

                return File(acuse.Scan1, "application/pdf");
            }
            catch (Exception ex)
            {
                return Content($"Ocurrió un error al obtener el acuse: {ex.Message}");
            }
        }

        [HttpGet("pedido/{folio}")]
        public async Task<IActionResult> DescargarPedidoPdf(string folio)
        {
            using var context = _contextFactory.CreateDbContext();
            var header = await context.VwResumenPedidos.FirstOrDefaultAsync(p => p.Pedido == folio);

            var detalles = await context.VwPedidosDetalles.Where(d => d.Pedido == folio).ToListAsync();

            if (header == null) return NotFound("Pedido no encontrado.");

            var document = new PedidoDocument(header, detalles);

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Pedido_{folio}.pdf");
        }
    }
}