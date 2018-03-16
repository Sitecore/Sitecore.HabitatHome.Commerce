
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Customers;
using Plugin.Demo.ImportOrders.Pipelines.Arguments;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.Plugin.Fulfillment;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Plugin.Demo.ImportOrders.Models;

namespace Plugin.Demo.ImportOrders.Pipelines.Blocks
{
    [PipelineDisplayName("Orders.block.ImportStoreOrder")]
    public class CreateOfflineOrderBlock : PipelineBlock<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>
    {
        private readonly IDeleteEntityPipeline deletePipeline;
        private readonly TelemetryClient telemetryClient;
        private readonly PerformanceCounterCommand performanceCounterCommand;
        private readonly IFindEntityPipeline findEntityPipeline;
        private readonly IGetEntityFromCachePipeline getEntityFromCachePipeline;
        


        public CreateOfflineOrderBlock(IGetEntityFromCachePipeline getEntityFromCachePipeline, IDeleteEntityPipeline deleteEntityPipeline, TelemetryClient telemetryClient, PerformanceCounterCommand performanceCounterCommand, IFindEntityPipeline findEntityPipeline)
          : base((string)null)
        {
            this.deletePipeline = deleteEntityPipeline;
            this.telemetryClient = telemetryClient;
            this.performanceCounterCommand = performanceCounterCommand;
            this.findEntityPipeline = findEntityPipeline;
            this.getEntityFromCachePipeline = getEntityFromCachePipeline;
        }


        public override async Task<Order> Run(OfflineStoreOrderArgument arg, CommercePipelineExecutionContext context)
        {
            CreateOfflineOrderBlock createOrderBlock = this;            
            Condition.Requires<OfflineStoreOrderArgument>(arg).IsNotNull<OfflineStoreOrderArgument>(string.Format("{0}: arg can not be null", (object)(createOrderBlock.Name)));
            
            CommercePipelineExecutionContext executionContext;
            if (string.IsNullOrEmpty(arg.Email))
            {
                executionContext = context;
                executionContext.Abort(await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error, "EmailIsRequired", new object[1], "Can not create order, email address is required."), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return (Order)null;
            }
            //Cart cart = arg.Cart;
            if (arg.Lines.Count() == 0)
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string error = context.GetPolicy<KnownResultCodes>().Error;
                string commerceTermKey = "OrderHasNoLines";                
                string defaultMessage = string.Format("Can not create order, cart  has no lines");
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, null, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return (Order)null;
            }            

            // Find contact using email
            string email = arg.Email;
            //var customer = context.CommerceContext.GetEntities<Customer>().Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
            ContactComponent contactComponent = new ContactComponent();
            EntityIndex entityIndex = await createOrderBlock.findEntityPipeline.Run(new FindEntityArgument(typeof(EntityIndex), string.Format("{0}{1}", (object)EntityIndex.IndexPrefix<Customer>("Id"), arg.Email), false), context) as EntityIndex;

            if (entityIndex != null)
            {
                var customerEntityId = entityIndex.EntityId;
                Customer entityCustomer = await createOrderBlock.findEntityPipeline.Run(new FindEntityArgument(typeof(Customer), customerEntityId, false), context) as Customer;
                if (entityCustomer != null)
                {
                    contactComponent.IsRegistered = true;
                    contactComponent.CustomerId = entityCustomer.Id;
                    contactComponent.ShopperId = entityCustomer.Id;
                    contactComponent.Name = entityCustomer.Name;
                }
            }

            contactComponent.Email = email;






            KnownOrderListsPolicy policy1 = context.GetPolicy<KnownOrderListsPolicy>();
            GlobalOrderPolicy policy2 = context.GetPolicy<GlobalOrderPolicy>();
            string orderId = string.Format("{0}{1:N}", (object)CommerceEntity.IdPrefix<Order>(), (object)Guid.NewGuid());
            Order storeOrder = new Order();

            storeOrder.Components = new List<Component>();
            storeOrder.Id = orderId;

