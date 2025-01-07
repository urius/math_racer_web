using System.Collections.Generic;

namespace Data
{
    public class BankProductsData
    {
        public readonly IReadOnlyList<BankProductData> GoldProducts;
        public readonly IReadOnlyList<BankProductData> CashProducts;

        public BankProductsData(IReadOnlyList<BankProductData> goldProducts, IReadOnlyList<BankProductData> cashProducts)
        {
            GoldProducts = goldProducts;
            CashProducts = cashProducts;
        }
    }
}