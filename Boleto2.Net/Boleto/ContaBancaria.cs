using Boleto2Net.Exceptions;
using static System.String;

namespace Boleto2Net
{
    public class ContaBancaria
    {
        public string Agencia { get; set; } = Empty;
        public string DigitoAgencia { get; set; } = Empty;
        public string Conta { get; set; } = Empty;
        public string DigitoConta { get; set; } = Empty;
        public string OperacaoConta { get; set; } = Empty;
        public string Carteira { get; set; } = Empty;
        public string VariacaoCarteira { get; set; } = Empty;
        public TipoCarteira TipoCarteira { get; set; } = TipoCarteira.CarteiraCobrancaSimples;
        public TipoFormaCadastramento TipoFormaCadastramento { get; set; } = TipoFormaCadastramento.ComRegistro;
        public TipoImpressaoBoleto TipoImpressaoBoleto { get; set; } = TipoImpressaoBoleto.Empresa;
        public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.Tradicional;
        public string LocalPagamento { get; set; } = "PAGÁVEL EM QUALQUER BANCO ATÉ A DATA DE VENCIMENTO.";
        public string CarteiraComVariacao => IsNullOrEmpty(Carteira) || IsNullOrEmpty(VariacaoCarteira) ? $"{Carteira}{VariacaoCarteira}" : $"{Carteira}/{VariacaoCarteira}";
        public int CodigoBancoCorrespondente { get; set; }
        public string NossoNumeroBancoCorrespondente { get; set; }

        public void FormatarDados(string localPagamento, int digitosConta)
        {
            var agencia = Agencia;
            Agencia = agencia.Length <= 4 ? agencia.PadLeft(4, '0') : throw Boleto2NetException.AgenciaInvalida(agencia, 4);

            var conta = Conta;
            Conta = conta.Length <= digitosConta ? conta.PadLeft(digitosConta, '0') : throw Boleto2NetException.ContaInvalida(conta, digitosConta);

            LocalPagamento = localPagamento;
        }
    }
}
