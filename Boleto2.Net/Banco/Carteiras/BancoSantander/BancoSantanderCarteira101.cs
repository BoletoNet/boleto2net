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
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 7 dígitos
            if (boleto.NossoNumero.Length > 7)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 7 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(7, '0');
            boleto.NossoNumeroDV = boleto.NossoNumero.CalcularDVSantander();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"9{boleto.Banco.Cedente.Codigo}00000{boleto.NossoNumero}{boleto.NossoNumeroDV}0101";
        }
    }
}
