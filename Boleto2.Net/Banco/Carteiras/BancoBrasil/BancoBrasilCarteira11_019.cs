using System;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("11/019")]
    internal class BancoBrasilCarteira11_019 : ICarteira<BancoBrasil>
    {
        internal static Lazy<ICarteira<BancoBrasil>> Instance { get; } = new Lazy<ICarteira<BancoBrasil>>(() => new BancoBrasilCarteira11_019());

        private BancoBrasilCarteira11_019()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var codigoCedente = boleto.Banco.Cedente.Codigo;
            if (codigoCedente.Length != 7)
                throw new Exception("Não foi possível formatar o nosso número.");
            
            // Carteira 11 - Variação 019: Convênio de 7 dígitos
            // Ou deve estar em branco (o banco irá gerar)
            // Ou deve estar com 17 posições, iniciando com o código do convênio
            var nossoNumero = boleto.NossoNumero;
            if (!IsNullOrWhiteSpace(nossoNumero) && (nossoNumero.Length != 17 || !nossoNumero.StartsWith(codigoCedente)))
                throw new Exception($"Nosso Número ({nossoNumero}) deve iniciar com \"{codigoCedente}\" e conter 17 dígitos.");
            // Para convênios com 7 dígitos, não existe dígito de verificação do nosso número
            boleto.NossoNumeroDV = "";
            boleto.NossoNumeroFormatado = nossoNumero;
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = boleto.Banco.Cedente.ContaBancaria.Carteira;
            if (string.IsNullOrWhiteSpace(boleto.NossoNumero) || string.IsNullOrWhiteSpace(carteira))
                return "";
            return $"000000{boleto.NossoNumero}{carteira}";
        }
    }
}
