using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web.UI;
//Envio por email
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

[assembly: WebResource("Boleto2Net.BoletoImpressao.BoletoNet.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("Boleto2Net.Imagens.barra.gif", "image/gif")]

namespace Boleto2Net
{
    using System.Linq;

    [Serializable(),
    Designer(typeof(BoletoBancarioDesigner)),
    ToolboxBitmap(typeof(BoletoBancario)),
    ToolboxData("<{0}:BoletoBancario Runat=\"server\"></{0}:BoletoBancario>")]
    public class BoletoBancario : System.Web.UI.Control
    {
        String vLocalLogoCedente = String.Empty;

        #region Variaveis

        private string _instrucoesHtml = string.Empty;
        private bool _mostrarCodigoCarteira = false;
        private bool _formatoCarne = false;
        #endregion Variaveis

        #region Propriedades

        /// <summary>
        /// Mostra o código da carteira
        /// </summary>
        [Browsable(true), Description("Mostra a descrição da carteira")]
        public bool MostrarCodigoCarteira
        {
            get { return _mostrarCodigoCarteira; }
            set { _mostrarCodigoCarteira = value; }
        }

        [Browsable(true), Description("Gera um relatório com os valores que deram origem ao boleto")]
        public bool ExibirDemonstrativo { get; set; }

        /// <summary>
        /// Mostra o código da carteira
        /// </summary>
        [Browsable(true), Description("Formata o boleto no layout de carnê")]
        public bool FormatoCarne
        {
            get { return _formatoCarne; }
            set { _formatoCarne = value; }
        }

        [Browsable(false)]
        public Boleto boleto { get; set; }

        [Browsable(false)]
        public Banco Banco { get; set; }
        //{
        //	get
        //	{
        //		if ((_ibanco == null) ||
        //			(_ibanco.Codigo != _codigoBanco))
        //		{
        //			_ibanco = new Banco(_codigoBanco);
        //		}

        //		if (_boleto != null)
        //			_boleto.Banco = _ibanco;

        //		return _ibanco;
        //	}
        //}

        #region Propriedades
        [Browsable(true), Description("Mostra o comprovante de entrega sem dados para marcar")]
        public bool MostrarComprovanteEntregaLivre
        {
            get { return Utils.ToBool(ViewState["1"]); }
            set { ViewState["1"] = value; }
        }

        [Browsable(true), Description("Mostra o comprovante de entrega")]
        public bool MostrarComprovanteEntrega
        {
            get { return Utils.ToBool(ViewState["2"]); }
            set { ViewState["2"] = value; }
        }

        [Browsable(true), Description("Oculta as intruções do boleto")]
        public bool OcultarEnderecoSacado
        {
            get { return Utils.ToBool(ViewState["3"]); }
            set { ViewState["3"] = value; }
        }

        [Browsable(true), Description("Oculta as intruções do boleto")]
        public bool OcultarInstrucoes
        {
            get { return Utils.ToBool(ViewState["4"]); }
            set { ViewState["4"] = value; }
        }

        [Browsable(true), Description("Oculta o recibo do sacado do boleto")]
        public bool OcultarReciboSacado
        {
            get { return Utils.ToBool(ViewState["5"]); }
            set { ViewState["5"] = value; }
        }

        [Browsable(true), Description("Gerar arquivo de remessa")]
        public bool GerarArquivoRemessa
        {
            get { return Utils.ToBool(ViewState["6"]); }
            set { ViewState["6"] = value; }
        }
        /// <summary> 
        /// Mostra o termo "Contra Apresentação" na data de vencimento do boleto
        /// </summary>
        public bool MostrarContraApresentacaoNaDataVencimento
        {
            get { return Utils.ToBool(ViewState["7"]); }
            set { ViewState["7"] = value; }
        }

        [Browsable(true), Description("Mostra o endereço do Cedente")]
        public bool MostrarEnderecoCedente
        {
            get { return Utils.ToBool(ViewState["8"]); }
            set { ViewState["8"] = value; }
        }
        #endregion Propriedades

        #endregion Propriedades

        public static string UrlLogo(int banco)
        {
            var page = System.Web.HttpContext.Current.CurrentHandler as Page;
            return page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "Boleto2Net.Imagens." + Utils.FormatCode(banco.ToString(), 3) + ".jpg");
        }

        #region Override
        protected override void OnPreRender(EventArgs e)
        {
            string alias = "Boleto2Net.BoletoImpressao.BoletoNet.css";
            string csslink = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" +
                Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), alias) + "\" />";

            var include = new LiteralControl(csslink);
            Page.Header.Controls.Add(include);

            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void Render(HtmlTextWriter output)
        {
            string urlImagemLogo = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "Boleto2Net.Imagens." + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg");
            string urlImagemBarra = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "Boleto2Net.Imagens.barra.gif");
            //string urlImagemBarraInterna = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "BoletoNet.Imagens.barrainterna.gif");
            //string urlImagemCorte = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "BoletoNet.Imagens.corte.gif");
            //string urlImagemPonto = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "BoletoNet.Imagens.ponto.gif");

            //Atribui os valores ao html do boleto bancário
            //output.Write(MontaHtml(urlImagemCorte, urlImagemLogo, urlImagemBarra, urlImagemPonto, urlImagemBarraInterna,
            //    "<img src=\"ImagemCodigoBarra.ashx?code=" + Boleto.CodigoBarra.Codigo + "\" alt=\"Código de Barras\" />"));
            output.Write(MontaHtml(urlImagemLogo, urlImagemBarra, "<img src=\"ImagemCodigoBarra.ashx?code=" + boleto.CodigoBarra.CodigoDeBarras + "\" alt=\"Código de Barras\" />"));
        }
        #endregion Override

        #region Html
        public string GeraHtmlInstrucoes()
        {
            try
            {
                var html = new StringBuilder();

                string titulo = "Instruções de Impressão";
                string instrucoes = "Imprimir em impressora jato de tinta (ink jet) ou laser em qualidade normal. (Não use modo econômico).<br>Utilize folha A4 (210 x 297 mm) ou Carta (216 x 279 mm) - Corte na linha indicada<br>";

                html.Append(Html.Instrucoes);
                html.Append("<br />");

                return html.ToString()
                    .Replace("@TITULO", titulo)
                    .Replace("@INSTRUCAO", instrucoes);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a execução da transação.", ex);
            }
        }

        private string GeraHtmlCarne(string telefone, string htmlBoleto)
        {
            var html = new StringBuilder();

            html.Append(Html.Carne);

            return html.ToString()
                .Replace("@TELEFONE", telefone)
                .Replace("#BOLETO#", htmlBoleto);
        }
        public string GeraHtmlReciboSacado()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(Html.ReciboSacadoParte1);
                html.Append("<br />");
                html.Append(Html.ReciboSacadoParte2);
                html.Append(Html.ReciboSacadoParte3);
                if (MostrarEnderecoCedente)
                {
                    html.Append(Html.ReciboSacadoParte10);
                }
                html.Append(Html.ReciboSacadoParte4);
                html.Append(Html.ReciboSacadoParte5);
                html.Append(Html.ReciboSacadoParte6);
                html.Append(Html.ReciboSacadoParte7);
                html.Append(Html.ReciboSacadoParte8);
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a execução da transação.", ex);
            }
        }

        public string GeraHtmlReciboCedente()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(Html.ReciboCedenteParte1);
                html.Append(Html.ReciboCedenteParte2);
                html.Append(Html.ReciboCedenteParte3);
                html.Append(Html.ReciboCedenteParte4);
                html.Append(Html.ReciboCedenteParte5);
                html.Append(Html.ReciboCedenteParte6);
                html.Append(Html.ReciboCedenteParte7);
                html.Append(Html.ReciboCedenteParte8);
                html.Append(Html.ReciboCedenteParte9);
                html.Append(Html.ReciboCedenteParte10);
                html.Append(Html.ReciboCedenteParte11);
                html.Append(Html.ReciboCedenteParte12);
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro na execução da transação.", ex);
            }
        }

        public string HtmlComprovanteEntrega
        {
            get
            {
                var html = new StringBuilder();

                html.Append(Html.ComprovanteEntrega1);
                html.Append(Html.ComprovanteEntrega2);
                html.Append(Html.ComprovanteEntrega3);
                html.Append(Html.ComprovanteEntrega4);
                html.Append(Html.ComprovanteEntrega5);
                html.Append(Html.ComprovanteEntrega6);

                html.Append(MostrarComprovanteEntregaLivre ? Html.ComprovanteEntrega71 : Html.ComprovanteEntrega7);

                html.Append("<br />");
                return html.ToString();
            }
        }

        private string MontaHtml(string urlImagemLogo, string urlImagemBarra, string imagemCodigoBarras)
        {
            var html = new StringBuilder();
            string enderecoCedente = "";

            //Oculta o cabeçalho das instruções do boleto
            if (!OcultarInstrucoes)
                html.Append(GeraHtmlInstrucoes());

            if (this.ExibirDemonstrativo && this.boleto.Demonstrativos.Any())
            {
                html.Append(Html.ReciboCedenteRelatorioValores);
                html.Append(Html.ReciboCedenteParte5);

                html.Append(Html.CabecalhoTabelaDemonstrativo);

                var grupoDemonstrativo = new StringBuilder();

                foreach (var relatorio in this.boleto.Demonstrativos)
                {
                    var first = true;

                    foreach (var item in relatorio.Itens)
                    {
                        grupoDemonstrativo.Append(Html.GrupoDemonstrativo);

                        if (first)
                        {
                            grupoDemonstrativo = grupoDemonstrativo.Replace("@DESCRICAOGRUPO", relatorio.Descricao);

                            first = false;
                        }
                        else
                        {
                            grupoDemonstrativo = grupoDemonstrativo.Replace("@DESCRICAOGRUPO", string.Empty);
                        }

                        grupoDemonstrativo = grupoDemonstrativo.Replace("@DESCRICAOITEM", item.Descricao);
                        grupoDemonstrativo = grupoDemonstrativo.Replace("@REFERENCIAITEM", item.Referencia);
                        grupoDemonstrativo = grupoDemonstrativo.Replace("@VALORITEM", item.Valor.ToString("R$ ##,##0.00"));
                    }

                    grupoDemonstrativo.Append(Html.TotalDemonstrativo);
                    grupoDemonstrativo = grupoDemonstrativo.Replace(
                        "@VALORTOTALGRUPO",
                        relatorio.Itens.Sum(c => c.Valor).ToString("R$ ##,##0.00"));
                }

                html = html.Replace("@ITENSDEMONSTRATIVO", grupoDemonstrativo.ToString());
            }

            if (!FormatoCarne)
            {
                //Mostra o comprovante de entrega
                if (MostrarComprovanteEntrega | MostrarComprovanteEntregaLivre)
                {
                    html.Append(HtmlComprovanteEntrega);
                    //Html da linha pontilhada
                    if (OcultarReciboSacado)
                        html.Append(Html.ReciboSacadoParte8);
                }

                //Oculta o recibo do sacabo do boleto
                if (!OcultarReciboSacado)
                {
                    html.Append(GeraHtmlReciboSacado());

                    //Caso mostre o Endereço do Cedente
                    if (MostrarEnderecoCedente)
                    {
                        if (boleto.Banco.Cedente.Endereco == null)
                            throw new ArgumentNullException("Endereço do Cedente");

                        enderecoCedente = string.Format("{0} - {1} - {2}/{3} - CEP: {4}",
                                                            boleto.Banco.Cedente.Endereco.FormataLogradouro(0),
                                                            boleto.Banco.Cedente.Endereco.Bairro,
                                                            boleto.Banco.Cedente.Endereco.Cidade,
                                                            boleto.Banco.Cedente.Endereco.UF,
                                                            Utils.FormataCEP(boleto.Banco.Cedente.Endereco.CEP));
                    }
                }
            }

            // Dados do Sacado
            var sacado = string.Empty;
            var infoSacado = string.Empty;
            if (boleto.Sacado.CPFCNPJ == string.Empty)
                sacado = boleto.Sacado.Nome;
            else
                if (boleto.Sacado.CPFCNPJ.Length <= 11)
                sacado = string.Format("{0} - CPF: {1}", boleto.Sacado.Nome, Utils.FormataCPF(boleto.Sacado.CPFCNPJ));
            else
                sacado = string.Format("{0} - CNPJ: {1}", boleto.Sacado.Nome, Utils.FormataCNPJ(boleto.Sacado.CPFCNPJ));

            if (boleto.Sacado.Observacoes != string.Empty)
                sacado += " - " + boleto.Sacado.Observacoes;

            if (!OcultarEnderecoSacado)
            {
                infoSacado = boleto.Sacado.Endereco.FormataLogradouro(0) + "<br />" + string.Format("{0} - {1}/{2}", boleto.Sacado.Endereco.Bairro, boleto.Sacado.Endereco.Cidade, boleto.Sacado.Endereco.UF);
                if (boleto.Sacado.Endereco.CEP != String.Empty)
                    infoSacado += string.Format(" - CEP: {0}", Utils.FormataCEP(boleto.Sacado.Endereco.CEP));
            }

            // Dados do Avalista
            var avalista = string.Empty;
            if (boleto.Avalista.Nome != string.Empty & boleto.Avalista.CPFCNPJ == string.Empty)
                avalista = "- " + boleto.Avalista.Nome;
            else
                if (boleto.Avalista.CPFCNPJ.Length <= 11)
                avalista = string.Format("- {0} - CPF: {1}", boleto.Avalista.Nome, Utils.FormataCPF(boleto.Avalista.CPFCNPJ));
            else
                avalista = string.Format("- {0} - CNPJ: {1}", boleto.Avalista.Nome, Utils.FormataCNPJ(boleto.Avalista.CPFCNPJ));

            if (boleto.Avalista.Observacoes != string.Empty)
                avalista += " - " + boleto.Avalista.Observacoes;


            if (!FormatoCarne)
                html.Append(GeraHtmlReciboCedente());
            else
            {
                html.Append(GeraHtmlCarne("", GeraHtmlReciboCedente()));
            }

            string dataVencimento = boleto.DataVencimento.ToString("dd/MM/yyyy");

            if (MostrarContraApresentacaoNaDataVencimento)
                dataVencimento = "Contra Apresentação";

            if (String.IsNullOrWhiteSpace(vLocalLogoCedente))
                vLocalLogoCedente = urlImagemLogo;

            return html
                .Replace("@CODIGOBANCO", Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3))
                .Replace("@DIGITOBANCO", boleto.Banco.Digito.ToString())
                .Replace("@URLIMAGEMLOGO", urlImagemLogo)
                .Replace("@URLIMGCEDENTE", vLocalLogoCedente)
                .Replace("@URLIMAGEMBARRA", urlImagemBarra)
                .Replace("@LINHADIGITAVEL", boleto.CodigoBarra.LinhaDigitavel)
                .Replace("@LOCALPAGAMENTO", boleto.Banco.Cedente.ContaBancaria.LocalPagamento)
                .Replace("@DATAVENCIMENTO", dataVencimento)
                .Replace("@CEDENTE_BOLETO", !boleto.Banco.Cedente.MostrarCNPJnoBoleto ? boleto.Banco.Cedente.Nome : string.Format("{0}&nbsp;&nbsp;&nbsp;CNPJ: {1}", boleto.Banco.Cedente.Nome, boleto.Banco.Cedente.CPFCNPJ))
                .Replace("@CEDENTE", boleto.Banco.Cedente.Nome)
                .Replace("@DATADOCUMENTO", boleto.DataEmissao.ToString("dd/MM/yyyy"))
                .Replace("@NUMERODOCUMENTO", boleto.NumeroDocumento)
                .Replace("@ESPECIEDOCUMENTO", boleto.SiglaEspecieDocumento)
                .Replace("@DATAPROCESSAMENTO", boleto.DataProcessamento.ToString("dd/MM/yyyy"))
                .Replace("@NOSSONUMERO", boleto.NossoNumeroFormatado)
                .Replace("@CARTEIRA", boleto.Banco.Cedente.ContaBancaria.Carteira)
                .Replace("@ESPECIE", boleto.EspecieMoeda)
                .Replace("@QUANTIDADE", (boleto.QuantidadeMoeda == 0 ? "" : boleto.QuantidadeMoeda.ToString()))
                .Replace("@VALORDOCUMENTO", boleto.ValorMoeda)
                .Replace("@=VALORDOCUMENTO", (boleto.ValorTitulo == 0 ? "" : boleto.ValorTitulo.ToString("R$ ##,##0.00")))
                .Replace("@VALORCOBRADO", (boleto.ValorPago == 0 ? "" : boleto.ValorPago.ToString("R$ ##,##0.00")))
                .Replace("@OUTROSACRESCIMOS", "")
                .Replace("@OUTRASDEDUCOES", "")
                .Replace("@DESCONTOS", (boleto.ValorDesconto == 0 ? "" : boleto.ValorDesconto.ToString("R$ ##,##0.00")))
                .Replace("@AGENCIACONTA", boleto.Banco.Cedente.CodigoFormatado)
                .Replace("@SACADO", sacado)
                .Replace("@INFOSACADO", infoSacado)
                .Replace("@AVALISTA", avalista)
                .Replace("@AGENCIACODIGOCEDENTE", boleto.Banco.Cedente.CodigoFormatado)
                .Replace("@CPFCNPJ", boleto.Banco.Cedente.CPFCNPJ)
                .Replace("@MORAMULTA", (boleto.ValorMulta == 0 ? "" : boleto.ValorMulta.ToString("R$ ##,##0.00")))
                .Replace("@AUTENTICACAOMECANICA", "")
                .Replace("@USODOBANCO", boleto.UsoBanco)
                .Replace("@IMAGEMCODIGOBARRA", imagemCodigoBarras)
                .Replace("@ACEITE", boleto.Aceite).ToString()
                .Replace("@ENDERECOCEDENTE", MostrarEnderecoCedente ? enderecoCedente : "")
                .Replace("@INSTRUCOES", boleto.MensagemInstrucoesCaixa);
        }

        #endregion Html

        #region Geração do Html OffLine

        /// <summary>
        /// Função utilizada para gerar o html do boleto sem que o mesmo esteja dentro de uma página Web.
        /// </summary>
        /// <param name="srcLogo">Local apontado pela imagem de logo.</param>
        /// <param name="srcBarra">Local apontado pela imagem de barra.</param>
        /// <param name="srcCodigoBarra">Local apontado pela imagem do código de barras.</param>
        /// <returns>StringBuilder conténdo o código html do boleto bancário.</returns>
        protected StringBuilder HtmlOffLine(string textoNoComecoDoEmail, string srcLogo, string srcBarra, string srcCodigoBarra, bool usaCSSPDF = false)
        {//protected StringBuilder HtmlOffLine(string srcCorte, string srcLogo, string srcBarra, string srcPonto, string srcBarraInterna, string srcCodigoBarra)
            this.OnLoad(EventArgs.Empty);

            StringBuilder html = new StringBuilder();
            HtmlOfflineHeader(html, usaCSSPDF);
            if (textoNoComecoDoEmail != null && textoNoComecoDoEmail != "")
            {
                html.Append(textoNoComecoDoEmail);
            }
            html.Append(MontaHtml(srcLogo, srcBarra, "<img src=\"" + srcCodigoBarra + "\" alt=\"Código de Barras\" />"));
            HtmlOfflineFooter(html);
            return html;
        }




        /// <summary>
        /// Monta o Header de um email com pelo menos um boleto dentro.
        /// </summary>
        /// <param name="saida">StringBuilder onde o conteudo sera salvo.</param>
        protected static void HtmlOfflineHeader(StringBuilder html, bool usaCSSPDF = false)
        {
            html.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n");
            html.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">\n");
            html.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
            html.Append("<meta charset=\"utf-8\"/>\n");
            html.Append("<head>");
            html.Append("    <title>Boleto.Net</title>\n");

            #region Css
            {
                string arquivoCSS = usaCSSPDF ? "Boleto2Net.BoletoImpressao.BoletoNetPDF.css" : "Boleto2Net.BoletoImpressao.BoletoNet.css";
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(arquivoCSS);

                using (StreamReader sr = new StreamReader(stream))
                {
                    html.Append("<style>\n");
                    html.Append(sr.ReadToEnd());
                    html.Append("</style>\n");
                    sr.Close();
                    sr.Dispose();
                }
            }
            #endregion Css

            html.Append("     </head>\n");
            html.Append("<body>\n");
        }


        /// <summary>
        /// Monta o Footer de um email com pelo menos um boleto dentro.
        /// </summary>
        /// <param name="saida">StringBuilder onde o conteudo sera salvo.</param>
        protected static void HtmlOfflineFooter(StringBuilder saida)
        {
            saida.Append("</body>\n");
            saida.Append("</html>\n");
        }


        /// <summary>
        /// Junta varios boletos em uma unica AlternateView, para todos serem mandados juntos no mesmo email
        /// </summary>
        /// <param name="arrayDeBoletos">Array contendo os boletos a serem mesclados</param>
        /// <returns></returns>
        public static AlternateView GeraHtmlDeVariosBoletosParaEmail(BoletoBancario[] arrayDeBoletos)
        {
            return GeraHtmlDeVariosBoletosParaEmail(null, arrayDeBoletos);
        }

        /// <summary>
        /// Junta varios boletos em uma unica AlternateView, para todos serem mandados juntos no mesmo email
        /// </summary>
        /// <param name="textoNoComecoDoEmail">Texto em HTML a ser adicionado no comeco do email</param>
        /// <param name="arrayDeBoletos">Array contendo os boletos a serem mesclados</param>
        /// <returns>AlternateView com os dados de todos os boleto.</returns>
        public static AlternateView GeraHtmlDeVariosBoletosParaEmail(string textoNoComecoDoEmail, BoletoBancario[] arrayDeBoletos)
        {
            var corpoDoEmail = new StringBuilder();

            var linkedResources = new List<LinkedResource>();
            HtmlOfflineHeader(corpoDoEmail);
            if (textoNoComecoDoEmail != null && textoNoComecoDoEmail != "")
            {
                corpoDoEmail.Append(textoNoComecoDoEmail);
            }
            foreach (var umBoleto in arrayDeBoletos)
            {
                if (umBoleto != null)
                {
                    LinkedResource lrImagemLogo;
                    LinkedResource lrImagemBarra;
                    LinkedResource lrImagemCodigoBarra;
                    umBoleto.GeraGraficosParaEmailOffLine(out lrImagemLogo, out lrImagemBarra, out lrImagemCodigoBarra);
                    var theOutput = umBoleto.MontaHtml(
                        "cid:" + lrImagemLogo.ContentId,
                        "cid:" + lrImagemBarra.ContentId,
                        "<img src=\"cid:" + lrImagemCodigoBarra.ContentId + "\" alt=\"Código de Barras\" />");

                    corpoDoEmail.Append(theOutput);

                    linkedResources.Add(lrImagemLogo);
                    linkedResources.Add(lrImagemBarra);
                    linkedResources.Add(lrImagemCodigoBarra);
                }
            }
            HtmlOfflineFooter(corpoDoEmail);



            AlternateView av = AlternateView.CreateAlternateViewFromString(corpoDoEmail.ToString(), Encoding.Default, "text/html");
            foreach (var theResource in linkedResources)
            {
                av.LinkedResources.Add(theResource);
            }



            return av;
        }


        /// <summary>
        /// Função utilizada gerar o AlternateView necessário para enviar um boleto bancário por e-mail.
        /// </summary>
        /// <returns>AlternateView com os dados do boleto.</returns>
        public AlternateView HtmlBoletoParaEnvioEmail()
        {
            return HtmlBoletoParaEnvioEmail(null);
        }


        /// <summary>
        /// Função utilizada gerar o AlternateView necessário para enviar um boleto bancário por e-mail.
        /// </summary>
        /// <param name="textoNoComecoDoEmail">Texto (em HTML) a ser incluido no começo do Email.</param>
        /// <returns>AlternateView com os dados do boleto.</returns>
        public AlternateView HtmlBoletoParaEnvioEmail(string textoNoComecoDoEmail)
        {
            LinkedResource lrImagemLogo;
            LinkedResource lrImagemBarra;
            LinkedResource lrImagemCodigoBarra;

            GeraGraficosParaEmailOffLine(out lrImagemLogo, out lrImagemBarra, out lrImagemCodigoBarra);
            StringBuilder html = HtmlOffLine(textoNoComecoDoEmail, "cid:" + lrImagemLogo.ContentId, "cid:" + lrImagemBarra.ContentId, "cid:" + lrImagemCodigoBarra.ContentId);

            AlternateView av = AlternateView.CreateAlternateViewFromString(html.ToString(), Encoding.Default, "text/html");

            av.LinkedResources.Add(lrImagemLogo);
            av.LinkedResources.Add(lrImagemBarra);
            av.LinkedResources.Add(lrImagemCodigoBarra);
            return av;
        }

        /// <summary>
        /// Gera as tres imagens necessárias para o Boleto
        /// </summary>
        /// <param name="lrImagemLogo">O Logo do Banco</param>
        /// <param name="lrImagemBarra">A Barra Horizontal</param>
        /// <param name="lrImagemCodigoBarra">O Código de Barras</param>
        void GeraGraficosParaEmailOffLine(out LinkedResource lrImagemLogo, out LinkedResource lrImagemBarra, out LinkedResource lrImagemCodigoBarra)
        {
            this.OnLoad(EventArgs.Empty);

            var randomSufix = new Random().Next().ToString(); // para podermos colocar no mesmo email varios boletos diferentes

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Boleto2Net.Imagens." + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg");
            lrImagemLogo = new LinkedResource(stream, MediaTypeNames.Image.Jpeg)
            {
                ContentId = "logo" + randomSufix
            };

            MemoryStream ms = new MemoryStream(Utils.ConvertImageToByte(Html.barra));
            lrImagemBarra = new LinkedResource(ms, MediaTypeNames.Image.Gif)
            {
                ContentId = "barra" + randomSufix
            };

            BarCode2of5i cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);
            ms = new MemoryStream(Utils.ConvertImageToByte(cb.ToBitmap()));

            lrImagemCodigoBarra = new LinkedResource(ms, MediaTypeNames.Image.Gif)
            {
                ContentId = "codigobarra" + randomSufix
            };

        }


        /// <summary>
        /// Função utilizada para gravar em um arquivo local o conteúdo do boleto. Este arquivo pode ser aberto em um browser sem que o site esteja no ar.
        /// </summary>
        /// <param name="fileName">Path do arquivo que deve conter o código html.</param>
        public void MontaHtmlNoArquivoLocal(string fileName)
        {
            using (FileStream f = new FileStream(fileName, FileMode.Create))
            {
                StreamWriter w = new StreamWriter(f, System.Text.Encoding.Default);
                w.Write(MontaHtml());
                w.Close();
                f.Close();
            }
        }

        /// <summary>
        /// Monta o Html do boleto bancário
        /// </summary>
        /// <returns>string</returns>
        public string MontaHtml()
        {
            return MontaHtml(null, null);
        }


        /// <summary>
        /// Monta o Html do boleto bancário
        /// </summary>
        /// <param name="fileName">Caminho do arquivo</param>
        /// <param name="fileName">Caminho do logo do cedente</param>
        /// <returns>Html do boleto gerado</returns>
        public string MontaHtml(string fileName, string logoCedente)
        {
            if (fileName == null)
                fileName = System.IO.Path.GetTempPath();

            if (logoCedente != null)
                vLocalLogoCedente = logoCedente;

            this.OnLoad(EventArgs.Empty);

            string fnLogo = fileName + @"BoletoNet" + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg";

            if (!System.IO.File.Exists(fnLogo))
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Boleto2Net.Imagens." + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg");
                using (Stream file = File.Create(fnLogo))
                {
                    CopiarStream(stream, file);
                }
            }

            string fnBarra = fileName + @"BoletoNetBarra.gif";
            if (!File.Exists(fnBarra))
            {
                ImageConverter imgConverter = new ImageConverter();
                byte[] imgBuffer = (byte[])imgConverter.ConvertTo(Html.barra, typeof(byte[]));
                MemoryStream ms = new MemoryStream(imgBuffer);

                using (Stream stream = File.Create(fnBarra))
                {
                    CopiarStream(ms, stream);
                    ms.Flush();
                    ms.Dispose();
                }
            }

            string fnCodigoBarras = System.IO.Path.GetTempFileName();
            BarCode2of5i cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);
            cb.ToBitmap().Save(fnCodigoBarras);

            //return HtmlOffLine(fnCorte, fnLogo, fnBarra, fnPonto, fnBarraInterna, fnCodigoBarras).ToString();
            return HtmlOffLine(null, fnLogo, fnBarra, fnCodigoBarras).ToString();
        }

        /// <summary>
        /// Monta o Html do boleto bancário para View ASP.Net MVC
        /// <code>
        /// <para>Exemplo:</para>
        /// <para>public ActionResult VisualizarBoleto(string Id)</para>
        /// <para>{</para>
        /// <para>    BoletoBancario bb = new BoletoBancario();</para>
        /// <para>    //...</para>
        /// <para>    ViewBag.Boleto = bb.MontaHtml("/Content/Boletos/", "teste1");</para>
        /// <para>    return View();</para>
        /// <para>}</para>
        /// <para>//Na view</para>
        /// <para>@{Layout = null;}@Html.Raw(ViewBag.Boleto)</para>
        /// </code>
        /// </summary>
        /// <param name="Url">Pasta dos boletos. Exemplo MontaHtml("/Content/Boletos/", "000100")</param>
        /// <param name="fileName">Nome do arquivo para o boleto</param>
        /// <returns>Html do boleto gerado</returns>
        /// <desenvolvedor>Sandro Ribeiro</desenvolvedor>
        /// <criacao>16/11/2012</criacao>
        public string MontaHtml(string url, string fileName, bool useMapPathSecure = true)
        {
            //Variável para o caminho físico do servidor
            string pathServer = "";

            //Verifica se o usuário informou uma url válida
            if (url == null)
            {
                //Obriga o usuário a especificar uma url válida
                throw new ArgumentException("Você precisa informar uma pasta padrão.");
            }
            else
            {
                if (useMapPathSecure)
                {
                    //Verifica se o usuário usou barras no início e no final da url
                    if (url.Substring(url.Length - 1, 1) != "/")
                        url = url + "/";
                    if (url.Substring(0, 1) != "/")
                        url = url + "/";
                    //Mapeia o caminho físico dos arquivos
                    pathServer = MapPathSecure(string.Format("~{0}", url));
                }

                //Verifica se o caminho existe
                if (!Directory.Exists(pathServer))
                    throw new ArgumentException("A o caminho físico '{0}' não existe.", pathServer);
            }
            //Verifica o nome do arquivo
            if (fileName == null)
            {
                fileName = DateTime.Now.Ticks.ToString();
            }
            else
            {
                if (fileName == "")
                    fileName = DateTime.Now.Ticks.ToString();
            }

            //Mantive o padrão 
            this.OnLoad(EventArgs.Empty);

            //Prepara o arquivo da logo para ser salvo
            string fnLogo = pathServer + @"BoletoNet" + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg";
            //Prepara o arquivo da logo para ser usado no html
            string fnLogoUrl = url + @"BoletoNet" + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg";

            //Salvo a imagem apenas 1 vez com o código do banco
            if (!System.IO.File.Exists(fnLogo))
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Boleto2Net.Imagens." + Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3) + ".jpg");
                using (Stream file = File.Create(fnLogo))
                {
                    CopiarStream(stream, file);
                }
            }

            //Prepara o arquivo da barra para ser salvo
            string fnBarra = pathServer + @"BoletoNetBarra.gif";
            //Prepara o arquivo da barra para ser usado no html
            string fnBarraUrl = url + @"BoletoNetBarra.gif";

            //Salvo a imagem apenas 1 vez
            if (!File.Exists(fnBarra))
            {
                ImageConverter imgConverter = new ImageConverter();
                byte[] imgBuffer = (byte[])imgConverter.ConvertTo(Html.barra, typeof(byte[]));
                MemoryStream ms = new MemoryStream(imgBuffer);

                using (Stream stream = File.Create(fnBarra))
                {
                    CopiarStream(ms, stream);
                    ms.Flush();
                    ms.Dispose();
                }
            }

            //Prepara o arquivo do código de barras para ser salvo
            string fnCodigoBarras = string.Format("{0}{1}_codigoBarras.jpg", pathServer, fileName);
            //Prepara o arquivo do código de barras para ser usado no html
            string fnCodigoBarrasUrl = string.Format("{0}{1}_codigoBarras.jpg", url, fileName);

            BarCode2of5i cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);

            //Salva o arquivo conforme o fileName
            cb.ToBitmap().Save(fnCodigoBarras);

            //Retorna o Html para ser usado na view
            return HtmlOffLine(null, fnLogoUrl, fnBarraUrl, fnCodigoBarrasUrl).ToString();
        }

        /// <summary>
        /// Monta o Html do boleto bancário com as imagens embutidas no conteúdo, sem necessidade de links externos
        /// de acordo com o padrão http://en.wikipedia.org/wiki/Data_URI_scheme
        /// </summary>
        /// <param name="convertLinhaDigitavelToImage">Converte a Linha Digitável para imagem, com o objetivo de evitar malwares.</param>
        /// <returns>Html do boleto gerado</returns>
        /// <desenvolvedor>Iuri André Stona</desenvolvedor>
        /// <criacao>23/01/2014</criacao>
        /// <alteracao>08/08/2014</alteracao>

        public string MontaHtmlEmbedded(bool convertLinhaDigitavelToImage = false, bool usaCSSPDF = false)
        {
            OnLoad(EventArgs.Empty);

            var assembly = Assembly.GetExecutingAssembly();

            var streamLogo = assembly.GetManifestResourceStream("Boleto2Net.Imagens." + boleto.Banco.Codigo.ToString("000") + ".jpg");
            string base64Logo = Convert.ToBase64String(new BinaryReader(streamLogo).ReadBytes((int)streamLogo.Length));
            string fnLogo = string.Format("data:image/gif;base64,{0}", base64Logo);

            var streamBarra = assembly.GetManifestResourceStream("Boleto2Net.Imagens.barra.gif");
            string base64Barra = Convert.ToBase64String(new BinaryReader(streamBarra).ReadBytes((int)streamBarra.Length));
            string fnBarra = string.Format("data:image/gif;base64,{0}", base64Barra);

            var cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);
            string base64CodigoBarras = Convert.ToBase64String(cb.ToByte());
            string fnCodigoBarras = string.Format("data:image/gif;base64,{0}", base64CodigoBarras);

            if (convertLinhaDigitavelToImage)
            {

                string linhaDigitavel = boleto.CodigoBarra.LinhaDigitavel.Replace("  ", " ").Trim();

                var imagemLinha = Utils.DrawText(linhaDigitavel, new Font("Arial", 30, FontStyle.Bold), Color.Black, Color.White);
                string base64Linha = Convert.ToBase64String(Utils.ConvertImageToByte(imagemLinha));

                string fnLinha = string.Format("data:image/gif;base64,{0}", base64Linha);

                boleto.CodigoBarra.LinhaDigitavel = @"<img style=""max-width:420px; margin-bottom: 2px"" src=" + fnLinha + " />";
            }

            string s = HtmlOffLine(null, fnLogo, fnBarra, fnCodigoBarras, usaCSSPDF).ToString();

            if (convertLinhaDigitavelToImage)
            {
                s = s.Replace(".w500", "");
            }

            return s;
        }

        public byte[] MontaBytesPDF(bool convertLinhaDigitavelToImage = false)
        {

            return (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(this.MontaHtmlEmbedded(convertLinhaDigitavelToImage, true));
        }
        #endregion Geração do Html OffLine

        public System.Drawing.Image GeraImagemCodigoBarras(Boleto boleto)
        {
            BarCode2of5i cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);
            return cb.ToBitmap();
        }

        private void CopiarStream(Stream entrada, Stream saida)
        {
            int bytesLidos = 0;
            byte[] imgBuffer = new byte[entrada.Length];

            while ((bytesLidos = entrada.Read(imgBuffer, 0, imgBuffer.Length)) > 0)
            {
                saida.Write(imgBuffer, 0, bytesLidos);
            }
        }
    }
}
