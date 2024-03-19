using System;
using System.Collections.Generic;
using System.Web.UI;
using Boleto2Net.Exceptions;
using Boleto2Net.Extensions;
using static System.String;

[assembly: WebResource("BoletoNet.Imagens.246.jpg", "image/jpg")]

namespace Boleto2Net
{
    internal sealed class BancoABC : IBanco
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new BancoABC());

        public Cedente Cedente { get; set; }
        public int Codigo { get; } = 246;
        public string Nome { get; } = "ABC Brasil";
        public string Digito { get; } = "1";
        public List<string> IdsRetornoCnab400RegistroDetalhe => throw new NotImplementedException();
        public bool RemoveAcentosArquivoRemessa => throw new NotImplementedException();

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoABC>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw Boleto2NetException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM TODA REDE BANCÁRIA", "", "", 9);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia} / {contaBancaria.Conta}";
        }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoABC>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoABC>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        public string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            throw new NotImplementedException();
        }

        public string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
            ref int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
            throw new NotImplementedException();
        }

        public string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            throw new NotImplementedException();
        }

        private string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        private string GerarDetalheRemessaCNAB400Registro2(Boleto boleto, ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        private string GerarDetalheRemessaCNAB400Registro5(Boleto boleto, ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            throw new NotImplementedException();
        }

        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            throw new NotImplementedException();
        }

        private string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
        {
            throw new NotImplementedException();
        }
    }
}
