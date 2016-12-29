using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Boleto2Net.Testes
{
    sealed class Utils
    {

        internal static Cedente GerarCedente(string codigoCedente, string digitoCodigoCedente, ContaBancaria contaBancaria)
        {
            return new Cedente
            {
                CPFCNPJ = "12.123.123/1234-46",
                Nome = "Cedente Teste",
                Codigo = codigoCedente,
                CodigoDV = digitoCodigoCedente,
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

        static int contador = 1;
        internal static Sacado GerarSacado()
        {
            if (contador % 2 == 0)
            {
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

        static int proximoNossoNumero = 1;
        internal static Boletos GerarBoletos(Banco banco, int quantidadeBoletos)
        {
            var boletos = new Boletos
            {
                Banco = banco
            };
            for (int i = 1; i <= quantidadeBoletos; i++)
            {
                boletos.Add(GerarBoleto(banco, i));
            }
            return boletos;
        }
        internal static Boleto GerarBoleto(Banco banco, int i)
        {
            var boleto = new Boleto
            {
                Banco = banco,
                Sacado = Utils.GerarSacado(),
                DataEmissao = DateTime.Now.AddDays(-3),
                DataProcessamento = DateTime.Now,
                DataVencimento = DateTime.Now.AddMonths(i),
                ValorTitulo = (decimal)100 * i,
                NossoNumero = (223344+proximoNossoNumero).ToString(),
                NumeroDocumento = "BB" + proximoNossoNumero.ToString("D6") + (char)(64 + i),
                EspecieDocumento = TipoEspecieDocumento.DM,
                Aceite = (contador % 2) == 0 ? "N" : "A",
                CodigoInstrucao1 = "11",
                CodigoInstrucao2 = "22",
                DataDesconto = DateTime.Now.AddMonths(i),
                ValorDesconto = (decimal)(100 * i * 0.10),
                DataMulta = DateTime.Now.AddMonths(i),
                PercentualMulta = (decimal)0.02,
                ValorMulta = (decimal)(100 * i * 0.02),
                DataJuros = DateTime.Now.AddMonths(i),
                PercentualJurosDia = (decimal)0.002,
                ValorJurosDia = (decimal)(100 * i * 0.002),
                MensagemArquivoRemessa = "Mensagem para o arquivo remessa",
                MensagemInstrucoesCaixa = "Mensagem para instruções do caixa",
                NumeroControleParticipante = "CHAVEPRIMARIA="+ proximoNossoNumero.ToString()
            };
            if (contador % 3 == 0)
            {
                boleto.Avalista = GerarSacado();
                boleto.Avalista.Nome = boleto.Avalista.Nome.Replace("Sacado", "Avalista");
            }
                
            boleto.ValidarDados();
            contador++;
            proximoNossoNumero++;
            return boleto;
        }

        internal static void TestarBoletoPDF(Banco banco, string nomeCarteira)
        {
            int quantidadeBoletosParaTeste = 3;
            var boletos = GerarBoletos(banco, quantidadeBoletosParaTeste);
            Assert.AreEqual(quantidadeBoletosParaTeste, boletos.Count, "Quantidade de boletos diferente de "+ quantidadeBoletosParaTeste.ToString());

            // Define o nome do arquivo.
            var nomeArquivo = Path.GetTempPath() + "Boleto2Net\\" + nomeCarteira + "_Arquivo.PDF";

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

        internal static void TestarArquivoRemessa(Banco banco, TipoArquivo tipoArquivo, string nomeCarteira)
        {
            int quantidadeBoletosParaTeste = 36;
            var boletos = GerarBoletos(banco, quantidadeBoletosParaTeste);
            Assert.AreEqual(quantidadeBoletosParaTeste, boletos.Count, "Quantidade de boletos diferente de "+ quantidadeBoletosParaTeste.ToString());

            // Define o nome do arquivo.
            var nomeArquivo = Path.GetTempPath() + "Boleto2Net\\" + nomeCarteira + "_Arquivo" + tipoArquivo.ToString() + ".REM";

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