using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Configuration;
using Sitecore.HabitatHome.Foundation.Promotions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Sitecore.HabitatHome.Foundation.Promotions.Managers
{
    public class PromotionsManager : IPromotionsManager
    {
        public IEnumerable<Promotion> GetActivePromotions(string productId)
        {
            List<Promotion> allPromos = GetAllPromotions().ToList();
            List<Promotion> activePromos = new List<Promotion>();

            foreach (var promo in allPromos.Where(p => DateTime.Now > p.ValidFrom && DateTime.Now < p.ValidTo))
            {
                if (promo.Qualifications != null)
                {
                    foreach (var qual in promo.Qualifications)
                    {
                        foreach (var prop in qual["Properties"])
                        {
                            var propName = prop["Name"];
                            if (propName == "TargetItemId")
                            {
                                string propValue = prop["Value"];
                                if (propValue.Contains(productId) && !activePromos.Contains(promo))
                                    activePromos.Add(promo);
                            }
                        }
                    }
                }
                if (promo.Benefits != null)
                {
                    foreach (var benefit in promo.Benefits)
                    {
                        foreach (var prop in benefit["Properties"])
                        {
                            var propName = prop["Name"];
                            if (propName == "TargetItemId")
                            {
                                string propValue = prop["Value"];
                                if (propValue.Contains(productId) && !activePromos.Contains(promo))
                                {
                                    activePromos.Add(promo);
                                }
                            }
                        }
                    }
                }
            }
            return activePromos;
        }

        public IEnumerable<Promotion> GetAllPromotions()
        {
            List<Promotion> allPromotions = new List<Promotion>();
            var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);
            var uri = new System.Uri(EngineConnectUtility.EngineConfiguration.ShopsServiceUrl);
            var client = this.GetClient(ceConfig);
            var result = client.GetAsync("Promotions").Result;
            if (result.IsSuccessStatusCode)
            {
                var resultContent = result.Content.ReadAsStringAsync().Result;
                JObject resultList = JObject.Parse(resultContent);

                foreach (var promotion in resultList["value"])
                {
                    Promotion returnedPromotion = new Promotion();
                    returnedPromotion.Id = promotion["Id"].ToString();
                    returnedPromotion.DisplayText = promotion["DisplayText"].ToString();
                    returnedPromotion.DisplayCartText = promotion["DisplayCartText"].ToString();
                    DateTime validFrom = new DateTime();
                    DateTime validTo = new DateTime();

                    if (DateTime.TryParse(promotion["ValidFrom"].ToString(), out validFrom))
                    {
                        returnedPromotion.ValidFrom = validFrom;
                    }
                    if (DateTime.TryParse(promotion["ValidTo"].ToString(), out validTo))
                    {
                        returnedPromotion.ValidTo = validTo;
                    }

                    var promoPolicyList = promotion["Policies"].ToArray().ToList();
                    promoPolicyList.ForEach((Action<JToken>)(policy =>
                    {
                        JObject policyObject = (JObject)policy;
                        if (policyObject.Property("Qualifications") != null)
                        {
                            var qualifications = policy["Qualifications"].ToArray();
                            returnedPromotion.Qualifications.AddRange(qualifications);
                        }
                        if (policyObject.Property("Benefits") != null)
                        {
                            var benefits = policy["Benefits"].ToArray();
                            returnedPromotion.Benefits.AddRange(benefits);
                        }
                    }));

                    allPromotions.Add(returnedPromotion);
                }
            }
            return allPromotions;
        }

        private HttpClient GetClient(CommerceEngineConfiguration config)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(config.ShopsServiceUrl)
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

            httpClient.Timeout = new System.TimeSpan(0, 0, 600);
            return httpClient;
        }
    }
}