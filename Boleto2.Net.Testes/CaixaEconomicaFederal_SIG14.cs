using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boleto2Net.Testes
{
    [TestClass]
    public class Banco104_CaixaEconomicaFederal_SIG14
    {
        Banco banco;
        Boletos boletos;

        [TestMethod]
        public void Banco104_CaixaEconomicaFederal_SIG14_Testes()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                Carteira = "SIG14",
                TipoCarteira = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            banco = new Banco(104)
            {
                Cedente = Utils.GerarCedente("123456", contaBancaria)
            };
            banco.FormataCedente();

            boletos = new Boletos
            {
                Banco = banco
            };

            Banco104_CaixaEconomicaFederal_SIG14_DV1();
            Banco104_CaixaEconomicaFederal_SIG14_DV2();
            Banco104_CaixaEconomicaFederal_SIG14_DV3();
            Banco104_CaixaEconomicaFederal_SIG14_DV4();
            Banco104_CaixaEconomicaFederal_SIG14_DV5();
            Banco104_CaixaEconomicaFederal_SIG14_DV6();
            Banco104_CaixaEconomicaFederal_SIG14_DV7();
            Banco104_CaixaEconomicaFederal_SIG14_DV8();
            Banco104_CaixaEconomicaFederal_SIG14_DV9();

            Utils.TestarArquivoRemessa(TipoArquivo.CNAB240, boletos, nameof(Banco104_CaixaEconomicaFederal_SIG14));

            Utils.TestarBoletoPDF(boletos, nameof(Banco104_CaixaEconomicaFederal_SIG14));

        }

        private void Banco104_CaixaEconomicaFederal_SIG14_DV1()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 01, 05),
                ValorTitulo = (decimal)500,
                NossoNumero = "6",
                NumeroDocumento = "BO123456F",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("1", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 1");
            Assert.AreEqual("14000000000000006-5", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10491703000000500001234560000100040000000064", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000646 1 70300000050000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }


        private void Banco104_CaixaEconomicaFederal_SIG14_DV2()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 11, 10),
                ValorTitulo = (decimal)300,
                NossoNumero = "4",
                NumeroDocumento = "BO123456D",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("2", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 2");
            Assert.AreEqual("14000000000000004-9", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10492697400000300001234560000100040000000048", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000489 2 69740000030000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }

        private void Banco104_CaixaEconomicaFederal_SIG14_DV3()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 12, 08),
                ValorTitulo = (decimal)409,
                NossoNumero = "5",
                NumeroDocumento = "BO123456E",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("3", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 3");
            Assert.AreEqual("14000000000000005-7", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10493700200000409001234560000100040000000056", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000562 3 70020000040900", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }

        private void Banco104_CaixaEconomicaFederal_SIG14_DV4()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 12, 08),
                ValorTitulo = (decimal)400,
                NossoNumero = "5",
                NumeroDocumento = "BO123456E",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("4", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 4");
            Assert.AreEqual("14000000000000005-7", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10494700200000400001234560000100040000000056", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000562 4 70020000040000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }

        private void Banco104_CaixaEconomicaFederal_SIG14_DV5()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 3, 11),
                ValorTitulo = (decimal)700,
                NossoNumero = "8",
                NumeroDocumento = "BO123456B",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("5", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 5");
            Assert.AreEqual("14000000000000008-1", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10495709500000700001234560000100040000000080", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000802 5 70950000070000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }


        private void Banco104_CaixaEconomicaFederal_SIG14_DV6()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 2, 07),
                ValorTitulo = (decimal)600,
                NossoNumero = "7",
                NumeroDocumento = "BO123456G",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("6", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 6");
            Assert.AreEqual("14000000000000007-3", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10496706300000600001234560000100040000000072", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000729 6 70630000060000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }



        private void Banco104_CaixaEconomicaFederal_SIG14_DV7()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 3, 31),
                ValorTitulo = (decimal)800,
                NossoNumero = "9",
                NumeroDocumento = "BO123456B",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("7", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 7");
            Assert.AreEqual("14000000000000009-0", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10497711500000800001234560000100040000000099", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000992 7 71150000080000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }



        private void Banco104_CaixaEconomicaFederal_SIG14_DV8()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 9, 15),
                ValorTitulo = (decimal)100,
                NossoNumero = "2",
                NumeroDocumento = "BO123456B",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("8", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 8");
            Assert.AreEqual("14000000000000002-2", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10498691800000100001234560000100040000000021", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000216 8 69180000010000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }

        private void Banco104_CaixaEconomicaFederal_SIG14_DV9()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 10, 14),
                ValorTitulo = (decimal)200,
                NossoNumero = "3",
                NumeroDocumento = "BO123456C",
                SiglaEspecieDocumento = "DM",
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.Valida();
            Assert.AreEqual("9", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 9");
            Assert.AreEqual("14000000000000003-0", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("10499694700000200001234560000100040000000030", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("10491.23456 60000.100044 00000.000307 9 69470000020000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
            boletos.Add(boleto);
        }


    }
}