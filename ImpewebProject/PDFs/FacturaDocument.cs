using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ImpewebProject.Models;

namespace ImpewebProject.Pdfs
{
    public class FacturaDocument : IDocument
    {
        private readonly VwFacturaPdfHeader _header;
        private readonly List<VwDetalleFactura> _detalles;
        private readonly byte[] _qrImagen;

        public FacturaDocument(VwFacturaPdfHeader header, List<VwDetalleFactura> detalles, byte[] qrImagen)
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
                // Columna 1: Logos Izquierdos (Puedes cargar la imagen después)
                row.RelativeItem(2).Column(col =>
                {
                    col.Item().Height(50).Image("wwwroot/img/logo amplify.jpg");
                    col.Item().Height(50).Image("wwwroot/img/hp green lake 2.jpg");
                });

                // Columna 2: Datos de la Empresa Impeweb
                row.RelativeItem(4).AlignCenter().Column(col =>
                {
                    col.Item().Text("IMPEWEB SOLUCIONES SA DE CV").Bold().FontSize(10);
                    col.Item().Text($"RFC: {_header.EmisorRfc ?? "ISO150212C50"}").Bold();
                    col.Item().Text("Blvd. Chichimecas 210\nCol. Las Bugambilias\nC.P. 37270\nLeón, Gto. Mex.\nTELS (477) 7 11 44 86 - 7 11 05 91");
                });

                // Columna 3: Cuadro Azul de Factura
                row.RelativeItem(3).Column(col =>
                {
                    col.Item().Height(40).Image("wwwroot/img/logo impeweb.jpg");
                    col.Item().PaddingTop(5).Background("#0066B3").Padding(5).AlignCenter().Column(c =>
                    {
                        c.Item().Text("FACTURA").FontColor(Colors.White).Bold().FontSize(10);
                        c.Item().Text(_header.Factura ?? "S/F").FontColor(Colors.White).Bold().FontSize(12);
                    });
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item().Element(ComposeClientDetails);
                column.Item().PaddingTop(10).Element(ComposeTable);
                column.Item().PaddingTop(10).Element(ComposeTotals);
                column.Item().PaddingTop(10).Element(ComposeSatInfo);
            });
        }

        void ComposeClientDetails(IContainer container)
        {
            var addressParts = new[] { _header.Calle, _header.noExterior, _header.noInterior, _header.Colonia, _header.Municipio, _header.Estado, _header.Pais, _header.CodigoPostal };
            string fullAddress = string.Join(", ", addressParts.Where(p => !string.IsNullOrWhiteSpace(p)));

            container.Border(1).BorderColor(Colors.Black).Padding(5).Row(row =>
            {
                row.RelativeItem(6).Column(col =>
                {
                    col.Item().Text(t => { t.Span("Nombre: ").Bold(); t.Span(_header.ReceptorNombre ?? "CLIENTE NO ENCONTRADO"); });
                    col.Item().Text(t => { t.Span("Dirección: ").Bold(); t.Span(fullAddress); });
                    col.Item().Text(t => { t.Span("Agente: ").Bold(); t.Span(_header.NombreAgente ?? "SIN AGENTE"); });
                    col.Item().Text(t => { t.Span("UsoCFDI: ").Bold(); t.Span(_header.UsoCfdi ?? ""); });
                    col.Item().Text(t => { t.Span("Régimen Receptor: ").Bold(); t.Span(_header.RegimenFiscal ?? ""); });
                });
                row.RelativeItem(4).Column(col =>
                {
                    col.Item().Text(t => { t.Span("RFC: ").Bold(); t.Span(_header.ReceptorRfc ?? ""); });
                    col.Item().Text(t => { t.Span("Plazo: ").Bold(); t.Span($"{_header.Plazo} Días"); });
                    col.Item().Text(t => { t.Span("Emisión: ").Bold(); t.Span(_header.Fecha?.ToString("dd/MM/yyyy hh:mm:ss tt") ?? ""); });
                    col.Item().Text(t => { t.Span("Vence: ").Bold(); t.Span(_header.FechaVencimiento?.ToString("dd/MM/yyyy") ?? ""); });
                    col.Item().Text(t => { t.Span("Forma Pago: ").Bold(); t.Span(_header.FormaPago ?? ""); });
                    col.Item().Text(t => { t.Span("Método Pago: ").Bold(); t.Span(_header.MetodoPago ?? ""); });
                });
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1); // Cantidad
                    columns.RelativeColumn(2); // Codigo
                    columns.RelativeColumn(5); // Descripcion
                    columns.RelativeColumn(2); // Precio
                    columns.RelativeColumn(1); // Dcto
                    columns.RelativeColumn(2); // SubTotal
                });

