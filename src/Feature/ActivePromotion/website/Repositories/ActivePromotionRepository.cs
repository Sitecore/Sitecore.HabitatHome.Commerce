using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.HabitatHome.Feature.ActivePromotion.Models;
using Sitecore.HabitatHome.Foundation.Promotions.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ActivePromotion.Repositories
{
    public class ActivePromotionRepository : BaseCatalogRepository, IActivePromotionRepository
    {
        public ActivePromotionRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager, IPromotionsManager promotionsManager) : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager)
        {
            _promotionsManager = promotionsManager;
        }
        private IPromotionsManager _promotionsManager;
        public virtual ActivePromotionRenderingModel GetActivePromotionRenderingModel(IVisitorContext visitorContext)
        {
            ActivePromotionRenderingModel renderingModel = new ActivePromotionRenderingModel();          
            renderingModel.ProductItemRenderingModel = this.GetProduct(visitorContext);
            renderingModel.ActivePromotions = _promotionsManager.GetActivePromotions(this.GetProduct(visitorContext).ProductId).ToList();
            renderingModel.HasActivePromotions = renderingModel.ActivePromotions.Count > 0;

            return renderingModel;
        }
    }
}