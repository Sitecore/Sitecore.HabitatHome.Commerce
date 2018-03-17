using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Feature.NearestStore.Models;
using Sitecore.Feature.NearestStore.Repositories;
using Sitecore.Feature.NearestStore.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.NearestStore.Controllers
{
    public class NearestStoreController : BaseCommerceStandardController
    {
        private IStoresRepository StoresRepository { get; }

        public ActionResult NearestStore()
        {
            UserLocation ul = GeoUtil.GetUserLocation();
            return (ActionResult)this.View("~/Views/NearestStore/NearestStore.cshtml", ul);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetStores(dynamic request)
        {
            string latitude = request.Latitude;
            string longitude = request.Longitude;

            BaseJsonResult baseJsonResult;
            try
            {
                dynamic dyResult = new System.Dynamic.ExpandoObject();

                UserLocation ul = new UserLocation();
                ul.Latitude = latitude;
                ul.Longitude = longitude;
                var stores = this.StoresRepository.GetNearestStores(ul);

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