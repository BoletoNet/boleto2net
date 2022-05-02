using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Boleto2Net;

namespace Boleto2.Net.AppTeste.Models
{
    public class GeraBoletoViewModel
    {

        #region Boleto
        [DisplayName("Código da Moeda")]
        public int Boleto_CodigoMoeda { get; set; } = 790;


        [DisplayName("Espécie da Moeda")]
        public string Boleto_EspecieMoeda { get; set; } = "R$";


        [DisplayName("Quantidade da Moeda")]
        public int Boleto_QuantidadeMoeda { get; set; } = 900;


        [DisplayName("Valor da Moeda")]
        public string Boleto_ValorMoeda { get; set; } = "4,7025";


        [DisplayName("Espécie do Documento")]
        public TipoEspecieDocumento Boleto_EspecieDocumento { get; set; }


        [DisplayName("Nosso Número")]
        public string Boleto_NossoNumero { get; set; } = "453";


        [DisplayName("Data de Processamento")]
        public DateTime Boleto_DataProcessamento { get; set; } = DateTime.Now;


        [DisplayName("Data de Emissão")]
        public DateTime Boleto_DataEmissao { get; set; } = DateTime.Now;


        [DisplayName("Data de Vencimento")]
        public DateTime Boleto_DataVencimento { get; set; } 


        [DisplayName("Nùmero Controle Participante")]
        public string Boleto_NumeroControleParticipante { get; set; } = "1";


        [DisplayName("Nùmero Documento")]
        public string Boleto_NumeroDocumento { get; set; } = "123456";


        [DisplayName("Aceite")]
        public string Boleto_Aceite { get; set; } = "N";


        [DisplayName("Valor Título")]
        public decimal Boleto_ValorTitulo { get; set; } = 12m;

#endregion 

        #region Cedente


        [DisplayName("CPFCNPJ")]
        public string Cedente_CPFCNPJ { get; set; }


        [DisplayName("Código")]
        public string Cedente_Codigo { get; set; }


        [DisplayName("Código DV")]
        public string Cedente_CodigoDV { get; set; }


        [DisplayName("Código Transmissão")]
        public string Cedente_CodigoTransmissao { get; set; }


        [DisplayName("Razão Social")]
        public string Cedente_Nome { get; set; }


        [DisplayName("N°. Banco")]
        public int Cedente_NumeroBanco { get; set; }


        [DisplayName("Agência")]
        public string Cedente_Agencia { get; set; }


        [DisplayName("Dígito Agência")]
        public string Cedente_DigitoAgencia { get; set; }


        [DisplayName("Operação Conta")]
        public string Cedente_OperacaoConta { get; set; }


        [DisplayName("Conta")]
        public string Cedente_Conta { get; set; }


        [DisplayName("Dígito Conta")]
        public string Cedente_DigitoConta { get; set; }


        [DisplayName("Tipo Forma Cadastramento")]
        public TipoFormaCadastramento Cedente_TipoFormaCadastramento { get; set; }


        [DisplayName("Tipo Impressão Boleto")]
        public TipoImpressaoBoleto Cedente_TipoImpressaoBoleto { get; set; }


        [DisplayName("Tipo Documento")]
        public TipoDocumento Cedente_TipoDocumento { get; set; }


        [DisplayName("Tipo Carteira")]
        public TipoCarteira Cedente_TipoCarteira { get; set; }


        [DisplayName("Carteira")]
        public string Cedente_Carteira { get; set; }


        [DisplayName("Variação Carteira")]
        public string Cedente_VariacaoCarteira { get; set; }


        [DisplayName("Mensagem Fixa Topo Boleto")]
        public string Cedente_MensagemFixaTopoBoleto { get; set; } = "";


        [DisplayName("Mensagem Fixa Sacado")]
        public string Cedente_MensagemFixaSacado { get; set; } = "";


        [DisplayName("Tipo Distribuição")]
        public TipoDistribuicaoBoleto Cedente_TipoDistribuicao { get; set; } = TipoDistribuicaoBoleto.ClienteDistribui;


        [DisplayName("Observações")]
        public string Cedente_Observacoes { get; set; }


        [DisplayName("Logradouro")]
        public string Cedente_LogradouroEndereco { get; set; }


        [DisplayName("Número")]
        public string Cedente_LogradouroNumero { get; set; }


        [DisplayName("Complemento")]
        public string Cedente_LogradouroComplemento { get; set; }


        [DisplayName("Bairro")]
        public string Cedente_Bairro { get; set; }


        [DisplayName("Cidade")]
        public string Cedente_Cidade { get; set; }


        [DisplayName("UF")]
        public string Cedente_UF { get; set; }


        [DisplayName("CEP")]
        public string Cedente_CEP { get; set; }


        #endregion

        #region Sacado


        [DisplayName("CPFCNPJ")]
        public string Sacado_CPFCNPJ { get; set; }


        [DisplayName("Razão Social")]
        public string Sacado_Nome { get; set; }


        [DisplayName("Observações")] 
        public string Sacado_Observacoes { get; set; }


        [DisplayName("Logradouro")]
        public string Sacado_LogradouroEndereco { get; set; }


        [DisplayName("Número")]
        public string Sacado_LogradouroNumero { get; set; }


        [DisplayName("Complemento")]
        public string Sacado_LogradouroComplemento { get; set; }


        [DisplayName("Bairro")]
        public string Sacado_Bairro { get; set; }


        [DisplayName("Cidade")]
        public string Sacado_Cidade { get; set; }


        [DisplayName("UF")]
        public string Sacado_UF { get; set; }

        [DisplayName("CEP")]
        public string Sacado_CEP { get; set; }


