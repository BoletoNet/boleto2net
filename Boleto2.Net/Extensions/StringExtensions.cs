using System;

namespace Boleto2Net.Extensions
{
    public static class StringExtensions
    {
        public static string CalcularDVCaixa(this string texto)
        {
            string digito;
            int pesoMaximo = 9, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                soma = soma + Convert.ToInt32(texto.Substring(i, 1)) * peso;
                if (peso == pesoMaximo)
                    peso = 2;
                else
                    peso = peso + 1;
            }
            var resto = soma % 11;
            if (resto <= 1)
                digito = "0";
            else
                digito = (11 - resto).ToString();
            return digito;
        }

        public static string CalcularDVSicoob(this string texto)
        {
            string digito, fatorMultiplicacao = "319731973197319731973";
            int soma = 0;
            for (int i = 0; i < 21; i++)
            {
                soma += Convert.ToInt16(texto.Substring(i, 1)) * Convert.ToInt16(fatorMultiplicacao.Substring(i, 1));
            }
            int resto = (soma % 11);
            if (resto <= 1)
                digito = "0";
            else
                digito = (11 - resto).ToString();
            return digito;
        }
    }
}