namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoResponse
    {
        public required IEnumerable<MonotributoCategory> Categorias { get; set; }
    }
}
