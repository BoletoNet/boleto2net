using System;

namespace Boleto2Net
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CarteiraCodigoAttribute : Attribute
    {
        internal CarteiraCodigoAttribute(string codigo)
        {
            Codigo = codigo;
        }

        internal string Codigo { get; }
    }
}