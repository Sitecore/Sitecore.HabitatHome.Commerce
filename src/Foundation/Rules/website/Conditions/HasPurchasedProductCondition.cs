using System.Linq;
using Sitecore.HabitatHome.Foundation.Orders.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.HabitatHome.Foundation.Rules.Conditions
{
    public class HasPurchasedProductCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string PurchasedProductId { get; set; }       
        public int Days { get; set; }
        protected override bool Execute(T ruleContext)
        {
            if (Sitecore.Context.IsLoggedIn)
                return ValidateCase(PurchasedProductId, Days);
            else
                return false;
        }

        protected bool ValidateCase(string productId, int pastDaysAmount)
        {
            OrderOutcomesManager outcomesManager = new OrderOutcomesManager();
            var orderOutcomes = outcomesManager.GetSubmittedOrderOutcomes(pastDaysAmount);
            if (orderOutcomes.Count() > 0)
            {
                if (orderOutcomes.Where(o => o.Order.CartLines.Where(c => (c.Product.ProductId.Contains("|") ? c.Product.ProductId.Split('|')[1] : c.Product.ProductId) == productId).FirstOrDefault() != null).FirstOrDefault() != null)
                    return true;              
                
                return false;
            }
            else
                return false;
        }
    }
}