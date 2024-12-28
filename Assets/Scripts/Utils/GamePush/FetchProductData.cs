using GamePush;

namespace Utils.GamePush
{
    public class FetchProductData
    {
        public int Id;
        public string Tag;
        public string Name;
        public string Description;
        public string Icon;
        public string IconSmall;
        public int Price;
        public string Currency;
        public string CurrencySymbol;
        public bool IsSubscription;
        public int Period;
        public int TrialPeriod;

        public FetchProductData(FetchProducts fetchProducts)
        {
            Id = fetchProducts.id;
            Tag = fetchProducts.tag;
            Name = fetchProducts.name;
            Description = fetchProducts.description;
            Icon = fetchProducts.icon;
            IconSmall = fetchProducts.iconSmall;
            Price = fetchProducts.price;
            Currency = fetchProducts.currency;
            CurrencySymbol = fetchProducts.currencySymbol;
            IsSubscription = fetchProducts.isSubscription;
            Period = fetchProducts.period;
            TrialPeriod = fetchProducts.trialPeriod;
        }
    }
}