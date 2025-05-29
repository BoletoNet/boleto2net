using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Boleto2Net
{
    [Serializable(), Browsable(true)]
    public class Sacado
    {
        private string _cpfcnpj = string.Empty;

        public string Nome { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public Endereco Endereco { get; set; } = new Endereco();
        public string CPFCNPJ
        {
            get
            {
                return _cpfcnpj;
            }
            set
            {
                string numero = value.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
                if (numero == null || (numero.Length != 11 && numero.Length != 14))
                    throw new ArgumentException("CPF/CNPJ inv�lido: Utilize 11 d�gitos para CPF ou 14 para CPNJ.");
                _cpfcnpj = numero;
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
    }
}

