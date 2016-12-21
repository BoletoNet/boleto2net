using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boleto2Net.Testes
{
    [TestClass]
    public class Brasil_11019
    {
        Banco banco;
        Boletos boletos;

        [TestMethod]
        public void Banco001_Brasil_17019_Testes()
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
                Cedente = Utils.GerarCedente("1234567", contaBancaria)
            };
            banco.FormataCedente();

            boletos = new Boletos
            {
                Banco = banco
            };

            //Banco001_Brasil_11019_DV1();
            //Banco001_Brasil_11019_DV2();
            //Banco001_Brasil_11019_DV3();
            //Banco001_Brasil_11019_DV4();
            //Banco001_Brasil_11019_DV5();
            //Banco001_Brasil_11019_DV6();
            //Banco001_Brasil_11019_DV7();
            //Banco001_Brasil_11019_DV8();
            //Banco001_Brasil_11019_DV9();

            //Utils.TestarArquivoRemessa(TipoArquivo.CNAB240, boletos, nameof(Banco001_Brasil_11019));

            //Utils.TestarArquivoRemessa(TipoArquivo.CNAB400, boletos, nameof(Banco001_Brasil_11019));

            //Utils.TestarBoletoPDF(boletos, nameof(Banco001_Brasil_11019));

            Assert.Inconclusive("Aguardando boletos de exemplo (gerados pelo banco - segunda via) para implementar o teste.");

        }

    }
}