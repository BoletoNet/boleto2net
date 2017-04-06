using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Boleto2Net
{
    public class Banco : AbstractBanco
    {
        private readonly IBanco _banco;

        public Banco(int codigoBanco)
        {
            try
            {
                switch (codigoBanco)
                {
                    case 001:
                        _banco = new BancoBrasil();
                        break;
                    case 104:
                        _banco = new BancoCaixa();
                        break;
                    case 237:
                        _banco = new BancoBradesco();
                        break;
                    case 756:
                        _banco = new BancoSicoob();
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
            get { return _banco.Codigo; }
            set { _banco.Codigo = value; }
        }
        public override string Digito => _banco.Digito;

        public override string Nome => _banco.Nome;

        public override bool RemoveAcentosArquivoRemessa => _banco.RemoveAcentosArquivoRemessa;

        public override Cedente Cedente
        {
            get { return _banco.Cedente; }
            set { _banco.Cedente = value; }
        }
        public override List<string> IdsRetornoCnab400RegistroDetalhe => _banco.IdsRetornoCnab400RegistroDetalhe;

        #endregion

        public override void FormataCedente()
        {
            try
            {
                _banco.FormataCedente();
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
                return _banco.FormataCodigoBarraCampoLivre(boleto);
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
                _banco.FormataNossoNumero(boleto);
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
                _banco.ValidaBoleto(boleto);
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
                return _banco.GerarHeaderRemessa(tipoArquivo, numeroArquivoRemessa, ref numeroRegistroGeral);
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
                return _banco.GerarDetalheRemessa(tipoArquivo, boleto, ref numeroRegistro);
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
                return _banco.GerarTrailerRemessa(tipoArquivo, numeroArquivoRemessa,
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
            _banco.LerDetalheRetornoCNAB240SegmentoT(ref boleto, registro);
        }

        public override void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            _banco.LerDetalheRetornoCNAB240SegmentoU(ref boleto, registro);
        }

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            _banco.LerHeaderRetornoCNAB400(registro);
        }

        public override void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            _banco.LerDetalheRetornoCNAB400Segmento1(ref boleto, registro);
        }

        public override void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            _banco.LerDetalheRetornoCNAB400Segmento7(ref boleto, registro);
        }

        public override void LerTrailerRetornoCNAB400(string registro)
        {
            _banco.LerTrailerRetornoCNAB400(registro);
        }

        #endregion
    }
}
