using System.Web.Mvc;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.HabitatHome.Feature.Cart.Managers;

namespace Sitecore.HabitatHome.Feature.Cart.Controllers
{
    public class GiftCardBalanceController : BaseCommerceStandardController
    {                                   
        [AllowAnonymous]
        [HttpPost]        
        public JsonResult GetBalance(string cardId)
        {
            GiftCardBalanceManager gm = new GiftCardBalanceManager();
            JsonResult baseJsonResult;
                        
            var result = gm.GetGiftCardBalance(cardId);
            baseJsonResult = this.Json(result);                
           
            return this.Json((object)baseJsonResult);
        }                                                  
    }
}