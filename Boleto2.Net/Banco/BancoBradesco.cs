using System;
using System.Collections.Generic;
using System.Web.UI;
using Boleto2Net.Exceptions;
using static System.String;

[assembly: WebResource("BoletoNet.Imagens.237.jpg", "image/jpg")]

namespace Boleto2Net
{
    internal sealed class BancoBradesco : IBanco
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new BancoBradesco());

        public Cedente Cedente { get; set; }
        public int Codigo { get; } = 237;
        public string Nome { get; } = "Bradesco";
        public string Digito { get; } = "2";
        public List<string> IdsRetornoCnab400RegistroDetalhe { get; } = new List<string> { "1" };
        public bool RemoveAcentosArquivoRemessa { get; } = true;

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoBradesco>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw Boleto2NetException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("AT� O VENCIMENTO EM QUALQUER BANCO. AP�S O VENCIMENTO SOMENTE NO BRADESCO.", "", 7);

            var codigoCedente = Cedente.Codigo;
            Cedente.Codigo = codigoCedente.Length <= 20 ? codigoCedente.PadLeft(20, '0') : throw Boleto2NetException.CodigoCedenteInvalido(codigoCedente, 20);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoBradesco>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoBradesco>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var header = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Cabe�alho do Arquivo
                        header += GerarHeaderRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        // Cabe�alho do Lote
                        header += Environment.NewLine;
                        header += GerarHeaderLoteRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    case TipoArquivo.CNAB400:
                        header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return header;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerarRegistroHeaderDoArquivoRemessa(ex);
            }
        }

        public string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                string detalhe = Empty, strline = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Segmento P (Obrigat�rio)
                        detalhe += GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro);

                        // Segmento Q (Obrigat�rio)
                        detalhe += Environment.NewLine;
                        detalhe += GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);

                        // Segmento R (Opcional)
                        strline = GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }
                        // Segmento S (Opcional)
                        strline = GerarDetalheSegmentoSRemessaCNAB240(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }

                        break;
                    case TipoArquivo.CNAB400:
                        detalhe += GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);
                        strline = GerarDetalheRemessaCNAB400Registro2(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return detalhe;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerarRegistroDetalheDoArquivoRemessa(ex);
            }
        }

        public string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
            ref int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                var trailer = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Trailler do Lote
                        trailer += GerarTrailerLoteRemessaCNAC240(ref numeroRegistroGeral,
                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                        // Trailler do Arquivo
                        trailer += Environment.NewLine;
                        trailer += GerarTrailerRemessaCNAB240(ref numeroRegistroGeral);
                        break;

                    case TipoArquivo.CNAB400:
                        trailer = GerarTrailerRemessaCNAB400(ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return trailer;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerrarRegistroTrailerDoArquivoRemessa(ex);
            }
        }



        #region CNAB240

        #region RemessaCNAB240
        private string GerarHeaderRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0033, 020, 0, Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 005, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 012, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0072, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "BRADESCO", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 0152, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "084", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0167, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0192, 020, 0, Cedente.Observacoes, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0212, 004, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0216, 025, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarHeaderLoteRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "R", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0012, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0014, 003, 0, "042", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0034, 020, 0, Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 005, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0059, 001, 0, Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0060, 012, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0072, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0073, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0104, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0144, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0184, 008, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0192, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0200, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 033, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "P", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 005, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 012, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0037, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0041, 005, 0, 0, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0046, 011, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0057, 001, 0, boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0058, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoCarteiraPadrao, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0059, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoFormaCadastramento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0060, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0062, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoDistribuicao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 015, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0078, 008, 0, boleto.DataVencimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 015, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 005, 0, 0, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, "0", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, (int)boleto.EspecieDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0110, 008, 0, boleto.DataEmissao, '0');
                if (boleto.ValorJurosDia == 0)
                {
                    // Sem Juros Mora
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 2, "3", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0119, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, 0, '0');
                }
                else
                {
                    // Com Juros Mora ($)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 2, "1", '0');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0119, 008, 0, boleto.DataJuros, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, boleto.ValorJurosDia, '0');
                }
                if (boleto.ValorDesconto == 0)
                {
                    // Sem Desconto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0142, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0151, 015, 2, "0", '0');
                }
                else
                {
                    // Com Desconto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0142, 001, 0, "1", '0');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0143, 008, 0, boleto.DataDesconto, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0151, 015, 2, boleto.ValorDesconto, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0166, 015, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0181, 015, 2, boleto.ValorAbatimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0196, 025, 0, boleto.NumeroControleParticipante, ' ');
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.NaoProtestar:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 3, '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 1, '0');
                        break;
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 0, '0');
                        break;
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, boleto.DiasProtesto, '0');
                switch (boleto.CodigoBaixaDevolucao)
                {
                    case TipoCodigoBaixaDevolucao.NaoBaixarNaoDevolver:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, 2, '0');
                        break;
                    case TipoCodigoBaixaDevolucao.BaixarDevolver:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, 1, '0');
                        break;
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, 0, '0');
                        break;
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0225, 003, 0, boleto.DiasBaixaDevolucao, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0228, 002, 0, "09", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0230, 010, 2, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0240, 001, 0, Empty, ' ');
                reg.CodificarLinha();
                var vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento P no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarDetalheSegmentoQRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "Q", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, boleto.Sacado.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0034, 040, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0114, 015, 0, boleto.Sacado.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0129, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0137, 015, 0, boleto.Sacado.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0152, 002, 0, boleto.Sacado.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0154, 001, 0, boleto.Avalista.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0155, 015, 0, boleto.Avalista.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0170, 040, 0, boleto.Avalista.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0210, 003, 0, boleto.Banco.Cedente.ContaBancaria.CodigoBancoCorrespondente, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 020, 0, boleto.Banco.Cedente.ContaBancaria.NossoNumeroBancoCorrespondente, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0233, 008, 0, Empty, ' ');
                reg.CodificarLinha();
                var vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarDetalheSegmentoRRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                string codMulta;
                if (boleto.ValorMulta > 0)
                    codMulta = "1";
                else
                    codMulta = "0";
                var msg = boleto.MensagemArquivoRemessa.PadRight(500, ' ');
                var msg3 = msg.Substring(00, 40).FitStringLength(40, ' ');
                var msg4 = msg.Substring(40, 40).FitStringLength(40, ' ');
                if ((codMulta == "0") & IsNullOrWhiteSpace(msg3) & IsNullOrWhiteSpace(msg4))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "R", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0043, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0051, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, codMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0067, 008, 0, boleto.DataMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 015, 2, boleto.ValorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0100, 040, 0, msg3, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 040, 0, msg4, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 050, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0230, 011, 0, Empty, ' ');
                reg.CodificarLinha();
                var vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarDetalheSegmentoSRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                var msg5A9 = boleto.MensagemArquivoRemessa.PadRight(500, ' ').Substring(0, 200).FitStringLength(200, ' ');
                if (IsNullOrWhiteSpace(msg5A9))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "S", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0019, 200, 0, msg5A9, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0219, 022, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento S no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarTrailerLoteRemessaCNAC240(ref int numeroRegistroGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O n�mero de registros no lote � igual ao n�mero de registros gerados + 2 (header e trailler do lote)
                var numeroRegistrosNoLote = numeroRegistroGeral + 2;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistroCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 017, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 006, 0, numeroRegistroCobrancaCaucionada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 017, 2, valorCobrancaCaucionada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0070, 006, 0, numeroRegistroCobrancaDescontada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0076, 017, 2, valorCobrancaDescontada, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0093, 031, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0124, 117, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar TRAILER do lote no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarTrailerRemessaCNAB240(ref int numeroRegistroGeral)
        {
            try
            {
                // O n�mero de registros no arquivo � igual ao n�mero de registros gerados + 4 (header e trailler do lote / header e trailler do arquivo)
                var numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "237", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 205, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar TRAILER do arquivo de remessa do CNAB240.", ex);
            }
        }
        #endregion

        #region RetornoCNAB240
        public void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            ////144 - 151 Data de gera��o do arquivo N 008 DDMMAAAA
            //arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####"));
            ////158 - 163 N� seq�encial do arquivo N 006
            //arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(157, 6));
        }

        public void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                //N� Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(105, 25);

                //Carteira
                boleto.Carteira = registro.Substring(57, 1);
                switch (boleto.Carteira)
                {
                    case "1":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                    case "2":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVinculada;
                        break;
                    case "3":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaCaucionada;
                        break;
                    case "4":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;
                        break;
                    case "5":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVendor;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identifica��o do T�tulo no Banco
                string tmp = registro.Substring(37,20);
                boleto.NossoNumero = tmp.Substring(8, 11);
                boleto.NossoNumeroDV = registro.Substring(1, 19);

                //Identifica��o de Ocorr�ncia
                boleto.CodigoOcorrencia = registro.Substring(15, 2);
                boleto.DescricaoOcorrencia = Cnab.OcorrenciaCnab240(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(213, 10);

                //N�mero do Documento
                boleto.NumeroDocumento = registro.Substring(58, 15);
                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;

                //Valor do T�tulo
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(81, 15)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(198, 15)) / 100;

                //Data Vencimento do T�tulo
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(73, 8)).ToString("##-##-####"));

                //Dados Sacado
                boleto.Sacado = new Sacado();
                string str = registro.Substring(133, 15);
                boleto.Sacado.CPFCNPJ = str.Substring(str.Length - 14, 14);
                boleto.Sacado.Nome = registro.Substring(148, 40);


                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / T.", ex);
            }
        }

        public void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            try
            {
                //Valor do T�tulo
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(17, 15)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(32, 15)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(47, 15)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(62, 15)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(77, 15)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(92, 15)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(107, 15)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(122, 15)) / 100;


                //Data Ocorr�ncia no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(137, 8)).ToString("##-##-####"));

                // Data do Cr�dito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(145, 8)).ToString("##-##-####"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / U.", ex);
            }
        }
        #endregion

        #endregion


        #region CNAB400
        public void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo n�o � do tipo \"02RETORNO\"");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //N� Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira (no arquivo retorno, vem com 1 caracter. Ajustamos para 2 caracteres, como no manual do Bradesco.
                boleto.Carteira = registro.Substring(107, 1).PadLeft(2, '0');
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identifica��o do T�tulo no Banco
                boleto.NossoNumero = registro.Substring(70, 11); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(81, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identifica��o de Ocorr�ncia
                boleto.CodigoOcorrencia = registro.Substring(108, 2);
                boleto.DescricaoOcorrencia = DescricaoOcorrenciaCnab400(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(318, 10);

                //N�mero do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do T�tulo
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorr�ncia no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do T�tulo
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Cr�dito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }

        private string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 008, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "237BRADESCO", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "MX", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 277, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0007, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0013, 007, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0025, 005, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 007, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0037, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 003, 0, "0", '0');

                // 0=sem multa, 2=com multa (1, N)
                if (boleto.PercentualMulta > 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "2", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 004, 2, boleto.PercentualMulta, '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 004, 2, "0", '0');
                }

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 011, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0082, 001, 0, boleto.NossoNumeroDV, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0083, 010, 0, "0", '0');

                switch (boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto)
                {
                    case TipoImpressaoBoleto.Banco:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 001, 0, "1", '0');
                        break;
                    case TipoImpressaoBoleto.Empresa:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 001, 0, "2", '0');
                        break;
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0094, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0095, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0107, 002, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoOcorrencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, AjustaEspecieCnab400(boleto.EspecieDocumento), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0'); // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0'); // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Sacado.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), '0');
                if (IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // N�o tem avalista.
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 060, 0, Empty, ' ');
                }
                else if (boleto.Avalista.TipoCPFCNPJ("A") == "F")
                {
                    // Avalista Pessoa F�sica
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 009, 0, boleto.Avalista.CPFCNPJ.Substring(0, 9), '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0344, 004, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0348, 002, 0, boleto.Avalista.CPFCNPJ.Substring(9, 2), '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 043, 0, boleto.Avalista.Nome, ' ');
                }
                else
                {
                    // Avalista Pessoa Juridica
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 015, 0, boleto.Avalista.CPFCNPJ, '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 043, 0, boleto.Avalista.Nome, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 1.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro2(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (IsNullOrWhiteSpace(boleto.MensagemArquivoRemessa))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 320, 0, boleto.MensagemArquivoRemessa, ' '); // 4 campos de 80 caracteres cada.
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0322, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0328, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0341, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0347, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0360, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0367, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0370, 005, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0375, 007, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0382, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0383, 011, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0394, 001, 0, boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 2.", ex);
            }
        }

        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a gera��o do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada";
                case "06":
                    return "Liquida��o normal";
                case "09":
                    return "Baixado Automaticamente via Arquivo";
                case "10":
                    return "Baixado conforme instru��es da Ag�ncia";
                case "11":
                    return "Em Ser - Arquivo de T�tulos pendentes";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Liquida��o em Cart�rio";
                case "17":
                    return "Liquida��o ap�s baixa ou T�tulo n�o registrado";
                case "18":
                    return "Acerto de Deposit�ria";
                case "19":
                    return "Confirma��o Recebimento Instru��o de Protesto";
                case "20":
                    return "Confirma��o Recebimento Instru��o Susta��o de Protesto";
                case "21":
                    return "Acerto do Controle do Participante";
                case "23":
                    return "Entrada do T�tulo em Cart�rio";
                case "24":
                    return "Entrada rejeitada por CEP Irregular";
                case "27":
                    return "Baixa Rejeitada";
                case "28":
                    return "D�bito de tarifas/custas";
                case "30":
                    return "Altera��o de Outros Dados Rejeitados";
                case "32":
                    return "Instru��o Rejeitada";
                case "33":
                    return "Confirma��o Pedido Altera��o Outros Dados";
                case "34":
                    return "Retirado de Cart�rio e Manuten��o Carteira";
                case "35":
                    return "Desagendamento ) d�bito autom�tico";
                case "68":
                    return "Acerto dos dados ) rateio de Cr�dito";
                case "69":
                    return "Cancelamento dos dados ) rateio";
                default:
                    return "";
            }
        }

        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            switch (codigoEspecie)
            {
                case "01":
                    return TipoEspecieDocumento.DM;
                case "02":
                    return TipoEspecieDocumento.NP;
                case "03":
                    return TipoEspecieDocumento.NS;
                case "05":
                    return TipoEspecieDocumento.RC;
                case "10":
                    return TipoEspecieDocumento.LC;
                case "11":
                    return TipoEspecieDocumento.ND;
                case "12":
                    return TipoEspecieDocumento.DS;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        private string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
        {
            switch (especieDocumento)
            {
                case TipoEspecieDocumento.DM:
                    return "01";
                case TipoEspecieDocumento.NP:
                    return "02";
                case TipoEspecieDocumento.NS:
                    return "03";
                case TipoEspecieDocumento.RC:
                    return "05";
                case TipoEspecieDocumento.LC:
                    return "10";
                case TipoEspecieDocumento.ND:
                    return "11";
                case TipoEspecieDocumento.DS:
                    return "12";
                default:
                    return "99";
            }
        }
        #endregion
    }
}