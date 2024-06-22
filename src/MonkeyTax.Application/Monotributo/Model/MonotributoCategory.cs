namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoCategory
    {
        public required string Categoria { get; set; }
        public MonotributoUnit<decimal>? IngresosBrutosAnuales { get; set; }
        public required string Actividad { get; set; }
        //public required string CantidadMinimaDeEmpleados { get; set; }
        public MonotributoUnit<int>? SuperficieMaximaAfectada { get; set; }
        public MonotributoUnit<int>? EnergiaElectricaMaximaAnual { get; set; }
        public MonotributoUnit<decimal>? AlquileresDevengadosAnuales { get; set; }
        public MonotributoUnit<decimal>? PrecioUnitarioMaximoVentaCosasMuebles { get; set; }
        public required MonotributoIntegratedTax ImpuestoIntegrado { get; set; }
        public required MonotributoMonthlyContributions AportesMensuales { get; set; }
        public required MonotributoMonthlyCosts CostosMensuales { get; set; }
    }
}
