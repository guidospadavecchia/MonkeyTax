namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoMonthlyContributions
    {
        public required MonotributoUnit<decimal>? SistemaPrevisional { get; set; }
        public required MonotributoUnit<decimal>? ObraSocial { get; set; }
    }
}
