using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Feature.NearestStore.Models;
using Sitecore.Feature.NearestStore.Models.Responses;
using Sitecore.Feature.NearestStore.Repositories;
using Sitecore.Feature.NearestStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.NearestStore.Controllers
{
    public class NearestStoreController : BaseCommerceStandardController
    {
        private IStoresRepository StoresRepository { get; }


        //TODO: ADD DEPENDENCY INJECTION
        public NearestStoreController(IStoresRepository storesRepository)
        {
            this.StoresRepository = storesRepository;
        }
        //public NearestStoreController()
        //{
        //    this.StoresRepository = new StoresRepository();
        //}
        public ActionResult NearestStore()
        {
            UserLocation ul = GeoUtil.GetUserLocation();
            return (ActionResult)this.View("~/Views/NearestStore/NearestStore.cshtml", ul);
        }


        [AllowAnonymous]
        [HttpPost]
        //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetStores(string userLatitude, string userLongitude, string pid)
        {
            JsonResult baseJsonResult;
            try
            {
                UserLocation ul = new UserLocation();
                ul.Latitude = userLatitude;
                ul.Longitude = userLongitude;
                dynamic stores = from s in this.StoresRepository.GetNearestStores(ul, pid) where s != null select s.GetViewModel();
                baseJsonResult = this.Json(stores);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.Json((object)baseJsonResult);
        }



        [AllowAnonymous]
        [HttpPost]
        //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetInventory(string pid)
        {
            JsonResult baseJsonResult;
            try
            {                
                dynamic stores = from s in this.StoresRepository.GetStoresInventory(pid) where s != null select s.GetViewModel();
                baseJsonResult = this.Json(stores);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.Json((object)baseJsonResult);
        }
    }
}