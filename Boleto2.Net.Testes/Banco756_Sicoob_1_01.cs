using System;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    public class Banco756_Sicoob_1_01
    {
        Banco banco;
        public Banco756_Sicoob_1_01()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "4277",
                DigitoAgencia = "3",
                Conta = "6498",
                DigitoConta = "0",
                Carteira = "1",
                VariacaoCarteira = "01",
                TipoCarteira = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            banco = new Banco(756)
            {
                Cedente = Utils.GerarCedente("17227", "8", contaBancaria)
            };
            banco.FormataCedente();
        }

        [Test]
        public void Banco756_Sicoob_1_01_REM240()
        {
            Utils.TestarArquivoRemessa(banco, TipoArquivo.CNAB240, nameof(Banco756_Sicoob_1_01));
        }
        [Test]
        public void Banco756_Sicoob_1_01_REM400()
        {
            Utils.TestarArquivoRemessa(banco, TipoArquivo.CNAB400, nameof(Banco756_Sicoob_1_01));
        }

        [Test]
        public void Banco756_Sicoob_1_01_PDF()
        {
            Utils.TestarBoletoPDF(banco, nameof(Banco756_Sicoob_1_01));
        }

        [Test]
        public void Banco756_Sicoob_1_01_DV1()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 11, 10),
                ValorTitulo = (decimal)300,
                NossoNumero = "4",
                NumeroDocumento = "BO123456D",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("1", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 1");
            Assert.AreEqual("0000004-0", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75691697400000300001427701017227800000040001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.400010 1 69740000030000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco756_Sicoob_1_01_DV2()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 03, 27),
                ValorTitulo = (decimal)300,
                NossoNumero = "3",
                NumeroDocumento = "BO123456D",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("2", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 2");
            Assert.AreEqual("0000003-3", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75692711100000300001427701017227800000033001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.330019 2 71110000030000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco756_Sicoob_1_01_DV3()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 04, 27),
                ValorTitulo = (decimal)400,
                NossoNumero = "4",
                NumeroDocumento = "BO123456D",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("3", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 3");
            Assert.AreEqual("0000004-0", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75693714200000400001427701017227800000040001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.400010 3 71420000040000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco756_Sicoob_1_01_DV4()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 01, 05),
                ValorTitulo = (decimal)500,
                NossoNumero = "6",
                NumeroDocumento = "BO123456F",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("4", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 4");
            Assert.AreEqual("0000006-5", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75694703000000500001427701017227800000065001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.650010 4 70300000050000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }



        [Test]
        public void Banco756_Sicoob_1_01_DV5()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 09, 27),
                ValorTitulo = (decimal)900,
                NossoNumero = "9",
                NumeroDocumento = "BO123456F",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("5", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 5");
            Assert.AreEqual("0000009-7", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75695729500000900001427701017227800000097001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.970012 5 72950000090000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }


        [Test]
        public void Banco756_Sicoob_1_01_DV6()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 2, 07),
                ValorTitulo = (decimal)600,
                NossoNumero = "7",
                NumeroDocumento = "BO123456G",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("6", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 6");
            Assert.AreEqual("0000007-2", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75696706300000600001427701017227800000072001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.720011 6 70630000060000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

        [Test]
        public void Banco756_Sicoob_1_01_DV7()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 12, 28),
                ValorTitulo = (decimal)4011.24,
                NossoNumero = "12349",
                NumeroDocumento = "BO123456F",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("7", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 7");
            Assert.AreEqual("0012349-2", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75697702200004011241427701017227800123492001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 01234.920013 7 70220000401124", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }


        [Test]
        public void Banco756_Sicoob_1_01_DV8()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2017, 3, 11),
                ValorTitulo = (decimal)700,
                NossoNumero = "8",
                NumeroDocumento = "BO123456B",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("8", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 8");
            Assert.AreEqual("0000008-0", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75698709500000700001427701017227800000080001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.800011 8 70950000070000", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }


        [Test]
        public void Banco756_Sicoob_1_01_DV9()
        {
            var boleto = new Boleto
            {
                DataVencimento = new DateTime(2016, 12, 08),
                ValorTitulo = (decimal)409,
                NossoNumero = "5",
                NumeroDocumento = "BO123456E",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Banco = banco,
                Sacado = Utils.GerarSacado()
            };
            boleto.ValidarDados();
            Assert.AreEqual("9", boleto.CodigoBarra.DigitoVerificador, "Dígito Verificador diferente de 9");
            Assert.AreEqual("0000005-8", boleto.NossoNumeroFormatado, "Nosso número inválido");
            Assert.AreEqual("75699700200000409001427701017227800000058001", boleto.CodigoBarra.CodigoDeBarras, "Código de Barra inválido");
            Assert.AreEqual("75691.42776 01017.227800 00000.580019 9 70020000040900", boleto.CodigoBarra.LinhaDigitavel, "Linha digitável inválida");
        }

    }
}