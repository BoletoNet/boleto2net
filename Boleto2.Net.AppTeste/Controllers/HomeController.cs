using Boleto2.Net.AppTeste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boleto2.Net.AppTeste.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new GeraBoletoViewModel();

            model.ConfiguraModel();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        public ActionResult GeraBoleto(GeraBoletoViewModel model)
        {
            var mensagemErro = "";

            try
            {
                var boletoProxy = new Boleto2Net.Boleto2NetProxy();


                boletoProxy.SetupCobranca(model.Cedente_CPFCNPJ, model.Cedente_Nome, model.Cedente_LogradouroEndereco, model.Cedente_LogradouroNumero,
                    model.Cedente_LogradouroComplemento, model.Cedente_Bairro, model.Cedente_Cidade, model.Cedente_UF,
                    model.Cedente_CEP, model.Cedente_Observacoes, model.Cedente_NumeroBanco, model.Cedente_Agencia, model.Cedente_DigitoAgencia,
                    model.Cedente_OperacaoConta, model.Cedente_Conta, model.Cedente_DigitoAgencia, model.Cedente_Codigo, model.Cedente_CodigoDV,
                    model.Cedente_CodigoTransmissao, model.Cedente_Carteira, model.Cedente_VariacaoCarteira, (int)model.Cedente_TipoCarteira, (int)model.Cedente_TipoFormaCadastramento,
                    (int)model.Cedente_TipoImpressaoBoleto, (int)model.Cedente_TipoDocumento, ref mensagemErro);

                boletoProxy.NovoBoleto(ref mensagemErro);
                boletoProxy.DefinirBoleto("DM", "123456", "453", DateTime.Now, DateTime.Now, DateTime.Now.AddDays(10), 141.50m, "1", "N", ref mensagemErro);
                boletoProxy.DefinirSacado("32145698700199", "Empresa Sacado", "Rua do sacado", "0", "Quadra 18", "Jardim Jardim", "Pindamoiangaba", "AC", "11000-000", "Pessoa", ref mensagemErro);
                boletoProxy.DefinirJuros(DateTime.Now, 18, 2, ref mensagemErro);
                boletoProxy.DefinirDesconto(DateTime.Now.AddDays(10), 50, ref mensagemErro);
                boletoProxy.DefinirMulta(DateTime.Now.AddDays(11), 50, 10, ref mensagemErro);
                boletoProxy.DefinirProtesto(1, 15, ref mensagemErro);
                boletoProxy.DefinirInstrucoes("Instrução Caixa <br> Instrução 2 <br/> Instrução 3 <br /> Instrução 4 <br> Instrução 5", "Mensagem remessa", "Instrução 1", "Instrucao 1 Aux", "Instrução 2", "Instrução 2 Aux", "Instrução 3", "Instrução 3 Aux", true, ref mensagemErro);
                boletoProxy.FecharBoleto(ref mensagemErro);

                //boletoProxy.boleto.NossoNumeroFormatado = $"{boletoProxy.boleto.NossoNumero}-{boletoProxy.boleto.NossoNumeroDV}";
                //boletoProxy.boleto.Banco.Cedente.ContaBancaria.MensagemFixaSacado = "Instrução Mensagem Fixa Sacado";

                var boletoBancario = new Boleto2Net.BoletoBancario();

                boletoBancario.Boleto = boletoProxy.boleto;

                boletoBancario.MostrarComprovanteEntrega = true;
                boletoBancario.MostrarComprovanteEntregaLivre = false;

                var pdf = boletoBancario.MontaBytesPDF();

                return File(pdf, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            catch (Exception ex)
            {
                return Json($"{ex.Message}");
            }

        }

    }
}