using System.Web;
using System.Web.Mvc;

namespace Boleto2.Net.AppTeste
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