        #endregion

        #region Avalista


        [DisplayName("CPFCNPJ")]
        public string Avalista_CPFCNPJ { get; set; }


        [DisplayName("Razão Social")]
        public string Avalista_Nome { get; set; }


        [DisplayName("Observações")]
        public string Avalista_Observacoes { get; set; }


        [DisplayName("Logradouro")]
        public string Avalista_LogradouroEndereco { get; set; }


        [DisplayName("Número")]
        public string Avalista_LogradouroNumero { get; set; }


        [DisplayName("Complemento")]
        public string Avalista_LogradouroComplemento { get; set; }


        [DisplayName("Bairro")]
        public string Avalista_Bairro { get; set; }


        [DisplayName("Cidade")]
        public string Avalista_Cidade { get; set; }


        [DisplayName("UF")]
        public string Avalista_UF { get; set; }

        [DisplayName("CEP")]
        public string Avalista_CEP { get; set; }


        #endregion

        #region Juros / Desconto / Protesto


        [DisplayName("Valor Juros/Dia")]
        public decimal Juros_ValorJurosDia { get; set; }


        [DisplayName("Percentual Juros/Dia")]
        public decimal Juros_PercentualJurosDia { get; set; }


        [DisplayName("Valor IOF")]
        public decimal Juros_ValorIOF { get; set; }


        [DisplayName("Data do Juros")]
        public DateTime Juros_DataJuros { get; set; }


        [DisplayName("Valor Desconto")]
        public decimal Desconto_ValorDesconto { get; set; }


        [DisplayName("Data de Desconto")]
        public DateTime Desconto_DataDesconto { get; set; }


        [DisplayName("Código Protesto")]
        public TipoCodigoProtesto Protesto_CodigoProtesto { get; set; } = TipoCodigoProtesto.NaoProtestar;


        [DisplayName("Dias Protesto")]
        public int Protesto_DiasProtesto { get; set; } = 0;


        #endregion

        #region Instruções


        [DisplayName("Código Instrução 1")]
        public string Instruções_CodigoInstrucao1 { get; set; } = string.Empty;


        [DisplayName("Complemento Instrução 1")]
        public string Instruções_ComplementoInstrucao1 { get; set; } = string.Empty;


        [DisplayName("Código Instrução 2")]
        public string Instruções_CodigoInstrucao2 { get; set; } = string.Empty;


        [DisplayName("Complemento Instrução 2")]
        public string Instruções_ComplementoInstrucao2 { get; set; } = string.Empty;


        [DisplayName("Código Instrução 3")]
        public string Instruções_CodigoInstrucao3 { get; set; } = string.Empty;


        [DisplayName("Complemento Instrução 3")]
        public string Instruções_ComplementoInstrucao3 { get; set; } = string.Empty;


        [DisplayName("Mensagem Instruções Caixa")]
        public string Instruções_MensagemInstrucoesCaixa { get; set; } = string.Empty;


        [DisplayName("Mensagem Arquivo Remessa")]
        public string Instruções_MensagemArquivoRemessa { get; set; } = string.Empty;


        [DisplayName("Registro Arquivo Retorno")]
        public string Instruções_RegistroArquivoRetorno { get; set; } = string.Empty;


        #endregion

        public void ConfiguraModel()
        {

            Boleto_DataVencimento = Boleto_DataEmissao.AddDays(40);
            Juros_DataJuros = Boleto_DataEmissao.AddDays(60);
            Desconto_DataDesconto = Boleto_DataEmissao.AddDays(60);
            ConfiguraCedente();
            ConfiguraSacado();

            Cedente_TipoDistribuicao = TipoDistribuicaoBoleto.ClienteDistribui;
            Cedente_TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
            Cedente_TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro;
            Cedente_TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            Cedente_TipoDocumento = TipoDocumento.Escritural;

        }

        public void ConfiguraCedente() 
        {

            Cedente_CPFCNPJ = "12.123.123/1234-46";
            Cedente_Codigo = "1213141";
            Cedente_CodigoDV = "";
            Cedente_CodigoTransmissao = "";
            Cedente_Nome = "Cedente Teste";
            Cedente_NumeroBanco = 237;
            Cedente_Observacoes = "Observacoes do Cedente";
            Cedente_Agencia = "1234";
            Cedente_DigitoAgencia = "X";
            Cedente_OperacaoConta = "001";
            Cedente_Conta = "123456";
            Cedente_DigitoConta = "X";
            Cedente_MensagemFixaTopoBoleto = "";
            Cedente_MensagemFixaSacado = "";
            Cedente_Carteira = "09";
            Cedente_VariacaoCarteira = "";

            Cedente_LogradouroEndereco = "Av Testador";
            Cedente_LogradouroNumero = "12";
            Cedente_LogradouroComplemento = "Casa de esquina";
            Cedente_Bairro = "Centro";
            Cedente_Cidade = "Testópolis";
            Cedente_UF = "SP";
            Cedente_CEP = "11223-445";
            Cedente_CodigoTransmissao = "";

        }
    
        public void ConfiguraSacado()
        {
            Sacado_CPFCNPJ = "32.145.698/7001-99";
            Sacado_Nome = "Empresa Sacado";
            Sacado_Observacoes = "Pessoa";
            Sacado_LogradouroEndereco = "Rua do Sacado";
            Sacado_LogradouroNumero = "0";
            Sacado_LogradouroComplemento = "Quadra 18";
            Sacado_Bairro = "Jardim Jardim";
            Sacado_Cidade = "Pindamoiangaba";
            Sacado_UF = "AC";
            Sacado_CEP = "11000-000";

        }
    }
}