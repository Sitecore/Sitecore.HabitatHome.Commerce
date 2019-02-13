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

namespace Sitecore.HabitatHome.Feature.Customers.Controllers
{
    public class CustomersController : AccountController
    {
        public CustomersController(ILoginRepository loginRepository, IRegistrationRepository registrationRepository, IForgotPasswordRepository forgotPasswordRepository, IChangePasswordRepository changePasswordRepository, IAccountManager accountManager, IStorefrontContext storefrontContext, IModelProvider modelProvider, IContext sitecoreContext, IRegisterUserRepository registerUserRepository, ILoginUserRepository loginUserRepository) 
            : base(loginRepository, registrationRepository, forgotPasswordRepository, changePasswordRepository, accountManager, storefrontContext, modelProvider, sitecoreContext, registerUserRepository, loginUserRepository)
        {
        }
                                                                               
        [StorefrontSessionState(SessionStateBehavior.ReadOnly), AllowAnonymous, OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public  ActionResult ExtendedRegistration()
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
                Assert.ArgumentNotNull(inputModel, "RegistrationInputModel");
                RegistrationBaseJsonResult registrationBaseJsonResult = new RegistrationBaseJsonResult(StorefrontContext, SitecoreContext);
                ValidateModel(registrationBaseJsonResult);
                if (registrationBaseJsonResult.HasErrors)
                {
                    result = Json(registrationBaseJsonResult, JsonRequestBehavior.AllowGet);
                }
                else
                {                                              
                    ManagerResponse<CreateUserResult, CommerceUser> managerResponse = AccountManager.RegisterUser(StorefrontContext, inputModel.UserName, inputModel.Password, inputModel.UserName);
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

    }
}