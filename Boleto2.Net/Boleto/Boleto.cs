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

        public TipoEspecieDocumento EspecieDocumento { get; set; } = TipoEspecieDocumento.NaoDefinido;

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
        public decimal ValorCredito { get; set; }
        public decimal ValorJurosDia { get; set; }
        public decimal ValorMulta { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTarifas { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorOutrosCreditos { get; set; }
        public decimal ValorIOF { get; set; }
        public decimal ValorAbatimento { get; set; }

        // Juros
        public decimal PercentualJurosDia { get; set; }
        public DateTime DataJuros { get; set; }
        // Multa
        public decimal PercentualMulta { get; set; }
        public DateTime DataMulta { get; set; }
        // Desconto
        public DateTime DataDesconto { get; set; }


        public string CodigoOcorrencia { get; set; } = string.Empty;
        public string DescricaoOcorrencia { get; set; } = string.Empty;
        public string CodigoOcorrenciaAuxiliar { get; set; } = string.Empty;

        public TipoCodigoProtesto CodigoProtesto { get; set; } = TipoCodigoProtesto.NaoProtestar;
        public int DiasProtesto { get; set; } = 0;
        public TipoCodigoBaixaDevolucao CodigoBaixaDevolucao { get; set; } = TipoCodigoBaixaDevolucao.BaixarDevolver;
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
        public Sacado Sacado { get; set; } = new Sacado();
        public Sacado Avalista { get; set; } = new Sacado();
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

            // Aceite
            if (this.Aceite != "A" & this.Aceite != "N")
                throw new Exception("Aceite do Boleto deve ser definido com A ou N");

            this.Banco.ValidaBoleto(this);
        }
    }
}
