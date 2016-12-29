using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boleto2Net.Testes
{
    [TestClass]
    public class Banco001_Brasil_11_019
    {
        Banco banco;
        public Banco001_Brasil_11_019()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                Carteira = "11",
                VariacaoCarteira = "019",
                TipoCarteira = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            banco = new Banco(001)
            {
                Cedente = Utils.GerarCedente("1234567", "", contaBancaria)
            };
            banco.FormataCedente();


        }
        [TestMethod]
        public void Banco001_Brasil_11_019_TestePendente()
        {
            Assert.Inconclusive("Aguardando boletos de exemplo (gerados pelo banco - segunda via) para implementar os testes:/n" +
                                "Remessa Cnab240/n" +
                                "Remessa Cnab400/n" +
                                "Arquivo PDF/n" +
                                "DV de 1 a 9");
        }

    }
}