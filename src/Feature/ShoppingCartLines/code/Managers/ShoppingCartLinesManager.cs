using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Sitecore.Feature.ShoppingCartLines.Managers
{
    public class ShoppingCartLinesManager
    {
        public dynamic GetCurrentCartLines(string cartId)
        {
            dynamic cartResult = new System.Dynamic.ExpandoObject();
            List<dynamic> cartLineList = new List<dynamic>();
            
            if (!string.IsNullOrEmpty(cartId))
            {
                var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
                var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);

                var client = this.GetClient(ceConfig);

                var result = client.GetAsync("Carts('" + cartId + "')?$expand=Lines($expand=CartLineComponents)").Result;

                if (result.IsSuccessStatusCode)
                {
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                    cartResult = JObject.Parse(resultContent);

                    JArray lines = (JArray)cartResult["Lines"];

                    foreach (var lineItem in lines)
                    {
                        dynamic line = new System.Dynamic.ExpandoObject();
                        line.ExternalCartLineId = lineItem["Id"].ToString();
                        string itemId = lineItem["ItemId"].ToString();
                        line.ProductId = itemId.Split('|')[1];
                        line.VariantId = itemId.Split('|')[2];
                        line.Comments = lineItem["Comments"];
                        cartLineList.Add(line);
                    }
                }
            }
            return cartLineList;
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