            Totals totals = new Totals() { GrandTotal = new Money(arg.CurrencyCode, arg.GrandTotal), SubTotal = new Money(arg.CurrencyCode, arg.SubTotal), AdjustmentsTotal = new Money(arg.CurrencyCode, arg.Discount), PaymentsTotal = new Money(arg.CurrencyCode, arg.GrandTotal) };
            storeOrder.Totals = totals;
            IList<CartLineComponent> lines = new List<CartLineComponent>();

            foreach(var line in arg.Lines)
            {
                CartLineComponent lineComponent = new CartLineComponent
                {
                    ItemId = line.ItemId,
                    Quantity = line.Quantity,
                    Totals = new Totals() { SubTotal = new Money(arg.CurrencyCode, line.SubTotal), GrandTotal = new Money(arg.CurrencyCode, line.SubTotal) },
                    UnitListPrice = new Money() { CurrencyCode = arg.CurrencyCode, Amount = line.UnitListPrice },
                    Id = Guid.NewGuid().ToString("N"),
                    Policies = new List<Policy>()
                };
                lineComponent.Policies.Add(new PurchaseOptionMoneyPolicy() { PolicyId = "c24f0ed4f2f1449b8a488403b6bf368a", SellPrice = new Money() { CurrencyCode = arg.CurrencyCode, Amount = line.SubTotal } });

                lineComponent.ChildComponents.Add(new CartProductComponent() { DisplayName = line.ProductName});

                // if it has a variant
                var items = line.ItemId.Split('|');
                if (items.Count() > 2)
                {
                    // Set VariantionId
                    lineComponent.ChildComponents.Add(new Sitecore.Commerce.Plugin.Catalog.ItemVariationSelectedComponent() { VariationId = items[2] });
                }

                lines.Add(lineComponent);
            }

            storeOrder.Lines = lines;

         
           IList <AwardedAdjustment> adjustments = new List<AwardedAdjustment>();

            if (arg.Discount > 0)
            {
                var discountAdjustment = new CartLevelAwardedAdjustment
                {
                    AdjustmentType = "Discount",
                    Adjustment = new Money(arg.CurrencyCode, -arg.Discount),
                    IsTaxable = false,
                    IncludeInGrandTotal = true,
                    Name = "Discount",
                    AwardingBlock = "In Store Discount"                   

                };
                adjustments.Add(discountAdjustment);
            }

            if (arg.TaxTotal > 0)
            {
                var taxAdjustment = new CartLevelAwardedAdjustment
                {
                    AdjustmentType = "Tax",
                    Adjustment = new Money(arg.CurrencyCode, arg.TaxTotal),
                    IsTaxable = false,
                    IncludeInGrandTotal = true,
                    Name = "TaxFee",
                    AwardingBlock = "Tax:block:calculatecarttax"
                };
                adjustments.Add(taxAdjustment);
            }

            storeOrder.Adjustments = adjustments;

            // Payment
            FederatedPaymentComponent paymentComponent = new FederatedPaymentComponent(new Money(arg.CurrencyCode, arg.GrandTotal))
            {
                PaymentInstrumentType = arg.PaymentInstrumentType,
                CardType = arg.CardType,
                ExpiresMonth = arg.ExpiresMonth,
                ExpiresYear = arg.ExpiresYear,
                TransactionId = arg.TransactionId,
                TransactionStatus = arg.TransactionStatus,
                MaskedNumber = arg.MaskedNumber,
                Amount = new Money(arg.CurrencyCode, arg.GrandTotal),
                Comments = "Store Payment",
                PaymentMethod = new EntityReference("Card", new Guid().ToString()),
                BillingParty = new Party(),
                Name = "Store Payment"
            };
            storeOrder.Components.Add(paymentComponent);


            // Fulfillment
            PhysicalFulfillmentComponent physicalFulfillmentComponent = new PhysicalFulfillmentComponent
            {
                //ShippingParty = new Party() { AddressName = arg.StoreDetails.Name, Address1 = arg.StoreDetails.Address, City = arg.StoreDetails.City, State = arg.StoreDetails.State,
                //                              StateCode = arg.StoreDetails.State, Country =arg.StoreDetails.Country, CountryCode = arg.StoreDetails.Country, ZipPostalCode = arg.StoreDetails.ZipCode.ToString(), ExternalId = "0" },
                FulfillmentMethod = new EntityReference() { Name = "Offline Store Order By Customer", EntityTarget = "b146622d-dc86-48a3-b72a-05ee8ffd187a" }
            };


