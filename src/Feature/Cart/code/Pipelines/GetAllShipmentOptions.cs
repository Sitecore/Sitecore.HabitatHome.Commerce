using Microsoft.OData.Client;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Commerce.Services.Shipping;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Engine.Connect.Pipelines;

namespace Sitecore.HabitatHome.Feature.Cart
{
    public class GetAllShipmentOptions : PipelineProcessor
    {
        public GetAllShipmentOptions(IEntityFactory entityFactory)
        {
            Assert.ArgumentNotNull((object)entityFactory, nameof(entityFactory));
            this.EntityFactory = entityFactory;
        }

        public IEntityFactory EntityFactory { get; set; }

        public override void Process(ServicePipelineArgs args)
        {
            GetShippingOptionsRequest request = args.Request as GetShippingOptionsRequest; ;
            GetShippingOptionsResult result = args.Result as GetShippingOptionsResult;
            //Sitecore.Commerce.Engine.Connect.Pipelines.PipelineUtility.ValidateArguments<GetShippingOptionsRequest, GetShippingOptionsResult>(args, out request, out result);

            try
            {
                //Assert.ArgumentNotNull((object)request.Cart, "request.Cart");
                //Assert.IsTrue(request.Cart.Lines.Count > 0, "request.Cart.Lines");
                string cartId = request.Cart.ExternalId;
                List<CartFulfillment> list1 = Proxy.Execute<CartFulfillment>(this.GetContainer(request.Cart.ShopName, request.Cart.UserId, request.Cart.CustomerId, "", args.Request.CurrencyCode, new DateTime?()).GetCartWithFulfillmentOptions(cartId).Expand("FulfillmentOptions")).ToList<CartFulfillment>();
                DataServiceCollection<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption> fulfillmentOptions1 = list1.FirstOrDefault<CartFulfillment>((Func<CartFulfillment, bool>)(k => k.TargetId == cartId)).FulfillmentOptions;
                if (!fulfillmentOptions1.Any<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption>())
                {
                    //result.Success = false;
                    return;
                }
                List<ShippingOption> list2 = fulfillmentOptions1.Select<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption, ShippingOption>(new Func<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption, ShippingOption>(this.TranslateShippingOption)).ToList<ShippingOption>();
                List<LineShippingOption> lineShippingOptionList = new List<LineShippingOption>();
                foreach (CartLine line1 in request.Cart.Lines)
                {
                    CartLine line = line1;
                    DataServiceCollection<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption> fulfillmentOptions2 = list1.FirstOrDefault<CartFulfillment>((Func<CartFulfillment, bool>)(k => k.TargetId == line.ExternalCartLineId)).FulfillmentOptions;
                    if (!fulfillmentOptions2.Any<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption>())
                    {
                        result.Success = false;
                        return;
                    }
                    List<ShippingOption> list3 = fulfillmentOptions2.Select<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption, ShippingOption>(new Func<Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption, ShippingOption>(this.TranslateShippingOption)).ToList<ShippingOption>();
                    LineShippingOption lineShippingOption = this.EntityFactory.Create<LineShippingOption>("LineShippingOption");
                    lineShippingOption.LineId = line.ExternalCartLineId;
                    lineShippingOption.ShippingOptions = list3;
                    lineShippingOptionList.Add(lineShippingOption);
                }
                result.ShippingOptions = list2.AsReadOnly();
                result.LineShippingPreferences = lineShippingOptionList.AsReadOnly();
            }
            catch (ArgumentException ex)
            {
                //result.Success = false;
                //result.SystemMessages.Add(PipelineUtility.CreateSystemMessage((Exception)ex));
            }
            base.Process(args);
        }

        protected virtual ShippingOption TranslateShippingOption(Sitecore.Commerce.Plugin.Fulfillment.FulfillmentOption fulfillment)
        {
            Assert.ArgumentNotNull((object)fulfillment, nameof(fulfillment));
            ShippingOption shippingOption = this.EntityFactory.Create<ShippingOption>("ShippingOption");
            shippingOption.Name = fulfillment.Name;
            shippingOption.Description = fulfillment.DisplayName;
            shippingOption.ExternalId = fulfillment.Id;
            string lowerInvariant = fulfillment.FulfillmentType.ToLowerInvariant();
            shippingOption.ShippingOptionType = lowerInvariant == "shiptome" ? ShippingOptionType.ShipToAddress : (lowerInvariant == "digital" ? ShippingOptionType.ElectronicDelivery : (lowerInvariant == "splitshipping" ? ShippingOptionType.DeliverItemsIndividually : (lowerInvariant == "shiptostore" ? ShippingOptionType.PickupFromStore : fulfillment.DisplayName.ToLower() == "pickup from store" ? ShippingOptionType.PickupFromStore : ShippingOptionType.None)));
            return shippingOption;
        }

    }
}