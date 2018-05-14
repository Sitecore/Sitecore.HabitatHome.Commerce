using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Feature.NearestStore.Repositories;
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
        private readonly IVisitorContext _visitorContext;

        //TODO: ADD DEPENDENCY INJECTION
        public NearestStoreController(IStoresRepository storesRepository, IVisitorContext visitorContext)
        {
            this.StoresRepository = storesRepository;
            _visitorContext = visitorContext;
        }

        public ActionResult NearestStore()
        {
            return (ActionResult)this.View("~/Views/NearestStore/NearestStore.cshtml", this.StoresRepository.GetNearestStoreRenderingModel(_visitorContext));
        }


        [AllowAnonymous]
        [HttpPost]
        //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetStores(string pid)
        {
            JsonResult baseJsonResult;
            try
            {
                dynamic stores = from s in this.StoresRepository.GetNearestStores(pid) where s != null select s.GetViewModel();
                baseJsonResult = this.Json(stores);
            }
            catch (Exception ex)
            {
                throw;
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