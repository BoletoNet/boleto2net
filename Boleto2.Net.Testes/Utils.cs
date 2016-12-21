using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Boleto2Net.Testes
{
    sealed class Utils
    {

        internal static Cedente GerarCedente(string codigoCedente, ContaBancaria contaBancaria)
        {
            return new Cedente
            {
                CPFCNPJ = "12.123.123/1234-46",
                Nome = "Cedente Teste",
                Codigo = codigoCedente,
                Endereco = new Endereco
                {
                    LogradouroEndereco = "Rua Teste do Cedente",
                    LogradouroNumero = "789",
                    LogradouroComplemento = "Cj 333",
                    Bairro = "Bairro",
                    Cidade = "Cidade",
                    UF = "SP",
                    CEP = "65432987"
                },
                ContaBancaria = contaBancaria
            };
        }

        static int proximoTipoSacado = 1;
        internal static Sacado GerarSacado()
        {
            if (proximoTipoSacado == 1)
            {
                proximoTipoSacado = 2;
                return new Sacado
                {
                    CPFCNPJ = "123.456.789-09",
                    Nome = "Sacado Teste PF",
                    Observacoes = "Matricula 678/9",
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = "Rua Testando",
                        LogradouroNumero = "456",
                        Bairro = "Bairro",
                        Cidade = "Cidade",
                        UF = "SP",
                        CEP = "56789012"
                    }
                };
            }
            else
            {
                proximoTipoSacado = 1;
                return new Sacado
                {
                    CPFCNPJ = "98.765.432/1098-74",
                    Nome = "Sacado Teste PJ",
                    Observacoes = "Matricula 123/4",
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = "Avenida Testando",
                        LogradouroNumero = "123",
                        Bairro = "Bairro",
                        Cidade = "Cidade",
                        UF = "SP",
                        CEP = "12345678"
                    }
                };
            }
        }

        internal static void TestarBoletoPDF(Boletos boletos, string nomeCarteira)
        {
            // Esperamos receber 9 boletos, com DV entre 1 e 9.
            Assert.AreEqual(9, boletos.Count, "Quantidade de boletos diferente de 9");

            // Define o nome do arquivo.
            var nomeArquivo = Path.GetTempPath() + "Boleto2Net\\" + nomeCarteira + "_Boletos.PDF";

            // Cria pasta para os arquivos
            if (Directory.Exists(Path.GetDirectoryName(nomeArquivo)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(nomeArquivo));

            // Se o arquivo já existir (testes anteriores), apaga o arquivo.
            if (File.Exists(nomeArquivo))
            {
                File.Delete(nomeArquivo);
                if (File.Exists(nomeArquivo))
                    Assert.Fail("Arquivo Boletos (PDF) não foi excluído: " + nomeArquivo);
            }

            // Gera arquivo PDF
            try
            {
                var html = new StringBuilder();
                foreach (Boleto boletoTmp in boletos)
                {
                    if (html.Length != 0)
                    {
                        // Já existe um boleto, inclui quebra de linha.
                        html.Append("</br></br></br></br></br></br></br></br></br></br>");
                    }
                    using (BoletoBancario imprimeBoleto = new BoletoBancario
                    {
                        boleto = boletoTmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = true,
                        MostrarEnderecoCedente = true
                    })
                    {
                        html.Append(imprimeBoleto.MontaHtml());
                    }
                    var pdf = new NReco.PdfGenerator.HtmlToPdfConverter().GeneratePdf(html.ToString());
                    using (FileStream fs = new FileStream(nomeArquivo, FileMode.Create))
                    {
                        fs.Write(pdf, 0, pdf.Length);
                        fs.Close();
                    }
                }
            }
            catch (System.Exception e)
            {
                if (File.Exists(nomeArquivo))
                    File.Delete(nomeArquivo);
                Assert.Fail(e.InnerException.ToString());
            }

            // Se o arquivo existir, considera o teste OK
            Assert.IsTrue(File.Exists(nomeArquivo), "Arquivo Boletos (PDF) não encontrado: " + nomeArquivo);
        }

        internal static void TestarArquivoRemessa(TipoArquivo tipoArquivo, Boletos boletos, string nomeCarteira)
        {
            // Esperamos receber 9 boletos, com DV entre 1 e 9.
            Assert.AreEqual(9, boletos.Count, "Quantidade de boletos diferente de 9");

            // Define o nome do arquivo.
            var nomeArquivo = Path.GetTempPath() + "Boleto2Net\\" + nomeCarteira + "_Remessa_" + tipoArquivo.ToString() + ".REM";

            // Cria pasta para os arquivos
            if (Directory.Exists(Path.GetDirectoryName(nomeArquivo)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(nomeArquivo));

            // Se o arquivo já existir (testes anteriores), apaga o arquivo.
            if (File.Exists(nomeArquivo))
            {
                File.Delete(nomeArquivo);
                if (File.Exists(nomeArquivo))
                    Assert.Fail("Arquivo Remessa não foi excluído: " + nomeArquivo);
            }

            // Gera o arquivo remessa.
            try
            {
                var arquivoRemessa = new ArquivoRemessa(boletos.Banco, tipoArquivo, 1);
                using (var fileStream = new FileStream(nomeArquivo, FileMode.Create))
                {
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);
                }
            }
            catch (System.Exception e)
            {
                if (File.Exists(nomeArquivo))
                    File.Delete(nomeArquivo);
                Assert.Fail(e.InnerException.ToString());
            }

            // Se o arquivo existir, considera o teste OK
            Assert.IsTrue(File.Exists(nomeArquivo), "Arquivo Remessa não encontrado: " + nomeArquivo);
        }

    }
};