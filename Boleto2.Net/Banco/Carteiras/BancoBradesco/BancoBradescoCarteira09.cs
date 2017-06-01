using System;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("09")]
    internal class BancoBradescoCarteira09 : ICarteira<BancoBradesco>
    {
        internal static Lazy<ICarteira<BancoBradesco>> Instance { get; } = new Lazy<ICarteira<BancoBradesco>>(() => new BancoBradescoCarteira09());

        private BancoBradescoCarteira09()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            // O nosso número não pode ser em branco.
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");
            
            // Nosso número não pode ter mais de 11 dígitos
            if (boleto.NossoNumero.Length > 11)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 11 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(11, '0');
            boleto.NossoNumeroDV = (boleto.Banco.Cedente.ContaBancaria.Carteira + boleto.NossoNumero).CalcularDVBradesco();
            boleto.NossoNumeroFormatado = $"{boleto.Banco.Cedente.ContaBancaria.Carteira.PadLeft(3, '0')}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Cedente.ContaBancaria;
            return $"{contaBancaria.Agencia}{contaBancaria.Carteira}{boleto.NossoNumero}{contaBancaria.Conta}{"0"}";
        }
    }
}
