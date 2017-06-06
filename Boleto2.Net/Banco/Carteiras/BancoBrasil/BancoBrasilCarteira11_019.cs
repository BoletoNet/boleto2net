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
            if (boleto.Banco.Cedente.Codigo.Length != 7)
                throw new Exception("Não foi possível formatar o nosso número.");
            
            // Carteira 11 - Variação 019: Convênio de 7 dígitos
            // Ou deve estar em branco (o banco irá gerar)
            // Ou deve estar com 17 posições, iniciando com o código do convênio
            if (!IsNullOrWhiteSpace(boleto.NossoNumero) && (boleto.NossoNumero.Length != 17 || !boleto.NossoNumero.StartsWith(boleto.Banco.Cedente.Codigo)))
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Cedente.Codigo}\" e conter 17 dígitos.");
            
            // Para convênios com 7 dígitos, não existe dígito de verificação do nosso número
            boleto.NossoNumeroDV = "";
            boleto.NossoNumeroFormatado = boleto.NossoNumero;
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"000000{boleto.NossoNumero}{boleto.Banco.Cedente.ContaBancaria.Carteira}";
        }
    }
}
