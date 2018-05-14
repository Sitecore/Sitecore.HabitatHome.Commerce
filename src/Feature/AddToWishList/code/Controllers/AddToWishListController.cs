using System;
using System.Collections.Generic;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Feature.AddToWishList.Controllers
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
            return (ActionResult)this.View("~/Views/AddToWishList/AddToWishList.cshtml");
        }
    }
}