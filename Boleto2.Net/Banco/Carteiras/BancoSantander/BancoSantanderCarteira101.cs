using System;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("101")]
    internal class BancoSantanderCarteira101 : ICarteira<BancoSantander>
    {
        internal static Lazy<ICarteira<BancoSantander>> Instance { get; } = new Lazy<ICarteira<BancoSantander>>(() => new BancoSantanderCarteira101());

        private BancoSantanderCarteira101()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var nossoNumero = boleto.NossoNumero;
            var contaBancaria = boleto.Banco.Cedente.ContaBancaria;

            if (IsNullOrWhiteSpace(nossoNumero))
                throw new Exception("Nosso Número não informado.");
            // Nosso número não pode ter mais de 7 dígitos
            if (nossoNumero.Length > 7)
                throw new Exception($"Nosso Número ({nossoNumero}) deve conter 7 dígitos.");
            boleto.NossoNumero = nossoNumero = nossoNumero.PadLeft(7, '0');
            boleto.NossoNumeroDV = nossoNumero.CalcularDVSantander();
            boleto.NossoNumeroFormatado = $"{nossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var cedente = boleto.Banco.Cedente;
            var contaBancaria = cedente.ContaBancaria;
            return $"9{cedente.Codigo}00000{boleto.NossoNumero}{boleto.NossoNumeroDV}0101";
        }
    }
}