            var shippingParty = new FakeParty()
            {
                AddressName = arg.StoreDetails.Name,
                Address1 = arg.StoreDetails.Address,
                City = arg.StoreDetails.City,
                State = arg.StoreDetails.State,
                StateCode = arg.StoreDetails.State,
                Country = arg.StoreDetails.Country,
                CountryCode = arg.StoreDetails.Country,
                ZipPostalCode = arg.StoreDetails.ZipCode.ToString(),
                ExternalId = "0"
            };

            // Have to do the following because we cannot set external id on party directly
            var tempStorage = JsonConvert.SerializeObject(shippingParty);
            Party party = JsonConvert.DeserializeObject<Party>(tempStorage);
            physicalFulfillmentComponent.ShippingParty = party;

            storeOrder.Components.Add(physicalFulfillmentComponent);


            storeOrder.Components.Add(contactComponent);
            storeOrder.Name = "InStoreOrder";

            storeOrder.ShopName = arg.ShopName;            
            storeOrder.FriendlyId = orderId;            
            storeOrder.OrderConfirmationId = arg.OrderConfirmationId;            
            storeOrder.OrderPlacedDate = Convert.ToDateTime(arg.OrderPlacedDate);
            //string createdOrderStatus = policy2.CreatedOrderStatus;
            storeOrder.Status = arg.Status;
            Order order = storeOrder;
            string str3 = contactComponent.IsRegistered ? policy1.AuthenticatedOrders : policy1.AnonymousOrders;
            ListMembershipsComponent membershipsComponent1 = new ListMembershipsComponent()
            {
                Memberships = (IList<string>)new List<string>()
                    {
                      CommerceEntity.ListName<Order>(),
                      str3
                    }
            };


            if (contactComponent.IsRegistered && !string.IsNullOrEmpty(contactComponent.CustomerId))
                membershipsComponent1.Memberships.Add(string.Format(context.GetPolicy<KnownOrderListsPolicy>().CustomerOrders, (object)contactComponent.CustomerId));
            order.SetComponent((Component)membershipsComponent1);


            Order order2 = order;
            TransientListMembershipsComponent membershipsComponent2 = new TransientListMembershipsComponent();
            membershipsComponent2.Memberships = (IList<string>)new List<string>()
              {
                policy1.PendingOrders
              };
                     

            context.Logger.LogInformation(string.Format("Offline Orders.ImportOrder: OrderId={0}|GrandTotal={1} {2}", (object)orderId, (object)order.Totals.GrandTotal.CurrencyCode, (object)order.Totals.GrandTotal.Amount), Array.Empty<object>());
            context.CommerceContext.AddModel((Model)new CreatedOrder()
            {
                OrderId = orderId
            });


            Dictionary<string, double> dictionary = new Dictionary<string, double>()
              {
                {
                  "GrandTotal",
                  Convert.ToDouble(order.Totals.GrandTotal.Amount)
                }
              };

            createOrderBlock.telemetryClient.TrackEvent("OrderCreated", (IDictionary<string, string>)null, (IDictionary<string, double>)dictionary);
            int int32 = Convert.ToInt32(Math.Round(order.Totals.GrandTotal.Amount, 0));

            if (context.GetPolicy<PerformancePolicy>().WriteCounters)
            {
                int num1 = await createOrderBlock.performanceCounterCommand.IncrementBy("SitecoreCommerceMetrics", "MetricCount", string.Format("Orders.GrandTotal.{0}", (object)order.Totals.GrandTotal.CurrencyCode), (long)int32, context.CommerceContext) ? 1 : 0;
                int num2 = await createOrderBlock.performanceCounterCommand.Increment("SitecoreCommerceMetrics", "MetricCount", "Orders.Count", context.CommerceContext) ? 1 : 0;
            }
            return order;
        }

        

    }
}
