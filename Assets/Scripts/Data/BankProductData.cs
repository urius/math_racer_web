namespace Data
{
    public class BankProductData
    {
        public readonly int Id;
        public readonly string Tag;
        public readonly string Name;
        public readonly int Price;
        public readonly string Currency;
        
        public readonly int ProductAmount;
        public readonly string ProductType;

        public BankProductData(
            int id,
            string tag,
            string name,
            int price,
            string currency,
            int productAmount,
            string productType)
        {
            Id = id;
            Tag = tag;
            Name = name;
            Price = price;
            Currency = currency;
            ProductAmount = productAmount;
            ProductType = productType;
        }
    }
}