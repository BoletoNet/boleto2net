using System;
using Boleto2Net.Exceptions;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("1")]
    internal class BancoSafraCarteira1 : ICarteira<BancoSafra>
    {
        internal static Lazy<ICarteira<BancoSafra>> Instance { get; } = new Lazy<ICarteira<BancoSafra>>(() => new BancoSafraCarteira1());

        private BancoSafraCarteira1()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 8 dígitos
            if (boleto.NossoNumero.Length > 8)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 8 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(8, '0');
            boleto.NossoNumeroDV = boleto.NossoNumero.CalcularDVSafra();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            if (boleto.Banco.Cedente.ContaBancaria.Agencia.Length != 4)
            {
                throw Boleto2NetException.AgenciaInvalida(boleto.Banco.Cedente.ContaBancaria.Agencia, 4);
            }

            if (boleto.Banco.Cedente.ContaBancaria.DigitoAgencia.Length != 1)
            {
                throw Boleto2NetException.AgenciaDigitoInvalido(boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, 1);
            }

            if (boleto.Banco.Cedente.ContaBancaria.Conta.Length != 6)
            {
                throw Boleto2NetException.ContaInvalida(boleto.Banco.Cedente.ContaBancaria.Conta, 6);
            }

            if (boleto.Banco.Cedente.ContaBancaria.DigitoConta.Length != 1)
            {
                throw Boleto2NetException.ContaDigitoInvalido(boleto.Banco.Cedente.ContaBancaria.DigitoConta, 1);
            }

            if (boleto.NossoNumero.Length != 8)
            {
                throw Boleto2NetException.NossoNumeroInvalido(boleto.NossoNumero, 8);
            }

            if (boleto.NossoNumeroDV.Length != 1)
            {
                throw Boleto2NetException.NossoNumeroInvalido(boleto.NossoNumeroDV, 1);
            }

            return $"{boleto.Banco.Digito}{boleto.Banco.Cedente.ContaBancaria.Agencia}{boleto.Banco.Cedente.ContaBancaria.DigitoAgencia}00{boleto.Banco.Cedente.ContaBancaria.Conta}{boleto.Banco.Cedente.ContaBancaria.DigitoConta}{boleto.NossoNumero}{boleto.NossoNumeroDV}2";
        }
    }
}