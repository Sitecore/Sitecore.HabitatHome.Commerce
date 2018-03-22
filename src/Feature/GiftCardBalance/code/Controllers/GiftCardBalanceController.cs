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
        //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetBalance(string cardId)
        {
            GiftCardBalanceManager gm = new GiftCardBalanceManager();
            JsonResult baseJsonResult;
            try
            {
                dynamic result = new System.Dynamic.ExpandoObject();
                result = gm.GetGiftCardBalance(cardId);
                baseJsonResult = this.Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.Json((object)baseJsonResult);
        }


    }
}