﻿using System;
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
                CPFCNPJ = "86.875.666/0001-09",
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
                    CPFCNPJ = "443.316.101-28",
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
                CPFCNPJ = "71.738.978/0001-01",
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

        internal static Boletos GerarBoletos(IBanco banco, int quantidadeBoletos, string aceite, int NossoNumeroInicial)
        {
            var boletos = new Boletos
            {
                Banco = banco
            };
            for (var i = 1; i <= quantidadeBoletos; i++)
                boletos.Add(GerarBoleto(banco, i, aceite, NossoNumeroInicial));
            return boletos;
        }

        internal static Boleto GerarBoleto(IBanco banco, int i, string aceite, int NossoNumeroInicial)
        {
            if (aceite == "?")
                aceite = _contador % 2 == 0 ? "N" : "A";

            var boleto = new Boleto(banco)
            {
                Sacado = GerarSacado(),
                DataEmissao = DateTime.Now.AddDays(-3),
                DataProcessamento = DateTime.Now,
                DataVencimento = DateTime.Now.AddMonths(i),
                ValorTitulo = (decimal)100 * i,
                NossoNumero = NossoNumeroInicial == 0 ? "" : (NossoNumeroInicial + _proximoNossoNumero).ToString(),
                NumeroDocumento = "BB" + _proximoNossoNumero.ToString("D6") + (char)(64 + i),
                EspecieDocumento = TipoEspecieDocumento.DM,
                Aceite = aceite,
                CodigoInstrucao1 = "11",
                CodigoInstrucao2 = "22",
                DataDesconto = DateTime.Now.AddMonths(i),
                ValorDesconto = (decimal)(100 * i * 0.10),
                DataMulta = DateTime.Now.AddMonths(i),
                PercentualMulta = (decimal)2.00,
                ValorMulta = (decimal)(100 * i * (2.00 / 100)),
                DataJuros = DateTime.Now.AddMonths(i),
                PercentualJurosDia = (decimal)0.2,
                ValorJurosDia = (decimal)(100 * i * (0.2 / 100)),
                MensagemArquivoRemessa = "Mensagem para o arquivo remessa",
                NumeroControleParticipante = "CHAVEPRIMARIA=" + _proximoNossoNumero
            };
            // Mensagem - Instruções do Caixa
            StringBuilder msgCaixa = new StringBuilder();
            if (boleto.ValorDesconto > 0)
                msgCaixa.AppendLine($"Conceder desconto de {boleto.ValorDesconto.ToString("R$ ##,##0.00")} até {boleto.DataDesconto.ToString("dd/MM/yyyy")}. ");
            if (boleto.ValorMulta > 0)
                msgCaixa.AppendLine($"Cobrar multa de {boleto.ValorMulta.ToString("R$ ##,##0.00")} após o vencimento. ");
            if (boleto.ValorJurosDia > 0)
                msgCaixa.AppendLine($"Cobrar juros de {boleto.ValorJurosDia.ToString("R$ ##,##0.00")} por dia de atraso. ");
            boleto.MensagemInstrucoesCaixa = msgCaixa.ToString();
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

        internal static void TestarHomologacao(IBanco banco, TipoArquivo tipoArquivo, string nomeCarteira, int quantidadeBoletos, bool gerarPDF, string aceite, int NossoNumeroInicial)
        {
            var boletos = GerarBoletos(banco, quantidadeBoletos, aceite, NossoNumeroInicial);
            Assert.AreEqual(quantidadeBoletos, boletos.Count, "Quantidade de boletos diferente de " + quantidadeBoletos);

            // Define os nomes dos arquivos, cria pasta e apaga arquivos anteriores
            var nomeArquivoREM = Path.Combine(Path.GetTempPath(), "Boleto2Net", $"{nomeCarteira}_{tipoArquivo}.REM");
            var nomeArquivoPDF = Path.Combine(Path.GetTempPath(), "Boleto2Net", $"{nomeCarteira}_{tipoArquivo}.PDF");
            if (!Directory.Exists(Path.GetDirectoryName(nomeArquivoREM)))
                Directory.CreateDirectory(Path.GetDirectoryName(nomeArquivoREM));
            if (File.Exists(nomeArquivoREM))
            {
                File.Delete(nomeArquivoREM);
                if (File.Exists(nomeArquivoREM))
                    Assert.Fail("Arquivo Remessa não foi excluído: " + nomeArquivoREM);
            }
            if (File.Exists(nomeArquivoPDF))
            {
                File.Delete(nomeArquivoPDF);
                if (File.Exists(nomeArquivoPDF))
                    Assert.Fail("Arquivo Boletos (PDF) não foi excluído: " + nomeArquivoPDF);
            }

            // Arquivo Remessa.
            try
            {
                var arquivoRemessa = new ArquivoRemessa(boletos.Banco, tipoArquivo, 1);
                using (var fileStream = new FileStream(nomeArquivoREM, FileMode.Create))
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);
                if (!File.Exists(nomeArquivoREM))
                    Assert.Fail("Arquivo Remessa não encontrado: " + nomeArquivoREM);
            }
            catch (Exception e)
            {
                if (File.Exists(nomeArquivoREM))
                    File.Delete(nomeArquivoREM);
                Assert.Fail(e.InnerException.ToString());
            }

            if (gerarPDF)
            {
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
                        using (var fs = new FileStream(nomeArquivoPDF, FileMode.Create))
                            fs.Write(pdf, 0, pdf.Length);
                        if (!File.Exists(nomeArquivoPDF))
                            Assert.Fail("Arquivo Boletos (PDF) não encontrado: " + nomeArquivoPDF);

                    }
                }
                catch (Exception e)
                {
                    if (File.Exists(nomeArquivoPDF))
                        File.Delete(nomeArquivoPDF);
                    Assert.Fail(e.InnerException.ToString());
                }
            }
        }
    }
}