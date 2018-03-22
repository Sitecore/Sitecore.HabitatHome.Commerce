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
        public string GetGiftCardBalance(string cid)
        {
            double amount = 0;
            string returnValue = string.Empty;
            if (!string.IsNullOrEmpty(cid))
            {
                var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);

                amount = Math.Round(100.00, 2);

                returnValue = $"{amount} {ceConfig.DefaultShopCurrency}";
            }
            return returnValue;
        }        
    }
}