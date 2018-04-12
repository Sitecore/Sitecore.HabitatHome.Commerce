using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Engine.Connect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Sitecore.Foundation.Commerce.StoreLocator.Models;
using Sitecore.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Sitecore.Foundation.Commerce.StoreLocator.Utilities;
using Sitecore.Diagnostics;

namespace Sitecore.Foundation.Commerce.StoreLocator.Managers
{
    public class StoreLocatorManager
    {
        public IEnumerable<InventoryStore> GetNearestStores()
        {
            List<InventoryStore> storeList = new List<InventoryStore>();
            if (!HttpContext.Current.Response.Cookies.AllKeys.Contains("sxa_site_shops_stores"))
            {
                storeList = SaveNearestStores().ToList();
            }
            else
            {
                storeList = GetSavedStores().ToList();
            }
            return storeList;
        }

        public IEnumerable<InventoryStore> SaveNearestStores()
        {            
            UserLocation userLocation = GeoUtility.GetUserLocation();
            Assert.IsNotNull((object)userLocation, nameof(userLocation));

            List<InventoryStore> storeList = new List<InventoryStore>();
            var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
            var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);
            var client = this.GetClient(ceConfig);
            var result = client.GetAsync("NearestStoreLocator('" + userLocation.Latitude + "|" + userLocation.Longitude + "')").Result;
            if (result.IsSuccessStatusCode)
            {
                var resultContent = result.Content.ReadAsStringAsync().Result;
                JObject resultList = JObject.Parse(resultContent);

                foreach (var store in resultList["value"])
                {
                    dynamic newStore = new System.Dynamic.ExpandoObject();
                    newStore.Id = store["Id"];
                    newStore.InventoryStoreId = store["InventoryStoreId"];
                    newStore.DisplayName = store["DisplayName"];
                    newStore.Distance = store["Distance"];
                    storeList.Add(new InventoryStore(newStore));
                }
                string storeJson = new JavaScriptSerializer().Serialize(storeList.GetRange(0, 2));
                HttpCookie storesCookie = new HttpCookie("sxa_site_shops_stores", storeJson)
                {
                    Expires = DateTime.Now.AddHours(1)
                };
                HttpContext.Current.Response.Cookies.Add(storesCookie);
            }
            else
            {
                return null;
            }
            return storeList.Take(2);
        }
        public IEnumerable<InventoryStore> GetSavedStores()
        {
            List<InventoryStore> storeList = new List<InventoryStore>();
            HttpCookie storesCookie = HttpContext.Current.Request.Cookies["sxa_site_shops_stores"];
            dynamic savedStores = JsonConvert.DeserializeObject(storesCookie.Value);
            foreach (var store in savedStores)
            {
                dynamic newStore = new System.Dynamic.ExpandoObject();
                newStore.Id = store.Id;
                newStore.InventoryStoreId = store.InventoryStoreId;
                newStore.DisplayName = store.DisplayName;
                newStore.Distance = store.Distance;

                storeList.Add(new InventoryStore(newStore));
            }
            return storeList.Take(2);
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