using System;
using System.Web.UI;

[assembly: WebResource("BoletoNet.Imagens.001.jpg", "image/jpg")]

namespace Boleto2Net
{
    internal class Banco_Brasil : AbstractBanco, IBanco
    {
        internal Banco_Brasil()
        {
            this.Codigo = 1;
            this.Digito = "9";
            this.Nome = "Banco do Brasil";
            this.IdsRetornoCnab400RegistroDetalhe.Add("7");
        }

        public override void FormataCedente()
        {
            if (this.Cedente.ContaBancaria.Agencia.Length > 4)
                throw new Exception("O número da agência (" + this.Cedente.ContaBancaria.Agencia + ") deve conter 4 dígitos.");
            else if (this.Cedente.ContaBancaria.Agencia.Length < 4)
                this.Cedente.ContaBancaria.Agencia = this.Cedente.ContaBancaria.Agencia.PadLeft(4, '0');

            if (this.Cedente.ContaBancaria.Conta.Length > 8)
                throw new Exception("O número da conta (" + this.Cedente.ContaBancaria.Conta + ") deve conter 8 dígitos.");
            else if (this.Cedente.ContaBancaria.Conta.Length < 8)
                this.Cedente.ContaBancaria.Conta = this.Cedente.ContaBancaria.Conta.PadLeft(8, '0');

            if (this.Cedente.Codigo.Length != 7)
                throw new Exception("O código do cedente (" + this.Cedente.Codigo + ") deve conter 7 dígitos.");

            this.Cedente.CodigoFormatado = String.Format("{0}/{1}", this.Cedente.ContaBancaria.Agencia, this.Cedente.Codigo);

            this.Cedente.ContaBancaria.LocalPagamento = "PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO. APÓS, ATUALIZE O BOLETO NO SITE BB.COM.BR";

            if (this.Cedente.ContaBancaria.CarteiraComVariacao != "11/019" & this.Cedente.ContaBancaria.CarteiraComVariacao != "17/019")
            {
                throw new NotImplementedException("Carteira não implementada: " + this.Cedente.ContaBancaria.CarteiraComVariacao);
            }

        }

        public override void ValidaBoleto(Boleto boleto)
        {
        }

