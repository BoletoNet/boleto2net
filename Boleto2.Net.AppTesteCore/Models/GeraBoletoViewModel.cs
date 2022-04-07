using Boleto2Net;

namespace Boleto2.Net.AppTesteCore.Models
{
    public class GeraBoletoViewModel
    {
        public string Cnpj { get; set; } 
        public string RazaoSocial { get; set; }    
        public string EnderecoLogradouro { get; set; } 
        public string EnderecoNumero { get; set; } 
        public string EnderecoComplemento { get; set; }
        public string EnderecoBairro { get; set; } 
        public string EnderecoCidade { get; set; } 
        public string EnderecoEstado { get; set; } 
        public string EnderecoCep { get; set; } 
        public string Observacoes { get; set; }
        public int NumeroBanco { get; set; } 
        public string Agencia { get; set; } 
        public string DigitoAgencia { get; set; } 
        public string OperacaoConta { get; set; } 
        public string Conta { get; set; }
        public string DigitoConta { get; set; }
        public string CodigoCedente { get; set; } 
        public string DigitoCodigoCedente { get; set; } 
        public string CodigoTransmissao { get; set; }
        public string Carteira { get; set; } 
        public string VariacaoCarteira { get; set; }
        public int TipoCarteira { get; set; } 
        public int TipoFormaCadastramento { get; set; } 
        public int TipoImpressaoBoleto { get; set; } 
        public int TipoDocumento { get; set; }

        public void ConfiguraModel()
        {
            Cnpj = "12.123.123/1234-46";
            RazaoSocial = "Cedente Teste";
            EnderecoLogradouro = "Av Testador";
            EnderecoNumero = "12";
            EnderecoComplemento = "sala 30";
            EnderecoBairro = "Centro";
            EnderecoCidade = "Cidade";
            EnderecoEstado = "SP";
            EnderecoCep = "11223-445";
            Observacoes = "Observacoes do Cedente";
            NumeroBanco = 237;
            Agencia = "1234";
            DigitoAgencia = "X";
            OperacaoConta = "";
            Conta = "123456";
            DigitoConta = "X";
            CodigoCedente = "1213141";
            DigitoCodigoCedente = "0";
            CodigoTransmissao = "";
            Carteira = "09";
            VariacaoCarteira = "";

            TipoCarteira = (int)Boleto2Net.TipoCarteira.CarteiraCobrancaSimples;
            TipoFormaCadastramento = (int)Boleto2Net.TipoFormaCadastramento.ComRegistro;
            TipoImpressaoBoleto = (int)Boleto2Net.TipoImpressaoBoleto.Empresa;
            TipoDocumento = (int)Boleto2Net.TipoDocumento.Escritural;
        }
    }
}
