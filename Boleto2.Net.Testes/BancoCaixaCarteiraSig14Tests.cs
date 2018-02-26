﻿using System;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    public class BancoCaixaCarteiraSig14Tests
    {
        readonly IBanco _banco;

        public BancoCaixaCarteiraSig14Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "SIG14",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Caixa);
            _banco.Cedente = Utils.GerarCedente("123456", "0", "", contaBancaria);
            _banco.FormataCedente();
        }

        [Test]
        public void Caixa_SIG14_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoCaixaCarteiraSig14Tests), 5, true, "?", 223344);
        }

        [Test]
        public void Caixa_SIG14_REM400()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoCaixaCarteiraSig14Tests), 5, true, "?", 223344);
        }


        [TestCase(500, "6", "BO123456F", "1", "14000000000000006-5", "10491703000000500001234560000100040000000064", "10491.23456 60000.100044 00000.000646 1 70300000050000", 2017, 01, 05)]
        [TestCase(300, "4", "BO123456D", "2", "14000000000000004-9", "10492697400000300001234560000100040000000048", "10491.23456 60000.100044 00000.000489 2 69740000030000", 2016, 11, 10)]
        [TestCase(409, "5", "BO123456E", "3", "14000000000000005-7", "10493700200000409001234560000100040000000056", "10491.23456 60000.100044 00000.000562 3 70020000040900", 2016, 12, 08)]
        [TestCase(400, "5", "BO123456E", "4", "14000000000000005-7", "10494700200000400001234560000100040000000056", "10491.23456 60000.100044 00000.000562 4 70020000040000", 2016, 12, 08)]
        [TestCase(700, "8", "BO123456B", "5", "14000000000000008-1", "10495709500000700001234560000100040000000080", "10491.23456 60000.100044 00000.000802 5 70950000070000", 2017, 3, 11)]
        [TestCase(600, "7", "BO123456G", "6", "14000000000000007-3", "10496706300000600001234560000100040000000072", "10491.23456 60000.100044 00000.000729 6 70630000060000", 2017, 2, 07)]
        [TestCase(800, "9", "BO123456B", "7", "14000000000000009-0", "10497711500000800001234560000100040000000099", "10491.23456 60000.100044 00000.000992 7 71150000080000", 2017, 3, 31)]
        [TestCase(100, "2", "BO123456B", "8", "14000000000000002-2", "10498691800000100001234560000100040000000021", "10491.23456 60000.100044 00000.000216 8 69180000010000", 2016, 9, 15)]
        [TestCase(200, "3", "BO123456C", "9", "14000000000000003-0", "10499694700000200001234560000100040000000030", "10491.23456 60000.100044 00000.000307 9 69470000020000", 2016, 10, 14)]
        public void Caixa_SIG14_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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