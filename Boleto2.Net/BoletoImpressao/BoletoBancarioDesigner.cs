using System;
using System.ComponentModel;

namespace Boleto2Net
{
#if (NET40_OR_GREATER)
    
    internal class BoletoBancarioDesigner : System.Web.UI.Design.ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

        // Make this control resizeable on the design surface
        public override bool AllowResize
        {
            get
            {
                return true;
            }
        }

        public override string GetDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Boleto.Net.BoletoBancario");
        }


        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            string pattern = "Boleto.Net while creating design time HTML:<br/>{0}";
            return String.Format(pattern, e.Message);
        }
    }

#endif
}
