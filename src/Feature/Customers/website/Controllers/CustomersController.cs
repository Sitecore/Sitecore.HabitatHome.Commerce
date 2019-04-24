using System;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using Sitecore.Commerce;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Commerce.XA.Feature.Account.Controllers;
using Sitecore.Commerce.XA.Feature.Account.Models;
using Sitecore.Commerce.XA.Feature.Account.Models.JsonResults;
using Sitecore.Commerce.XA.Feature.Account.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Attributes;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.HabitatHome.Feature.Customers.Models;
using Sitecore.Commerce.XA.Foundation.Connect.ExtensionMethods;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Utils;
using Sitecore.Diagnostics;
using System.Web.Security;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;

namespace Sitecore.HabitatHome.Feature.Customers.Controllers
{
    public class CustomersController : AccountController
    {
        public CustomerServiceProvider _customerServiceProvider
        {
            get;
            set;
        }

        public CustomersController(ILoginRepository loginRepository, IRegistrationRepository registrationRepository, IForgotPasswordRepository forgotPasswordRepository, IChangePasswordRepository changePasswordRepository, IAccountManager accountManager, IStorefrontContext storefrontContext, IModelProvider modelProvider, IContext sitecoreContext, IRegisterUserRepository registerUserRepository, ILoginUserRepository loginUserRepository, IConnectServiceProvider connectServiceProvider)
            : base(loginRepository, registrationRepository, forgotPasswordRepository, changePasswordRepository, accountManager, storefrontContext, modelProvider, sitecoreContext, registerUserRepository, loginUserRepository)
        {
            _customerServiceProvider = connectServiceProvider.GetCustomerServiceProvider();
        }

        [StorefrontSessionState(SessionStateBehavior.ReadOnly), AllowAnonymous, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ExtendedRegistration()
        {
            RegistrationUserRenderingModel registrationModel = RegistrationRepository.GetRegistrationModel();
            return View("~/Views/Customers/Registration.cshtml", registrationModel);
        }

        [ValidateHttpPostHandler, AllowAnonymous, HttpPost, OutputCache(NoStore = true, Location = OutputCacheLocation.None), ValidateAntiForgeryToken]
        public JsonResult ExtendedRegistration(RegistrationInputModel inputModel)
        {
            JsonResult result;
            try
            {
                Diagnostics.Assert.ArgumentNotNull(inputModel, "RegistrationInputModel");
                RegistrationBaseJsonResult registrationBaseJsonResult = new RegistrationBaseJsonResult(StorefrontContext, SitecoreContext);
                ValidateModel(registrationBaseJsonResult);
                if (registrationBaseJsonResult.HasErrors)
                {
                    result = Json(registrationBaseJsonResult, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ManagerResponse<CreateUserResult, CommerceUser> managerResponse = this.RegisterUser(StorefrontContext, UpdateUsername(inputModel.UserName), inputModel.Password, inputModel.UserName);
                    if (managerResponse.ServiceProviderResult.Success && managerResponse.Result != null)
                    {
                        registrationBaseJsonResult.Initialize(managerResponse.Result);
                        AccountManager.Login(StorefrontContext, VisitorContext, managerResponse.Result.UserName, inputModel.Password, false);
                        AccountManager.UpdateUser(VisitorContext, inputModel.FirstName, inputModel.LastName, string.Empty, inputModel.UserName);
                    }
                    else
                    {
                        registrationBaseJsonResult.SetErrors(managerResponse.ServiceProviderResult);
                    }
                    result = Json(registrationBaseJsonResult);
                }
            }
            catch (Exception exception)
            {
                result = Json(new BaseJsonResult("Registration", exception, StorefrontContext, SitecoreContext), JsonRequestBehavior.AllowGet);
            }
            return result;
        }
        public virtual ManagerResponse<CreateUserResult, CommerceUser> RegisterUser(IStorefrontContext storefrontContext, string userName, string password, string email, StringPropertyCollection propertyBag = null)
        {
            Diagnostics.Assert.ArgumentNotNull(storefrontContext, "storefrontContext");
            Diagnostics.Assert.ArgumentNotNullOrEmpty(userName, "userName");
            Diagnostics.Assert.ArgumentNotNullOrEmpty(password, "password");
            CreateUserResult createUserResult;
            try
            {
                CreateUserRequest request = new CreateUserRequest(userName, password, email, storefrontContext.CurrentStorefront.ShopName);
                request.CopyPropertyBag(propertyBag);
                createUserResult = _customerServiceProvider.CreateUser(request);
                if (!createUserResult.Success)
                {
                    Helpers.LogSystemMessages(createUserResult.SystemMessages, createUserResult);
                }
                else if (createUserResult.Success && createUserResult.CommerceUser == null && createUserResult.SystemMessages.Count == 0)
                {
                    createUserResult.Success = false;
                    createUserResult.SystemMessages.Add(new SystemMessage
                    {
                        Message = storefrontContext.GetSystemMessage("User Already Exists")
                    });
                }
            }
            catch (MembershipCreateUserException ex)
            {
                createUserResult = new CreateUserResult
                {
                    Success = false
                };
                createUserResult.SystemMessages.Add(new SystemMessage
                {
                    Message = ex.StatusCode.ToString()
                });
            }
            catch (Exception ex)
            {
                createUserResult = new CreateUserResult
                {
                    Success = false
                };
                createUserResult.SystemMessages.Add(new SystemMessage
                {
                    Message = storefrontContext.GetSystemMessage("Unknown Membership Provider Error")
                });
            }
            return new ManagerResponse<CreateUserResult, CommerceUser>(createUserResult, createUserResult.CommerceUser);
        }

        protected virtual string UpdateUsername(string userName)
        {
            Diagnostics.Assert.ArgumentNotNullOrEmpty(userName, "userName");
            string userDomain = StorefrontContext.CurrentStorefront.UserDomain;
            if (userName.StartsWith(userDomain, StringComparison.OrdinalIgnoreCase))
            {
                return userName;
            }
            return userDomain + "\\" + userName;
        }

    }
}