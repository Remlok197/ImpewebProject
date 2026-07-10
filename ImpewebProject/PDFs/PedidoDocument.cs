using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ImpewebProject.Models;

namespace ImpewebProject.Pdfs
{
    public class PedidoDocument : IDocument
    {
        private readonly VwResumenPedido _header;
        private readonly List<VwDetallePedido> _detalles;

        public PedidoDocument(VwResumenPedido header, List<VwDetallePedido> detalles)
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

                row.RelativeItem(4).AlignCenter().Column(col =>
                {
                    col.Item().Text("IMPEWEB SOLUCIONES SA DE CV").Bold().FontSize(10);
                    col.Item().Text("RFC: ISO150212C50").Bold();
                    col.Item().Text("Blvd. Chichimecas 210\nCol. Las Bugambilias\nC.P. 37270\nLeón, Gto. Mex.\nTELS (477) 7 11 44 86 - 7 11 05 91");
                });

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
                column.Item().Background("#0066B3").PaddingHorizontal(8).PaddingVertical(4).Row(row =>
                {
                    row.RelativeItem().Text($"Pedido No. {_header.Pedido ?? "S/N"}").FontColor(Colors.White).Bold().FontSize(9);
                    row.RelativeItem().AlignRight().Text($"Fecha: {_header.Fecha?.ToString("dd/MM/yyyy")}    Tipo: {_header.Tipo}    Plazo: {_header.Plazo ?? 0} días").FontColor(Colors.White).FontSize(8);
                });

                column.Item().BorderHorizontal(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).Row(row =>
                {
                    row.RelativeItem(6).Column(col =>
                    {
                        col.Item().Text(t => { t.Span("Cliente: ").Bold(); t.Span($"{_header.ClienteNombre} ( {_header.NumeroCliente} )"); });
                        col.Item().Text(t => { t.Span("RFC: ").Bold(); t.Span(_header.ClienteRFC ?? "S/R"); });
                        col.Item().Text(t => { t.Span("Obs: ").Bold(); t.Span(_header.Obs ?? "Sin observaciones."); });
                    });
                    row.RelativeItem(4).Column(col =>
                    {
                        col.Item().AlignRight().Text(t => { t.Span("Referencia: ").Bold(); t.Span(_header.Referencia ?? "ORDEN EN MXN"); });

                        if (!string.IsNullOrEmpty(_header.FacturaRelacionada))
                        {
                            col.Item().AlignRight().Text(t => { t.Span("Factura Vinculada: ").Bold().FontColor("#0066B3"); t.Span(_header.FacturaRelacionada).Bold(); });
                        }
                    });
                });

                column.Item().PaddingTop(10).Element(ComposeTable);

                column.Item().PaddingTop(10).Element(ComposeTotalsAndStatus);

                column.Item().PaddingTop(15).Element(ComposeTermsAndConditions);
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1); // Código
                    columns.RelativeColumn(6); // Descripción
                    columns.RelativeColumn(1);   // Unidad
                    columns.RelativeColumn(1);   // Cantidad
                    columns.RelativeColumn(1); // Precio
                    columns.RelativeColumn(1); // Total
                });

                table.Header(header =>
                {
                    header.Cell().Background("#0066B3").Padding(3).Text("Código").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).Text("Descripción").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).Text("Uda").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).AlignRight().Text("Cant.").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).AlignRight().Text("Precio").FontColor(Colors.White).Bold();
                    header.Cell().Background("#0066B3").Padding(3).AlignRight().Text("Total").FontColor(Colors.White).Bold();
                });

                foreach (var item in _detalles)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(item.Codigo).Bold();
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(item.Descripcion).WrapAnywhere();
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(item.Unidad ?? "PZ");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).AlignRight().Text(item.Cantidad?.ToString("N2"));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).AlignRight().Text(item.Precio?.ToString("N2"));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).AlignRight().Text(item.Total?.ToString("N2")).Bold();
                }
            });
        }

        void ComposeTotalsAndStatus(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem(3).Column(col =>
                {
                    col.Item().PaddingTop(15).Text(t => { t.Span("Estado de la Orden: ").Bold(); t.Span(_header.Status ?? "PROCESO").Underline(); });

                    if (!string.IsNullOrEmpty(_header.MotivoBaja))
                    {
                        col.Item().PaddingTop(2).Text(t => { t.Span("Motivo Cancelación: ").Bold().FontColor(Colors.Red.Medium); t.Span(_header.MotivoBaja); });
                    }
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
                col.Item().Background("#0066B3").PaddingVertical(3).AlignCenter().Text("Términos y condiciones de Suministro").FontColor(Colors.White).Bold().FontSize(9);
                col.Item().PaddingTop(4).Column(list =>
                {
                    string[] terminos = new[]
                    {
                        "Los tiempos de entrega corren a partir de la liberación de la orden y confirmación de pago.",
                        "Toda mercancía viaja por cuenta y riesgo del comprador.",
                        "Las cancelaciones de pedidos de línea causarán penalizaciones del 20% sobre el valor total.",
                        "Para reclamaciones por faltantes o daños físicos en el envío, cuenta con 24 horas naturales tras recibir el paquete.",
                        "Los precios pactados en esta orden no están sujetos a cambios salvo variaciones cambiarias extremas.",
                        "Para dudas sobre el surtido de este pedido, contacte directamente a su agente asignado."
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