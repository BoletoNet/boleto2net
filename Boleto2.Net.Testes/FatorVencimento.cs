using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boleto2Net.Testes
{
    [TestClass]
    public class Boleto2Net_FatorVencimento
    {
        [TestMethod]
        public void Boleto2Net_FatorVencimentos()
        {
            Assert.AreEqual(1000, AbstractBanco.FatorVencimento(new Boleto2Net.Boleto { DataVencimento = new DateTime(2025, 02, 22, 0, 0, 0) }));
            Assert.AreEqual(1001, AbstractBanco.FatorVencimento(new Boleto2Net.Boleto { DataVencimento = new DateTime(2025, 02, 23, 0, 0, 0) }));
            Assert.AreEqual(5947, AbstractBanco.FatorVencimento(new Boleto2Net.Boleto { DataVencimento = new DateTime(2014, 01, 18, 0, 0, 0) } ));
            Assert.AreEqual(6000, AbstractBanco.FatorVencimento(new Boleto2Net.Boleto { DataVencimento = new DateTime(2014, 03, 12, 0, 0, 0) }));
            Assert.AreEqual(7046, AbstractBanco.FatorVencimento(new Boleto2Net.Boleto { DataVencimento = new DateTime(2017, 01, 21, 0, 0, 0) }));
            Assert.AreEqual(9999, AbstractBanco.FatorVencimento(new Boleto2Net.Boleto { DataVencimento = new DateTime(2025, 02, 21, 0, 0, 0) }));
        }
    }
}
