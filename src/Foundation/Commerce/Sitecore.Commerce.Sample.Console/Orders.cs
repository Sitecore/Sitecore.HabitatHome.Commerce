namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using EntityViews;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Plugin.Entitlements;
    using Plugin.GiftCards;

    using Core;
    using Core.Commands;
    using Engine;
    using Extensions;
    using Plugin.Availability;
    using Plugin.Catalog;
    using Plugin.Orders;
    using Plugin.Pricing;
    using Contexts;
    using Scenarios;
    using ServiceProxy;
    using Plugin.Fulfillment;
    using Plugin.Customers;
    using Bogus;

    public static class Orders
    {
        private static int _requestedTestRuns = 1;
        private static string _createdGiftCard;

        private static decimal _totalOrderDollars;
        private static int _totalOrders;
        
        public static void RunScenarios()
        {
            var value = Properties.Settings.Default.RequestedTestRuns;
            _requestedTestRuns = value;

            var opsUser = new DevOpAndre();

            var watch = new Stopwatch();
            watch.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine($"Beginning Order Scenario pass.  Requested Runs: {_requestedTestRuns}");
            Console.WriteLine("---------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Set the randomzier seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(3897234);

            var testParties = new Faker<Party>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.AddressName, f => "FulfillmentPartyName")
                .RuleFor(u => u.Address1, f => f.Address.StreetAddress(true))
                .RuleFor(u => u.City, f => f.Address.City())
                .RuleFor(u => u.StateCode, f => "WA")
                .RuleFor(u => u.State, f => "Washington")
                .RuleFor(u => u.ZipPostalCode, f => "93612")
                .RuleFor(u => u.CountryCode, f => "US")
                .RuleFor(u => u.Country, f => "United States")
                .FinishWith((f, u) =>
                {
                    Console.WriteLine($"BogusUser Address1={u.Address1}|City={u.City}");
                });


            for (var i = 1; i <= _requestedTestRuns; i++)
            {
                try
                {
                    ////Anonymous Customer Jeff Order Samples
                    var bogusParty = testParties.Generate();
                    var jeff = new AnonymousCustomerJeff();

                    jeff.Context.Components.OfType<ElectronicFulfillmentComponent>().First().EmailAddress = bogusParty.Email;

                    var physicalFulfillmentComponent = jeff.Context.Components.OfType<PhysicalFulfillmentComponent>().FirstOrDefault();
                    if (physicalFulfillmentComponent != null)
                    {
                        physicalFulfillmentComponent.ShippingParty = bogusParty;
                    }
                    
                    var danaHab = new RegisteredHabitatCustomerDana();
                    //// bogusUser = testUsers.Generate();
                    bogusParty = testParties.Generate();
                    danaHab.Context.Components.OfType<ElectronicFulfillmentComponent>().First().EmailAddress = bogusParty.Email;

                    physicalFulfillmentComponent = danaHab.Context.Components.OfType<PhysicalFulfillmentComponent>().FirstOrDefault();
                    if (physicalFulfillmentComponent != null)
                    {
                        physicalFulfillmentComponent.ShippingParty = bogusParty;
                    }

                    danaHab.Context.CustomerId = AddCustomer(
                        danaHab.Context.Components.OfType<ElectronicFulfillmentComponent>().First().EmailAddress,
                        danaHab.Context, bogusParty.FirstName, bogusParty.LastName);

                    // RegisteredCustomer Dana Order Samples
                    var danaAdv = new RegisteredCustomerDana();

                    bogusParty = testParties.Generate();
                    danaAdv.Context.Components.OfType<ElectronicFulfillmentComponent>().First().EmailAddress = bogusParty.Email;

                    danaAdv.Context.CustomerId = AddCustomer(
                        danaAdv.Context.Components.OfType<ElectronicFulfillmentComponent>().First().EmailAddress,
                        danaAdv.Context, bogusParty.FirstName, bogusParty.LastName);

                    var lastSimplePhysical = SimplePhysical2Items.Run(jeff.Context).Result;
                    var lastGiftCardOrder = BuyGiftCards.Run(jeff.Context).Result;
                    _createdGiftCard = jeff.Context.GiftCards.First();

                    var lastBuyWithGiftCard = BuyWithGiftCard.Run(jeff.Context).Result;

                    SimplePhysicalRtrn15Coupon.Run(jeff.Context).Wait();
                    AddRemovePayment.Run(jeff.Context).Wait();
                    AddRemoveCoupon.Run(jeff.Context).Wait();

                    UseLimitedCouponMoreThanOnce.Run(jeff.Context).Wait();

                    OnSaleItem.Run(jeff.Context).Wait();
                    BuyBackOrderedItem.Run(jeff.Context).Wait();
                    BuyPreOrderableItem.Run(jeff.Context).Wait();
                    BuyAvailabilitySplitItem.Run(jeff.Context).Wait();
                    AddRemoveCartLine.Run(jeff.Context).Wait();
                    var lastSplitShipment = BuySplitShipment.Run(jeff.Context).Result;

                    ////International
                    var katrina = new InternationalShopperKatrina();

                    var result = Proxy.GetValue(katrina.Context.ShopsContainer().SellableItems.ByKey("Adventure Works Catalog,AW055 01,33").Expand("Components($expand=ChildComponents($expand=ChildComponents($expand=ChildComponents)))"));
                    result.Should().NotBeNull();
                    result.ListPrice.CurrencyCode.Should().Be("EUR");
                    result.ListPrice.Amount.Should().Be(1.0M);

                    ////These samples leverage EntityViews and EntityActions
                    ////For post order management
                    ////Place a Split shipment order and then put it on hold
                    SplitShipmentOnHold.Run(jeff.Context).Wait();
                    ////Place a Split shipment order, place it on hold and delete one of the lines
                    SplitShipmentThenDeleteLine.Run(jeff.Context).Wait();

                    ////Retrieve the Business User "View" for the SplitShipment end order.  The Order should still be in a Pending State
                    Proxy.GetValue(jeff.Context.ShopsContainer().GetEntityView(lastSplitShipment, "Master", string.Empty, string.Empty));

                    var cancelOrderId = Buy3Items.Run(jeff.Context).Result;
                    if (!string.IsNullOrEmpty(cancelOrderId))
                    {
                        CancelOrder(cancelOrderId);
                    }

                    ////RegisteredCustomer Dana Order Samples
                    SimplePhysical2Items.Run(danaAdv.Context).Wait();
                    BuyGiftCards.Run(danaAdv.Context).Wait();
                    BuyWarranty.Run(danaHab.Context, 1).Wait();

                    danaHab.GoShopping();

                    ////List Counts before the Minions run
                    WriteListCounts(opsUser.Context);

                    ////Force the pending orders Minion to run
                    RunPendingOrdersMinion(opsUser.Context);
                    WriteListCounts(opsUser.Context);

                    ////Force the settle Sales Activities Minion to run
                    RunSettleSalesActivitiesMinion(opsUser.Context);
                    WriteListCounts(opsUser.Context);

                    ////Force the Released Orders Minion to run
                    RunReleasedOrdersMinion(opsUser.Context);
                    WriteListCounts(opsUser.Context);

                    ////The Following represent examples of using the Business User (BizOps) Api to handle Orders.
                    ////At this stage, the orders have been released and only actions allowed on Released or Problem orders can occur

                    ////Get the last SimplePhysical order to show how it is changed by the Minions
                    var orderMaster = Proxy.GetEntityView(jeff.Context.ShopsContainer(), lastSimplePhysical, "Master", string.Empty, string.Empty);

                    if (orderMaster.ChildViews.Count(p => p.Name == "Shipments") == 0)
                    {
                        ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"LastSimplePhysical.MissingShipments: OrderId={lastSimplePhysical}");
                    }

                    ////Get the last SimplePhysical order to show how it is changed by the Minions
                    var lastGiftCardMaster = Proxy.GetEntityView(jeff.Context.ShopsContainer(), lastGiftCardOrder, "Master", string.Empty, string.Empty);

                    ////There should not be a Shipments child EntityView because this was a Digital order
                    lastGiftCardMaster.ChildViews.Count(p => p.Name == "Shipments").Should().Be(0);
                    
                    var giftCardNew = jeff.Context.ShopsContainer().Entitlements.ByKey("Entity-GiftCard-" + _createdGiftCard).Expand("Components($expand=ChildComponents)").GetValue() as GiftCard;
                    giftCardNew.Should().NotBeNull();

                    var lastBuyWithGiftCardView = Proxy.GetEntityView(jeff.Context.ShopsContainer(), lastBuyWithGiftCard, "Master", string.Empty, string.Empty);
                    var salesActivities = lastBuyWithGiftCardView.ChildViews.First(p => p.Name == "SalesActivities") as EntityView;
                    salesActivities.ChildViews.Count.Should().Be(2);

                    //var lastWarrantyMaster = Proxy.GetEntityView(dana.Context.ShopsContainer(), lastWarrantyId, "Master", string.Empty, string.Empty);
                    //// Should have one entry for the Warranty in the Order Entitlements ChildView
                    //lastWarrantyMaster.ChildViews.First(p => p.Name == "OrderEntitlements").As<EntityView>().ChildViews.Count().Should().Be(1);

                    // Example of updating the Order Status from via a service.  This is for external systems to push status updates
                    SetOrderStatus();
                }
                catch (Exception ex)
                {
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Test Exception - {ex.Message}");
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Test Exception - {ex.StackTrace}");
                }
                
                decimal elapsedMinutes = 0;
                if (watch.ElapsedMilliseconds > 60000)
                {
                    elapsedMinutes = (watch.ElapsedMilliseconds / 1000) / 60;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($"Test pass {i} of {_requestedTestRuns} Complete - {watch.ElapsedMilliseconds} ms ({elapsedMinutes} Min)");
                Console.WriteLine($"Orders:{_totalOrders} Dollars:{_totalOrderDollars}");
                Console.ForegroundColor = ConsoleColor.Cyan;   
            }

            watch.Stop();

            Console.WriteLine($"End Orders :{watch.ElapsedMilliseconds} ms");
        }

        #region Helpers
        public static string AddCustomer(string email = "", ShopperContext context = null, string firstName = "FirstName", string lastName = "LastName")
        {
            Console.WriteLine("Begin CustomerDetails for Add View");

            var container = context.ShopsContainer();

            var view = Proxy.GetValue(container.GetEntityView(string.Empty, "Details", "AddCustomer", string.Empty));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            view.Action.Should().Be("AddCustomer");
            view.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Email")).Value = email;
            view.Properties.FirstOrDefault(p => p.Name.Equals("Password")).Value = "Password01";

            view.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = firstName;
            view.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = lastName;

            var action = Proxy.DoCommand(container.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties.FirstOrDefault(p => p.Name.Equals("AccountStatus")).Value = "ActiveAccount";

            action = Proxy.DoCommand(container.DoAction(view));
            if (action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Orders.AddCustomer.Error returned");
            }


            action.Models.OfType<CustomerAdded>().FirstOrDefault().Should().NotBeNull();

            return action.Models.OfType<CustomerAdded>().FirstOrDefault()?.CustomerId;
        }

        public static Order GetOrder(Container container, string orderId)
        {
            try
            {
                var order = Proxy.GetValue(
                    container.Orders.ByKey(orderId).Expand("Lines($expand=CartLineComponents),Components"));
                
                return order;
            }
            catch (DataServiceQueryException ex)
            {
                Console.WriteLine($"Exception Retrieving Order: {ex} OrderId:{orderId}");
                return null;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"Exception Retrieving Order: {ex} OrderId:{orderId}");
                return null;
            }
        }

        public static CommerceCommand CreateOrder(Container container, string cartId, ShopperContext context)
        {
            var resolveEmail = "no@email.com";
            var electronicFulfillmentComponent = context.Components.OfType<ElectronicFulfillmentComponent>().FirstOrDefault();
            if (electronicFulfillmentComponent != null)
            {
                resolveEmail = electronicFulfillmentComponent.EmailAddress;
            }

            var command = Proxy.DoCommand(
                container.CreateOrder(cartId, resolveEmail));
            if (command.ResponseCode != "Ok")
            {
                Console.WriteLine($"Create Order Failed:{command.Messages.FirstOrDefault(m => m.Code.Equals("Error"))?.Text}");
            }
            else
            {
                var totals = command.Models.OfType<Plugin.Carts.Totals>().FirstOrDefault();

                if (totals == null)
                {
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, "CreateOrder - NoTotals");
                }
                else
                {
                    _totalOrderDollars = _totalOrderDollars + totals.GrandTotal.Amount;
                    _totalOrders += 1;
                }

            }

            return command;
        }

        public static Order CreateAndValidateOrder(Container container, string cartId, ShopperContext context)
        {
            var commandResponse = CreateOrder(container, cartId, context);

            if (commandResponse.ResponseCode != "Ok")
            {
                throw new Exception($"Order Creation Failed: {commandResponse.Messages.First().Text}");
            }

            var order = GetOrder(container, commandResponse.Models.OfType<CreatedOrder>().First().OrderId);           

            ValidateOrder(order);

            return order;
        }

        public static Order ValidateOrder(Order order)
        {
            order.Id.Should().NotBeNull();
            order.OrderConfirmationId.Should().NotBeNull();
            order.Lines.Should().NotBeEmpty();
            order.Status.Should().Be("Pending");

            order.Totals.PaymentsTotal.Amount.Should().Be(order.Totals.GrandTotal.Amount);

            foreach (var line in order.Lines)
            {
                line.ItemId.Should().NotBeNullOrEmpty();
                line.Quantity.Should().NotBe(0);

                line.Totals.GrandTotal.Amount.Should().BeGreaterThan(0);

                var cartProductComponent = line.CartLineComponents.OfType<Plugin.Carts.CartProductComponent>().FirstOrDefault();
                cartProductComponent.Should().NotBeNull();

                var itemAvailabilityComponent = line.CartLineComponents.OfType<ItemAvailabilityComponent>().FirstOrDefault();
                if (cartProductComponent.Policies.OfType<AvailabilityAlwaysPolicy>().Any())
                {
                    // This item is always available and will have no ItemAvailabilityComponent
                    itemAvailabilityComponent.Should().BeNull($"ItemAvailabilityComponent Should be null");
                }
                else
                {
                    itemAvailabilityComponent.Should().NotBeNull();
                    if (itemAvailabilityComponent.ItemId != line.ItemId)
                    {
                        ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"ItemAvailability.ItemId does not match: Expected:{line.ItemId} Received:{itemAvailabilityComponent.ItemId}");
                    }
                }

                line.Policies.OfType<PurchaseOptionMoneyPolicy>().Should().HaveCount(1);

                var purchaseOptionMoneyPolicy = line.Policies.OfType<PurchaseOptionMoneyPolicy>().First();

                purchaseOptionMoneyPolicy.SellPrice.Amount.Should().BeGreaterThan(0);
            }

            return order;
        }

        public static void RunPendingOrdersMinion(ShopperContext context)
        {
            Console.WriteLine("Begin RunPendingOrdersMinion");

            var result = Proxy.GetValue(new MinionRunner().Context.MinionsContainer()
                .RunMinion("Sitecore.Commerce.Plugin.Orders.PendingOrdersMinionBoss, Sitecore.Commerce.Plugin.Orders", ResolveMinionEnvironment(context), null));
            if (result.ResponseCode.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"RunPendingOrdersMinion: {result.Messages.FirstOrDefault(m => m.Code.Equals("Error", StringComparison.Ordinal))?.Text}");
            }
        }

        public static void RunReleasedOrdersMinion(ShopperContext context)
        {
            Console.WriteLine("Begin RunReleasedOrdersMinion");

            var policies =
                new Collection<CommerceOps.Sitecore.Commerce.Core.Policy>
                    {
                        new CommerceOps.Sitecore.Commerce.Core.RunMinionPolicy { RunChildren = false }
                    };
            var result = Proxy.GetValue(
                new MinionRunner().Context.MinionsContainer()
                .RunMinion("Sitecore.Commerce.Plugin.Orders.ReleasedOrdersMinion, Sitecore.Commerce.Plugin.Orders", ResolveMinionEnvironment(context), policies));

            if (result.ResponseCode.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"RunReleasedOrdersMinion: {result.Messages.FirstOrDefault(m => m.Code.Equals("Error", StringComparison.Ordinal))?.Text}");
            }

            result = Proxy.GetValue(
                new MinionRunner().Context.MinionsContainer()
                    .RunMinion("Sitecore.Commerce.Plugin.Orders.ReleasedOrdersMinion, Sitecore.Commerce.Plugin.Orders", "HabitatMinions", policies));

            if (result.ResponseCode.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"RunReleasedOrdersMinion: {result.Messages.FirstOrDefault(m => m.Code.Equals("Error", StringComparison.Ordinal))?.Text}");
            }
        }

        public static void RunSettleSalesActivitiesMinion(ShopperContext context)
        {
            Console.WriteLine("Begin RunSettleSalesActivitiesMinion");

            var policies =
                new Collection<CommerceOps.Sitecore.Commerce.Core.Policy>
                    {
                        new CommerceOps.Sitecore.Commerce.Core.RunMinionPolicy { RunChildren = false }
                    };
            var result = Proxy.GetValue(
                new MinionRunner().Context.MinionsContainer()
                .RunMinion("Sitecore.Commerce.Plugin.Orders.SettleSalesActivitiesMinion, Sitecore.Commerce.Plugin.Orders", ResolveMinionEnvironment(context), policies));

            if (result.ResponseCode.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"RunSettleSalesActivitiesMinion: {result.Messages.FirstOrDefault(m => m.Code.Equals("Error", StringComparison.Ordinal))?.Text}");
            }

            result = Proxy.GetValue(
                new MinionRunner().Context.MinionsContainer()
                    .RunMinion("Sitecore.Commerce.Plugin.Orders.SettleSalesActivitiesMinion, Sitecore.Commerce.Plugin.Orders", "HabitatMinions", policies));

            if (result.ResponseCode.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"RunSettleSalesActivitiesMinion: {result.Messages.FirstOrDefault(m => m.Code.Equals("Error", StringComparison.Ordinal))?.Text}");
            }
        }

        #endregion

        private static void CancelOrder(string orderId)
        {
            Console.WriteLine($"Cancel order: '{orderId}'");

            var result = Proxy.DoCommand(new AnonymousCustomerJeff().Context.ShopsContainer().CancelOrder(orderId));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void SetOrderStatus()
        {
            Console.WriteLine("Set order status");

            var container = new AnonymousCustomerJeff();

            var orderId = Buy3Items.Run(container.Context).Result;
            var result = Proxy.DoCommand(new AnonymousCustomerJeff().Context.ShopsContainer().SetOrderStatus(orderId, "CustomStatus"));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var order = GetOrder(container.Context.ShopsContainer(), orderId);
            order.Should().NotBeNull();
            order.Status.Should().Be("CustomStatus");
        }

        private static void WriteListCounts(ShopperContext shopperContext)
        {
            var backupColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            var jeff = shopperContext;

            Console.WriteLine("---------------------------------------------------");
            var ordersMetadata = jeff.ShopsContainer().GetListMetadata("Orders").GetValue();
            Console.WriteLine($"List:{ordersMetadata.ListName} Count:{ordersMetadata.Count}");

            var pendingOrdersMetadata = jeff.ShopsContainer().GetListMetadata("PendingOrders").GetValue();
            Console.WriteLine($"List:{pendingOrdersMetadata.ListName} Count:{pendingOrdersMetadata.Count}");

            var pendingOrders1Metadata = jeff.ShopsContainer().GetListMetadata("PendingOrders.1").GetValue();
            Console.WriteLine($"List:{pendingOrders1Metadata.ListName} Count:{pendingOrders1Metadata.Count}");

            var pendingOrders2Metadata = jeff.ShopsContainer().GetListMetadata("PendingOrders.2").GetValue();
            Console.WriteLine($"List:{pendingOrders2Metadata.ListName} Count:{pendingOrders2Metadata.Count}");

            var problemOrdersMetadata = jeff.ShopsContainer().GetListMetadata("ProblemOrders").GetValue();
            Console.WriteLine($"List:{problemOrdersMetadata.ListName} Count:{problemOrdersMetadata.Count}");

            var onHoldOrdersMetadata = jeff.ShopsContainer().GetListMetadata("OnHoldOrders").GetValue();
            Console.WriteLine($"List:{onHoldOrdersMetadata.ListName} Count:{onHoldOrdersMetadata.Count}");

            var releasedOrdersMetadata = jeff.ShopsContainer().GetListMetadata("ReleasedOrders").GetValue();
            Console.WriteLine($"List:{releasedOrdersMetadata.ListName} Count:{releasedOrdersMetadata.Count}");

            var completedOrdersMetadata = jeff.ShopsContainer().GetListMetadata("CompletedOrders").GetValue();
            Console.WriteLine($"List:{completedOrdersMetadata.ListName} Count:{completedOrdersMetadata.Count}");

            var waitingForAvailability = jeff.ShopsContainer().GetListMetadata("WaitingForAvailability").GetValue();
            Console.WriteLine($"List:{waitingForAvailability.ListName} Count:{waitingForAvailability.Count}");

            var settleSalesActivities = jeff.ShopsContainer().GetListMetadata("SettleSalesActivities").GetValue();
            Console.WriteLine($"List:{settleSalesActivities.ListName} Count:{settleSalesActivities.Count}");

            var settledSalesActivities = jeff.ShopsContainer().GetListMetadata("SettledSalesActivities").GetValue();
            Console.WriteLine($"List:{settledSalesActivities.ListName} Count:{settledSalesActivities.Count}");

            var problemSalesActivities = jeff.ShopsContainer().GetListMetadata("ProblemSalesActivities").GetValue();
            Console.WriteLine($"List:{problemSalesActivities.ListName} Count:{problemSalesActivities.Count}");

            Console.WriteLine("---------------------------------------------------");

            Console.ForegroundColor = backupColor;
        }

        private static string ResolveMinionEnvironment(ShopperContext context)
        {
            switch (context.Environment)
            {
                case EnvironmentConstants.AdventureWorksShops:
                    return EnvironmentConstants.AdventureWorksMinions;
                case EnvironmentConstants.HabitatShops:
                    return EnvironmentConstants.HabitatMinions;
            }

            throw new InvalidOperationException($"{context.Environment} is not a valid environment.");
        }
    }
}
