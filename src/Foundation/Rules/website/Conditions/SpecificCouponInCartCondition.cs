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
    public class SpecificCouponInCartCondition<T> : BaseCartStringOperatorCondition<T> where T : RuleContext
    {
        public string CouponCode { get; set; }

        public SpecificCouponInCartCondition():base(){}

        protected override bool CartEvaluationLogic(IEnumerable<CommerceCart> cartList)
        {
            if (string.IsNullOrEmpty(CouponCode))
                return false;

            return cartList.Any(cart =>
                cart != null && cart.OrderForms.Any(form =>
                    form != null && form.PromoCodes.Any(promoCode =>
                        promoCode != null && promoCode.Equals(CouponCode))));
        }
    }
}