using System;
using System.Collections.Generic;
using Boleto2Net.Extensions;

namespace Boleto2Net
{
    public abstract class AbstractBanco : IBanco
    {

        public virtual int Codigo { get; set; } = 0;

        public virtual string Digito { get; set; } = "0";

        public virtual string Nome { get; set; } = string.Empty;

        public virtual List<string> IdsRetornoCnab400RegistroDetalhe { get; set; } = new List<string>();

        public virtual bool RemoveAcentosArquivoRemessa { get; set; } = true;

        public virtual Cedente Cedente { get; set; } = null;

        /// <summary>
        /// Formata campo livre - Cada banco implementa de uma maneira diferente...
        /// </summary>
        public virtual void FormataCedente()
        {
            throw new NotImplementedException("FormataCedente - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }


        /// <summary>
        /// Formata código de barras
        ///	O código de barra para cobrança contém 44 posições dispostas da seguinte forma:
        ///    01 a 03 - 3 - Identificação  do  Banco
        ///    04 a 04 - 1 - Código da Moeda
        ///    05 a 05 – 1 - Dígito verificador do Código de Barras
        ///    06 a 09 - 4 - Fator de vencimento
        ///    10 a 19 - 10 - Valor
        ///    20 a 44 – 25 - Campo Livre
        /// </summary>      
        public void FormataCodigoBarra(Boleto boleto)
        {
            boleto.CodigoBarra.CampoLivre = FormataCodigoBarraCampoLivre(boleto);
            if (String.IsNullOrWhiteSpace(boleto.CodigoBarra.CampoLivre))
            {
                boleto.CodigoBarra.CodigoBanco = String.Empty;
                boleto.CodigoBarra.Moeda = 0;
                boleto.CodigoBarra.FatorVencimento = 0;
                boleto.CodigoBarra.ValorDocumento = String.Empty;
            }
            else
            {
                boleto.CodigoBarra.CodigoBanco = Utils.FitStringLength(Codigo.ToString(), 3, 3, '0', 0, true, true, true);
                boleto.CodigoBarra.Moeda = boleto.CodigoMoeda;
                boleto.CodigoBarra.FatorVencimento = boleto.DataVencimento.FatorVencimento();
                boleto.CodigoBarra.ValorDocumento = boleto.ValorTitulo.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(10, '0');
            }
        }


        /// <summary>
        /// A linha digitável será composta por cinco campos:
        ///      1º campo
        ///          composto pelo código de Banco, código da moeda, as cinco primeiras posições do campo 
        ///          livre e o dígito verificador deste campo;
        ///      2º campo
        ///          composto pelas posições 6ª a 15ª do campo livre e o dígito verificador deste campo;
        ///      3º campo
        ///          composto pelas posições 16ª a 25ª do campo livre e o dígito verificador deste campo;
        ///      4º campo
        ///          composto pelo dígito verificador do código de barras, ou seja, a 5ª posição do código de 
        ///          barras;
        ///      5º campo
        ///          Composto pelo fator de vencimento com 4(quatro) caracteres e o valor do documento com 10(dez) caracteres, sem separadores e sem edição.
        /// </summary>
        public void FormataLinhaDigitavel(Boleto boleto)
        {
            if (String.IsNullOrWhiteSpace(boleto.CodigoBarra.CampoLivre))
            {
                boleto.CodigoBarra.LinhaDigitavel = "";
                return;
            }
            //BBBMC.CCCCD1 CCCCC.CCCCCD2 CCCCC.CCCCCD3 D4 FFFFVVVVVVVVVV
            #region Campo 1
            // POSIÇÃO 1 A 3 DO CODIGO DE BARRAS
            string bbb = boleto.CodigoBarra.CodigoDeBarras.Substring(0, 3);
            // POSIÇÃO 4 DO CODIGO DE BARRAS
            string m = boleto.CodigoBarra.CodigoDeBarras.Substring(3, 1);
            // POSIÇÃO 20 A 24 DO CODIGO DE BARRAS
            string ccccc = boleto.CodigoBarra.CodigoDeBarras.Substring(19, 5);
            // Calculo do Dígito
            string d1 = CalcularDvModulo10(bbb + m + ccccc).ToString();
            // Formata Grupo 1
            string grupo1 = string.Format("{0}{1}{2}.{3}{4} ", bbb, m, ccccc.Substring(0, 1), ccccc.Substring(1, 4), d1);
            #endregion Campo 1

            #region Campo 2
            //POSIÇÃO 25 A 34 DO COD DE BARRAS
            string d2A = boleto.CodigoBarra.CodigoDeBarras.Substring(24, 10);
            // Calculo do Dígito
            string d2B = CalcularDvModulo10(d2A).ToString();
            // Formata Grupo 2
            string grupo2 = string.Format("{0}.{1}{2} ", d2A.Substring(0, 5), d2A.Substring(5, 5), d2B);
            #endregion Campo 2

            #region Campo 3
            //POSIÇÃO 35 A 44 DO CODIGO DE BARRAS
            string d3A = boleto.CodigoBarra.CodigoDeBarras.Substring(34, 10);
            // Calculo do Dígito
            string d3B = CalcularDvModulo10(d3A).ToString();
            // Formata Grupo 3
            string grupo3 = string.Format("{0}.{1}{2} ", d3A.Substring(0, 5), d3A.Substring(5, 5), d3B);
            #endregion Campo 3

            #region Campo 4
            // Dígito Verificador do Código de Barras
            string grupo4 = string.Format("{0} ", boleto.CodigoBarra.DigitoVerificador);
            #endregion Campo 4

            #region Campo 5
            //POSICAO 6 A 9 DO CODIGO DE BARRAS
            string d5A = boleto.CodigoBarra.CodigoDeBarras.Substring(5, 4);
            //POSICAO 10 A 19 DO CODIGO DE BARRAS
            string d5B = boleto.CodigoBarra.CodigoDeBarras.Substring(9, 10);
            // Formata Grupo 5
            string grupo5 = string.Format("{0}{1}", d5A, d5B);
            #endregion Campo 5

            boleto.CodigoBarra.LinhaDigitavel = grupo1 + grupo2 + grupo3 + grupo4 + grupo5;

        }


        /// <summary>
        /// Formata campo livre - Cada banco implementa de uma maneira diferente...
        /// </summary>
        public virtual string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            throw new NotImplementedException("FormataCodigoBarraCampoLivre - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        /// <summary>
        /// Formata nosso número
        /// </summary>
        public virtual void FormataNossoNumero(Boleto boleto)
        {
            throw new NotImplementedException("FormataNossoNumero - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        /// <summary>
        /// Valida o boleto
        /// </summary>
        public virtual void ValidaBoleto(Boleto boleto)
        {
            throw new NotImplementedException("ValidaBoleto - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        /// <summary>
        /// Gera os registros de header do aquivo de remessa
        /// </summary>
        public virtual string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            throw new NotImplementedException("GerarHeaderRemessa - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        /// <summary>
        /// Gera registros de detalhe do arquivo remessa
        /// </summary>
        public virtual string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            throw new NotImplementedException("GerarDetalheRemessa - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        /// <summary>
        /// Gera os registros de Trailer do arquivo de remessa
        /// </summary>
        public virtual string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
                                            ref int numeroRegistroGeral, decimal valorBoletoGeral,
                                            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
                                            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
                                            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
                                            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            throw new NotImplementedException("GerarTrailerRemessa - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        public virtual void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException("LerDetalheRetornoCNAB240SegmentoT - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }
        public virtual void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException("LerDetalheRetornoCNAB240SegmentoU - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }
        public virtual void LerHeaderRetornoCNAB400(string registro)
        {
            throw new NotImplementedException("LerHeaderRetornoCNAB400 - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        public virtual void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException("LerDetalheRetornoCNAB400Segmento1 - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }
        public virtual void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException("LerDetalheRetornoCNAB400Segmento7 - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }
        public virtual void LerTrailerRetornoCNAB400(string registro)
        {
            throw new NotImplementedException("LerTrailerRetornoCNAB400 - Função não implementada na classe filha. Implemente na classe que está sendo criada.");
        }

        private int CalcularDvModulo10(string texto)
        {
            int digito, soma = 0, peso = 2, resto;
            for (int i = texto.Length; i > 0; i--)
            {
                resto = (Convert.ToInt32(Microsoft.VisualBasic.Strings.Mid(texto, i, 1)) * peso);
                if (resto > 9)
                    resto = (resto / 10) + (resto % 10);
                soma += resto;
                if (peso == 2)
                    peso = 1;
                else
                    peso = peso + 1;
            }
            digito = ((10 - (soma % 10)) % 10);
            return digito;
        }


    }
}
