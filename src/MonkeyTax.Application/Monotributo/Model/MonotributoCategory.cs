namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoCategory
    {
        public required string Categoria { get; set; }
        public required MonotributoUnit<decimal>? IngresosBrutosAnuales { get; set; }
        public required string Actividad { get; set; }
        public required string CantidadMinimaDeEmpleados { get; set; }
        public required MonotributoUnit<int>? SuperficieMaximaAfectada { get; set; }
        public required MonotributoUnit<int>? EnergiaElectricaMaximaAnual { get; set; }
        public required MonotributoUnit<decimal>? AlquileresDevengadosAnuales { get; set; }
        public required MonotributoUnit<decimal>? PrecioUnitarioMaximoVentaCosasMuebles { get; set; }
        public required MonotributoIntegratedTax ImpuestoIntegrado { get; set; }
        public required MonotributoMonthlyContributions AportesMensuales { get; set; }
        public required MonotributoMonthlyCosts CostosMensuales { get; set; }
    }
}
