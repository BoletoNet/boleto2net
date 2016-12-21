using System;
using System.ComponentModel;

namespace Boleto2Net
{
    [Serializable, Browsable(false)]
    public class Cedente
    {
        private string _cpfcnpj;
        public string Codigo { get; set; } = string.Empty;
        public string CodigoDV { get; set; } = string.Empty;
        public string CodigoFormatado { get; set; } = string.Empty;
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
                    throw new ArgumentException("CPF/CNPJ inválido: Utilize 11 dígitos para CPF ou 14 para CPNJ.");
                _cpfcnpj = o;
            }
        }
        public string Tipo1CPF2CNPJ { get { return CPFCNPJ.Length <= 11 ? "1" : "2"; } }
        public string Nome { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public ContaBancaria ContaBancaria { get; set; } = new ContaBancaria();
        public Endereco Endereco { get; set; } = new Endereco();
        public bool MostrarCNPJnoBoleto { get; set; } = false;
    }
}
