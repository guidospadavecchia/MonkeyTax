namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoMonthlyCosts
    {
        public required MonotributoUnit<decimal>? PrestacionServicios { get; set; }
        public required MonotributoUnit<decimal>? VentaCosasMuebles { get; set; }
    }
}
