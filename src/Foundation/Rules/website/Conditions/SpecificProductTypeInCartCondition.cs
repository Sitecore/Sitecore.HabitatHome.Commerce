using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Rules;
using Sitecore.Commerce.Rules.Conditions;
using Sitecore.Rules.Conditions;
using Sitecore.Commerce.Multishop;
using Sitecore.Configuration;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Sites;
using Sitecore.Commerce;
using Sitecore.Commerce.Engine.Connect.Entities;

namespace Sitecore.HabitatHome.Foundation.Rules.Conditions
{
    public class SpecificProductTypeInCartCondition<T> : BaseCartStringOperatorCondition<T> where T : RuleContext
    {
        public string ProductType { get; set; }

        public SpecificProductTypeInCartCondition():base(){}

        protected override bool CartEvaluationLogic(IEnumerable<CommerceCart> cartList)
        {
            if (string.IsNullOrEmpty(ProductType))
                return false;

            return cartList.Any(cart =>
                cart != null && cart.Lines.Any(cartline =>
                    cartline != null && cartline.GetProperties().TryGetValue("ItemType", out object property) && 
                    property != null && string.Equals(property.ToString(), ProductType, StringComparison.OrdinalIgnoreCase)));
        }
    }
}