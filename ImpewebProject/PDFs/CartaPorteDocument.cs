using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ImpewebProject.Models;

namespace ImpewebProject.Pdfs
{
    public class CartaPorteDocument : IDocument
    {
        private readonly VwCartaPortePdfHeader _header;
        private readonly List<VwCartaPortePdfUbicacion> _ubicaciones;
        private readonly List<VwCartaPortePdfMercancia> _mercancias;
        private readonly List<VwCartaPortePdfRemolque> _remolques;
        private readonly List<VwCartaPortePdfFigura> _figuras;
        private readonly byte[]? _qrImagen;
        private readonly string ColorBarra = "#0066B3";

        public CartaPorteDocument(VwCartaPortePdfHeader h, List<VwCartaPortePdfUbicacion> u, List<VwCartaPortePdfMercancia> m, List<VwCartaPortePdfRemolque> r, List<VwCartaPortePdfFigura> f, byte[]? qr)
        {
            _header = h; _ubicaciones = u; _mercancias = m; _remolques = r; _figuras = f; _qrImagen = qr;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.Letter);
                page.DefaultTextStyle(x => x.FontSize(7.5f).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(10).Row(row =>
            {
                row.RelativeItem(3).Height(60).Image("wwwroot/img/impeweb.jpg");
                row.RelativeItem(4).PaddingLeft(10).Column(col =>
                {
                    col.Item().Text("IMPEWEB SOLUCIONES SA DE CV").Bold().FontSize(9);
                    col.Item().Text("Blvd. Chichimecas 210\nCol. Las Bugambilias\nC.P. 37270\nLeón, Gto. Mex.\nTELS (477) 7 11 44 86 - 7 11 05 91\nRFC: ISO150212C50");
                });
                row.RelativeItem(5).Column(col =>
                {
                    col.Item().Background(ColorBarra).PaddingVertical(3).AlignCenter().Text("CartaPorte de Traslado").FontColor(Colors.White).Bold().FontSize(9);
                    col.Item().PaddingTop(3).AlignCenter().Text($"{_header.Serie ?? ""}{_header.Folio}").FontSize(8);
                    col.Item().PaddingTop(3).AlignCenter().Text(_header.FechaTimbrado ?? "").FontSize(8);
                    col.Item().PaddingTop(3).AlignCenter().Text(_header.UUID ?? "").FontSize(8);
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.Column(col =>
            {
                // 1. UBICACIONES: ORIGEN
                var origenes = _ubicaciones.Where(u => u.TipoUbicacion == "Origen").ToList();
                col.Item().Background(ColorBarra).PaddingHorizontal(5).PaddingVertical(2).Text("Ubicaciones: Origen").FontColor(Colors.White).Bold().FontSize(8.5f);
                col.Item().PaddingTop(2).Table(t => {
                    t.ColumnsDefinition(c => { c.RelativeColumn(2); c.RelativeColumn(2); c.RelativeColumn(5); c.RelativeColumn(2); });
                    t.Header(h => {
                        h.Cell().BorderBottom(0.5f).Text("FechaHoraSalida").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("RFCRemitente").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("NombreRemitenteDomicilio").FontSize(7);
                        h.Cell().BorderBottom(0.5f).AlignRight().Text("DistRecorrida").FontSize(7);
                    });
                    foreach (var o in origenes)
                    {
                        t.Cell().Text(o.FechaHoraSalidaLlegada?.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        t.Cell().Text(o.RFCRemitenteDestinatario);
                        t.Cell().Text(o.NombreDomicilio);
                        t.Cell().AlignRight().Text(o.DistanciaRecorrida?.ToString("N2"));
                    }
                });

                // 2. UBICACIONES: DESTINO
                var destinos = _ubicaciones.Where(u => u.TipoUbicacion == "Destino").ToList();
                col.Item().PaddingTop(8).Background(ColorBarra).PaddingHorizontal(5).PaddingVertical(2).Text("Ubicaciones: Destino").FontColor(Colors.White).Bold().FontSize(8.5f);
                col.Item().PaddingTop(2).Table(t => {
                    t.ColumnsDefinition(c => { c.RelativeColumn(2); c.RelativeColumn(2); c.RelativeColumn(5); c.RelativeColumn(2); });
                    t.Header(h => {
                        h.Cell().BorderBottom(0.5f).Text("FechaHoraProgLlegada").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("RFCDestinatario").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("NombreDestinatarioDomicilio").FontSize(7);
                        h.Cell().BorderBottom(0.5f).AlignRight().Text("DistRecorrida").FontSize(7);
                    });
                    foreach (var d in destinos)
                    {
                        t.Cell().Text(d.FechaHoraSalidaLlegada?.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        t.Cell().Text(d.RFCRemitenteDestinatario);
                        t.Cell().Text(d.NombreDomicilio);
                        t.Cell().AlignRight().Text(d.DistanciaRecorrida?.ToString("N2"));
                    }
                });

                // 3. MERCANCÍAS
                col.Item().PaddingTop(8).Background(ColorBarra).PaddingHorizontal(5).PaddingVertical(2).Text("Mercancías").FontColor(Colors.White).Bold().FontSize(8.5f);
                col.Item().PaddingTop(4).Grid(g => {
                    g.Columns(3);
                    g.Item().PaddingLeft(15).Text($"PesoBrutoTotal: {_header.PesoBrutoTotal?.ToString("N3")}");
                    g.Item().AlignCenter().Text($"NumTotalMercancias: {_header.NumTotalMercancias}");
                    g.Item().AlignRight().Text($"CargoPorTasacion: {_header.CargoPorTasacion?.ToString("N2")}");

                    g.Item().PaddingLeft(15).Text($"UnidadPeso: {_header.UnidadPeso}");
                    g.Item().AlignCenter().Text($"CantidadMercancias: {_header.NumTotalMercancias}");
                    g.Item().Text("");

                    g.Item().PaddingLeft(15).Text($"PesoNetoTotal: {_header.PesoNetoTotal?.ToString("N3")}");
                });

                col.Item().PaddingTop(6).Table(t => {
                    t.ColumnsDefinition(c => {
                        c.RelativeColumn(1.5f); c.RelativeColumn(3.5f); c.RelativeColumn(1); c.RelativeColumn(1);
                        c.RelativeColumn(1); c.RelativeColumn(1.5f); c.RelativeColumn(1); c.RelativeColumn(1.5f);
                    });
                    t.Header(h => {
                        h.Cell().BorderBottom(0.5f).Text("BienesTransp").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("Descripcion").FontSize(7);
                        h.Cell().BorderBottom(0.5f).AlignRight().Text("Cantidad").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("Unidad").FontSize(7);
                        h.Cell().BorderBottom(0.5f).AlignRight().Text("PesoEnKg").FontSize(7);
                        h.Cell().BorderBottom(0.5f).AlignRight().Text("ValorMercancia").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("Moneda").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("Dimensiones").FontSize(7);
                    });

                    foreach (var m in _mercancias)
                    {
                        t.Cell().Text(m.BienesTransp);
                        t.Cell().Text(m.Descripcion);
                        t.Cell().AlignRight().Text(m.Cantidad?.ToString("N0"));
                        t.Cell().Text(m.ClaveUnidad);
                        t.Cell().AlignRight().Text(m.PesoEnKg?.ToString("N2"));
                        t.Cell().AlignRight().Text(m.ValorMercancia?.ToString("N2"));
                        t.Cell().Text(m.Moneda);
                        t.Cell().Text(m.Dimensiones);
                        t.Cell().ColumnSpan(8).PaddingBottom(4).Text($"       MaterialPeligroso: {m.MaterialPeligroso ?? "No"}").FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                    }
                });

                // 4. AUTOTRANSPORTE Y REMOLQUES
                col.Item().PaddingTop(8).Background(ColorBarra).PaddingHorizontal(5).PaddingVertical(2).Text("Autotransporte").FontColor(Colors.White).Bold().FontSize(8.5f);
                col.Item().PaddingTop(4).Grid(g => {
                    g.Columns(3);
                    g.Item().AlignRight().PaddingRight(5).Text($"PermSCT: {_header.PermSCT}");
                    g.Item().AlignRight().PaddingRight(5).Text($"AseguraRespCivil: {_header.AseguraRespCivil}");
                    g.Item().AlignRight().Text($"ConfigVehicular: {_header.ConfigVehicular}");

                    g.Item().AlignRight().PaddingRight(5).Text($"NumPermisoSCT: {_header.NumPermisoSCT}");
                    g.Item().AlignRight().PaddingRight(5).Text($"PolizaRespCivil: {_header.PolizaRespCivil}");
                    g.Item().AlignRight().Text($"PlacaVM: {_header.PlacaVM}");

                    g.Item().Text("");
                    g.Item().AlignRight().PaddingRight(5).Text($"AseguraMedAmbiente: {_header.AseguraMedAmbiente}");
                    g.Item().AlignRight().Text($"AnioModeloVM: {_header.AnioModeloVM}");

                    g.Item().Text("");
                    g.Item().AlignRight().PaddingRight(5).Text($"PolizaMedAmbiente: {_header.PolizaMedAmbiente}");
                    g.Item().Text("");
                });

                // Tablita anidada de Remolques
                if (_remolques.Any())
                {
                    col.Item().PaddingTop(4).PaddingLeft(20).Width(200).Table(t => {
                        t.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                        t.Header(h => {
                            h.Cell().BorderBottom(0.5f).Text("Remolque (SubTipo)").FontSize(7);
                            h.Cell().BorderBottom(0.5f).Text("Placa").FontSize(7);
                        });
                        foreach (var r in _remolques)
                        {
                            t.Cell().Text(r.SubTipoRem);
                            t.Cell().Text(r.Placa);
                        }
                    });
                }

                // 5. FIGURA TRANSPORTE (Múltiples)
                col.Item().PaddingTop(8).Background(ColorBarra).PaddingHorizontal(5).PaddingVertical(2).Text("Figura Transporte").FontColor(Colors.White).Bold().FontSize(8.5f);
                col.Item().PaddingTop(2).Table(t => {
                    t.ColumnsDefinition(c => { c.RelativeColumn(2); c.RelativeColumn(2); c.RelativeColumn(4); c.RelativeColumn(2); });
                    t.Header(h => {
                        h.Cell().BorderBottom(0.5f).Text("TipoFigura").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("RFC").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("NombreDomicilio").FontSize(7);
                        h.Cell().BorderBottom(0.5f).Text("Licencia").FontSize(7);
                    });
                    foreach (var f in _figuras)
                    {
                        t.Cell().Text(f.TipoFigura);
                        t.Cell().Text(f.RFCFigura);
                        t.Cell().Text(f.NombreDomicilio);
                        t.Cell().Text(f.NumLicencia);
                    }
                });

                // 6. OBSERVACIONES
                col.Item().PaddingTop(8).Background(ColorBarra).PaddingHorizontal(5).PaddingVertical(2).Text("Observaciones").FontColor(Colors.White).Bold().FontSize(8.5f);
                col.Item().PaddingTop(2).PaddingLeft(15).Column(c => {
                    c.Item().Text($"Atencion: {_header.Atencion}");
                    c.Item().Text($"Referencia: {_header.Referencia}");
                    c.Item().Text($"Obs: {_header.Obs}");
                });

                // 7. SAT (Timbre)
                if (_qrImagen != null)
                {
                    col.Item().PaddingTop(15).Row(row =>
                    {
                        row.ConstantItem(110).Height(110).Image(_qrImagen);
                        row.RelativeItem().PaddingLeft(10).Column(c =>
                        {
                            c.Item().Text(t => { t.Span("Sello digital CFDI").Bold(); t.Span($"               Certificado SAT: {_header.CertificadoSAT ?? ""}").Bold(); });
                            c.Item().Text(_header.SelloDigitalCFDI ?? "").FontSize(5.5f);

                            c.Item().PaddingTop(4).Text(t => { t.Span("Sello digital SAT").Bold(); t.Span($"                Certificado del Emisor: {_header.CertificadoEmisor ?? ""}").Bold(); });
                            c.Item().Text(_header.SelloDigitalSAT ?? "").FontSize(5.5f);

                            c.Item().PaddingTop(4).Text("Cadena Original del Complemento de Certificación Digital del SAT").Bold().FontSize(7);
                            c.Item().Text(_header.CadenaOriginal ?? "").FontSize(5.5f);
                        });
                    });
                }
            });
        }
    }
}