using System;
using System.Web.UI;

[assembly: WebResource("BoletoNet.Imagens.237.jpg", "image/jpg")]

namespace Boleto2Net
{
    internal class Banco_Bradesco : AbstractBanco, IBanco
    {
        internal Banco_Bradesco()
        {
            this.Codigo = 237;
            this.Digito = "2";
            this.Nome = "Bradesco";
            this.IdsRegistroDetalheCnab400.Add("1");
        }

        public override void FormataCedente()
        {

            if (this.Cedente.ContaBancaria.Agencia.Length > 4)
                throw new Exception("O número da agência (" + this.Cedente.ContaBancaria.Agencia + ") deve conter 4 dígitos.");
            else if (this.Cedente.ContaBancaria.Agencia.Length < 4)
                this.Cedente.ContaBancaria.Agencia = this.Cedente.ContaBancaria.Agencia.PadLeft(4, '0');

            if (this.Cedente.ContaBancaria.Conta.Length > 7)
                throw new Exception("O número da conta (" + this.Cedente.ContaBancaria.Conta + ") deve conter 7 dígitos.");
            else if (this.Cedente.ContaBancaria.Conta.Length < 7)
                this.Cedente.ContaBancaria.Conta = this.Cedente.ContaBancaria.Conta.PadLeft(7, '0');

            var codigoCedenteSemFormatacao = this.Cedente.Codigo;

            if (this.Cedente.Codigo.Length > 20)
                throw new Exception("O código do Cedente (" + this.Cedente.Codigo + ") deve conter 20 dígitos.");
            else if (this.Cedente.Codigo.Length < 20)
                this.Cedente.Codigo = this.Cedente.Codigo.PadLeft(20, '0');

            this.Cedente.CodigoDV = "";

            this.Cedente.CodigoFormatado = String.Format("{0}/{1}", this.Cedente.ContaBancaria.Agencia, codigoCedenteSemFormatacao);

            this.Cedente.ContaBancaria.LocalPagamento = "ATÉ O VENCIMENTO EM QUALQUER BANCO. APÓS O VENCIMENTO SOMENTE NO BRADESCO.";

            if (this.Cedente.ContaBancaria.CarteiraComVariacao != "09")
            {
                throw new NotImplementedException("Carteira não implementada: " + this.Cedente.ContaBancaria.CarteiraComVariacao);
            }

        }

        public override void ValidaBoleto(Boleto boleto)
        {

            switch (boleto.SiglaEspecieDocumento)
            {
                case "DM":
                    boleto.CodigoEspecieDocumento = "01";
                    break;
                case "NP":
                    boleto.CodigoEspecieDocumento = "02";
                    break;
                case "ND":
                    boleto.CodigoEspecieDocumento = "11";
                    break;
                case "DS":
                    boleto.CodigoEspecieDocumento = "12";
                    break;
                default:
                    throw new Exception("Espécie do documento (" + boleto.SiglaEspecieDocumento + ") inválida. Informe: DM, NP, ND ou DS.");
            }

        }

        public override void FormataNossoNumero(Boleto boleto)
        {
            if (boleto.Banco.Cedente.ContaBancaria.CarteiraComVariacao == "09")
            {
                FormataNossoNumero09(boleto);
            }
            else
            {
                throw new NotImplementedException("Não foi possível formatar o nosso número do boleto.");
            }
        }

        private static void FormataNossoNumero09(Boleto boleto)
        {
            // Carteira 09: Dúvida: Não sei se na carteira 09, o banco também emite o boleto. Se emitir, será necessário tirar a trava do nosso número em branco:
            // Se for só a empresa, devemos tratar aqui, que o nosso número não
            // O nosso número não pode ser em branco.
            if (String.IsNullOrWhiteSpace(boleto.NossoNumero))
            {
                throw new Exception("Nosso Número não informado.");
            }
            else
            {
                // Nosso número não pode ter mais de 11 dígitos
                if (boleto.NossoNumero.Length > 11)
                    throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve conter 11 dígitos.");
                else
                    boleto.NossoNumero = boleto.NossoNumero.PadLeft(11, '0');
            }
            boleto.NossoNumeroDV = Utils.Modulo11(boleto.Banco.Cedente.ContaBancaria.Carteira + boleto.NossoNumero, 7, Modulo11Algoritmo.Bradesco);
            boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.Banco.Cedente.ContaBancaria.Carteira, boleto.NossoNumero, boleto.NossoNumeroDV);
        }

