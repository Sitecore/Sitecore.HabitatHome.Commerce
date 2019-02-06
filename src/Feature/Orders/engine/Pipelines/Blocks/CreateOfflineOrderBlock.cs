using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Orders.Engine.Models;
using Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Orders.block.ImportStoreOrder")]
    public class CreateOfflineOrderBlock : PipelineBlock<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>
    {                                                               
        private readonly TelemetryClient _telemetryClient;
        private readonly PerformanceCounterCommand _performanceCounterCommand;
        private readonly IFindEntityPipeline _findEntityPipeline;                   
        
        public CreateOfflineOrderBlock(TelemetryClient telemetryClient, PerformanceCounterCommand performanceCounterCommand, IFindEntityPipeline findEntityPipeline)
        {                                                   
            _telemetryClient = telemetryClient;
            _performanceCounterCommand = performanceCounterCommand;
            _findEntityPipeline = findEntityPipeline;                     
        }
                                                 
        public override async Task<Order> Run(OfflineStoreOrderArgument arg, CommercePipelineExecutionContext context)
        {
            CreateOfflineOrderBlock createOrderBlock = this;            
            Condition.Requires(arg).IsNotNull($"{createOrderBlock.Name}: arg can not be null");
            
            CommercePipelineExecutionContext executionContext;
            if (string.IsNullOrEmpty(arg.Email))
            {
                executionContext = context;
                executionContext.Abort(await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error, "EmailIsRequired", new object[1], "Can not create order, email address is required.").ConfigureAwait(false), context);
                executionContext = null;
                return null;
            }

            if (!arg.Lines.Any())
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string error = context.GetPolicy<KnownResultCodes>().Error;
                string commerceTermKey = "OrderHasNoLines";                
                string defaultMessage = "Can not create order, cart  has no lines";
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, null, defaultMessage).ConfigureAwait(false), context);
                executionContext = null;
                return null;
            }            

            // Find contact using email
            string email = arg.Email;
            ContactComponent contactComponent = new ContactComponent();
            EntityIndex entityIndex = await createOrderBlock._findEntityPipeline
                .Run(new FindEntityArgument(typeof(EntityIndex), string.Format("{0}{1}\\{2}", EntityIndex.IndexPrefix<Customer>("Id"), arg.Domain, arg.Email)), context)
                .ConfigureAwait(false)
                as EntityIndex;

            if (entityIndex != null)
            {
                var customerEntityId = entityIndex.EntityId;
                Customer entityCustomer = await createOrderBlock._findEntityPipeline
                    .Run(new FindEntityArgument(typeof(Customer), customerEntityId), context)
                    .ConfigureAwait(false)
                    as Customer;

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
            string orderId = string.Format("{0}{1:N}", CommerceEntity.IdPrefix<Order>(), Guid.NewGuid());
            Order storeOrder = new Order();

            storeOrder.Components = new List<Component>();
            storeOrder.Id = orderId;

            Totals totals = new Totals() {
                GrandTotal = new Money(arg.CurrencyCode, arg.GrandTotal),
                SubTotal = new Money(arg.CurrencyCode, arg.SubTotal),
                AdjustmentsTotal = new Money(arg.CurrencyCode, arg.Discount),
                PaymentsTotal = new Money(arg.CurrencyCode, arg.GrandTotal)
            };

            storeOrder.Totals = totals;
            IList<CartLineComponent> lines = new List<CartLineComponent>();

            foreach(var line in arg.Lines)
            {
                var items = line.ItemId.Split('|');
                CommerceEntity entity = await createOrderBlock._findEntityPipeline
                    .Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-" + (items.Count() > 2 ? items[1] : line.ItemId), false), context)
                    .ConfigureAwait(false);

                CartLineComponent lineComponent = new CartLineComponent
                {
                    ItemId = line.ItemId,
                    Quantity = line.Quantity,
                    Totals = new Totals() { SubTotal = new Money(arg.CurrencyCode, line.SubTotal), GrandTotal = new Money(arg.CurrencyCode, line.SubTotal) },
                    UnitListPrice = new Money() { CurrencyCode = arg.CurrencyCode, Amount = line.UnitListPrice },
                    Id = Guid.NewGuid().ToString("N"),                    
                    //Policies = new List<Policy>() //todo: determine if this is required
                };
                lineComponent.Policies.Add(new PurchaseOptionMoneyPolicy() { PolicyId = "c24f0ed4f2f1449b8a488403b6bf368a", SellPrice = new Money() { CurrencyCode = arg.CurrencyCode, Amount = line.SubTotal } });

                if (entity is SellableItem)
                {
                    SellableItem sellableItem = entity as SellableItem;
                    lineComponent.ChildComponents.Add(new CartProductComponent()
                    {
                        DisplayName = line.ProductName,
                        Id = items.Count() > 2 ? items[1] : line.ItemId,
                        Image = new Image()
                        {
                            ImageName = line.ProductName,
                            AltText = line.ProductName,
                            Height = 50, Width = 50,
                            SitecoreId = sellableItem.GetComponent<ImagesComponent>().Images.FirstOrDefault()
                        }
                    });
                }
                // if it has a variant

                if (items.Count() > 2)
                {
                    // Set VariantionId
                    lineComponent.ChildComponents.Add(new ItemVariationSelectedComponent() { VariationId = items[2] });
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
                Memberships = new List<string>()
                    {
                      CommerceEntity.ListName<Order>(),
                      str3
                    }
            };


            if (contactComponent.IsRegistered && !string.IsNullOrEmpty(contactComponent.CustomerId))
                membershipsComponent1.Memberships.Add(string.Format(context.GetPolicy<KnownOrderListsPolicy>().CustomerOrders, contactComponent.CustomerId));
            order.SetComponent(membershipsComponent1);


            Order order2 = order;
            TransientListMembershipsComponent membershipsComponent2 = new TransientListMembershipsComponent();
            membershipsComponent2.Memberships = new List<string>()
              {
                policy1.PendingOrders
              };
                     

            context.Logger.LogInformation(string.Format("Offline Orders.ImportOrder: OrderId={0}|GrandTotal={1} {2}", orderId, order.Totals.GrandTotal.CurrencyCode, order.Totals.GrandTotal.Amount), Array.Empty<object>());
            context.CommerceContext.AddModel(new CreatedOrder()
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

            createOrderBlock._telemetryClient.TrackEvent("OrderCreated", null, dictionary);
            int orderTotal = Convert.ToInt32(Math.Round(order.Totals.GrandTotal.Amount, 0));

            if (context.GetPolicy<PerformancePolicy>().WriteCounters)
            {
                int num1 = await createOrderBlock._performanceCounterCommand.IncrementBy("SitecoreCommerceMetrics", "MetricCount", string.Format("Orders.GrandTotal.{0}", order.Totals.GrandTotal.CurrencyCode), orderTotal, context.CommerceContext).ConfigureAwait(false) ? 1 : 0;
                int num2 = await createOrderBlock._performanceCounterCommand.Increment("SitecoreCommerceMetrics", "MetricCount", "Orders.Count", context.CommerceContext).ConfigureAwait(false) ? 1 : 0;
            }
            return order;
        }

        

    }
}
