using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace Sitecore.HabitatHome.Feature.Cart.Managers
{
    public class ShoppingCartLinesManager
    {
        public ShoppingCartLinesManager(IStorefrontContext storefrontContext, ISearchManager searchManager)
        {
            this.SearchManager = searchManager;
            this.StorefrontContext = storefrontContext;
        }

        public ISearchManager SearchManager { get; set; }

        public IStorefrontContext StorefrontContext { get; set; }

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
                        line.IsKit = false;
                        line.IsBundle = false;
                        Item lineItemProduct = this.SearchManager.GetProduct(line.ProductId, this.StorefrontContext.CurrentStorefront.Catalog);
                        if(lineItemProduct != null)
                        {
                            line.IsKit = lineItemProduct["Tags"] != null && !String.IsNullOrEmpty(lineItemProduct["Tags"]) && lineItemProduct["Tags"].Split('|').Any(t => t.ToLower() == "kit");
                            line.IsBundle = lineItemProduct["Tags"] != null && !String.IsNullOrEmpty(lineItemProduct["Tags"]) && lineItemProduct["Tags"].Split('|').Any(t => t.ToLower() == "bundle");
                        } 
                        cartLineList.Add(line);
                    }
                }
            }
            return cartLineList;
        }

        private HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(config.ShopsServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Add("ShopName", config.DefaultShopName);
            httpClient.DefaultRequestHeaders.Add("Language", "en-US");
            httpClient.DefaultRequestHeaders.Add("Currency", config.DefaultShopCurrency);
            httpClient.DefaultRequestHeaders.Add("Environment", config.DefaultEnvironment);

            string certificate = config.GetCertificate();
            if (certificate != null)
            {
                httpClient.DefaultRequestHeaders.Add(config.CertificateHeaderName, certificate);
            }

            httpClient.Timeout = new TimeSpan(0, 0, 600);
            return httpClient;                                            
        }
    }
}