using System.Linq;
using System.Web.Mvc;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.NearestStore.Repositories;

namespace Sitecore.HabitatHome.Feature.NearestStore.Controllers
{
    public class NearestStoreController : BaseCommerceStandardController
    {
        private readonly IStoresRepository _storesRepository;
        private readonly IVisitorContext _visitorContext;
                                                   
        public NearestStoreController(IStoresRepository storesRepository, IVisitorContext visitorContext)
        {
            _storesRepository = storesRepository;
            _visitorContext = visitorContext;
        }

        public ActionResult NearestStore()
        {
            return View("~/Views/NearestStore/NearestStore.cshtml", _storesRepository.GetNearestStoreRenderingModel(_visitorContext));
        }


        [AllowAnonymous]
        [HttpPost]                                                               
        public JsonResult GetStores(string pid)
        {
            dynamic stores = from s in _storesRepository.GetNearestStores(pid) where s != null select s.GetViewModel(); 
            JsonResult baseJsonResult = this.Json(stores);

            return Json(baseJsonResult);
        }



        [AllowAnonymous]
        [HttpPost]                                                                 
        public JsonResult GetInventory(string pid)
        {
            dynamic stores = from s in _storesRepository.GetStoresInventory(pid) where s != null select s.GetViewModel();
            JsonResult baseJsonResult = this.Json(stores);

            return Json(baseJsonResult);
        }
    }
}