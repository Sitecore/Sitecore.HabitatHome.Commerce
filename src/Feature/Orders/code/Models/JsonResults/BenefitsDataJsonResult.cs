using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;


namespace Sitecore.HabitatHome.Feature.Orders.Models.JsonResults
{
    public class BenefitsDataJsonResult
    {
        public List<CartAdjustmentJsonResult> CartAdjustments { get; set; }
        public List<CartAdjustmentJsonResult> LineAdjustments { get; set; }
        public BenefitsDataJsonResult(Cart currentCart)
        {
            this.CartAdjustments = new List<CartAdjustmentJsonResult>();
            this.LineAdjustments = new List<CartAdjustmentJsonResult>();
            if(currentCart.Adjustments.Count() > 0)
                currentCart.Adjustments.ForEach(delegate (CartAdjustment adj){this.CartAdjustments.Add(new CartAdjustmentJsonResult{Amount = adj.Amount.ToCurrency(),Description = adj.Description });});
            if (currentCart.Lines.Any(l => l.Adjustments.Count() > 0))
            {
                foreach (var line in currentCart.Lines.Where(l => l.Adjustments.Count() > 0))
                {
                    line.Adjustments.ForEach(delegate (CartAdjustment adj){this.LineAdjustments.Add(new CartAdjustmentJsonResult{Amount = adj.Amount.ToCurrency(),Description = adj.Description});});
                }
            }
        }
    }
}