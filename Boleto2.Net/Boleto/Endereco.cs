using System;

namespace Boleto2Net
{
    /// <summary>
    /// Representa o endereço do Cedente ou Sacado.
    /// </summary>
    public class Endereco
    {
        public string LogradouroEndereco { get; set; } = string.Empty;
        public string LogradouroNumero { get; set; } = string.Empty;
        public string LogradouroComplemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;

        public string EnderecoFormatado
        {
            get
            {
                return string.Format("{0} - {1} - {2}/{3} - CEP: {4}",
                                                            this.FormataLogradouro(0),
                                                            this.Bairro,
                                                            this.Cidade,
                                                            this.UF,
                                                            Utils.FormataCEP(this.CEP));
            }
        }

        public string EnderecoFormatadoDuasLinhas 
        { 
            get
            {
                var endereco = this.FormataLogradouro(0) + "<br />" + string.Format("{0} - {1}/{2}", this.Bairro, this.Cidade, this.UF);
                if (this.CEP != String.Empty)
                    endereco += string.Format(" - CEP: {0}", Utils.FormataCEP(this.CEP));

                return endereco;
            } 
        }

        public string EnderecoFormatadoCompacto
        {
            get
            {
                return string.Format("{0} - CEP: {1}", this.FormataLogradouro(25), Utils.FormataCEP(this.CEP));
            }
        }

        public string FormataLogradouro(int tamanhoFinal)
        {
            var logradouroCompleto = string.Empty;
            if (!string.IsNullOrEmpty(LogradouroNumero))
                logradouroCompleto += " " + LogradouroNumero;
            if (!string.IsNullOrEmpty(LogradouroComplemento))
                logradouroCompleto += " " + (LogradouroComplemento.Length > 20 ? LogradouroComplemento.Substring(0, 20) : LogradouroComplemento);

            if (tamanhoFinal == 0)
                return LogradouroEndereco + logradouroCompleto;

            if (LogradouroEndereco.Length + logradouroCompleto.Length <= tamanhoFinal)
                return LogradouroEndereco + logradouroCompleto;

            return LogradouroEndereco.Substring(0, tamanhoFinal - logradouroCompleto.Length) + logradouroCompleto;
        }
    }
}
