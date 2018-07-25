using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;

namespace Sitecore.HabitatHome.Feature.Orders.Models
{
    public class OrderLinesViewModel : BaseCommerceRenderingModel
    {
        public OrderLinesViewModel(IModelProvider modelProvider, IStorefrontContext storefrontContext)
        {
            Assert.IsNotNull(modelProvider, nameof(modelProvider));
            this.ModelProvider = modelProvider;
            this.StorefrontContext = storefrontContext;
            this.Lines = new List<OrderLineVariantRenderingModel>();
        }

        public virtual string YourProductsHeaderTooltip { get; set; }

        public virtual string OrderId { get; set; }

        public bool? StoreOrder { get; set; }
        public virtual string StoreName { get; set; }

        public virtual string TrackingNumber { get; set; }

        public virtual List<OrderLineVariantRenderingModel> Lines { get; }

        public virtual Dictionary<string, string> VariantLabels { get; private set; }

        protected IStorefrontContext StorefrontContext { get; set; }

        protected IModelProvider ModelProvider { get; set; }

        public virtual void Initialize(Order order)
        {
            this.OrderId = order.OrderID;
            foreach (CartLine line in order.Lines)
            {
                CartLine orderLine = line;
                OrderLineVariantRenderingModel model = this.ModelProvider.GetModel<OrderLineVariantRenderingModel>();

                Party party = null;                                                              
                ShippingInfo shipping = order.Shipping.FirstOrDefault(s => s.LineIDs.ToList().Contains(orderLine.ExternalCartLineId));
                if (shipping != null)
                {
                    party = order.Parties.FirstOrDefault(p => p.ExternalId.Equals(shipping.PartyID, StringComparison.OrdinalIgnoreCase));
                }

                model.Initialize(orderLine, shipping, party);
                this.Lines.Add(model);
            }
            this.StoreOrder = order.Status.ToLower() == "storeorder" ? true : false;
            this.StoreName = !String.IsNullOrEmpty(order.ShopName) ? GetDisplayName(order.ShopName) : "";
            this.TrackingNumber = order.TrackingNumber;
            this.InitializeDataSourceValues();
        }

        protected virtual void InitializeDataSourceValues()
        {
            Rendering rendering = RenderingContext.CurrentOrNull.ValueOrDefault(context => context.Rendering);
            if (rendering == null)
            {
                ErrorMessage = "[Order Details Lines] Rendering not found.";
            }
            else
            {                                
                if (rendering.Item == null)
                {
                    ErrorMessage = "[Order Details Lines] Please set the rendering datasource appropriately";
                }
                else
                {
                    YourProductsHeaderTooltip = rendering.Item.Fields["Order Lines Header Tooltip"].Value;
                    VariantLabels = new Dictionary<string, string>()
                    {
                        {
                          "Color", StorefrontContext.GetVariantSpecificationLabels("Color", true)
                        },
                        {
                          "Size", StorefrontContext.GetVariantSpecificationLabels("Size", true)
                        },
                        {
                          "Style", StorefrontContext.GetVariantSpecificationLabels("Style", true)
                        }
                    };
                }
            }
        }

        private string GetDisplayName(string StoreId)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return (r.Replace(StoreId, " "));
        }
    }
}