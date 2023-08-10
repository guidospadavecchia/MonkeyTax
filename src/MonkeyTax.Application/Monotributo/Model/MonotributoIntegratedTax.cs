namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoIntegratedTax
    {
        public required MonotributoUnit<decimal>? Servicios { get; set; }
        public required MonotributoUnit<decimal>? VentaCosasMuebles { get; set; }
    }
}
