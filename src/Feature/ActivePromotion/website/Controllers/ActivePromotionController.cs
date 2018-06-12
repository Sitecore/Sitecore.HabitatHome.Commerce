using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.ActivePromotion.Repositories;
using Sitecore.Commerce.Plugin.Promotions;
using Sitecore.Commerce.Core.Commands;

namespace Sitecore.HabitatHome.Feature.ActivePromotion.Controllers
{
    public class ActivePromotionController : BaseCommerceStandardController
    {
        public ActivePromotionController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IActivePromotionRepository activePromotionRepository) : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));            
            _visitorContext = visitorContext;
            _activePromotionRepository = activePromotionRepository;
        }
        private readonly IActivePromotionRepository _activePromotionRepository;
        private readonly IVisitorContext _visitorContext;

        public ActionResult ActivePromotion()
        {

            return View("~/Views/ActivePromotion/ActivePromotion.cshtml", _activePromotionRepository.GetActivePromotionRenderingModel(_visitorContext));
        }
    }
}