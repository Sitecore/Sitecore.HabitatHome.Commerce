using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.Feature.WishListLines.Repositories;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.WishListLiness.Controllers
{
    public class WishListLinessController : BaseCommerceStandardController
    {
        
        protected IWishListLinesRepository WishListLinesRepository { get; set; }

        [HttpGet]
        public ActionResult WishListLines()
        {
            return (ActionResult)this.View("~/Views/WishListLines/WishListLines.cshtml", (object)this.WishListLinesRepository.GetWishListLinesModel()); ;
        }
    }
}