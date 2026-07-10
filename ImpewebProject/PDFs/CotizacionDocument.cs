using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ImpewebProject.Models;

namespace ImpewebProject.Pdfs
{
    public class CotizacionDocument : IDocument
    {
        private readonly VwCotizacionPdfHeader _header;
        private readonly List<VwDetalleCotizacion> _detalles;

        public CotizacionDocument(VwCotizacionPdfHeader header, List<VwDetalleCotizacion> detalles)
        {
            _header = header;
            _detalles = detalles;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.Letter);
                page.PageColor(Colors.White);

                page.DefaultTextStyle(x => x.FontSize(8).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem(2).Column(col =>
                {
                    col.Item().Height(50).Image("wwwroot/img/logo amplify.jpg").FitArea();
                    col.Item().Height(50).Image("wwwroot/img/hp green lake 2.jpg").FitArea();
                });

                // Columna 2: Datos de Impeweb
                row.RelativeItem(4).AlignCenter().Column(col =>
                {
                    col.Item().Text("IMPEWEB SOLUCIONES SA DE CV").Bold().FontSize(10);
                    col.Item().Text("RFC: ISO150212C50").Bold();
                    col.Item().Text("Blvd. Chichimecas 210\nCol. Las Bugambilias\nC.P. 37270\nLeón, Gto. Mex.\nTELS (477) 7 11 44 86 - 7 11 05 91");
                });

                // Columna 3: Logo de Impeweb Derecho
                row.RelativeItem(3).Column(col =>
                {
                    col.Item().Height(50).Image("wwwroot/img/impeweb.jpg").FitArea();
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // 1. Barra de Título Azul con Fechas
                column.Item().Background("#0066B3").PaddingHorizontal(8).PaddingVertical(4).Row(row =>
                {
                    row.RelativeItem().Text($"Cotización No. {_header.Cotizacion ?? "S/N"}").FontColor(Colors.White).Bold().FontSize(9);
                    row.RelativeItem().AlignRight().Text($"Fecha: {_header.Fecha?.ToString("dd/MM/yyyy")}    Vigencia: {_header.Vigencia ?? 0} días    Vence: {_header.FechaVencimiento?.ToString("dd/MM/yyyy")}").FontColor(Colors.White).FontSize(8);
                });

                // 2. Datos del Cliente y Atención
                column.Item().BorderHorizontal(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).Row(row =>
                {
                    row.RelativeItem(5).Column(col =>
                    {
                        col.Item().Text(t => { t.Span("Cliente: ").Bold(); t.Span($"{_header.ClienteNombre} ( {_header.ClienteNumero} )"); });
                        col.Item().Text(t => { t.Span("Le atendió: ").Bold(); t.Span(_header.AgenteNombre ?? "SIN AGENTE"); });
                        col.Item().Text(t => { t.Span("Obs: ").Bold(); t.Span(_header.Observaciones ?? ""); });
                    });
                    row.RelativeItem(5).Column(col =>
                    {
                        col.Item().AlignRight().Text(t => { t.Span("Referencia: ").Bold(); t.Span(_header.Referencia ?? "COTIZACION EN MXN"); });
                    });
                });

                // 3. Tabla de Partidas
                column.Item().PaddingTop(10).Element(ComposeTable);

                // 4. Totales e Iniciales de Elaboración
                column.Item().PaddingTop(10).Element(ComposeTotalsAndSign);

                // 5. Términos y Condiciones
                column.Item().PaddingTop(15).Element(ComposeTermsAndConditions);
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1); // Cantidad
                    columns.RelativeColumn(6); // Descripción
                    columns.RelativeColumn(2); // Precio
                    columns.RelativeColumn(1); // Dcto %
                    columns.RelativeColumn(2); // SubTotal
                });

                table.Header(header =>
                {
                    header.Cell().Background("#0066B3").Padding(3).Text("Cantidad").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).Text("Descripción").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).AlignRight().Text("Precio").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).AlignRight().Text("Dcto %").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).AlignRight().Text("SubTotal").FontColor(Colors.White).Bold();
                });

                foreach (var item in _detalles)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(item.Cantidad?.ToString("N2"));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(item.Descripcion).WrapAnywhere();
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).AlignRight().Text(item.Precio?.ToString("N2"));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).AlignRight().Text(item.Dcto?.ToString("N2"));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).AlignRight().Text(item.SubTotal?.ToString("N2"));
                }
            });
        }

        void ComposeTotalsAndSign(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem(3).Column(col =>
                {
                    col.Item().PaddingTop(20).Width(150).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                    col.Item().PaddingTop(2).Text("Elaboró Cotización").FontSize(7).FontColor(Colors.Grey.Darken1);
                    col.Item().Text(_header.Elaboro ?? "S/U").Bold().FontSize(8);
                });

                row.RelativeItem(2).Column(col =>
                {
                    col.Item().Row(r => {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text("Descuento $");
                        r.RelativeItem().AlignRight().Text(_header.Descuento?.ToString("N2") ?? "0.00").Bold();
                    });
                    col.Item().Row(r => {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text("SubTotal $");
                        r.RelativeItem().AlignRight().Text(_header.SubTotal?.ToString("N2") ?? "0.00").Bold();
                    });
                    col.Item().Row(r => {
                        r.RelativeItem().PaddingRight(5).AlignRight().Text("IVA $");
                        r.RelativeItem().AlignRight().Text(_header.Iva?.ToString("N2") ?? "0.00").Bold();
                    });
                    col.Item().Row(r => {
                        r.RelativeItem().Background("#0066B3").PaddingRight(5).AlignRight().Text("Total $").FontColor(Colors.White).Bold();
                        r.RelativeItem().Background("#0066B3").PaddingLeft(2).PaddingRight(2).AlignRight().Text(_header.Total?.ToString("N2") ?? "0.00").FontColor(Colors.White).Bold();
                    });
                });
            });
        }

        void ComposeTermsAndConditions(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Background("#0066B3").PaddingVertical(3).AlignCenter().Text("Términos y condiciones").FontColor(Colors.White).Bold().FontSize(9);
                col.Item().PaddingTop(4).Column(list =>
                {
                    string[] terminos = new[]
                    {
                        "Las compras en dólares serán pagadas en dólares.",
                        "Cualquier devolución, causará hasta un 20% de cargo sobre el precio del producto.",
                        "Las garantías son otorgadas por los fabricantes; son validadas de acuerdo con su póliza de garantía y son efectuadas en los centros de servicio autorizados por el fabricante directo.",
                        "No contamos con garantías en consumibles de cómputo.",
                        "Los precios no incluyen asesoría, capacitación y/o instalación de los productos.",
                        "Para defectos de fabricación en la mercancía cuenta con 2 días hábiles reportarlo con su agente de ventas.",
                        "Cualquier duda o aclaración comunicarse al teléfono: (477) 711-05-91."
                    };

                    for (int i = 0; i < terminos.Length; i++)
                    {
                        int numero = i + 1;
                        list.Item().PaddingBottom(2).Row(r =>
                        {
                            r.ConstantItem(15).Text($"{numero}.").Bold();
                            r.RelativeItem().Text(terminos[i]);
                        });
                    }
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Height(25).Image("wwwroot/img/pie de pagina.jpeg").FitArea();
        }
    }
}