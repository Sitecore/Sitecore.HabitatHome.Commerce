using Sitecore.Commerce.Engine.Connect;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Sitecore.Feature.GiftCardBalance.Managers
{
    public class GiftCardBalanceManager
    {
        public double GetGiftCardBalance(string cid)
        {
            double amount = 0;
            if (!string.IsNullOrEmpty(cid))
            {
                var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
                var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);

                var client = this.GetClient(ceConfig);

                //var result = client.GetAsync("$GiftCard('" + cid + "')").Result;

                //if (result.IsSuccessStatusCode)
                //{
                //    var resultContent = result.Content.ReadAsStringAsync().Result;
                //    JObject resultList = JObject.Parse(resultContent);
                //    amount = int.Parse(resultList["Balance"].ToString());
                //}

                amount = Math.Round(100.00, 2);
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