                table.Header(header =>
                {
                    header.Cell().BorderTop(1).BorderBottom(1).PaddingVertical(2).Text("Cantidad").Bold();
                    header.Cell().BorderTop(1).BorderBottom(1).PaddingVertical(2).Text("Código").Bold();
                    header.Cell().BorderTop(1).BorderBottom(1).PaddingVertical(2).Text("Descripción").Bold();
                    header.Cell().BorderTop(1).BorderBottom(1).PaddingVertical(2).AlignRight().Text("Precio").Bold();
                    header.Cell().BorderTop(1).BorderBottom(1).PaddingVertical(2).AlignRight().Text("Dcto %").Bold();
                    header.Cell().BorderTop(1).BorderBottom(1).PaddingVertical(2).AlignRight().Text("Importe").Bold();
                });

                foreach (var item in _detalles)
                {
                    table.Cell().PaddingVertical(2).Text(item.Cantidad?.ToString("N2"));
                    table.Cell().PaddingVertical(2).Text(item.Codigo);
                    table.Cell().PaddingVertical(2).Text(item.Descripcion);
                    table.Cell().PaddingVertical(2).AlignRight().Text(item.Precio?.ToString("N2"));
                    table.Cell().PaddingVertical(2).AlignRight().Text(item.Dcto?.ToString("N2"));
                    table.Cell().PaddingVertical(2).AlignRight().Text(item.Importe?.ToString("N2"));
                }
            });
        }

        void ComposeTotals(IContainer container)
        {
            container.Row(row =>
            {
                decimal totalFactura = _header.Total ?? 0;
                string totalConLetra = NumeroALetras(totalFactura);

                row.RelativeItem(3).PaddingTop(15).Text(totalConLetra).Bold();

                row.RelativeItem(2).Column(col =>
                {
                    col.Item().Row(r => {
                        r.RelativeItem().Background("#0066B3").PaddingRight(5).AlignRight().Text("SubTotal $").FontColor(Colors.White).Bold();
                        r.RelativeItem().AlignRight().Text(_header.SubTotal?.ToString("N2") ?? "0.00").Bold();
                    });
                    col.Item().Row(r => {
                        r.RelativeItem().Background("#0066B3").PaddingRight(5).AlignRight().Text("Iva $").FontColor(Colors.White).Bold();
                        r.RelativeItem().AlignRight().Text(_header.Iva?.ToString("N2") ?? "0.00").Bold();
                    });
                    col.Item().Row(r => {
                        r.RelativeItem().Background("#0066B3").PaddingRight(5).AlignRight().Text("Total $").FontColor(Colors.White).Bold();
                        r.RelativeItem().AlignRight().Text(_header.Total?.ToString("N2") ?? "0.00").Bold();
                    });
                });
            });
        }

        void ComposeSatInfo(IContainer container)
        {
            container.Row(row =>
            {
                row.ConstantItem(110).Column(c => {
                    if (_qrImagen != null && _qrImagen.Length > 0)
                    {
                        c.Item().Image(_qrImagen);
                    }
                });
                row.RelativeItem().PaddingLeft(15).Column(col =>
                {
                    col.Item().Text(t =>
                    {
                        t.Span("Sello digital CFDI").Bold().FontSize(7);
                        t.Span($"               UUID: {_header.UUID ?? ""}").Bold().FontSize(7);
                    });
                    col.Item().Text(_header.SelloDigitalCFDI ?? "").FontSize(5);

                    col.Item().PaddingTop(4).Text(t =>
                    {
                        t.Span("Sello digital SAT").Bold().FontSize(7);
                        t.Span($"                Certificado SAT: {_header.CertificadoSAT ?? ""}").Bold().FontSize(7);
                    });
                    col.Item().Text(_header.SelloDigitalSAT ?? "").FontSize(5);

                    col.Item().PaddingTop(4).Text("Cadena Original del Complemento de Certificación Digital del SAT").Bold().FontSize(7);
                    col.Item().Text(_header.CadenaOriginal ?? "").FontSize(5);
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            string fechaVencimiento = _header.FechaVencimiento?.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("es-MX")) ?? "";
            string fechaEmision = _header.Fecha?.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("es-MX")) ?? "";
            string totalNumerico = _header.Total?.ToString("N2") ?? "0.00";
            string totalConLetras = NumeroALetras(_header.Total ?? 0);

            var domParte1 = new[] { _header.Calle, _header.noExterior, _header.noInterior, _header.Colonia };
            string domicilio = string.Join(", ", domParte1.Where(p => !string.IsNullOrWhiteSpace(p)));

            var domParte2 = new[] { _header.Municipio, _header.Estado, _header.Pais, $"C.P. {_header.CodigoPostal}" };
            string ciudad = string.Join(", ", domParte2.Where(p => !string.IsNullOrWhiteSpace(p)));

            container.Column(col =>
            {
                col.Item().Border(1).Padding(4).Column(p =>
                {
                    p.Item().Background(Colors.Grey.Lighten3).Text($"PAGARÉ No. {_header.Factura}").Bold().FontSize(7);

                    p.Item().PaddingTop(2).Text($"Por el presente pagaré reconozco(emos) deber y me(nos) obligo(amos) a " +
                        $"pagar en esta ciudad o en cualquier otra en que se me(nos) requiera de pago a IMPEWEB SOLUCIONES " +
                        $"o a su orden el día de su vencimiento {fechaVencimiento}, la cantidad de {totalNumerico} ({totalConLetras}). " +
                        $"Valor recibido a mi(nuestra) entera satisfacción.\n" +
                        $"La cantidad que ampara este pagaré es parte de la cantidad mayor, " +
                        $"por la cual se otorgan otros pagarés con vencimientos posteriores y queda expresamente convenido que si " +
                        $"no es pagado este documento precisamente a su vencimiento, se dará por vencidos anticipadamente los demás " +
                        $"pagarés a los que se refiere esta cláusula. Este pagaré es mercantil y está regido por la ley general de " +
                        $"Títulos y Operaciones de Crédito en su artículo 173 parte final y demás artículos correlativos. " +
                        $"De no verificarse el pago de la cantidad que este pagaré expresa el día de su vencimiento, abonaré(mos) el " +
                        $"crédito de 6% mensual por todo tiempo que esté insoluto, sin prejuicio al cobro más los gastos que por " +
                        $"ellos se originen. Así mismo el otorgante se obliga en los términos del presente pagaré, por la persona que " +
                        $"los suscriba, basta que quien lo firme, sea trabajador o dependiente laboral y se tendrá como si lo suscribiera " +
                        $"el presente legal o dueño de la empresa otorgante.")
                        .FontSize(5);

                    p.Item().PaddingTop(6).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(40); 
                            c.RelativeColumn();   
                            c.RelativeColumn();   
                        });

                        t.Cell().Text("Otorgante:").FontSize(6);
                        t.Cell().ColumnSpan(2).Text(_header.ReceptorNombre ?? "").FontSize(6);

                        t.Cell().Text("Domicilio:").FontSize(6);
                        t.Cell().ColumnSpan(2).Text(domicilio.ToUpper()).FontSize(6);

                        t.Cell().Text("Ciudad:").FontSize(6);
                        t.Cell().Text(ciudad.ToUpper()).FontSize(6);
                        t.Cell().AlignRight().Text($"León, Gto. a {fechaEmision}").FontSize(6);
                    });

                    p.Item().PaddingTop(10).AlignCenter().Column(firma =>
                    {
                        firma.Item().Width(150).BorderBottom(1).BorderColor(Colors.Black);
                        firma.Item().PaddingTop(2).AlignCenter().Text("Firma").FontSize(6);
                    });
                });

                col.Item().Image("wwwroot/img/pie de pagina.jpeg");
            });
        }

        public static string NumeroALetras(decimal numero)
        {
            string[] unos = { "", "UN", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
            string[] dieces = { "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISEIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
            string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
            string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };

            long entero = (long)Math.Truncate(numero);
            int centavos = (int)Math.Round((numero - entero) * 100);

            string centavosTexto = $"{centavos:00}/100 M.N.";
            if (entero == 0) return $"CERO PESOS {centavosTexto}";
            if (entero == 1) return $"UN PESO {centavosTexto}";

            string ConvertirTresDigitos(int n)
            {
                string c = centenas[n / 100];
                string d = decenas[(n % 100) / 10];
                string u = unos[n % 10];

                if (n / 100 == 1 && n % 100 == 0) c = "CIEN";
                if ((n % 100) >= 10 && (n % 100) < 20)
                {
                    d = dieces[(n % 100) - 10];
                    u = "";
                }
                else if (n % 100 > 20 && (n % 10) > 0)
                {
                    d += " Y ";
                }
                else if (n % 100 == 20)
                {
                    d = "VEINTE";
                }
                else if (n % 100 > 20 && n % 100 < 30)
                {
                    d = "VEINTI" + unos[n % 10];
                    u = "";
                }

                return $"{c} {d}{u}".Replace("  ", " ").Trim();
            }

            string resultado = "";
            int millones = (int)(entero / 1000000);
            int miles = (int)((entero % 1000000) / 1000);
            int unidades = (int)(entero % 1000);

            if (millones > 0)
            {
                resultado += millones == 1 ? "UN MILLÓN " : $"{ConvertirTresDigitos(millones)} MILLONES ";
            }
            if (miles > 0)
            {
                resultado += miles == 1 ? "UN MIL " : $"{ConvertirTresDigitos(miles)} MIL ";
            }
            if (unidades > 0)
            {
                resultado += ConvertirTresDigitos(unidades);
            }

            resultado = resultado.Replace("UN MIL", "MIL").Trim();
            return $"{resultado} PESOS {centavosTexto}";
        }
    }
}