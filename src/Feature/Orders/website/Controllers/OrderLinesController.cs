using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Orders.Repositories;

namespace Sitecore.HabitatHome.Feature.Orders.Controllers
{
    public class OrderLinesController : BaseCommerceStandardController
    {
        private readonly ICartManager _cartManager;      
        private readonly IVisitorContext _visitorContext;
        private readonly IOrderLinesRepository _orderLinesRepository;
        private readonly IModelProvider _modelProvider;                

        public OrderLinesController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IOrderLinesRepository orderLinesRepository, IModelProvider modelProvider, ICartManager cartManager) : base(storefrontContext)
        {
            Assert.IsNotNull(storefrontContext, nameof(storefrontContext));
            Assert.IsNotNull(visitorContext, nameof(visitorContext));
            Assert.IsNotNull(orderLinesRepository, nameof(orderLinesRepository));
            _orderLinesRepository = orderLinesRepository;
            _modelProvider = modelProvider;
            _visitorContext = visitorContext;
            _cartManager = cartManager;                   
        }

        [HttpGet]
        [Authorize]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult OrderLines([Bind(Prefix = "id")] string orderId = "")
        {          

            return View("~/Views/Orders/OrderLines.cshtml", _orderLinesRepository.GetOrderLinesRenderingModel(_visitorContext, orderId));

        }

        [HttpPost]
        public JsonResult ReorderItem(string productId, string variantId, string quantity)
        {
            BaseJsonResult model = new BaseJsonResult(StorefrontContext);
            CommerceStorefront currentStorefront = StorefrontContext.CurrentStorefront;
            ManagerResponse<CartResult, Cart> currentCart = _cartManager.GetCurrentCart(_visitorContext, StorefrontContext, false);
            if (!currentCart.ServiceProviderResult.Success || currentCart.Result == null)
            {
                string systemMessage = StorefrontContext.GetSystemMessage("Cart Not Found Error", true);
                currentCart.ServiceProviderResult.SystemMessages.Add(new SystemMessage
                {
                    Message = systemMessage
                });
                model.SetErrors(currentCart.ServiceProviderResult);
                return model;
            }

            List<CartLineArgument> list = new List<CartLineArgument>();
            list.Add(new CartLineArgument
            {
                CatalogName = StorefrontContext.CurrentStorefront.Catalog,
                ProductId = productId,
                VariantId = variantId,
                Quantity = decimal.Parse(quantity)
            });

            ManagerResponse<CartResult, Cart> managerResponse = _cartManager.AddLineItemsToCart(currentStorefront, _visitorContext, currentCart.Result, list);
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors(managerResponse.ServiceProviderResult);
                return model;
            }
            model.Success = true;
            return base.Json(model);
        }                
    }
}