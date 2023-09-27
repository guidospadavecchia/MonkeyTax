namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoMonthlyCosts
    {
        public MonotributoUnit<decimal>? PrestacionServicios { get; set; }
        public MonotributoUnit<decimal>? VentaCosasMuebles { get; set; }
    }
}
