using System.Linq;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.Foundation.Commerce.OrderOutcomes.Managers;

namespace Sitecore.Foundation.Rules.Conditions
{
    public class PromotionByOrderHistoryCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string PurchasedProductId { get; set; }
        public string TargetProductId { get; set; }
        public int Days { get; set; }
        protected override bool Execute(T ruleContext)
        {
            if (Sitecore.Context.IsLoggedIn)
                return ValidateCase(PurchasedProductId, TargetProductId, Days);
            else
                return false;
        }

        protected bool ValidateCase(string productId, string targetProductId, int pastDaysAmount)
        {
            OrderOutcomesManager outcomesManager = new OrderOutcomesManager();
            var orderOutcomes = outcomesManager.GetSubmittedOrderOutcomes(pastDaysAmount);
            if (orderOutcomes.Count() > 0)
            {
                if (orderOutcomes.Where(o => o.Order.CartLines.Where(c => c.Product.ProductId.Split('|')[1] == productId).FirstOrDefault() != null).FirstOrDefault() == null)
                    return false;
                if (orderOutcomes.Where(o => o.Order.CartLines.Where(c => c.Product.ProductId.Split('|')[1] == targetProductId).FirstOrDefault() != null).FirstOrDefault() == null)
                    return true;
                return false;
            }
            else
                return false;
        }
    }
}