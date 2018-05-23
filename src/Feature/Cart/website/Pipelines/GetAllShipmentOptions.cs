using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Client;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Shipping;
using Sitecore.Diagnostics;

namespace Sitecore.HabitatHome.Feature.Cart.Pipelines
{
    public class GetAllShipmentOptions : PipelineProcessor
    {
        public GetAllShipmentOptions(IEntityFactory entityFactory)
        {
            Assert.ArgumentNotNull(entityFactory, nameof(entityFactory));
            this.EntityFactory = entityFactory;
        }

        public IEntityFactory EntityFactory { get; set; }

        public override void Process(ServicePipelineArgs args)
        {
            // validate args            
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Request.RequestContext, "args.Request.RequestContext");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            GetShippingOptionsRequest request = args.Request as GetShippingOptionsRequest;
            GetShippingOptionsResult result = args.Result as GetShippingOptionsResult;
            Assert.IsNotNull(request, "The parameter args.Request was not of the expected type.  Expected {0}.  Actual {1}.", typeof(GetShippingOptionsRequest).Name, args.Request.GetType().Name);
            Assert.IsNotNull(result, "The parameter args.Result was not of the expected type.  Expected {0}.  Actual {1}.", typeof(GetShippingOptionsResult).Name, args.Result.GetType().Name);

            try
            {
                Assert.ArgumentNotNull(request.Cart, "request.Cart");
                Assert.IsTrue(request.Cart.Lines.Count > 0, "request.Cart.Lines");
                string cartId = request.Cart.ExternalId;
                List<CartFulfillment> cartFulfillments = Proxy.Execute(this.GetContainer(request.Cart.ShopName, request.Cart.UserId, request.Cart.CustomerId, "", args.Request.CurrencyCode, new DateTime?()).GetCartWithFulfillmentOptions(cartId).Expand("FulfillmentOptions")).ToList();
                DataServiceCollection<FulfillmentOption> fulfillmentOptions = cartFulfillments.FirstOrDefault(k => k.TargetId == cartId)?.FulfillmentOptions;
                if (fulfillmentOptions == null || !fulfillmentOptions.Any())
                {
                    result.Success = false;
                    return;
                }
                List<ShippingOption> shippingOptions = fulfillmentOptions.Select(this.TranslateShippingOption).ToList();
                List<LineShippingOption> lineShippingOptionList = new List<LineShippingOption>();

                foreach (CartLine cartLine in request.Cart.Lines)
                {
                    CartLine line = cartLine;
                    DataServiceCollection<FulfillmentOption> lineFulfillmentOptions = cartFulfillments.FirstOrDefault(k => k.TargetId == line.ExternalCartLineId)?.FulfillmentOptions;
                    if (!lineFulfillmentOptions.Any())
                    {
                        result.Success = false;
                        return;
                    }
                    List<ShippingOption> lineShippingOptions = lineFulfillmentOptions.Select(this.TranslateShippingOption).ToList();
                    LineShippingOption lineShippingOption = this.EntityFactory.Create<LineShippingOption>("LineShippingOption");
                    lineShippingOption.LineId = line.ExternalCartLineId;
                    lineShippingOption.ShippingOptions = lineShippingOptions;
                    lineShippingOptionList.Add(lineShippingOption);
                }

                result.ShippingOptions = shippingOptions.AsReadOnly();
                result.LineShippingPreferences = lineShippingOptionList.AsReadOnly();
            }
            catch (ArgumentException ex)
            {
                result.Success = false;
                result.SystemMessages.Add(new SystemMessage(ex.Message));
            }
            base.Process(args);
        }

        protected virtual ShippingOption TranslateShippingOption(FulfillmentOption fulfillment)
        {
            Assert.ArgumentNotNull(fulfillment, nameof(fulfillment));
            ShippingOption shippingOption = this.EntityFactory.Create<ShippingOption>("ShippingOption");
            shippingOption.Name = fulfillment.Name;
            shippingOption.Description = fulfillment.DisplayName;
            shippingOption.ExternalId = fulfillment.Id;            

            switch (fulfillment.FulfillmentType.ToLowerInvariant())
            {
                case "shiptome":
                    shippingOption.ShippingOptionType = ShippingOptionType.ShipToAddress;
                    break;
                case "digital":
                    shippingOption.ShippingOptionType = ShippingOptionType.ElectronicDelivery;
                    break;
                case "splitshipping":
                    shippingOption.ShippingOptionType = ShippingOptionType.DeliverItemsIndividually;
                    break;
                case "shiptostore":
                    shippingOption.ShippingOptionType = ShippingOptionType.PickupFromStore;
                    break;
                case "pickupfromstore":
                    shippingOption.ShippingOptionType = ShippingOptionType.PickupFromStore;
                    break;
                default:
                    shippingOption.ShippingOptionType = fulfillment.DisplayName.ToLower() == "pickup from store" ? 
                        ShippingOptionType.PickupFromStore : ShippingOptionType.None;
                    break;
            }                                                                                                   
            return shippingOption;
        }

    }
}