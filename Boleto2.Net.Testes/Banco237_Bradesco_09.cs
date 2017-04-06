using System;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    public class Banco237_Bradesco_09
    {
        Banco banco;
        public Banco237_Bradesco_09()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                Carteira = "09",
                TipoCarteira = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            banco = new Banco(237)
            {
                Cedente = Utils.GerarCedente("1213141", "", contaBancaria)
            };
            banco.FormataCedente();
        }

        [Test]
        public void Banco237_Bradesco_09_REM400()
        {
            Utils.TestarArquivoRemessa(banco, TipoArquivo.CNAB400, nameof(Banco237_Bradesco_09));
        }

        [Test]
        public void Banco237_Bradesco_09_PDF()
        {
            Utils.TestarBoletoPDF(banco, nameof(Banco237_Bradesco_09));
        }


        [Test]
        public void Banco237_Bradesco_09_DV1()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 9, 1),
                ValorTitulo = (decimal)141.50,
                NossoNumero = "453",
                NumeroDocumento = "BB943A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("1", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 1");
            Assert.AreEqual("09/00000000453-P", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23791690400000141501234090000000045301234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 53012.345608 1 69040000014150", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco237_Bradesco_09_DV2()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 10, 1),
                ValorTitulo = (decimal)2717.16,
                NossoNumero = "456",
                NumeroDocumento = "BB874A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("2", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 2");
            Assert.AreEqual("09/00000000456-4", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23792693400002717161234090000000045601234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 56012.345601 2 69340000271716", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco237_Bradesco_09_DV3()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 9, 2),
                ValorTitulo = (decimal)297.21,
                NossoNumero = "444",
                NumeroDocumento = "BB834A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("3", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 3");
            Assert.AreEqual("09/00000000444-0", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23793690500000297211234090000000044401234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 44012.345607 3 69050000029721", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco237_Bradesco_09_DV4()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 10, 2),
                ValorTitulo = (decimal)297.21,
                NossoNumero = "468",
                NumeroDocumento = "BB856A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("4", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 4");
            Assert.AreEqual("09/00000000468-8", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23794693500000297211234090000000046801234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 68012.345606 4 69350000029721", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }


        [Test]
        public void Banco237_Bradesco_09_DV5()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 9, 2),
                ValorTitulo = (decimal)297.21,
                NossoNumero = "443",
                NumeroDocumento = "BB833A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("5", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 5");
            Assert.AreEqual("09/00000000443-2", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23795690500000297211234090000000044301234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 43012.345609 5 69050000029721", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }


        [Test]
        public void Banco237_Bradesco_09_DV6()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 8, 1),
                ValorTitulo = (decimal)649.39,
                NossoNumero = "414",
                NumeroDocumento = "BB815A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("6", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 6");
            Assert.AreEqual("09/00000000414-9", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23796687300000649391234090000000041401234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 14012.345600 6 68730000064939", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco237_Bradesco_09_DV7()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 1, 1),
                ValorTitulo = (decimal)270,
                NossoNumero = "561",
                NumeroDocumento = "BB932A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("7", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 7");
            Assert.AreEqual("09/00000000561-7", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23797702600000270001234090000000056101234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000050 61012.345601 7 70260000027000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }


        [Test]
        public void Banco237_Bradesco_09_DV8()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 9, 2),
                ValorTitulo = (decimal)2924.11,
                NossoNumero = "445",
                NumeroDocumento = "BB874A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("8", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 8");
            Assert.AreEqual("09/00000000445-9", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23798690500002924111234090000000044501234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000043 45012.345604 8 69050000292411", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco237_Bradesco_09_DV9()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 1, 1),
                ValorTitulo = (decimal)830,
                NossoNumero = "562",
                NumeroDocumento = "BB933A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("9", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 9");
            Assert.AreEqual("09/00000000562-5", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("23799702600000830001234090000000056201234560", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("23791.23405 90000.000050 62012.345609 9 70260000083000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

    }
}