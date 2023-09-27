namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoIntegratedTax
    {
        public MonotributoUnit<decimal>? Servicios { get; set; }
        public MonotributoUnit<decimal>? VentaCosasMuebles { get; set; }
    }
}
