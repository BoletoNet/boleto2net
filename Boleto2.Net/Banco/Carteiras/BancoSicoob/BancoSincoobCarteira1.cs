using System;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("1/01")]
    internal class BancoSincoobCarteira1: ICarteira<BancoSicoob>
    {
        internal static Lazy<ICarteira<BancoSicoob>> Instance { get; } = new Lazy<ICarteira<BancoSicoob>>(() => new BancoSincoobCarteira1());

        private BancoSincoobCarteira1()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto == TipoImpressaoBoleto.Empresa & boleto.NossoNumero == Empty)
                throw new Exception("Nosso Número não informado.");
            // Nosso número não pode ter mais de 7 dígitos
            if (boleto.NossoNumero.Length > 7)
                throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve conter 7 dígitos.");
            boleto.NossoNumero = boleto.NossoNumero.PadLeft(7, '0');
            // Base para calcular DV:
            // Agencia (4 caracteres)
            // Código do Cedente com dígito (10 caracteres)
            // Nosso Número (7 caracteres)
            boleto.NossoNumeroDV = CalcularDV(boleto.Banco.Cedente.ContaBancaria.Agencia + boleto.Banco.Cedente.Codigo.PadLeft(9, '0') + boleto.Banco.Cedente.CodigoDV + boleto.NossoNumero);
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        private static string CalcularDV(string texto)
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
