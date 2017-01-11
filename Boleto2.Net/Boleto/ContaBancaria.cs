namespace Boleto2Net
{
    public class ContaBancaria
    {
        public string Agencia { get; set; } = string.Empty;
        public string DigitoAgencia { get; set; } = string.Empty;
        public string Conta { get; set; } = string.Empty;
        public string DigitoConta { get; set; } = string.Empty;
        public string OperacaoConta { get; set; } = string.Empty;
        public string Carteira { get; set; } = string.Empty;
        public string VariacaoCarteira { get; set; } = string.Empty;
        public TipoCarteira TipoCarteira { get; set; } = TipoCarteira.CarteiraCobrancaSimples;
        public TipoFormaCadastramento TipoFormaCadastramento { get; set; } = TipoFormaCadastramento.ComRegistro;
        public TipoImpressaoBoleto TipoImpressaoBoleto { get; set; } = TipoImpressaoBoleto.Empresa;
        public string LocalPagamento { get; set; } = "PAGÁVEL EM QUALQUER BANCO ATÉ A DATA DE VENCIMENTO.";
        public string CarteiraComVariacao
        {
            get
            {
                if (string.IsNullOrEmpty(Carteira) || string.IsNullOrEmpty(VariacaoCarteira))
                    return Carteira + VariacaoCarteira;
                else
                    return Carteira + "/" + VariacaoCarteira;
            }
        }
        public int CodigoBancoCorrespondente { get; set; }
        public string NossoNumeroBancoCorrespondente { get; set; }

    }
}
