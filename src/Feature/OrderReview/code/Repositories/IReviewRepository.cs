using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Models.InputModels;
using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Feature.OrderReview.Models.JsonResults;
using Sitecore.XA.Foundation.SitecoreExtensions.Interfaces;

namespace Sitecore.Feature.OrderReview.Repositories
{
    public interface IReviewRepository
    {
        ReviewRenderingModel GetReviewRenderingModel(IRendering rendering);

        OrderReviewDataJsonResult GetReviewData(IVisitorContext visitorContext);
    }
}