using GamePush;

namespace Utils.GamePush
{
    public class FetchPurchaseData
    {
        public int ProductId;
        public string Tag;
        public string Payload;
        public string CreatedAt;
        public string ExpiredAt;
        
        public FetchPurchaseData(FetchPlayerPurchases fetchPlayerPurchases)
        {
            ProductId = fetchPlayerPurchases.productId;
            Tag = fetchPlayerPurchases.tag;
            Payload = fetchPlayerPurchases.payload;
            CreatedAt = fetchPlayerPurchases.createdAt;
            ExpiredAt = fetchPlayerPurchases.expiredAt;
        }
    }
}