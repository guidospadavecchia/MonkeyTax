namespace MonkeyTax.Application.Monotributo.Model
{
    public sealed class MonotributoUnit<T>
    {
        public T Valor { get; set; }
        public string Unidad { get; set; }

        public MonotributoUnit(T valor, string unidad)
        {
            Valor = valor;
            Unidad = unidad;
        }
    }
}
