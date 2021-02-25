﻿using System;
using System.Collections.Generic;
using System.Web.UI;
using Boleto2Net.Exceptions;
using static System.String;

[assembly: WebResource("BoletoNet.Imagens.422.jpg", "image/jpg")]

namespace Boleto2Net
{
    internal sealed class BancoSafra : IBanco
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new BancoSafra());

        public Cedente Cedente { get; set; }
        public int Codigo { get; } = 422;
        public string Nome { get; } = "Safra";
        public string Digito { get; } = "7";
        public List<string> IdsRetornoCnab400RegistroDetalhe { get; } = new List<string> { "1" };
        public bool RemoveAcentosArquivoRemessa { get; } = true;
        private int NumeroArquivoRemessaCNAB400 { get; set; } // Safra repete o número do arquivo remessa na linha de detalhe (CNAB 400)

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoSafra>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw Boleto2NetException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("Pagável em qualquer Banco do Sistema de Compensação","", "", 6);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoSafra>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoSafra>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return "";
        }
        public string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            this.NumeroArquivoRemessaCNAB400 = numeroArquivoRemessa;
            try
            {
                var header = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return header;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerarRegistroHeaderDoArquivoRemessa(ex);
            }
        }

        public string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                string detalhe = Empty, strline = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        detalhe += GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return detalhe;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerarRegistroDetalheDoArquivoRemessa(ex);
            }
        }

        public string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
            ref int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                var trailer = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        trailer = GerarTrailerRemessaCNAB400(ref numeroRegistroGeral, ref valorBoletoGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return trailer;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerrarRegistroTrailerDoArquivoRemessa(ex);
            }
        }

        public void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerHeaderRetornoCNAB400(ArquivoRetorno arquivoRetorno, string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");

                arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(94, 6)).ToString("##-##-##"));

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira
                boleto.Carteira = registro.Substring(107, 1);
                if (boleto.Carteira == "2")
                    boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVinculada;
                else
                    boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(62, 8);
                boleto.NossoNumeroDV = registro.Substring(70, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identificação de Ocorrência
                boleto.CodigoOcorrencia = registro.Substring(108, 2);
                boleto.DescricaoOcorrencia = DescricaoOcorrenciaCnab400(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(104, 3);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }

        private string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var numeroArquivoRemessa3Digitos = numeroArquivoRemessa.ToString().PadLeft(3, '0');
                numeroArquivoRemessa3Digitos = numeroArquivoRemessa3Digitos.Substring(numeroArquivoRemessa3Digitos.Length - 3);
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0031, 001, 0, Cedente.ContaBancaria.DigitoAgencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0034, 006, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0040, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0041, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, "422", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "BANCO SAFRA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 291, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0392, 003, 0, numeroArquivoRemessa3Digitos, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                var numeroArquivoRemessa3Digitos = this.NumeroArquivoRemessaCNAB400.ToString().PadLeft(3, '0');
                numeroArquivoRemessa3Digitos = numeroArquivoRemessa3Digitos.Substring(numeroArquivoRemessa3Digitos.Length - 3);
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Cedente.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 001, 0, Cedente.ContaBancaria.DigitoAgencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0023, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0025, 006, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0031, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0032, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 008, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 001, 0, boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 030, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0102, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0103, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 001, 0, Empty, ' ');

                if (boleto.CodigoInstrucao2 == "10")
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, boleto.DiasProtesto, '0');
                else
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, "0", '0');

                switch (boleto.TipoCarteira)
                {
                    case TipoCarteira.CarteiraCobrancaSimples:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0108, 001, 0, "1", '0');
                        break;
                    case TipoCarteira.CarteiraCobrancaVinculada:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0108, 001, 0, "2", '0');
                        break;
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0108, 001, 0, "?", '0');
                        break;
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoOcorrencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "422", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, AjustaEspecieCnab400(boleto.EspecieDocumento), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0'); // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0'); // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');

                if (boleto.CodigoOcorrencia == "01" && boleto.CodigoInstrucao1 == "16")
                {
                    // Conforme manual, item 6.1.8, quando operação for "entrada de título" e a primeira ocorrência for "16", utiliza campo para informar Multa
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0206, 006, 0, boleto.DataMulta, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0212, 004, 2, boleto.PercentualMulta, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0216, 003, 0, "000", '0');
                }
                else
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Sacado.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 010, 0, boleto.Sacado.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0325, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Sacado.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Sacado.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 030, 0, boleto.Avalista.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0382, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0389, 003, 0, "422", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0392, 003, 0, numeroArquivoRemessa3Digitos, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 1.", ex);
            }
        }

        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral, ref decimal valorBoletoGeral)
        {
            try
            {
                var numeroArquivoRemessa3Digitos = this.NumeroArquivoRemessaCNAB400.ToString().PadLeft(3, '0');
                numeroArquivoRemessa3Digitos = numeroArquivoRemessa3Digitos.Substring(numeroArquivoRemessa3Digitos.Length-3);
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 367, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0369, 008, 0, numeroRegistroGeral - 2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0377, 015, 2, valorBoletoGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0392, 003, 0, numeroArquivoRemessa3Digitos, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada";
                case "04":
                    return "Transferência de Carteira (Entrada)";
                case "05":
                    return "Transferência de Carteira (Baixa)";
                case "06":
                    return "Liquidação normal";
                case "09":
                    return "Baixado Automaticamente";
                case "10":
                    return "Baixado conforme instruções";
                case "11":
                    return "Títulos em Ser (Para Arquivo Mensal)";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Liquidação em Cartório";
                case "19":
                    return "Confirmação de instrução de protesto";
                case "20":
                    return "Confirmação de sustar protesto";
                case "21":
                    return "Transferência de beneficiário";
                case "23":
                    return "Título enviado a cartório";
                case "40":
                    return "Baixa de Título Protestado";
                case "41":
                    return "Liquidação de Título Baixado";
                case "42":
                    return "Título retirado do cartório";
                case "43":
                    return "Despesa de cartório";
                case "44":
                    return "Aceite do título DDA pelo pagador";
                case "45":
                    return "Não aceite do título DDA pelo pagador";
                case "51":
                    return "Valor do título alterado";
                case "52":
                    return "Acerto de Data de emissão";
                case "53":
                    return "Acerto de código de espécie de documento";
                case "54":
                    return "Alteração de seu número";
                case "56":
                    return "Instrução de negativação aceita";
                case "57":
                    return "Instrução de baixa de negativação aceita";
                case "58":
                    return "Instrução não negativar aceita";
                default:
                    return "";
            }
        }

        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            switch (codigoEspecie)
            {
                case "01":
                    return TipoEspecieDocumento.DM;
                case "02":
                    return TipoEspecieDocumento.NP;
                case "03":
                    return TipoEspecieDocumento.NS;
                case "05":
                    return TipoEspecieDocumento.RC;
                case "09":
                    return TipoEspecieDocumento.DS;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        private string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
        {
            switch (especieDocumento)
            {
                case TipoEspecieDocumento.DM:
                    return "01";
                case TipoEspecieDocumento.NP:
                    return "02";
                case TipoEspecieDocumento.NS:
                    return "03";
                case TipoEspecieDocumento.RC:
                    return "05";
                case TipoEspecieDocumento.DS:
                    return "09";
                default:
                    return "??";
            }
        }

        }
}


