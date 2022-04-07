using System;
using System.ComponentModel;

namespace Boleto2Net
{
    [Serializable]
#if NET40_OR_GREATER
    [Browsable(false)]
#endif
    public class Cedente
    {
        private string _cpfcnpj;
        public string Codigo { get; set; } = string.Empty;
        public string CodigoDV { get; set; } = string.Empty;
        public string CodigoFormatado { get; set; } = string.Empty;
        public string CodigoTransmissao { get; set; } = string.Empty;
        public string CPFCNPJ
        {
            get
            {
                return _cpfcnpj;
            }
            set
            {
                string o = value.Replace(".", "").Replace("-", "").Replace("/", "");
                if (o == null || (o.Length != 11 && o.Length != 14))
                    throw new ArgumentException("CPF/CNPJ inv�lido: Utilize 11 d�gitos para CPF ou 14 para CPNJ.");
                _cpfcnpj = o;
            }
        }
        public string TipoCPFCNPJ(string formatoRetorno)
        {
            if (CPFCNPJ == string.Empty)
                return "0";
            switch (formatoRetorno)
            {
                case "A":
                    return CPFCNPJ.Length <= 11 ? "F" : "J";
                case "0":
                    return CPFCNPJ.Length <= 11 ? "1" : "2";
                case "00":
                    return CPFCNPJ.Length <= 11 ? "01" : "02";
            }
            throw new Exception("TipoCPFCNPJ: Formato do retorno inv�lido.");
        }
        public string Nome { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public ContaBancaria ContaBancaria { get; set; } = new ContaBancaria();
        public Endereco Endereco { get; set; } = new Endereco();
        public bool MostrarCNPJnoBoleto { get; set; } = true;
    }
}
