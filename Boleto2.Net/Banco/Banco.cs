using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Boleto2Net
{
    public class Banco : AbstractBanco, IBanco
    {
        private IBanco _IBanco;

        public Banco(int codigoBanco)
        {
            try
            {
                switch (codigoBanco)
                {
                    case 001:
                        _IBanco = new Banco_Brasil();
                        break;
                    case 104:
                        _IBanco = new Banco_Caixa();
                        break;
                    case 237:
                        _IBanco = new Banco_Bradesco();
                        break;
                    case 756:
                        _IBanco = new Banco_Sicoob();
                        break;
                    default:
                        throw new Exception("Banco não implementando: " + codigoBanco);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao instanciar objeto.", ex);
            }
        }

        #region Propriedades da Interface

        public override int Codigo
        {
            get { return _IBanco.Codigo; }
            set { _IBanco.Codigo = value; }
        }
        public override string Digito
        {
            get { return _IBanco.Digito; }
        }
        public override string Nome
        {
            get { return _IBanco.Nome; }
        }
        public override Cedente Cedente
        {
            get { return _IBanco.Cedente; }
            set { _IBanco.Cedente = value; }
        }
        public override List<string> IdsRetornoCnab400RegistroDetalhe
        {
            get { return _IBanco.IdsRetornoCnab400RegistroDetalhe; }
        }

        #endregion

        public override void FormataCedente()
        {
            try
            {
                _IBanco.FormataCedente();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a formatação do cedente.", ex);
            }
        }
        public override string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            try
            {
                return _IBanco.FormataCodigoBarraCampoLivre(boleto);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a formatação do código de barra.", ex);
            }
        }
        public override void FormataNossoNumero(Boleto boleto)
        {
            try
            {
                _IBanco.FormataNossoNumero(boleto);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a formatação do nosso número.", ex);
            }
        }
        public override void ValidaBoleto(Boleto boleto)
        {
            try
            {
                // Valida os dados do Boleto na classe do banco responsável
                _IBanco.ValidaBoleto(boleto);
                // Formata nosso número (Classe Abstrata)
                FormataNossoNumero(boleto);
                // Formata o código de Barras (Classe Abstrata)
                FormataCodigoBarra(boleto);
                // Formata linha digitavel (Classe Abstrata)
                FormataLinhaDigitavel(boleto);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a validação do boleto.", ex);
            }
        }

        #region Gerar Arquivo Remessa
        public override string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                return _IBanco.GerarHeaderRemessa(tipoArquivo, numeroArquivoRemessa, ref numeroRegistroGeral);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro HEADER do arquivo de REMESSA.", ex);
            }
        }
        public override string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                return _IBanco.GerarDetalheRemessa(tipoArquivo, boleto, ref numeroRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração dos registros de DETALHE do arquivo de REMESSA.", ex);
            }
        }
        public override string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
                                            ref int numeroRegistroGeral, decimal valorBoletoGeral,
                                            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
                                            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
                                            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
                                            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                return _IBanco.GerarTrailerRemessa(tipoArquivo, numeroArquivoRemessa,
                                            ref numeroRegistroGeral, valorBoletoGeral,
                                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                            numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }
        #endregion

        #region Leitura do Arquivo Retorno

        public override void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            _IBanco.LerDetalheRetornoCNAB240SegmentoT(ref boleto, registro);
        }
        public override void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            _IBanco.LerDetalheRetornoCNAB240SegmentoU(ref boleto, registro);
        }
        public override void LerHeaderRetornoCNAB400(string registro)
        {
            _IBanco.LerHeaderRetornoCNAB400(registro);
        }
        public override void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            _IBanco.LerDetalheRetornoCNAB400Segmento1(ref boleto, registro);
        }
        public override void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            _IBanco.LerDetalheRetornoCNAB400Segmento7(ref boleto, registro);
        }
        public override void LerTrailerRetornoCNAB400(string registro)
        {
            _IBanco.LerTrailerRetornoCNAB400(registro);
        }

        #endregion
    }
}
