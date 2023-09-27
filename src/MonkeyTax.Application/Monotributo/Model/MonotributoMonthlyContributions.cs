namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoMonthlyContributions
    {
        public MonotributoUnit<decimal>? SistemaPrevisional { get; set; }
        public MonotributoUnit<decimal>? ObraSocial { get; set; }
    }
}
