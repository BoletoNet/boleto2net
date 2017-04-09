using System;

namespace Boleto2Net
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CarteiraCodigoAttribute : Attribute
    {
        public CarteiraCodigoAttribute(string codigo)
        {
            Codigo = codigo;
        }

        public string Codigo { get; }
    }
}