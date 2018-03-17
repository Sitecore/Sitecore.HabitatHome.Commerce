using Sitecore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Sitecore.Feature.NearestStore.Models;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;
using System.Threading.Tasks;

namespace Sitecore.Feature.NearestStore.Managers
{
    public class NearestStoreManager
    {
        public IEnumerable<InventoryStore> GetNearestStores(UserLocation userLocation)
        {
            List<InventoryStore> storeList = new List<InventoryStore>();

            var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
            var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);

            StringContent content = new StringContent(userLocation.Latitude + "|" + userLocation.Longitude);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");
            var client = this.GetClient(ceConfig);

            dynamic result = client.PutAsync("NearestStoreLocator", content).Result;

            if (result.IsSuccessStatusCode)
            {
                dynamic resultList = JObject.Parse(result);

             
            }
            else
            {
                return null;
            }

            return storeList;

        }

        public HttpClient GetClient(CommerceEngineConfiguration config)
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