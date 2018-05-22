using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Configuration;
using Sitecore.HabitatHome.Foundation.StoreLocator.Managers;
using Sitecore.HabitatHome.Foundation.StoreLocator.Models;

namespace Sitecore.HabitatHome.Feature.NearestStore.Managers
{
    public class NearestStoreManager
    {
        protected StoreLocatorManager LocatorManager;
        public NearestStoreManager()
        {
            this.LocatorManager = new StoreLocatorManager();
        }
        public IEnumerable<InventoryStore> GetNearestStores()
        {
            return this.LocatorManager.GetNearestStores();
        }

        public IEnumerable<InventoryStore> GetSavedStores()
        {
            return this.LocatorManager.GetSavedStores();
        }
        public int GetProductInventory(string inventoryStoreId, string pid)
        {
            int amount = 0;
            if(!string.IsNullOrEmpty(inventoryStoreId) && !string.IsNullOrEmpty(pid))
            {
                var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
                var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);
                var client = this.GetClient(ceConfig);
                var result = client.GetAsync("InventoryInformation('" + inventoryStoreId + "-" + pid + "')").Result;
                if (result.IsSuccessStatusCode)
                {
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                    JObject resultList = JObject.Parse(resultContent);
                    amount = int.Parse(resultList["Quantity"].ToString());
                }
            }
            return amount;
        }

        private HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(config.ShopsServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Add("ShopName", config.DefaultShopName);
            httpClient.DefaultRequestHeaders.Add("Language", "en-US");
            httpClient.DefaultRequestHeaders.Add("Currency", config.DefaultShopCurrency);
            httpClient.DefaultRequestHeaders.Add("Environment", config.DefaultEnvironment);

            string certificate = config.GetCertificate();
            if (certificate != null)
                httpClient.DefaultRequestHeaders.Add(config.CertificateHeaderName, certificate);
            httpClient.Timeout = new System.TimeSpan(0, 0, 600);
            return httpClient;

        }
       
    }
}