using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ImpewebProject.Models;

namespace ImpewebProject.Pdfs
{
    public class PagoDocument : IDocument
    {
        private readonly VwPagoPdfHeader _header;
        private readonly List<VwDetallePago> _detalles;
        private readonly byte[]? _qrImagen;

        public PagoDocument(VwPagoPdfHeader header, List<VwDetallePago> detalles, byte[]? qrImagen = null)
        {
            _header = header;
            _detalles = detalles;
            _qrImagen = qrImagen;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.Letter);
                page.PageColor(Colors.White);

                // Tipografía pequeña ideal para el desglose de impuestos del SAT
                page.DefaultTextStyle(x => x.FontSize(7.5f).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(10).Row(row =>
            {
                // 1. Logo izquierdo (Cambiamos Height por MaxHeight y aseguramos FitArea)
                row.RelativeItem(3).MaxHeight(60).Image("wwwroot/img/logo amplify.jpg").FitArea();
                row.RelativeItem(3).MaxHeight(60).Image("wwwroot/img/hp green lake 2.jpg").FitArea();

                // 2. Textos del Centro
                row.RelativeItem(5).PaddingLeft(10).AlignCenter().Column(col =>
                {
                    col.Item().Text("IMPEWEB SOLUCIONES SA DE CV").Bold().FontSize(9);
                    col.Item().Text("Blvd. Chichimecas 210\nCol. Las Bugambilias\nC.P. 37270\nLeón, Gto. Mex.\nTELS (477) 7 11 44 86 - 7 11 05 91\nRFC: ISO150212C50");
                });

                // 3. Logo derecho (Cambiamos Height por MaxHeight y aseguramos FitArea)
                row.RelativeItem(3).MaxHeight(60).AlignRight().Image("wwwroot/img/logo impeweb.jpg").FitArea();
            });
        }

