using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Feature.GiftCardBalance.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.GiftCardBalance.Controllers
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