using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Infra.CommandExecutor;
using Infra.Instance;
using Providers;
using UnityEngine;
using Utils.GamePush;

namespace Controller.Commands
{
    public class InitBankProductsCommand : IAsyncCommand
    {
        public async UniTask ExecuteAsync()
        {
            var sessionModel = Instance.Get<IModelsHolder>().GetSessionDataModel();
            
            var fetchedProducts = await GamePushWrapper.FetchProducts();

            Debug.Log("fetched products:");
            foreach (var fetchProductData in fetchedProducts)
            {
                Debug.Log(JsonUtility.ToJson(fetchProductData, true));
            }

            var goldProducts = GetSortedProducts(fetchedProducts, Constants.ProductTypeGold);
            var cashProducts = GetSortedProducts(fetchedProducts, Constants.ProductTypeCash);

            var bankProductsData = new BankProductsData(goldProducts, cashProducts);

            sessionModel.BankProductsData.Value = bankProductsData;
        }

        private static BankProductData[] GetSortedProducts(IEnumerable<FetchProductData> fetchedProducts, string productType)
        {
            return fetchedProducts
                .Where(p => p.Tag.IndexOf(productType, StringComparison.Ordinal) >= 0)
                .Select(ToBankProductData)
                .OrderBy(p => p.ProductAmount)
                .ToArray();
        }

        private static BankProductData ToBankProductData(FetchProductData data)
        {
            var splittedTag = data.Tag.Split("_");
            var productType = splittedTag[0];
            var productAmount = int.Parse(splittedTag[1]);
            
            return new BankProductData(
                data.Id,
                data.Tag,
                data.Name,
                data.Price,
                data.Currency,
                productAmount,
                productType);
        }
    }
}