        public override string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string FormataCampoLivre = "";
            if (boleto.Banco.Cedente.ContaBancaria.CarteiraComVariacao == "09")
            {
                /// Campo Livre
                ///    20 a 23 -  4 - Agência Cedente (Sem o digito verificador,completar com zeros a esquerda quandonecessário)
                ///    24 a 25 -  2 - Carteira
                ///    26 a 36 - 11 - Número do Nosso Número(Sem o digito verificador)
                ///    37 a 43 -  7 - Conta do Cedente (Sem o digito verificador,completar com zeros a esquerda quando necessário)
                ///    44 a 44	- 1 - Zero            
                FormataCampoLivre = string.Format("{0}{1}{2}{3}{4}",
                                                    boleto.Banco.Cedente.ContaBancaria.Agencia,
                                                    boleto.Banco.Cedente.ContaBancaria.Carteira,
                                                    boleto.NossoNumero,
                                                    boleto.Banco.Cedente.ContaBancaria.Conta,
                                                    "0");
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

                    case TipoArquivo.CNAB400:
                        _header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return _header;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do HEADER do arquivo de REMESSA.", ex);
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
                    case TipoArquivo.CNAB400:
                        _detalhe += GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);
                        // Segmento R (Opcional)
                        _strline = this.GerarDetalheRemessaCNAB400Registro2(boleto, ref numeroRegistro);
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
                    case TipoArquivo.CNAB400:
                        _trailer = GerarTrailerRemessaCNAB400(ref numeroRegistroGeral);
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, this.Cedente.Codigo.ToString(), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, this.Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "237BRADESCO", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 008, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "MX", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 277, 0, string.Empty, ' ');
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
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0007, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0013, 007, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 003, 0, boleto.Banco.Cedente.ContaBancaria.Carteira, '0');
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

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0094, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0095, 010, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0107, 002, 0, string.Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "01", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, boleto.CodigoEspecieDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJuros, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0');   // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0');   // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Sacado.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), '0');
                if (string.IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // Não tem avalista.
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 060, 0, string.Empty, ' ');
                }
                else if (boleto.Avalista.TipoCPFCNPJ("00") == "01")
                {
                    // Avalista Pessoa Física
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 009, 0, boleto.Avalista.CPFCNPJ.Substring(0, 9), '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0344, 004, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0348, 002, 0, boleto.Avalista.CPFCNPJ.Substring(9, 2), '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 043, 0, boleto.Avalista.Nome, ' ');
                }
                else
                {
                    // Avalista Pessoa Juridica
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 015, 0, boleto.Avalista.CPFCNPJ, '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 043, 0, boleto.Avalista.Nome, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro2(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(boleto.MensagemArquivoRemessa))
                    return "";

                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 320, 0, boleto.MensagemArquivoRemessa, ' '); // 4 campos de 80 caracteres cada.
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0322, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0328, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0341, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0347, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0360, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0367, 003, 0, boleto.Banco.Cedente.ContaBancaria.Carteira, '0');
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
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

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

        public override void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira
                boleto.Banco.Cedente.ContaBancaria.Carteira = registro.Substring(107, 1).PadLeft(2, '0');

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(70, 11);//Sem o DV
                boleto.NossoNumeroDV = registro.Substring(81, 1); //DV
                boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.Banco.Cedente.ContaBancaria.Carteira, boleto.NossoNumero, boleto.NossoNumeroDV);

                //Identificação de Ocorrência
                boleto.CodigoOcorrencia = registro.Substring(108, 2);
                boleto.DescricaoOcorrencia = OcorrenciaCnab400Bradesco(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(318, 10);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);

                //Espécie do Título
                boleto.CodigoEspecieDocumento = registro.Substring(173, 2);

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = (Convert.ToDecimal(registro.Substring(175, 13)) / 100);
                boleto.ValorOutrasDespesas = (Convert.ToDecimal(registro.Substring(188, 13)) / 100);
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJuros = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

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

        private string OcorrenciaCnab400Bradesco(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada";
                case "06":
                    return "Liquidação normal";
                case "09":
                    return "Baixado Automaticamente via Arquivo";
                case "10":
                    return "Baixado conforme instruções da Agência";
                case "11":
                    return "Em Ser - Arquivo de Títulos pendentes";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Liquidação em Cartório";
                case "17":
                    return "Liquidação após baixa ou Título não registrado";
                case "18":
                    return "Acerto de Depositária";
                case "19":
                    return "Confirmação Recebimento Instrução de Protesto";
                case "20":
                    return "Confirmação Recebimento Instrução Sustação de Protesto";
                case "21":
                    return "Acerto do Controle do Participante";
                case "23":
                    return "Entrada do Título em Cartório";
                case "24":
                    return "Entrada rejeitada por CEP Irregular";
                case "27":
                    return "Baixa Rejeitada";
                case "28":
                    return "Débito de tarifas/custas";
                case "30":
                    return "Alteração de Outros Dados Rejeitados";
                case "32":
                    return "Instrução Rejeitada";
                case "33":
                    return "Confirmação Pedido Alteração Outros Dados";
                case "34":
                    return "Retirado de Cartório e Manutenção Carteira";
                case "35":
                    return "Desagendamento ) débito automático";
                case "68":
                    return "Acerto dos dados ) rateio de Crédito";
                case "69":
                    return "Cancelamento dos dados ) rateio";
                default:
                    return "";
            }
        }

        private string MotivoRejeicao(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "02-Código do registro detalhe inválido";
                case "03":
                    return "03-Código da ocorrência inválida";
                case "04":
                    return "04-Código de ocorrência não permitida para a carteira";
                case "05":
                    return "05-Código de ocorrência não numérico";
                case "07":
                    return "07-Agência/conta/Digito - Inválido";
                case "08":
                    return "08-Nosso número inválido";
                case "09":
                    return "09-Nosso número duplicado";
                case "10":
                    return "10-Carteira inválida";
                case "16":
                    return "16-Data de vencimento inválida";
                case "18":
                    return "18-Vencimento fora do prazo de operação";
                case "20":
                    return "20-Valor do Título inválido";
                case "21":
                    return "21-Espécie do Título inválida";
                case "22":
                    return "22-Espécie não permitida para a carteira";
                case "24":
                    return "24-Data de emissão inválida";
                case "38":
                    return "38-Prazo para protesto inválido";
                case "44":
                    return "44-Agência Cedente não prevista";
                case "50":
                    return "50-CEP irregular - Banco Correspondente";
                case "63":
                    return "63-Entrada para Título já cadastrado";
                case "68":
                    return "68-Débito não agendado - erro nos dados de remessa";
                case "69":
                    return "69-Débito não agendado - Sacado não consta no cadastro de autorizante";
                case "70":
                    return "70-Débito não agendado - Cedente não autorizado pelo Sacado";
                case "71":
                    return "71-Débito não agendado - Cedente não participa da modalidade de débito automático";
                case "72":
                    return "72-Débito não agendado - Código de moeda diferente de R$";
                case "73":
                    return "73-Débito não agendado - Data de vencimento inválida";
                case "74":
                    return "74-Débito não agendado - Conforme seu pedido, Título não registrado";
                case "75":
                    return "75-Débito não agendado - Tipo de número de inscrição do debitado inválido";
                default:
                    return "";
            }
        }

    }
}
