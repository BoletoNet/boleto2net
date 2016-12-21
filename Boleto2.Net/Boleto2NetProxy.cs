using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace Boleto2Net
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId(nameof(Boleto2NetProxy))]
    [ComVisible(true)]
    public class Boleto2NetProxy
    {
        // 1.00 - Janeiro/2016
        //      Bradesco - Carteira 09
        // 1.01 - Julho/2016
        //      Caixa Economica Federal - Carteira SIG14
        // 1.02 - Julho/2016
        //      Banco do Brasil - Carteira 17 Variação 019
        // 1.03 - Julho/2016
        //      Banco do Brasil - Carteira 11 Variação 019
        // 1.04 - Agosto/2016
        //      Refatoração completa da classe ArquivoRetorno
        // 1.05 - Novembro/2016
        //      Banco do Brasil - Retorno
        // 1.10 - Dezembro/2016
        //      github.com/BoletoNet/boleto2net

        readonly public string Versao = "1.10";

        private Boletos boletos = new Boletos();
        public int quantidadeBoletos { get { return boletos.Count; } }
        public Boleto boleto { get; set; }
        private bool setupOk { get; set; } = false;

        public bool SetupCobranca(string cnpj, string razaoSocial,
                                    string enderecoLogradouro, string enderecoNumero, string enderecoComplemento, string enderecoBairro, string enderecoCidade, string enderecoEstado, string enderecoCep, string observacoes,
                                    int numeroBanco, string agencia, string digitoAgencia, string operacaoConta, string conta, string digitoConta,
                                    string codigoCedente, string carteira, string variacaoCarteira,
                                    int tipoCarteira, int tipoFormaCadastramento, int tipoImpressaoBoleto,
                                    ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    mensagemErro += "Cnpj não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(razaoSocial))
                {
                    mensagemErro += "Razão Social não informada." + Environment.NewLine;
                }
                if (numeroBanco == 0)
                {
                    mensagemErro += "Banco não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(agencia))
                {
                    mensagemErro += "Agência não informada." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(digitoAgencia))
                {
                    mensagemErro += "Dígito da agência não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(conta))
                {
                    mensagemErro += "Conta não informada." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(digitoConta))
                {
                    mensagemErro += "Dígito da conta não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(carteira))
                {
                    mensagemErro += "Carteira não informada." + Environment.NewLine;
                }
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    return false;
                }
                if (tipoCarteira < 1 || tipoCarteira > 5)
                {
                    mensagemErro += "Tipo da Carteira inválida: 1-Simples, 2-Vinculada, 3-Caucionada, 4-Descontada, 5-Vendor" + Environment.NewLine;
                }
                if (tipoFormaCadastramento < 1 || tipoFormaCadastramento > 2)
                {
                    mensagemErro += "Tipo da Forma de Cadastramento inválida: 1-Com Registro, 2-Sem Registro" + Environment.NewLine;
                }
                if (tipoImpressaoBoleto < 1 || tipoImpressaoBoleto > 2)
                {
                    mensagemErro += "Tipo da Impressão do Boleto inválida: 1-Banco, 2-Empresa" + Environment.NewLine;
                }

                // Banco, Cedente, Conta Corrente
                boletos.Banco = new Banco(numeroBanco)
                {
                    Cedente = new Cedente
                    {
                        CPFCNPJ = cnpj,
                        Nome = razaoSocial,
                        Observacoes = observacoes,
                        ContaBancaria = new ContaBancaria
                        {
                            Agencia = agencia,
                            DigitoAgencia = digitoAgencia,
                            OperacaoConta = operacaoConta,
                            Conta = conta,
                            DigitoConta = digitoConta,
                            Carteira = carteira,
                            VariacaoCarteira = variacaoCarteira,
                            TipoCarteira = (TipoCarteira)tipoCarteira,
                            TipoFormaCadastramento = (TipoFormaCadastramento)tipoFormaCadastramento,
                            TipoImpressaoBoleto = (TipoImpressaoBoleto)tipoImpressaoBoleto
                           },
                        Codigo = codigoCedente,
                        Endereco = new Endereco
                        {
                            LogradouroEndereco = enderecoLogradouro,
                            LogradouroNumero = enderecoNumero,
                            LogradouroComplemento = enderecoComplemento,
                            Bairro = enderecoBairro,
                            Cidade = enderecoCidade,
                            UF = enderecoEstado,
                            CEP = enderecoCep
                        }
                    }
                };
                boletos.Banco.FormataCedente();
                setupOk = true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
            return true;
        }
        public bool AcessarBoletoDocumento(string documento, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (string.IsNullOrWhiteSpace(documento))
                {
                    mensagemErro = "Documento não informado.";
                    return false;
                }
                foreach (Boleto boleto in boletos)
                {
                    if (documento == boleto.NumeroDocumento)
                    {
                        this.boleto = boleto;
                        return true;
                    }
                }
                mensagemErro = "Boleto não encontrado: " + documento;
                return false;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool AcessarBoletoIndice(int index, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                boleto = boletos[index];
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool NovoBoleto(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto = new Boleto
                {
                    Banco = boletos.Banco
                };
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirSacado(string cnpj, string razaoSocial, string endereco, string numero, string complemento, string bairro, string cidade, string uf, string cep, string observacoes, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    mensagemErro += "Cnpj não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(razaoSocial))
                {
                    mensagemErro += "Razão Social não informada." + Environment.NewLine;
                }
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    return false;
                }
                var sacado = new Sacado
                {
                    CPFCNPJ = cnpj,
                    Nome = razaoSocial,
                    Observacoes = observacoes,
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = endereco,
                        LogradouroNumero = numero,
                        LogradouroComplemento = complemento,
                        Bairro = bairro,
                        Cidade = cidade,
                        UF = uf,
                        CEP = cep
                    }
                };
                boleto.Sacado = sacado;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirBoleto(string siglaEspecieDocumento, string numeroDocumento, string nossoNumero, DateTime dataEmissao, DateTime dataProcessamento, DateTime dataVencimento, Decimal valorBoleto, string nrControle, string aceite, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.NumeroDocumento = numeroDocumento;
                boleto.NumeroControleParticipante = nrControle;
                boleto.NossoNumero = nossoNumero;
                boleto.DataEmissao = dataEmissao;
                boleto.DataProcessamento = dataProcessamento;
                boleto.DataVencimento = dataVencimento;
                boleto.ValorTitulo = valorBoleto;
                boleto.Aceite = aceite;
                boleto.SiglaEspecieDocumento = siglaEspecieDocumento;
                return true;

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirDesconto(DateTime dataDesconto, decimal valorDesconto, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.DataDesconto = dataDesconto;
                boleto.ValorDesconto = valorDesconto;
                return true;

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirMulta(DateTime dataMulta, decimal valorMulta, decimal percMulta, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.DataMulta = dataMulta;
                boleto.PercentualMulta = percMulta;
                boleto.ValorMulta = valorMulta;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirJuros(DateTime dataJuros, decimal valorJuros, decimal percJuros, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.DataJuros = dataJuros;
                boleto.PercentualJuros = percJuros;
                boleto.ValorJuros = valorJuros;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirInstrucoes(string instrucoesCaixa, string mensagemRemessa, string instrucao1, string instrucao1Aux, string instrucao2, string instrucao2Aux, string instrucao3, string instrucao3Aux, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.MensagemInstrucoesCaixa = instrucoesCaixa;
                boleto.MensagemArquivoRemessa = mensagemRemessa;
                boleto.CodigoInstrucao1 = instrucao1;
                boleto.ComplementoInstrucao1 = instrucao1Aux;
                boleto.CodigoInstrucao2 = instrucao2;
                boleto.ComplementoInstrucao2 = instrucao2Aux;
                boleto.CodigoInstrucao3 = instrucao3;
                boleto.ComplementoInstrucao3 = instrucao3Aux;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool FecharBoleto(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.Valida();
                boletos.Add(boleto);
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool LimparBoletos(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boletos.Clear();
                return true;

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool GerarBoletos(string nomeArquivo, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    mensagemErro = "Nome do arquivo não informado." + Environment.NewLine;
                    return false;
                }
                if (quantidadeBoletos == 0)
                {
                    mensagemErro = "Nenhum boleto encontrado." + Environment.NewLine;
                    return false;
                }
                var extensaoArquivo = nomeArquivo.Substring(nomeArquivo.Length - 3).ToUpper();
                if (extensaoArquivo != "HTM" && extensaoArquivo != "PDF")
                {
                    mensagemErro = "Tipo do arquivo inválido: HTM ou PDF" + Environment.NewLine;
                    return false;
                }
                var html = new StringBuilder();
                foreach (Boleto boletoTmp in boletos)
                {
                    if (html.Length != 0)
                    {
                        // Já existe um boleto, inclui quebra de linha.
                        html.Append("</br></br></br></br></br></br></br></br></br></br>");
                    }
                    using (BoletoBancario imprimeBoleto = new BoletoBancario
                    {
                        //CodigoBanco = (short)boletoTmp.Banco.Codigo,
                        boleto = boletoTmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = true,
                        MostrarEnderecoCedente = true
                    })
                    {
                        html.Append(imprimeBoleto.MontaHtml());
                    }
                }
                switch (extensaoArquivo.ToUpper())
                {
                    case "HTM":
                        GerarArquivoTexto(html.ToString(), nomeArquivo);
                        break;
                    case "PDF":
                        GerarArquivoPDF(html.ToString(), nomeArquivo);
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool GerarRemessa(int formatoArquivo, string nomeArquivo, int sequenciaArquivo, ref string mensagemErro)
        {
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                if (formatoArquivo != 0 & formatoArquivo != 1)
                {
                    // Formato do Arquivo - CNAB240 = 0 / CNAB400 = 1
                    mensagemErro = "Tipo do arquivo inválido: 0-CNAB240, 1-CNAB400";
                    return false;
                }
                var arquivoRemessa = new ArquivoRemessa(boletos.Banco, (TipoArquivo)formatoArquivo, sequenciaArquivo);
                using (var fileStream = new FileStream(nomeArquivo, FileMode.Create))
                {
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool LerRetorno(int formatoArquivo, string nomeArquivo, ref string mensagemErro)
        {
            try
            {
                if (boletos.Banco == null)
                {
                    mensagemErro = "Banco não definido.";
                    return false;
                }
                if (formatoArquivo != 0 & formatoArquivo != 1)
                {
                    // Formato do Arquivo - CNAB240 = 0 / CNAB400 = 1
                    mensagemErro = "Tipo do arquivo inválido: 0-CNAB240, 1-CNAB400";
                    return false;
                }
                boletos.Clear();

                var arquivoRetorno = new ArquivoRetorno(boletos.Banco, (TipoArquivo)formatoArquivo);
                using (var fileStream = new FileStream(nomeArquivo, FileMode.Open))
                {
                    boletos = arquivoRetorno.LerArquivoRetorno(fileStream);
                }

                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }

        private void GerarArquivoTexto(string texto, string nomeArquivo)
        {
            using (FileStream fs = new FileStream(nomeArquivo, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(texto);
                    sw.Close();
                    fs.Close();
                }
            }
        }
        private void GerarArquivoPDF(string html, string nomeArquivo)
        {
            var pdf = new NReco.PdfGenerator.HtmlToPdfConverter().GeneratePdf(html);
            using (FileStream fs = new FileStream(nomeArquivo, FileMode.Create))
            {
                fs.Write(pdf, 0, pdf.Length);
                fs.Close();
            }
        }
    }

}
