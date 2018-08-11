using System.Web.Mvc;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;

namespace Sitecore.HabitatHome.Feature.WishLists.Controllers
{
    public class AddToWishListController : BaseCommerceStandardController
    {                                                         
        public AddToWishListController()
        {                                          
        }

        public ActionResult AddToWishList()
        {
            return View("~/Views/Wishlists/AddToWishList.cshtml");
        }
    }
}