        void ComposeContent(IContainer container)
        {
            // Evaluamos si el documento está timbrado ante el SAT
            bool esTimbrado = !string.IsNullOrEmpty(_header.UUID);

            container.PaddingVertical(8).Column(column =>
            {
                // 1. Datos Generales y Cuadro de Folio (Rojo / Gris)
                column.Item().Row(row =>
                {
                    // Bloque Datos Cliente
                    row.RelativeItem(7).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Column(col =>
                    {
                        var addressParts = new[] { _header.Calle, _header.noExterior, _header.noInterior, _header.Colonia, _header.Municipio, _header.Estado, $"C.P. {_header.CodigoPostal}" };
                        string fullAddress = string.Join(", ", addressParts.Where(p => !string.IsNullOrWhiteSpace(p)));

                        col.Item().Text(t => { t.Span("Nombre: ").Bold(); t.Span(_header.ClienteNombre ?? ""); });
                        col.Item().Text(t => { t.Span("Dirección: ").Bold(); t.Span(fullAddress.ToUpper()); });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text(t => { t.Span("Emisor RFC: ").Bold(); t.Span(_header.EmisorRfc ?? "ISO150212C50"); });
                            r.RelativeItem().Text(t => { t.Span("Receptor RFC: ").Bold(); t.Span(_header.ReceptorRfc ?? ""); });
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text(t => { t.Span("Fecha Pago: ").Bold(); t.Span(_header.FechaPago?.ToString("dd/MM/yyyy") ?? ""); });
                            r.RelativeItem().Text(t => { t.Span("Forma de Pago: ").Bold(); t.Span(_header.FormaPago ?? ""); });
                            r.RelativeItem().Text(t => { t.Span("Régimen Receptor: ").Bold(); t.Span(_header.RegimenFiscalReceptor ?? "603"); });
                        });
                    });

                    // Bloque Folio (Idéntico a tu captura)
                    row.RelativeItem(3).PaddingLeft(5).Column(col =>
                    {
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text("PAGO").Bold().FontSize(9);
                        col.Item().BorderHorizontal(1).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignCenter().Text(_header.Pago ?? "S/N").FontColor(Colors.Red.Medium).Bold().FontSize(11);

                        if (esTimbrado)
                        {
                            col.Item().PaddingTop(2).AlignCenter().Text($"Emitida: {_header.FechaTimbrado?.ToString("dd/MM/yyyy hh:mm:ss tt")}").FontSize(6);
                        }
                    });
                });

                // 2. Tabla Principal: Documentos Relacionados
                column.Item().PaddingTop(8).Text("UUID del Documento / Facturas Relacionadas").Bold().FontSize(8).FontColor("#0066B3");
                column.Item().Element(ComposeDocumentsTable);

                // 3. Tabla Secundaria: Impuestos de los documentos relacionados
                column.Item().PaddingTop(6).Text("Impuestos Traslados DR").Bold().FontSize(7).FontColor(Colors.Grey.Darken2);
                column.Item().Element(ComposeTaxTable);

                // 4. Totales e Importe con letra
                column.Item().PaddingTop(8).Element(ComposeTotalsSection);

                // 5. Bloque Digital del SAT (Solo si está timbrado)
                if (esTimbrado)
                {
                    column.Item().PaddingTop(10).Element(ComposeSatInfo);
                }
                else
                {
                    column.Item().PaddingTop(15).Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(10).AlignCenter().Text("DOCUMENTO INTERNO NO TIMBRADO (COMPROBANTE AUXILIAR DE COBRANZA)").Bold().FontColor(Colors.Grey.Darken1);
                }
            });
        }

        void ComposeDocumentsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3.5f); // UUID / ID Documento
                    columns.RelativeColumn(1.2f); // Factura
                    columns.RelativeColumn(1.2f); // Fecha
                    columns.RelativeColumn(0.8f); // Plazo
                    columns.RelativeColumn(1.2f); // Vence
                    columns.RelativeColumn(1.2f); // Total
                    columns.RelativeColumn(0.6f); // Parc
                    columns.RelativeColumn(1.3f); // ImpSaldoAnt
                    columns.RelativeColumn(1.3f); // ImpPagado
                    columns.RelativeColumn(1.3f); // ImpSaldoInsoluto
                });

                table.Header(header =>
                {
                    string[] titles = { "UUID del Documento", "Factura", "Fecha", "Plazo", "Vence", "Total", "Parc", "ImpSaldoAnt", "ImpPagado", "ImpSaldoInsoluto" };
                    foreach (var title in titles)
                    {
                        var cell = header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(2);
                        if (title.StartsWith("Imp") || title == "Total") cell.AlignRight().Text(title).Bold().FontSize(6.5f);
                        else if (title == "Parc" || title == "Plazo") cell.AlignCenter().Text(title).Bold().FontSize(6.5f);
                        else cell.Text(title).Bold().FontSize(6.5f);
                    }
                });

                foreach (var item in _detalles)
                {
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.UuidDocumento ?? "NO TIMBRADA").FontSize(6);
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.Factura);
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.FechaFactura?.ToString("dd/MM/yyyy"));
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignCenter().Text(item.Plazo?.ToString());
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.VenceFactura?.ToString("dd/MM/yyyy"));
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignRight().Text(item.TotalFactura?.ToString("N2"));
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignCenter().Text(item.NumParcialidad?.ToString());
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignRight().Text(item.ImpSaldoAnt?.ToString("N2"));
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignRight().Text(item.ImpPagado?.ToString("N2"));
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignRight().Text(item.ImpSaldoInsoluto?.ToString("N2"));
                }
            });
        }

        void ComposeTaxTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Base
                    columns.RelativeColumn(2); // Impuesto
                    columns.RelativeColumn(2); // TipoFactor
                    columns.RelativeColumn(2); // TasaOCuota
                    columns.RelativeColumn(2); // Importe
                });

                table.Header(header =>
                {
                    string[] titles = { "Base", "Impuesto", "TipoFactor", "TasaOCuota", "Importe" };
                    foreach (var title in titles)
                    {
                        var cell = header.Cell().Background(Colors.Grey.Lighten4).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1);
                        if (title == "Base" || title == "Importe") cell.AlignRight().Text(title).Bold().FontSize(6);
                        else cell.AlignCenter().Text(title).Bold().FontSize(6);
                    }
                });
                foreach (var item in _detalles)
                {
                    decimal pagado = item.ImpPagado ?? 0;
                    decimal baseImp = Math.Round(pagado / 1.16m, 2);
                    decimal ivaImp = pagado - baseImp;

                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1).AlignRight().Text(baseImp.ToString("N2")).FontSize(6);
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1).AlignCenter().Text("002-IVA").FontSize(6);
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1).AlignCenter().Text("Tasa").FontSize(6);
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1).AlignCenter().Text("0.16").FontSize(6);
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1).AlignRight().Text(ivaImp.ToString("N2")).FontSize(6);
                }
            });
        }

        void ComposeTotalsSection(IContainer container)
        {
            decimal totalAbonos = _detalles.Sum(x => x.ImpPagado ?? 0);
            string letras = FacturaDocument.NumeroALetras(totalAbonos);

            container.Column(col =>
            {
                col.Item().AlignRight().Text(t =>
                {
                    t.Span("Total $ ").Bold();
                    t.Span(totalAbonos.ToString("N2")).Bold();
                });

                col.Item().PaddingTop(4).BorderHorizontal(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_header.Anotacion ?? "").Bold().FontColor(Colors.Blue.Darken3);
                        c.Item().Text($"1.0000 MXN    {letras}").FontSize(7);
                    });
                });
            });
        }

        void ComposeSatInfo(IContainer container)
        {
            container.Row(row =>
            {
                row.ConstantItem(100).Height(100).Column(c => {
                    if (_qrImagen != null) c.Item().Image(_qrImagen).FitArea();
                });

                row.RelativeItem().PaddingLeft(12).Column(col =>
                {
                    col.Item().Text(t => { t.Span("Sello digital CFDI - UUID: ").Bold(); t.Span(_header.UUID ?? ""); });
                    col.Item().Text(_header.SelloDigitalCFDI ?? "").FontSize(5);

                    col.Item().PaddingTop(3).Text(t => { t.Span("Sello digital SAT - Certificado SAT: ").Bold(); t.Span(_header.CertificadoSAT ?? ""); });
                    col.Item().Text(_header.SelloDigitalSAT ?? "").FontSize(5);

                    col.Item().PaddingTop(3).Text("Cadena Original del Complemento de Certificación Digital del SAT").Bold().FontSize(6.5f);
                    col.Item().Text(_header.CadenaOriginal ?? "").FontSize(5);

                    col.Item().PaddingTop(4).AlignCenter().Text("La reproducción no autorizada de este comprobante constituye un delito en los términos de las disposiciones fiscales").FontSize(5.5f).Italic();
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Height(35).AlignCenter().AlignMiddle().Image("wwwroot/img/pie de pagina.jpeg").FitArea();
        }
    }
}