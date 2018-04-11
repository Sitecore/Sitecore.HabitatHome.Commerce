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
        public string ProductId { get; set; }
        protected override bool Execute(T ruleContext)
        {
            //if Promo Cookie exists
                //Get Promo Cookie Values
          
            //if Nearest stores cookie exists
                    //Get store cookie values
            if(GetUserLocations(ProductId).Contains(InventoryStoreId))
            {
                return true;
            }
            return false;
        }

        private List<string> GetUserLocations(string productId)
        {
            List<string> userLocations = new List<string>();

            userLocations.Add("JeffersonCityStore");
            //GET NEAREST STORES FROM PROXY

            return userLocations;

        }
    }
}