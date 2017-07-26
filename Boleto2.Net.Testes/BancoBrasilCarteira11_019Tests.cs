using NUnit.Framework;

namespace Boleto2Net.Testes
{
    [TestFixture]
    public class BancoBrasilCarteira11019Tests
    {
        readonly Banco _banco;
        public BancoBrasilCarteira11019Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "11",
                VariacaoCarteiraPadrao = "019",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.NovaInstancia(001);
            _banco.Cedente = Utils.GerarCedente("1234567", "", "", contaBancaria);
            _banco.FormataCedente();


        }
        [Test]
        public void Brasil_11_019_TestePendente()
        {
            Assert.Inconclusive("Aguardando boletos de exemplo (gerados pelo banco - segunda via) para implementar os testes:/n" +
                                "Remessa Cnab240/n" +
                                "Remessa Cnab400/n" +
                                "Arquivo PDF/n" +
                                "DV de 1 a 9");
        }

    }
}