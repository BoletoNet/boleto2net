using System.Collections.Generic;

namespace Boleto2Net
{
    public class Boletos : List<Boleto>
    {
        public Banco Banco { get; set; }

        private new void Add(Boleto item)
        {
            item.Valida();
            this.Add(item);
        }

    }
}
