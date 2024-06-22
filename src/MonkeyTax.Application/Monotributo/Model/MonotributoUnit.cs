namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoUnit<T>(T valor, string unidad)
    {
        public T Valor { get; set; } = valor;
        public string Unidad { get; set; } = unidad;
    }
}
