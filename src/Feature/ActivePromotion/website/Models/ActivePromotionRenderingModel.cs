using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.HabitatHome.Foundation.Promotions.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ActivePromotion.Models
{
    public class ActivePromotionRenderingModel
    {        
        public CatalogItemRenderingModel ProductItemRenderingModel { get; set; }
        public bool HasActivePromotions { get; set; }
        public List<Promotion> ActivePromotions { get; set; }
    }
}