using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.ActivePromotion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ActivePromotion.Repositories
{
    public interface IActivePromotionRepository
    {
        ActivePromotionRenderingModel GetActivePromotionRenderingModel(IVisitorContext visitorContext);
    }
}