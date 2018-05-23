using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.Orders.Models;

namespace Sitecore.HabitatHome.Feature.Orders.Repositories
{
    public interface IOrderLinesRepository
    {
        OrderLinesViewModel GetOrderLinesRenderingModel(IVisitorContext visitorContext, string orderId);
    }
}