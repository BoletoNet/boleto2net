using System;
using Boleto2Net.Extensions;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    public class DateTimeExtensionsTest
    {
        [Test]
        public void GeracaoCorretaDeFatorVencimento()
        {
            var inicio = new DateTime(1997, 10, 07, 0, 0, 0);
            var ajusteRange = (DateTime.Now - inicio).Days - 3000;
            inicio = inicio.AddDays(ajusteRange);

            var totalDiasAnalisados = (new DateTime(2033, 08, 15, 0, 0, 0) - inicio).Days;
            
            for (int i = 0; i < totalDiasAnalisados; i++)
            {
                var fatorVencimento = ajusteRange + i;
                var dateTime = inicio.AddDays(i);

                Assert.AreEqual(fatorVencimento, dateTime.FatorVencimento());
                if (fatorVencimento == 9999)
                    ajusteRange = 999-i;
            }
            
        }
    }
}
