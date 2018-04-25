using Sitecore.Analytics;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Extensions;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using Sitecore.Owin.Authentication.Pipelines.CookieAuthentication.SignIn;
using System;              

namespace Sitecore.Feature.Customers.Pipelines.SignIn
{
    public class RegisterCommerceCustomer : SignInProcessor
    {
        private readonly ICartManager _cartManager;                                  
        private readonly IStorefrontContext _storefrontContext;
        private readonly CustomerServiceProvider _customerServiceProvider;
        private readonly IVisitorContext _visitorContext;

        public RegisterCommerceCustomer(ICartManager cartManager, IConnectServiceProvider connectServiceProvider, IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            _cartManager = cartManager;
            _storefrontContext = storefrontContext;
            _customerServiceProvider = connectServiceProvider.GetCustomerServiceProvider();
            _visitorContext = visitorContext;
        }

        public override void Process(SignInArgs args)
        {
            try
            {
                // get current user              
                string username = args.User.InnerUser.Profile.Email;
                GetUserRequest request = new GetUserRequest(username);
                GetUserResult user = _customerServiceProvider.GetUser(request);
                if (!user.Success || user.CommerceUser == null)
                {
                    // if user isn't a customer, run customer create pipeline                       
                    string password = Guid.NewGuid().ToString();
                    CreateUserRequest createUserRequest = new CreateUserRequest(username, password, username, _storefrontContext.CurrentStorefront.ShopName);                    
                    CreateUserResult createUserResult = _customerServiceProvider.CreateUser(createUserRequest);   
                }

                // identify commerce user and merge anonymous cart
                Cart result = _cartManager.GetCurrentCart(_visitorContext, _storefrontContext, false).Result;
                Tracker.Current.CheckForNull().Session.IdentifyAs("CommerceUser", username);
                _visitorContext.UserJustLoggedIn();
                _cartManager.MergeCarts(_storefrontContext.CurrentStorefront, _visitorContext, _visitorContext.CustomerId, result);

            }
            catch (Exception ex)
            {
                // log error
                Sitecore.Diagnostics.Log.Error(string.Format("Failed to create a customer for external user login {0}", args.User.UserName), this);
            }    
        }
    }
}