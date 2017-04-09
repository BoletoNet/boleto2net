using System;
using System.Collections.Generic;

namespace Boleto2Net
{
    internal static class BancoBrasilCarteiraFactory
    {
        private static readonly Dictionary<string, Lazy<ICarteira>> Carteiras = new Dictionary<string, Lazy<ICarteira>>
        {
            ["11/019"] = BancoBrasilCarteira11_019.Instance,
            ["17/019"] = BancoBrasilCarteira17_019.Instance,
        };

        internal static ICarteira ObterCarteira(string identificacao) 
            => Carteiras.ContainsKey(identificacao) ? Carteiras[identificacao].Value : throw new NotImplementedException("Não foi possível formatar o nosso número do boleto.");

        public static bool CarteiraEstaImplementada(string identificacao) 
            => Carteiras.ContainsKey(identificacao);
    }
}
