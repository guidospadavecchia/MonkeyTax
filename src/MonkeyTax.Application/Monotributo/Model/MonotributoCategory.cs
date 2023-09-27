namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoCategory
    {
        public string Categoria { get; set; } = null!;
        public MonotributoUnit<decimal>? IngresosBrutosAnuales { get; set; }
        public string Actividad { get; set; } = null!;
        public string CantidadMinimaDeEmpleados { get; set; } = null!;
        public MonotributoUnit<int>? SuperficieMaximaAfectada { get; set; }
        public MonotributoUnit<int>? EnergiaElectricaMaximaAnual { get; set; }
        public MonotributoUnit<decimal>? AlquileresDevengadosAnuales { get; set; }
        public MonotributoUnit<decimal>? PrecioUnitarioMaximoVentaCosasMuebles { get; set; }
        public MonotributoIntegratedTax ImpuestoIntegrado { get; set; } = null!;
        public MonotributoMonthlyContributions AportesMensuales { get; set; } = null!;
        public MonotributoMonthlyCosts CostosMensuales { get; set; } = null!;
    }
}
