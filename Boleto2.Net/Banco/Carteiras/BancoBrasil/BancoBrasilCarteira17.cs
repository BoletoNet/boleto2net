using System;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("17/019", "17/027", "17/035", "17/043")]
    internal class BancoBrasilCarteira17 : ICarteira<BancoBrasil>
    {
        internal static Lazy<ICarteira<BancoBrasil>> Instance { get; } = new Lazy<ICarteira<BancoBrasil>>(() => new BancoBrasilCarteira17());

        private BancoBrasilCarteira17()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            // Carteira 17 - Variação 019/027/035/043: Cliente emite o boleto
            // O nosso número não pode ser em branco.
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            switch (boleto.Banco.Cedente.Codigo.Length)
            {
                case 4:
                    FormataNossoNumero4dig(boleto);
                    break;
                case 6:
                    FormataNossoNumero6dig(boleto);
                    break;
                case 7:
                    FormataNossoNumero7dig(boleto);
                    break;
                default:
                    throw new NotImplementedException("Não foi possível formatar o nosso número: Código do Cedente não tem 4, 6 ou 7 dígitos.");
            }
        }

        private void FormataNossoNumero4dig(Boleto boleto)
        {
            boleto.NossoNumeroDV = Mod11(boleto.NossoNumero).ToString();
            // Se o convênio for de 4 dígitos,
            // o nosso número deve estar formatado corretamente (com 12 dígitos e iniciando com o código do convênio),
            if (boleto.NossoNumero.Length == 12)
            {
                if (!boleto.NossoNumero.StartsWith(boleto.Banco.Cedente.Codigo))
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 12 dígitos.");
            }
            else
            {
                // ou deve ser informado com até 7 posições (será formatado para 12 dígitos pelo Boleto.Net).
                if (boleto.NossoNumero.Length > 7)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 7 dígitos.");
                boleto.NossoNumero = $"{boleto.Banco.Cedente.Codigo}{boleto.NossoNumero.PadLeft(7, '0')}";
            }
            
            boleto.NossoNumeroFormatado = boleto.NossoNumero + boleto.NossoNumeroDV;
        }

        private void FormataNossoNumero6dig(Boleto boleto)
        {
            boleto.NossoNumeroDV = Mod11(boleto.NossoNumero).ToString();
            // Se o convênio for de 6 dígitos,
            // o nosso número deve estar formatado corretamente (com 12 dígitos e iniciando com o código do convênio),
            if (boleto.NossoNumero.Length == 12)
            {
                if (!boleto.NossoNumero.StartsWith(boleto.Banco.Cedente.Codigo))
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 12 dígitos.");
            }
            else
            {
                // ou deve ser informado com até 5 posições (será formatado para 12 dígitos pelo Boleto.Net).
                if (boleto.NossoNumero.Length > 5)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 5 dígitos.");
                boleto.NossoNumero = $"{boleto.Banco.Cedente.Codigo}{boleto.NossoNumero.PadLeft(5, '0')}";
            }
            boleto.NossoNumeroFormatado = boleto.NossoNumero + boleto.NossoNumeroDV;
        }

        private void FormataNossoNumero7dig(Boleto boleto)
        {
            // Se o convênio for de 7 dígitos,
            // o nosso número deve estar formatado corretamente (com 17 dígitos e iniciando com o código do convênio),
            if (boleto.NossoNumero.Length == 17)
            {
                if (!boleto.NossoNumero.StartsWith(boleto.Banco.Cedente.Codigo))
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 17 dígitos.");
            }
            else
            {
                // ou deve ser informado com até 10 posições (será formatado para 17 dígitos pelo Boleto.Net).
                if (boleto.NossoNumero.Length > 10)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 17 dígitos.");
                boleto.NossoNumero = $"{boleto.Banco.Cedente.Codigo}{boleto.NossoNumero.PadLeft(10, '0')}";
            }
            // Para convênios com 7 dígitos, não existe dígito de verificação do nosso número
            boleto.NossoNumeroDV = "";
            boleto.NossoNumeroFormatado = boleto.NossoNumero;
        }

        private int Mod11(string seq)
        {
            /* Variáveis
             * -------------
             * d - Dígito
             * s - Soma
             * p - Peso
             * b - Base
             * r - Resto
             */

            int d, s = 0, p = 2, b = 9;

            for (int i = seq.Length - 1; i >= 0; i--)
            {
                s = s + (Convert.ToInt32(seq.Substring(i, 1)) * p);
                if (p < b)
                    p = p + 1;
                else
                    p = 2;
            }

            d = 11 - (s % 11);
            if (d > 9)
                d = 0;
            return d;
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            switch (boleto.Banco.Cedente.Codigo.Length)
            {
                case 4:
                    return $"{boleto.NossoNumero}{boleto.Banco.Cedente.ContaBancaria.Agencia.PadLeft(4, '0')}{boleto.Banco.Cedente.ContaBancaria.Conta.PadLeft(8, '0')}{boleto.Carteira}";
                case 6:
                    return $"{boleto.NossoNumero}{boleto.Banco.Cedente.ContaBancaria.Agencia.PadLeft(4, '0')}{boleto.Banco.Cedente.ContaBancaria.Conta.PadLeft(8, '0')}{boleto.Carteira}";
                case 7:
                    return $"000000{boleto.NossoNumeroFormatado}{boleto.Carteira}";
                default:
                    throw new NotImplementedException("Código do Cedente deve conter 4, 6 ou 7 dígitos.");
            }
        }
    }
}
