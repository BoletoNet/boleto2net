using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Boleto2Net
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId(nameof(Boleto2NetProxy))]
    [ComVisible(true)]
    public class Boleto2NetProxy
    {
        // Esta classe é para permitir que essa DLL seja utilizada via COM, em linguagens que não usam o .NET - Por exemplo: Visual Fox Pro.
        // Janeiro/2016
        //      1.00 - Bradesco - Carteira 09
        // Julho/2016
        //      1.01 - Caixa Economica Federal - Carteira SIG14
        //      1.02 - Banco do Brasil - Carteira 17 Variação 019
        //      1.03 - Banco do Brasil - Carteira 11 Variação 019
        // Agosto/2016
        //      1.04 - Refatoração completa da classe ArquivoRetorno
        // Novembro/2016
        //      1.05 - Banco do Brasil - Retorno
        // Dezembro/2016
        //      1.10 - github.com/BoletoNet/boleto2net
        //      1.11 - Sicoob - Carteira 1 Variação 01
        //      1.12 - Classe Proxy - Métodos para definir Protesto e BaixaDevolucao / Quebra de página utilizando css - page-break-after
        //      1.13 - Correção Carteira Bradesco com 2 dígitos no retorno.
        //      1.14 - Correção Código da Espécie do Documento / Correções de Homologação do Sicoob
        // Abril/2017
        //      1.20 - Santander - Carteira 101
        //      1.21 - Correção na impressão do CNPJ do sacado (estava formatado como CPF)
        //      1.22 - Correção na impressão dos dados do beneficiario
        //      1.23 - Correções Santander
        //      1.24 - Ajuste Caixa - Carteira SIG14 - FormataNossoNumero
        //      1.25 - Santander - Carteira 101 Homologada
        //      1.26 - Itaú - Carteira 109 Homologada
        // Junho/2017
        //      1.30 - Correções no método FormataCedente e suas exceções - Quantidade de dígitos da conta e código do cedente
        //      1.31 - Correções na leitura do retorno do Itaú
        //      1.40 - Inclusao das propriedades Carteira, VariacaoCarteira, TipoCarteira também na classe Boleto.
        //             As propriedades que já existiam na classe ContaCorrente, foram renomeadas para CarteiraPadrao, VariacaoCarteiraPadrao, TipoCarteiraPadrao
        //             A classe Boleto passou a ter um construtor obrigatorio, onde recebe um objeto Banco, e já prepara as 3 propriedades com o padrão definido.
        //             Alteração foi necessária pois existem retornos com boletos em diferentes carteiras, e do modo que estava, aceitava apenas uma carteira para toda a lista de boletos.
        // Julho/2017
        //      1.41 - Banco do Brasil - Carteira 17 Variação 027
        //      1.42 - Banco do Brasil - Correção BB - Ficha de Compensação - AGÊNCIA/CÓDIGO DO BENEFICIÁRIO: Informe o prefixo da agência e número da conta de relacionamento com o BB no formato AAAA-Z / CCCCC-Z
        // Agosto/2017
        //      1.43 - Classe Boleto = Ajuste das propriedades ValorPago e ValorPagoCredito
        //      1.44 - Banco do Brasil - Correção BB - Ficha de Compensação - AGÊNCIA/CÓDIGO DO BENEFICIÁRIO: Informe o prefixo da agência e número da conta de relacionamento com o BB no formato AAAA-Z / CCCCC-Z
        //             Alteração na Impressão do Boleto Bancário: Comprovante de Entrega - Alterado de "Agência / Código do Cedente" para "Agência / Código do Beneficiário"
        //      1.45 - Santander - Correção na posição dos campos na leitura do arquivo retorno (Cnab240), segmento T.
        //             Santander - Correção do nosso número (de 7 para 12 posições) + dígito verificador = total 13 posições.
        //      1.46 - PullRequest #37
        // Novembro/2017
        //      1.50 - Caso não for definido o Nosso Número, Boleto2Net não irá gerar o código de barras e linha digitável.
        //      1.51 - Banrisul - Carteira 1
        //      1.52 - ContaBancaria.MensagemFixaTopoBoleto / Permite informar uma mensagem no topo do boleto (Ex: SAC BANRISUL: 0800 ...)
        //             IBanco.RemoveAcentosArquivoRemessa = true / Por padrão, todos os bancos retiram os acentos ao gerar o arquivo remessa.
        //      1.53 - Itaú - Carteira 112
        // Janeiro/2018
        //      1.54 - Itaú - Arquivo Remessa (Permite informar mensagem com 30 ou 40 caracteres, conforme instrução 93 ou 94)
        // Fevereiro/2018
        //      1.55 - Itaú - Revisão do cálculo Arquivo Remessa (Permite informar mensagem com 30 ou 40 caracteres, conforme instrução 93 ou 94)
        // Maio/2018
        //      1.56 - Itaú - Carteira 112 - Ajuste no cálculo do Nosso Número
        // Agosto/2018
        //      1.60 - Ajuste Boleto Padrão Caixa Econônica Federal (Mensagem Fixa Sacado)
        //             Alteração da classe Boleto: Adicionado propriedade para controlar a impressão do código da carteira no boleto
        //             Alteração da classe ContaBancaria: Adicionado propriedade para imprimir mensagem na área de instrução do sacado.
        // Setembro/2018
        //      1.61 - Caixa Economica - Correção Retorno CNAB240 Segmento T

        readonly public string Versao = "1.61";

        private Boletos boletos = new Boletos();
        public int quantidadeBoletos { get { return boletos.Count; } }
        public Boleto boleto { get; set; }
        private bool setupOk { get; set; } = false;

        public bool SetupCobranca(string cnpj, string razaoSocial,
                                    string enderecoLogradouro, string enderecoNumero, string enderecoComplemento, string enderecoBairro, string enderecoCidade, string enderecoEstado, string enderecoCep, string observacoes,
                                    int numeroBanco, string agencia, string digitoAgencia, string operacaoConta, string conta, string digitoConta,
                                    string codigoCedente, string digitoCodigoCedente, string codigoTransmissao,
                                    string carteira, string variacaoCarteira,
                                    int tipoCarteira, int tipoFormaCadastramento, int tipoImpressaoBoleto, int tipoDocumento,
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
                if (string.IsNullOrWhiteSpace(conta))
                {
                    mensagemErro += "Conta não informada." + Environment.NewLine;
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
                if (tipoDocumento < 1 || tipoDocumento > 2)
                {
                    mensagemErro += "Tipo do Documento do Boleto inválido: 1-Tradicional, 2-Escritural" + Environment.NewLine;
                }

                // Banco, Cedente, Conta Corrente
                boletos.Banco = Banco.Instancia(numeroBanco);
                boletos.Banco.Cedente = new Cedente
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
                        CarteiraPadrao = carteira,
                        VariacaoCarteiraPadrao = variacaoCarteira,
                        TipoCarteiraPadrao = (TipoCarteira)tipoCarteira,
                        TipoFormaCadastramento = (TipoFormaCadastramento)tipoFormaCadastramento,
                        TipoImpressaoBoleto = (TipoImpressaoBoleto)tipoImpressaoBoleto,
                        TipoDocumento = (TipoDocumento)tipoDocumento
                    },
                    Codigo = codigoCedente,
                    CodigoDV = digitoCodigoCedente,
                    CodigoTransmissao = codigoTransmissao,
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
                boleto = new Boleto(boletos.Banco);
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
                boleto.EspecieDocumento = Utils.ToEnum<TipoEspecieDocumento>(siglaEspecieDocumento, true, TipoEspecieDocumento.OU);
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
                boleto.PercentualJurosDia = percJuros;
                boleto.ValorJurosDia = valorJuros;
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
        public bool DefinirProtesto(int codigoProtesto, int diasProtesto, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.CodigoProtesto = (TipoCodigoProtesto)codigoProtesto;
                boleto.DiasProtesto = diasProtesto;
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
        public bool DefinirBaixaDevolucao(int codigoBaixaDevolucao, int diasBaixaDevolucao, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.CodigoBaixaDevolucao = (TipoCodigoBaixaDevolucao)codigoBaixaDevolucao;
                boleto.DiasBaixaDevolucao = diasBaixaDevolucao;
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
                boleto.ValidarDados();
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
                    using (BoletoBancario imprimeBoleto = new BoletoBancario
                    {
                        Boleto = boletoTmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = true,
                        MostrarEnderecoCedente = true
                    })
                    {
                        html.Append("<div style=\"page-break-after: always;\">");
                        html.Append(imprimeBoleto.MontaHtml());
                        html.Append("</div>");
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
