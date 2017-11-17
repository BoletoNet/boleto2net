using NUnit.Framework;

namespace Boleto2Net.Testes
{
    [TestFixture]
    public class BancoBrasilCarteira11019Tests
    {
        readonly IBanco _banco;
        public BancoBrasilCarteira11019Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0180",
                DigitoAgencia = "5",
                Conta = "43101",
                DigitoConta = "X",
                CarteiraPadrao = "11",
                VariacaoCarteiraPadrao = "019",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Banco
            };
            _banco = Banco.Instancia(Bancos.BancoDoBrasil);
            _banco.Cedente = Utils.GerarCedente("1740981", "", "", contaBancaria);
            _banco.FormataCedente();


        }

        [Test]
        public void Brasil_11_019_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBrasilCarteira11019Tests), 5, true, "?", 0);
        }

        [Test]
        public void Brasil_11_019_REM400()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBrasilCarteira11019Tests), 5, true, "?", 0);
        }

    }
}