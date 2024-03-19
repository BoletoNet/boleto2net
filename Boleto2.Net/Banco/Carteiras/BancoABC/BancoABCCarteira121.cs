using System;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("121")]
    internal class BancoABCCarteira121 : ICarteira<BancoABC>
    {
        internal static Lazy<ICarteira<BancoABC>> Instance { get; } = new Lazy<ICarteira<BancoABC>>(() => new BancoABCCarteira121());

        private BancoABCCarteira121()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 10 dígitos
            if (boleto.NossoNumero.Length > 10)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 10 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(10, '0');
            boleto.NossoNumeroDV = (boleto.Banco.Cedente.ContaBancaria.Agencia + boleto.Carteira + boleto.NossoNumero).CalcularDVABC();
            boleto.NossoNumeroFormatado = $"{boleto.Banco.Cedente.ContaBancaria.Agencia + boleto.Banco.Cedente.ContaBancaria.DigitoAgencia}/{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Cedente.ContaBancaria;
            return $"{contaBancaria.Agencia}{boleto.Carteira}{contaBancaria.OperacaoConta}{boleto.NossoNumero}{boleto.NossoNumeroDV}";
        }
    }
}
