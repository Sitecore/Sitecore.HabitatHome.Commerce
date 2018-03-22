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
using System.Web;
using System.Web.Script.Serialization;
using System;


namespace Sitecore.Feature.NearestStore.Managers
{
    public class NearestStoreManager
    {
        public IEnumerable<InventoryStore> GetNearestStores(UserLocation userLocation, string pid)
        {
            List<InventoryStore> storeList = new List<InventoryStore>();

            var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
            var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);

            var client = this.GetClient(ceConfig);

            var result = client.GetAsync("NearestStoreLocator('" + userLocation.Latitude +"|" + userLocation.Longitude +"')").Result;

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
                string storeJson = new JavaScriptSerializer().Serialize(storeList.GetRange(0,2));
                HttpCookie storesCookie = new HttpCookie("sxa_site_shops_stores", storeJson)
                {
                    Expires = DateTime.Now.AddDays(30)
                };
                HttpContext.Current.Response.Cookies.Add(storesCookie);               
                
            }
            else
            {
                return null;
            }

            return storeList;

        }

        public IEnumerable<InventoryStore> GetSavedStoresInventory(string pid)
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

            return storeList;

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