        public override void FormataNossoNumero(Boleto boleto)
        {
            if (boleto.Banco.Cedente.ContaBancaria.CarteiraComVariacao == "11/019")
            {
                FormataNossoNumeroCarteira_11_019(boleto);
            }
            else if (boleto.Banco.Cedente.ContaBancaria.CarteiraComVariacao == "17/019")
            {
                FormataNossoNumeroCarteira_17_019(boleto);
            }
            else
            {
                throw new NotImplementedException("Não foi possível formatar o nosso número do boleto.");
            }
        }
        private void FormataNossoNumeroCarteira_11_019(Boleto boleto)
        {
            if (boleto.Banco.Cedente.Codigo.Length == 7)
            {
                // Carteira 11 - Variação 019: Convênio de 7 dígitos
                // Ou deve estar em branco (o banco irá gerar)
                // Ou deve estar com 17 posições, iniciando com o código do convênio
                if (!String.IsNullOrWhiteSpace(boleto.NossoNumero) & (boleto.NossoNumero.Length != 17 || !boleto.NossoNumero.StartsWith(boleto.Banco.Cedente.Codigo)))
                {
                    throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve iniciar com \"" + boleto.Banco.Cedente.Codigo + "\" e conter 17 dígitos.");
                }
                // Para convênios com 7 dígitos, não existe dígito de verificação do nosso número
                boleto.NossoNumeroDV = "";
                boleto.NossoNumeroFormatado = boleto.NossoNumero;
            }
            else
            {
                throw new Exception("Não foi possível formatar o nosso número.");
            }
        }
        private void FormataNossoNumeroCarteira_17_019(Boleto boleto)
        {
            // Carteira 17 - Variação 019: Cliente emite o boleto
            // O nosso número não pode ser em branco.
            if (String.IsNullOrWhiteSpace(boleto.NossoNumero))
            {
                throw new Exception("Nosso Número não informado.");
            }
            if (boleto.Banco.Cedente.Codigo.Length == 7)
            {
                // Se o convênio for de 7 dígitos,
                // o nosso número deve estar formatado corretamente (com 17 dígitos e iniciando com o código do convênio),
                if (boleto.NossoNumero.Length == 17)
                {
                    if (!boleto.NossoNumero.StartsWith(boleto.Banco.Cedente.Codigo))
                        throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve iniciar com \"" + boleto.Banco.Cedente.Codigo + "\" e conter 17 dígitos.");
                }
                else
                {
                    // ou deve ser informado com até 10 posições (será formatado para 17 dígitos pelo Boleto.Net).
                    if (boleto.NossoNumero.Length > 10)
                        throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve iniciar com \"" + boleto.Banco.Cedente.Codigo + "\" e conter 17 dígitos.");
                    else
                        boleto.NossoNumero = string.Format("{0}{1}", boleto.Banco.Cedente.Codigo, boleto.NossoNumero.PadLeft(10, '0'));
                }
                // Para convênios com 7 dígitos, não existe dígito de verificação do nosso número
                boleto.NossoNumeroDV = "";
                boleto.NossoNumeroFormatado = boleto.NossoNumero;
            }
            else
            {
                throw new NotImplementedException("Não foi possível formatar o nosso número.");
            }
        }

        public override string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string FormataCampoLivre = "";
            if (boleto.Banco.Cedente.Codigo.Length == 7)
            {
                if (String.IsNullOrWhiteSpace(boleto.NossoNumero) || String.IsNullOrWhiteSpace(boleto.Banco.Cedente.ContaBancaria.Carteira))
                {
                    FormataCampoLivre = "";
                }
                else
                {
                    FormataCampoLivre = string.Format("{0}{1}{2}",
                                                "000000",
                                                boleto.NossoNumero,
                                                boleto.Banco.Cedente.ContaBancaria.Carteira);
                }
            }
            else
            {
                throw new NotImplementedException("Não foi possível formatar o campo livre do código de barras do boleto.");
            }
            return FormataCampoLivre;
        }

        public override string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                string _header = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Cabeçalho do Arquivo
                        _header += GerarHeaderRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        // Cabeçalho do Lote
                        _header += Environment.NewLine;
                        _header += GerarHeaderLoteRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    case TipoArquivo.CNAB400:
                        _header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Header - Tipo de arquivo inexistente.");
                }
                return _header;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do Header do arquivo de REMESSA.", ex);
            }
        }

        public override string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                string _detalhe = String.Empty;
                string _strline = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Segmento P (Obrigatório)
                        _detalhe += this.GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro);

                        // Segmento Q (Obrigatório)
                        _detalhe += Environment.NewLine;
                        _detalhe += this.GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);

                        // Segmento R (Opcional)
                        _strline = this.GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
                        if (!String.IsNullOrWhiteSpace(_strline))
                        {
                            _detalhe += Environment.NewLine;
                            _detalhe += _strline;
                        }

                        break;

                    case TipoArquivo.CNAB400:

                        // Registro 7 - Obrigatório
                        _detalhe += GerarDetalheRemessaCNAB400Registro7(boleto, ref numeroRegistro);

                        // Registro 5 - Registro Opcional - Multa
                        _strline = GerarDetalheRemessaCNAB400Registro5Multa(boleto, ref numeroRegistro);
                        if (!String.IsNullOrWhiteSpace(_strline))
                        {
                            _detalhe += Environment.NewLine;
                            _detalhe += _strline;
                        }

                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return _detalhe;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do DETALHE arquivo de REMESSA.", ex);
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
                string _trailer = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Trailler do Lote
                        _trailer += GerarTrailerLoteRemessaCNAB240(ref numeroRegistroGeral);
                        // Trailler do Arquivo
                        _trailer += Environment.NewLine;
                        _trailer += GerarTrailerRemessaCNAB240(ref numeroRegistroGeral);
                        break;

                    case TipoArquivo.CNAB400:
                        _trailer += GerarTrailerRemessaCNAB400(ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return _trailer;
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }


        #region Remessa - CNAB240
        private string GerarHeaderRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, this.Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, this.Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0033, 009, 0, this.Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0042, 004, 0, "0014", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0046, 002, 0, this.Cedente.ContaBancaria.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0048, 003, 0, this.Cedente.ContaBancaria.VariacaoCarteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0051, 002, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 005, 0, this.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, this.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 012, 0, this.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 001, 0, this.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, this.Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "BANCO DO BRASIL S.A.", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 0152, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0167, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 020, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0192, 020, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0212, 029, 0, string.Empty, ' ');
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
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "R", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 002, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0014, 003, 0, "000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, this.Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, this.Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0034, 009, 0, this.Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0043, 004, 0, "0014", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 002, 0, this.Cedente.ContaBancaria.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0049, 003, 0, this.Cedente.ContaBancaria.VariacaoCarteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0052, 002, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 005, 0, this.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0059, 001, 0, this.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0060, 012, 0, this.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, this.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 030, 0, this.Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0104, 040, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0144, 040, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0184, 008, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0192, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0200, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 033, 0, string.Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB400.", ex);
            }
        }
        private string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "P", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 005, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 012, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0037, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 020, 0, boleto.NossoNumero, ' ');
                int tipoCarteira = (int)boleto.Banco.Cedente.ContaBancaria.TipoCarteira;
                if (boleto.Banco.Cedente.ContaBancaria.Carteira == "17" & tipoCarteira == 1)
                {
                    tipoCarteira = 7; // Informar 7 – para carteira 17 modalidade Simples.
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0058, 001, 0, tipoCarteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoFormaCadastramento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0060, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0062, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 015, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0078, 008, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 015, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 2, boleto.EspecieDocumento.ToString(), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0110, 008, 0, boleto.DataEmissao, '0');
                if (boleto.ValorJurosDia == 0)
                {
                    // Sem Juros Mora
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 0, "3", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0119, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, 0, '0');
                }
                else {
                    // Com Juros Mora ($)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 0, "1", '0');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, (int)boleto.CodigoProtesto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, boleto.DiasProtesto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, (int)boleto.CodigoBaixaDevolucao, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0225, 003, 0, boleto.DiasBaixaDevolucao, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0228, 002, 0, "09", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0230, 010, 2, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0240, 001, 0, string.Empty, ' ');
                reg.CodificarLinha();
                string vLinha = reg.LinhaRegistro;
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
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "Q", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, string.Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0210, 003, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 020, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0233, 008, 0, string.Empty, ' ');
                reg.CodificarLinha();
                string vLinha = reg.LinhaRegistro;
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
                var CodMulta = "0";
                if (boleto.ValorMulta > 0)
                    CodMulta = "1";
                var msg3 = Utils.FitStringLength(boleto.MensagemArquivoRemessa.PadRight(500, ' ').Substring(00, 40), 40, 40, ' ', 0, true, true, false);
                if (CodMulta == "0" & String.IsNullOrWhiteSpace(msg3))
                    return "";

                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "R", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0043, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0051, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, CodMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0067, 008, 0, boleto.DataMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 015, 2, boleto.ValorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 010, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0100, 040, 0, msg3, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 040, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 020, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0200, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0211, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0216, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0217, 012, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0229, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0230, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0232, 009, 0, string.Empty, ' ');
                reg.CodificarLinha();
                string vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240.", ex);
            }
        }
        private string GerarTrailerLoteRemessaCNAB240(ref int numeroRegistroGeral)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                int numeroRegistrosNoLote = numeroRegistroGeral + 2;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0024, 217, 0, string.Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB400.", ex);
            }
        }
        private string GerarTrailerRemessaCNAB240(ref int numeroRegistroGeral)
        {
            try
            {
                // O número de registros no arquivo é igual ao número de registros gerados + 4 (header e trailler do lote / header e trailler do arquivo)
                int numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 006, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 205, 0, string.Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        #endregion

        #region Remessa - CNAB400
        private string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 008, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, this.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, this.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 008, 0, this.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0040, 001, 0, this.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0041, 006, 0, "000000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, this.Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "001BANCODOBRASIL", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 022, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0130, 007, 0, this.Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0137, 258, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }
        private string GerarDetalheRemessaCNAB400Registro7(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "7", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0023, 008, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0032, 007, 0, boleto.Banco.Cedente.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0039, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0064, 017, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0081, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0083, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0085, 003, 0, string.Empty, ' ');
                if (boleto.Avalista.Nome == string.Empty)
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0088, 001, 0, string.Empty, ' ');
                else
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0088, 001, 0, string.Empty, 'A');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0089, 003, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0092, 003, 0, boleto.Banco.Cedente.ContaBancaria.VariacaoCarteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0095, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0096, 006, 0, "0", '0');
                switch (boleto.Banco.Cedente.ContaBancaria.TipoCarteira)
                {
                    case TipoCarteira.CarteiraCobrancaSimples:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, string.Empty, ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVinculada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, "02VIN", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaDescontada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, "04DSC", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVendor:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, "08VDR", ' ');
                        break;
                    default:
                        throw new Exception("Tipo de carteira não suportada: (" + boleto.Banco.Cedente.ContaBancaria.TipoCarteira.ToString() + ").");
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, boleto.Banco.Cedente.ContaBancaria.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "01", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0147, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, AjustaEspecieCnab400(boleto.EspecieDocumento), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0');   // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0');   // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Sacado.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 037, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0272, 003, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Sacado.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Sacado.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Sacado.Endereco.UF, ' ');
                if (string.IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // Não tem avalista, utiliza a mensagem para o sacado
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 040, 0, boleto.MensagemArquivoRemessa, ' ');
                }
                else if (boleto.Avalista.TipoCPFCNPJ("A") == "F")
                {
                    // Avalista Pessoa Física
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 025, 0, boleto.Avalista.Nome, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0377, 001, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0378, 003, 0, "CPF", ' ');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0381, 011, 0, boleto.Avalista.CPFCNPJ, ' ');
                }
                else
                {
                    // Avalista Pessoa Juridica
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 021, 0, boleto.Avalista.Nome, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0373, 001, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0374, 004, 0, "CNPJ", ' ');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0378, 014, 0, boleto.Avalista.CPFCNPJ, ' ');
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0392, 002, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0394, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }
        private string GerarDetalheRemessaCNAB400Registro5Multa(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (boleto.ValorMulta == 0)
                    return "";
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, "99", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0005, 006, 0, boleto.DataMulta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0011, 012, 2, boleto.ValorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 372, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }
        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' ');            //001-001
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, string.Empty, ' ');   //002-393
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0'); //395-400
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }
        #endregion

        #region Retorno - CNAB400
        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                {
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public override void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(38, 25);

                //Carteira
                boleto.Banco.Cedente.ContaBancaria.Carteira = registro.Substring(106, 2);
                boleto.Banco.Cedente.ContaBancaria.VariacaoCarteira = registro.Substring(091, 3);
                switch (registro.Substring(80, 1))
                {
                    case "2":
                        boleto.Banco.Cedente.ContaBancaria.TipoCarteira = TipoCarteira.CarteiraCobrancaVinculada;
                        break;
                    case "4":
                        boleto.Banco.Cedente.ContaBancaria.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;
                        break;
                    case "8":
                        boleto.Banco.Cedente.ContaBancaria.TipoCarteira = TipoCarteira.CarteiraCobrancaVendor;
                        break;
                    default:
                        boleto.Banco.Cedente.ContaBancaria.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(63, 17); //Sem o DV
                boleto.NossoNumeroDV = ""; //DV
                boleto.NossoNumeroFormatado = boleto.NossoNumero;

                //Identificação de Ocorrência
                boleto.CodigoOcorrencia = registro.Substring(108, 2);
                boleto.DescricaoOcorrencia = DescricaoOcorrenciaCnab400(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(86, 2);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = (Convert.ToDecimal(registro.Substring(181, 7)) / 100);
                boleto.ValorOutrasDespesas = (Convert.ToDecimal(registro.Substring(188, 13)) / 100);
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(175, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public override void LerTrailerRetornoCNAB400(string registro)
        {
        }
        #endregion

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
                case "08":
                    return TipoEspecieDocumento.LC;
                case "10":
                    return TipoEspecieDocumento.CH;
                case "12":
                    return TipoEspecieDocumento.DS;
                case "13":
                    return TipoEspecieDocumento.ND;
                case "15":
                    return TipoEspecieDocumento.AP;
                case "25":
                    return TipoEspecieDocumento.DAU;
                case "26":
                    return TipoEspecieDocumento.DAE;
                case "27":
                    return TipoEspecieDocumento.DAM;
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
                    return "08";
                case TipoEspecieDocumento.CH:
                    return "10";
                case TipoEspecieDocumento.DS:
                    return "12";
                case TipoEspecieDocumento.ND:
                    return "13";
                case TipoEspecieDocumento.AP:
                    return "15";
                case TipoEspecieDocumento.DAU:
                    return "25";
                case TipoEspecieDocumento.DAE:
                    return "26";
                case TipoEspecieDocumento.DAM:
                    return "27";
                default:
                    return "99";
            }
        }
        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Confirmação de Entrada de Título";
                case "03":
                    return "Comando recusado";
                case "05":
                    return "Liquidado sem registro";
                case "06":
                    return "Liquidação normal";
                case "07":
                    return "Liquidação por Conta/Parcial";
                case "08":
                    return "Liquidação por Saldo";
                case "09":
                    return "Baixa de Titulo";
                case "10":
                    return "Baixa Solicitada";
                case "11":
                    return "Títulos em Ser";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Alteração de Vencimento do título";
                case "15":
                    return "Liquidação em Cartório";
                case "16":
                    return "Confirmação de alteração de juros de mora";
                case "19":
                    return "Confirmação de recebimento de instruções para protesto";
                case "20":
                    return "Débito em Conta";
                case "21":
                    return "Alteração do Nome do Sacado";
                case "22":
                    return "Alteração do Endereço do Sacado";
                case "23":
                    return "Indicação de encaminhamento a cartório";
                case "24":
                    return "Sustar Protesto";
                case "25":
                    return "Dispensar Juros de mora";
                case "26":
                    return "Alteração do número do título dado pelo Cedente";
                case "28":
                    return "Manutenção de titulo vencido";
                case "31":
                    return "Conceder desconto";
                case "32":
                    return "Não conceder desconto";
                case "33":
                    return "Retificar desconto";
                case "34":
                    return "Alterar data para desconto";
                case "35":
                    return "Cobrar Multa";
                case "36":
                    return "Dispensar Multa";
                case "37":
                    return "Dispensar Indexador";
                case "38":
                    return "Dispensar prazo limite para recebimento";
                case "39":
                    return "Alterar prazo limite para recebimento";
                case "41":
                    return "Alteração do número do controle do participante";
                case "42":
                    return "Alteração do número do documento do sacado";
                case "44":
                    return "Título pago com cheque devolvido";
                case "46":
                    return "Título pago com cheque, aguardando compensação";
                case "72":
                    return "Alteração de tipo de cobrança";
                case "73":
                    return "Confirmação de Instrução de Parâmetro de Pagamento Parcial";
                case "96":
                    return "Despesas de Protesto";
                case "97":
                    return "Despesas de Sustação de Protesto";
                case "98":
                    return "Débito de Custas Antecipadas";
                default:
                    return "";
            }
        }

    }
}
