using System;
using System.IO;
using System.Text;
using NReco.PdfGenerator;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    internal sealed class Utils
    {
        private static int _contador = 1;

        private static int _proximoNossoNumero = 1;

        internal static Cedente GerarCedente(string codigoCedente, string digitoCodigoCedente, string codigoTransmissao, ContaBancaria contaBancaria)
        {
            return new Cedente
            {
                CPFCNPJ = "08.367.311/0001-03",
                Nome = "Vagalume Acessorios e Rastreadores de Ve",
                Codigo = codigoCedente,
                CodigoDV = digitoCodigoCedente,
                CodigoTransmissao = codigoTransmissao,
                Endereco = new Endereco
                {
                    LogradouroEndereco = "Rua Silveira Martins",
                    LogradouroNumero = "224",
                    LogradouroComplemento = "",
                    Bairro = "Campos Eliseos",
                    Cidade = "Ribeirão Preto",
                    UF = "SP",
                    CEP = "14080110"
                },
                ContaBancaria = contaBancaria
            };
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

        internal static Sacado GerarSacado()
        {
            if (_contador % 2 == 0)
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

        internal static Boletos GerarBoletos(Banco banco, int quantidadeBoletos)
        {
            var boletos = new Boletos
            {
                Banco = banco
            };
            for (var i = 1; i <= quantidadeBoletos; i++)
                boletos.Add(GerarBoleto(banco, i));
            return boletos;
        }

        internal static Boleto GerarBoleto(Banco banco, int i)
        {
            var boleto = new Boleto
            {
                Banco = banco,
                Sacado = GerarSacado(),
                DataEmissao = DateTime.Now.AddDays(-3),
                DataProcessamento = DateTime.Now,
                DataVencimento = DateTime.Now.AddMonths(i),
                ValorTitulo = (decimal)100 * i,
                NossoNumero = (223344 + _proximoNossoNumero).ToString(),
                NumeroDocumento = "BB" + _proximoNossoNumero.ToString("D6") + (char)(64 + i),
                EspecieDocumento = TipoEspecieDocumento.DM,
                Aceite = _contador % 2 == 0 ? "N" : "A",
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
                NumeroControleParticipante = "CHAVEPRIMARIA=" + _proximoNossoNumero
            };
            // Avalista
            if (_contador % 3 == 0)
            {
                boleto.Avalista = GerarSacado();
                boleto.Avalista.Nome = boleto.Avalista.Nome.Replace("Sacado", "Avalista");
            }
            // Grupo Demonstrativo do Boleto
            var grupoDemonstrativo = new GrupoDemonstrativo { Descricao = "GRUPO 1" };
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 1, Item 1", Referencia = boleto.DataEmissao.AddMonths(-1).Month + "/" + boleto.DataEmissao.AddMonths(-1).Year, Valor = boleto.ValorTitulo * (decimal)0.15 });
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 1, Item 2", Referencia = boleto.DataEmissao.AddMonths(-1).Month + "/" + boleto.DataEmissao.AddMonths(-1).Year, Valor = boleto.ValorTitulo * (decimal)0.05 });
            boleto.Demonstrativos.Add(grupoDemonstrativo);
            grupoDemonstrativo = new GrupoDemonstrativo { Descricao = "GRUPO 2" };
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 2, Item 1", Referencia = boleto.DataEmissao.Month + "/" + boleto.DataEmissao.Year, Valor = boleto.ValorTitulo * (decimal)0.20 });
            boleto.Demonstrativos.Add(grupoDemonstrativo);
            grupoDemonstrativo = new GrupoDemonstrativo { Descricao = "GRUPO 3" };
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 3, Item 1", Referencia = boleto.DataEmissao.AddMonths(-1).Month + "/" + boleto.DataEmissao.AddMonths(-1).Year, Valor = boleto.ValorTitulo * (decimal)0.37 });
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 3, Item 2", Referencia = boleto.DataEmissao.Month + "/" + boleto.DataEmissao.Year, Valor = boleto.ValorTitulo * (decimal)0.03 });
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 3, Item 3", Referencia = boleto.DataEmissao.Month + "/" + boleto.DataEmissao.Year, Valor = boleto.ValorTitulo * (decimal)0.12 });
            grupoDemonstrativo.Itens.Add(new ItemDemonstrativo { Descricao = "Grupo 3, Item 4", Referencia = boleto.DataEmissao.AddMonths(+1).Month + "/" + boleto.DataEmissao.AddMonths(+1).Year, Valor = boleto.ValorTitulo * (decimal)0.08 });
            boleto.Demonstrativos.Add(grupoDemonstrativo);

            boleto.ValidarDados();
            _contador++;
            _proximoNossoNumero++;
            return boleto;
        }

        internal static void TestarBoletoPDF(Banco banco, string nomeCarteira)
        {
            var quantidadeBoletosParaTeste = 3;
            var boletos = GerarBoletos(banco, quantidadeBoletosParaTeste);
            Assert.AreEqual(quantidadeBoletosParaTeste, boletos.Count, "Quantidade de boletos diferente de " + quantidadeBoletosParaTeste);

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
                foreach (var boletoTmp in boletos)
                {
                    using (var boletoParaImpressao = new BoletoBancario
                    {
                        Boleto = boletoTmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = false,
                        MostrarEnderecoCedente = true,
                        ExibirDemonstrativo = true
                    })
                    {
                        html.Append("<div style=\"page-break-after: always;\">");
                        html.Append(boletoParaImpressao.MontaHtml());
                        html.Append("</div>");
                    }
                    var pdf = new HtmlToPdfConverter().GeneratePdf(html.ToString());
                    using (var fs = new FileStream(nomeArquivo, FileMode.Create))
                        fs.Write(pdf, 0, pdf.Length);
                }
            }
            catch (Exception e)
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
            const int quantidadeBoletosParaTeste = 3;
            var boletos = GerarBoletos(banco, quantidadeBoletosParaTeste);
            Assert.AreEqual(quantidadeBoletosParaTeste, boletos.Count, "Quantidade de boletos diferente de " + quantidadeBoletosParaTeste);

            // Define o nome do arquivo.
            var nomeArquivo = Path.Combine(Path.GetTempPath(), "Boleto2Net", $"{nomeCarteira}_Arquivo{tipoArquivo}.REM");

            // Cria pasta para os arquivos
            if (!Directory.Exists(Path.GetDirectoryName(nomeArquivo)))
                Directory.CreateDirectory(Path.GetDirectoryName(nomeArquivo));

            File.Delete(nomeArquivo);
            if (File.Exists(nomeArquivo))
                Assert.Fail("Arquivo Remessa não foi excluído: " + nomeArquivo);

            // Gera o arquivo remessa.
            try
            {
                var arquivoRemessa = new ArquivoRemessa(boletos.Banco, tipoArquivo, 1);
                using (var fileStream = new FileStream(nomeArquivo, FileMode.Create))
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);
            }
            catch (Exception e)
            {
                if (File.Exists(nomeArquivo))
                    File.Delete(nomeArquivo);
                Assert.Fail(e.InnerException.ToString());
            }

            // Se o arquivo existir, considera o teste OK
            Assert.IsTrue(File.Exists(nomeArquivo), "Arquivo Remessa não encontrado: " + nomeArquivo);
        }
    }
}