using System.Web.Mvc;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;

namespace Sitecore.HabitatHome.Feature.WishLists.Controllers
{
    public class AddToWishListController : BaseCommerceStandardController
    {
        private readonly IVisitorContext _visitorContext;
        public AddToWishListController(IVisitorContext visitorContext)
        {
            _visitorContext = visitorContext;
        }

        public ActionResult AddToWishList()
        {
            return (ActionResult)this.View("~/Views/Wishlists/AddToWishList.cshtml");
        }
    }
}