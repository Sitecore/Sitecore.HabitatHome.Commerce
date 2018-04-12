using Sitecore.Foundation.Commerce.StoreLocator.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Rules.Conditions
{
    public class PromotionByLocationCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string InventoryStoreId { get; set; }
        
        protected override bool Execute(T ruleContext)
        {
            StoreLocatorManager locatorManager = new StoreLocatorManager();
            var nearestStores = locatorManager.GetNearestStores();
            if(nearestStores.Where(x => x.InventoryStoreId == InventoryStoreId).FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }
    }
}