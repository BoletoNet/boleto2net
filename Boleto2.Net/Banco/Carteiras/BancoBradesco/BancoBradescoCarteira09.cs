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
            var nossoNumero = boleto.NossoNumero;
            var contaBancaria = boleto.Banco.Cedente.ContaBancaria;
            // Carteira 09: Dúvida: Não sei se na carteira 09, o banco também emite o boleto. Se emitir, será necessário tirar a trava do nosso número em branco:
            // Se for só a empresa, devemos tratar aqui, que o nosso número não
            // O nosso número não pode ser em branco.
            if (IsNullOrWhiteSpace(nossoNumero))
                throw new Exception("Nosso Número não informado.");
            // Nosso número não pode ter mais de 11 dígitos
            if (nossoNumero.Length > 11)
                throw new Exception($"Nosso Número ({nossoNumero}) deve conter 11 dígitos.");
            boleto.NossoNumero = nossoNumero = nossoNumero.PadLeft(11, '0');
            boleto.NossoNumeroDV = (contaBancaria.Carteira + nossoNumero).CalcularDVBradesco();
            boleto.NossoNumeroFormatado = $"{contaBancaria.Carteira.PadLeft(3, '0')}/{nossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Cedente.ContaBancaria;
            return $"{contaBancaria.Agencia}{contaBancaria.Carteira}{boleto.NossoNumero}{contaBancaria.Conta}{"0"}";
        }
    }
}
