using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Feature.NearestStore.Models;
using Sitecore.Feature.NearestStore.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.NearestStore.Controllers
{
    public class NearestStoreController : BaseCommerceStandardController
    {

        public ActionResult NearestStore()
        {
            UserLocation ul = GeoUtil.GetUserLocation();
            return (ActionResult)this.View("~/Views/NearestStore/NearestStore.cshtml", ul);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetStores()
        {
            
            BaseJsonResult baseJsonResult;
            try
            {
                dynamic dyResult = new System.Dynamic.ExpandoObject();
                baseJsonResult = this.Json(dyResult);
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.Json((object)baseJsonResult);
        }
    }
}