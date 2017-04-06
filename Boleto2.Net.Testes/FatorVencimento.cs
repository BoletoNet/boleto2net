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
            var inicio = new DateTime(2010, 01, 31, 0, 0, 0);
            var fim = new DateTime(2025, 02, 21, 0, 0, 0);
            var totalDays = (fim - inicio).TotalDays;
            for (int i = 0; i < totalDays; i++)
            {
                var dateTime = inicio.AddDays(i);
                Assert.AreEqual(4499 + i, dateTime.FatorVencimento());
            }
            Assert.AreEqual(1000, fim.AddDays(1).FatorVencimento());
            Assert.AreEqual(1001, fim.AddDays(2).FatorVencimento());
        }
    }
}
