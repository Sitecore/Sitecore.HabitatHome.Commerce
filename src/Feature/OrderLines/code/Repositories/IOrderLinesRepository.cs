using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Feature.OrderLines.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.OrderLines.Repositories
{
    public interface IOrderLinesRepository
    {
        OrderLinesViewModel GetOrderLinesRenderingModel(IVisitorContext visitorContext, string orderId);
    }
}