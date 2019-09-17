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
    public abstract class BaseCartStringOperatorCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        protected readonly CartServiceProvider CartService;
        protected readonly CustomerServiceProvider CustomerService;

        public BaseCartStringOperatorCondition()
        {
            this.CartService = (CartServiceProvider)Factory.CreateObject("cartServiceProvider", true);
            this.CustomerService = (CustomerServiceProvider)Factory.CreateObject("customerServiceProvider", true);
        }

        protected override bool Execute(T ruleContext)
        {
            SiteContext shopContext = Context.Site;
            Sitecore.Diagnostics.Assert.IsNotNull(shopContext, "Context site cannot be null.");

            var userId = this.GetCurrentUserId();
            var cartList = CartService.GetCarts(new GetCartsRequest(shopContext.Name) { UserIds = new string[] { userId } }).Carts
                .Select(cartBase => CartService
                    .LoadCart(new LoadCartRequest(shopContext.Name, cartBase.ExternalId, userId)).Cart as CommerceCart);

            if (cartList == null)
                return false;
            bool found = CartEvaluationLogic(cartList);

            return found;
        }

        protected abstract bool CartEvaluationLogic(IEnumerable<CommerceCart> cartList);

        /// <summary>Gets the current user identifier.</summary>
        /// <returns>The user id to use to search for the carts.</returns>
        protected virtual string GetCurrentUserId()
        {
            string userName = CommerceTracker.Current.ContactUserName;
            if (Context.User.IsAuthenticated)
            {
                GetUserResult user = CustomerService.GetUser(new GetUserRequest(userName));
                if (user != null && user.Success && user.CommerceUser != null)
                    userName = user.CommerceUser.ExternalId;
            }
            else
                userName = CommerceTracker.Current.ContactId;
            return userName;
        }
    }
}