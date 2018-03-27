using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using Sitecore.Commerce.XA.Feature.Account.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Feature.OrderLines.Models
{
    public class OrderLinesViewModel : BaseCommerceRenderingModel
    {
        public OrderLinesViewModel(IModelProvider modelProvider, IStorefrontContext storefrontContext)
        {
            Assert.IsNotNull((object)modelProvider, nameof(modelProvider));
            this.ModelProvider = modelProvider;
            this.StorefrontContext = storefrontContext;
            this.Lines = new List<OrderLineRenderingModel>();
        }

        public virtual string YourProductsHeaderTooltip { get; set; }

        public virtual string OrderId { get; set; }

        public bool? StoreOrder { get; set; }
        public virtual string StoreName { get; set; }

        public virtual string TrackingNumber { get; set; }

        public virtual List<OrderLineRenderingModel> Lines { get; }

        public virtual Dictionary<string, string> VariantLabels { get; private set; }

        protected IStorefrontContext StorefrontContext { get; set; }

        protected IModelProvider ModelProvider { get; set; }

        public virtual void Initialize(Order order)
        {
            this.OrderId = order.OrderID;
            foreach (CartLine line in order.Lines)
            {
                CartLine orderLine = line;
                OrderLineRenderingModel model = this.ModelProvider.GetModel<OrderLineRenderingModel>();
                Party party = (Party)null;
                ShippingInfo shipping = order.Shipping.FirstOrDefault<ShippingInfo>((Func<ShippingInfo, bool>)(s => s.LineIDs.ToList<string>().Contains(orderLine.ExternalCartLineId)));
                if (shipping != null)
                    party = order.Parties.FirstOrDefault<Party>((Func<Party, bool>)(p => p.ExternalId.Equals(shipping.PartyID, StringComparison.OrdinalIgnoreCase)));
                model.Initialize(orderLine, shipping, party);
                this.Lines.Add(model);
            }
            this.StoreOrder = order.Status.ToLower() == "storeorder" ? true : false;
            this.StoreName = GetDisplayName(order.ShopName);
            this.TrackingNumber = order.TrackingNumber;
            this.InitializeDataSourceValues();
        }

        protected virtual void InitializeDataSourceValues()
        {
            Rendering rendering = RenderingContext.CurrentOrNull.ValueOrDefault<RenderingContext, Rendering>((Func<RenderingContext, Rendering>)(context => context.Rendering));
            if (rendering == null)
            {
                this.ErrorMessage = "[Order Details Lines] Rendering not found.";
            }
            else
            {
                Item obj = rendering.Item;
                if (obj == null)
                {
                    this.ErrorMessage = "[Order Details Lines] Please set the rendering datasource appropriately";
                }
                else
                {
                    this.YourProductsHeaderTooltip = obj.Fields["Order Lines Header Tooltip"].Value;
                    this.VariantLabels = new Dictionary<string, string>()
          {
            {
              "Color",
              this.StorefrontContext.GetVariantSpecificationLabels("Color", true)
            },
            {
              "Size",
              this.StorefrontContext.GetVariantSpecificationLabels("Size", true)
            },
            {
              "Style",
              this.StorefrontContext.GetVariantSpecificationLabels("Style", true)
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