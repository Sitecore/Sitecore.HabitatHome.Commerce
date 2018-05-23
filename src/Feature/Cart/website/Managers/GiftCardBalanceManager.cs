using System;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Configuration;

namespace Sitecore.HabitatHome.Feature.Cart.Managers
{
    public class GiftCardBalanceManager
    {
        public string GetGiftCardBalance(string cid)
        {
            string returnValue = string.Empty;
            if (!string.IsNullOrEmpty(cid))
            {
                var ceConfig = (CommerceEngineConfiguration)Factory.CreateObject("commerceEngineConfiguration", true);

                double amount = Math.Round(100.00, 2);

                returnValue = $"{amount} {ceConfig.DefaultShopCurrency}";
            }
            return returnValue;
        }        
    }
}