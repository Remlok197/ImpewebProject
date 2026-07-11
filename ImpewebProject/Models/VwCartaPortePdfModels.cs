using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ImpewebProject.Models
{
    [Table("VwCartaPortePdfHeader")]
    [Keyless]
    public class VwCartaPortePdfHeader
    {
        public string? CartaPorte { get; set; }
        public string? Serie { get; set; }
        public int? Folio { get; set; }
        public DateTime? Fecha { get; set; }
        public string? UUID { get; set; }
        public string? FechaTimbrado { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? PesoBrutoTotal { get; set; }
        public string? UnidadPeso { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? PesoNetoTotal { get; set; }
        public int? NumTotalMercancias { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? CargoPorTasacion { get; set; }
        public string? Atencion { get; set; }
        public string? Referencia { get; set; }
        public string? Obs { get; set; }
        public string? PermSCT { get; set; }
        public string? NumPermisoSCT { get; set; }
        public string? ConfigVehicular { get; set; }
        public string? PlacaVM { get; set; }
        public string? AnioModeloVM { get; set; }
        public string? AseguraRespCivil { get; set; }
        public string? PolizaRespCivil { get; set; }
        public string? AseguraMedAmbiente { get; set; }
        public string? PolizaMedAmbiente { get; set; }
        public string? AseguraCarga { get; set; }
        public string? PolizaCarga { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? PrimaSeguro { get; set; }
        public string? CertificadoEmisor { get; set; }
        public string? CertificadoSAT { get; set; }
        public string? SelloDigitalCFDI { get; set; }
        public string? SelloDigitalSAT { get; set; }
        public string? CadenaOriginal { get; set; }
        public string? ClienteRfc { get; set; } 
    }

    [Table("VwCartaPortePdfUbicaciones")]
    [Keyless]
    public class VwCartaPortePdfUbicacion
    {
        public string? CartaPorte { get; set; }
        public string? TipoUbicacion { get; set; }
        public DateTime? FechaHoraSalidaLlegada { get; set; }
        public string? RFCRemitenteDestinatario { get; set; }
        public string? NombreDomicilio { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? DistanciaRecorrida { get; set; }
    }

    [Table("VwCartaPortePdfMercancias")] // Mercancías
    [Keyless]
    public class VwCartaPortePdfMercancia
    {
        public string? CartaPorte { get; set; }
        public string? BienesTransp { get; set; }
        public string? Descripcion { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? Cantidad { get; set; }
        public string? ClaveUnidad { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? PesoEnKg { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? ValorMercancia { get; set; }
        public string? Moneda { get; set; }
        public string? Dimensiones { get; set; }
        public string? MaterialPeligroso { get; set; }
    }

    [Table("VwCartaPortePdfRemolques")]
    [Keyless]
    public class VwCartaPortePdfRemolque
    {
        public string? CartaPorte { get; set; }
        public string? SubTipoRem { get; set; }
        public string? Placa { get; set; }
    }

    [Table("VwCartaPortePdfFiguras")]
    [Keyless]
    public class VwCartaPortePdfFigura
    {
        public string? CartaPorte { get; set; }
        public string? TipoFigura { get; set; }
        public string? RFCFigura { get; set; }
        public string? NombreDomicilio { get; set; }
        public string? NumLicencia { get; set; }
    }
}