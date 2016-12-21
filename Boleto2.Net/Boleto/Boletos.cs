using System;
using System.Collections.Generic;

namespace Boleto2Net
{
    public class Boletos : List<Boleto>
    {
        public Banco Banco { get; set; }
    }
}
