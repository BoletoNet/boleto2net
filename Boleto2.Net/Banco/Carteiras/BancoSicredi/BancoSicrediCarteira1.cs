﻿using System;

namespace Boleto2Net
{
    [CarteiraCodigo("1/A")]
    internal class BancoSicrediCarteira1 : ICarteira<BancoSicredi>
    {
        internal static Lazy<ICarteira<BancoSicredi>> Instance { get; } = new Lazy<ICarteira<BancoSicredi>>(() => new BancoSicrediCarteira1());

        private BancoSicrediCarteira1()
        {

        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string CampoLivre = boleto.Carteira + "1" +
                boleto.NossoNumero +
                boleto.Banco.Cedente.ContaBancaria.Agencia +
                boleto.Banco.Cedente.ContaBancaria.OperacaoConta +
                boleto.Banco.Cedente.Codigo + "10";

            CampoLivre += Mod11(CampoLivre);

            return CampoLivre;
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var dataDocumento = boleto.DataEmissao.ToString("yy");
            var nossoNumero = boleto.NossoNumero;

            var numeroOpcional = "2";

            if (ValidarNumeroOpcional(boleto.CodigoInstrucao1))
            {
                numeroOpcional = boleto.CodigoInstrucao1;
            }

            boleto.NossoNumero = $"{dataDocumento}{numeroOpcional}{nossoNumero.PadLeft(5, '0')}";

            boleto.NossoNumeroDV = Mod11(Sequencial(boleto)).ToString();
            boleto.NossoNumero = string.Concat(boleto.NossoNumero, Mod11(Sequencial(boleto)));

            boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumero.Substring(8));
        }


        private bool ValidarNumeroOpcional(string numeroOpcional)
        {
            if (!int.TryParse(numeroOpcional, out var valorOpcional))
            {
                return false;
            }

            /*
             * Página 7, seção 5.3 (váriavel B)
             * https://www.sicredi.com.br/html/para-voce/recebimentos/cobranca/arquivos/manual-cnab-400---2019.pdf
             *
             * Valor 1 só pode ser utilizado pelo banco (e menor impossível), e deve ser no máximo 9
             */
            if (valorOpcional <= 1 || valorOpcional > 9)
            {
                return false;
            }

            return true;
        }

        public int Mod11(string seq)
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

        public string Sequencial(Boleto boleto)
        {
            string agencia = boleto.Banco.Cedente.ContaBancaria.Agencia;     //código da cooperativa de crédito/agência beneficiária (aaaa)
            string posto = boleto.Banco.Cedente.ContaBancaria.OperacaoConta; //código do posto beneficiário (pp)

            if (string.IsNullOrEmpty(posto))
            {
                throw new Exception($"Posto beneficiário não preenchido");
            }

            string cedente = boleto.Banco.Cedente.Codigo;                    //código do beneficiário (ccccc)
            string nossoNumero = boleto.NossoNumero;                         //ano atual (yy), indicador de geração do nosso número (b) e o número seqüencial do beneficiário (nnnnn);

            return string.Concat(agencia, posto, cedente, nossoNumero); // = aaaappcccccyybnnnnn
        }
    }
}
