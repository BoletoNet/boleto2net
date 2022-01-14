using System;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    [TestFixture]
    [Category("Bradesco Carteira 02")]
    public class BancoBradescoCarteira02Tests
    {
        readonly IBanco _banco;
        public BancoBradescoCarteira02Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "02",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Bradesco);
            _banco.Cedente = Utils.GerarCedente("1213141", "", "", contaBancaria);
            _banco.FormataCedente();
        }

        [Test]
        public void Bradesco_09_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBradescoCarteira02Tests), 5, true, "?", 223344);
        }
        [Test]
        public void Bradesco_09_REM400()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBradescoCarteira02Tests), 5, true, "?", 223344);
        }


        [TestCase(25387.72, "125", "045640-1-2/3", "9", "002/00000000125-P", "23799891200025387721234020000000012501234560", "23791.23405 20000.000016 25012.345606 9 89120002538772", 2022, 3, 2)]
        [TestCase(2830.68, "4", "045638-1-5/5", "2", "002/00000000004-0", "23792891200002830681234020000000000401234560", "23791.23405 20000.000008 04012.345601 2 89120000283068", 2022, 3, 2)]
        [TestCase(25387.73, "124", "045640-1-1/3", "3", "002/00000000124-1", "23793889800025387731234020000000012401234560", "23791.23405 20000.000016 24012.345609 3 88980002538773", 2022, 2, 16)]
        [TestCase(2830.68, "3", "045638-1-2/5", "1", "002/00000000003-2", "23791889100002830681234020000000000301234560", "23791.23405 20000.000008 03012.345603 1 88910000283068", 2022, 2, 9)]
        [TestCase(2830.68, "2", "045638-1-1/5", "5", "002/00000000002-4", "23795888400002830681234020000000000201234560", "23791.23405 20000.000008 02012.345605 5 88840000283068", 2022, 2, 2)]
        [TestCase(19290.06, "1", "045639-1-1/1", "7", "002/00000000001-6", "23797892400019290061234020000000000101234560", "23791.23405 20000.000008 01012.345607 7 89240001929006", 2022, 3, 14)]
        [TestCase(2830.68, "1", "045638-1-3/5", "3", "002/00000000001-6", "23793889800002830681234020000000000101234560", "23791.23405 20000.000008 01012.345607 3 88980000283068", 2022, 2, 16)]
        [TestCase(25387.73, "126", "045640-1-3/3", "6", "002/00000000126-8", "23796890500025387731234020000000012601234560", "23791.23405 20000.000016 26012.345604 6 89050002538773", 2022, 2, 23)]
        [TestCase(2830.68, "5", "045638-1-4/5", "2", "002/00000000005-9", "23792890500002830681234020000000000501234560", "23791.23405 20000.000008 05012.345608 2 89050000283068", 2022, 2, 23)]
        public void Bradesco_02_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Sacado = Utils.GerarSacado()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}