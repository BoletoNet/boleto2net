namespace Boleto2Net
{
    internal interface ICarteira
    {
        void FormataNossoNumero(Boleto boleto);
        string FormataCodigoBarraCampoLivre(Boleto boleto);
    }
    internal interface ICarteira<T> : ICarteira
        where T : IBanco
    {
    }
}
