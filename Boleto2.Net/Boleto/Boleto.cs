namespace Boleto2Net
{
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    [Serializable, Browsable(false)]
    public class Boleto
    {
        public int CodigoMoeda { get; set; } = 9;
        public string EspecieMoeda { get; set; } = "R$";
        public int QuantidadeMoeda { get; set; } = 0;
        public string ValorMoeda { get; set; } = string.Empty;

        public string CodigoEspecieDocumento { get; set; } = string.Empty;
        public string SiglaEspecieDocumento { get; set; } = string.Empty;

        public string NossoNumero { get; set; } = string.Empty;
        public string NossoNumeroDV { get; set; } = string.Empty;
        public string NossoNumeroFormatado { get; set; } = string.Empty;

        public DateTime DataProcessamento { get; set; } = DateTime.Now;
        public DateTime DataEmissao { get; set; } = DateTime.Now;
        public DateTime DataVencimento { get; set; }
        public DateTime DataCredito { get; set; }

        public string NumeroDocumento { get; set; } = string.Empty;
        public string NumeroControleParticipante { get; set; } = string.Empty;
        public string Aceite { get; set; } = "N";
        public string UsoBanco { get; set; } = string.Empty;

        // Valores do Boleto
        public decimal ValorTitulo { get; set; }
        public decimal ValorPago { get; set; }
        // Se houver apenas um valor (CNAB400) utilizar a propriedade ValorPago
        // Se houver dois valores (CNAB240) utilizar as propriedades ValorPago e ValorCredito
        public decimal ValorCredito { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorMulta { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTarifas { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorOutrosCreditos { get; set; }
        public decimal ValorIOF { get; set; }
        public decimal ValorAbatimento { get; set; }

        // Juros
        public decimal PercentualJuros { get; set; }
        public DateTime DataJuros { get; set; }
        // Multa
        public decimal PercentualMulta { get; set; }
        public DateTime DataMulta { get; set; }
        // Desconto
        public DateTime DataDesconto { get; set; }


        public string CodigoOcorrencia { get; set; } = string.Empty;
        public string DescricaoOcorrencia { get; set; } = string.Empty;
        public string CodigoOcorrenciaAuxiliar { get; set; } = string.Empty;

        public int CodigoProtesto { get; set; } = 3; // 1-Protestar, 3-Não Protestar, 9-Cancelamento Protesto Automatico
        public int DiasProtesto { get; set; } = 0;
        public int CodigoBaixaDevolucao { get; set; } = 1; // 1-Baixar/Devolver, 2-Não Baixar/Não Devolver
        public int DiasBaixaDevolucao { get; set; } = 60;

        public string CodigoInstrucao1 { get; set; } = string.Empty;
        public string ComplementoInstrucao1 { get; set; } = string.Empty;
        public string CodigoInstrucao2 { get; set; } = string.Empty;
        public string ComplementoInstrucao2 { get; set; } = string.Empty;
        public string CodigoInstrucao3 { get; set; } = string.Empty;
        public string ComplementoInstrucao3 { get; set; } = string.Empty;

        public string MensagemInstrucoesCaixa { get; set; } = string.Empty;
        public string MensagemArquivoRemessa { get; set; } = string.Empty;
        public string RegistroArquivoRetorno { get; set; } = string.Empty;

        public IBanco Banco { get; set; }
        public Sacado Sacado { get; set; }
        public CodigoBarra CodigoBarra { get; } = new CodigoBarra();
        public ObservableCollection<GrupoDemonstrativo> Demonstrativos { get; } = new ObservableCollection<GrupoDemonstrativo>();

        public void ValidarDados()
        {
            // Banco Obrigatório
            if (this.Banco == null)
                throw new Exception("Boleto não possui Banco.");

            // Cedente Obrigatório
            if (this.Banco.Cedente == null)
                throw new Exception("Boleto não possui cedente.");

            // Conta Bancária Obrigatória
            if (this.Banco.Cedente.ContaBancaria == null)
                throw new Exception("Boleto não possui conta bancária.");

            // Sacado Obrigatório
            if (this.Sacado == null)
                throw new Exception("Boleto não possui sacado.");

            // Verifica se data do processamento é valida
            if (this.DataProcessamento == DateTime.MinValue)
                this.DataProcessamento = DateTime.Now;

            // Verifica se data de emissão é valida
            if (this.DataEmissao == DateTime.MinValue)
                this.DataEmissao = DateTime.Now;

            this.Banco.ValidaBoleto(this);
        }
    }
}
