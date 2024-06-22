namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoResponse
    {
        public IEnumerable<MonotributoCategory> Categorias { get; set; } = [];
    }
}
