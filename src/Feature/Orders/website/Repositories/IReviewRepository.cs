using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.Orders.Models.JsonResults;
using Sitecore.XA.Foundation.SitecoreExtensions.Interfaces;

namespace Sitecore.HabitatHome.Feature.Orders.Repositories
{
    public interface IReviewRepository
    {
        ReviewRenderingModel GetReviewRenderingModel(IRendering rendering);

        OrderReviewDataJsonResult GetReviewData(IVisitorContext visitorContext);
    }
}