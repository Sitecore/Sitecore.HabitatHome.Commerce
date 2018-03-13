namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Contexts;
    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.ServiceProxy;

    public static class OrdersUX
    {
        private static string _orderId;

        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin OrdersUX");

            try
            {
                // Run CredtCard scenarios
                var jeff = new AnonymousCustomerJeff();
                _orderId = Scenarios.Simple2PhysicalDigitalItems.Run(jeff.Context).Result;
                _orderId.Should().NotBeNull();
                HoldOrder(_orderId);

                OrderFulfillments();

                SetFulfillment_Digital();
                SetFulfillment_ShipToMe();
                SetFulfillment_Split();

                OrderPayments();
                VoidPayment_Federated();
                AddPayment_Federated(64.23M);

                UndoOnHoldOrder();

                OrderEntitlements(jeff.Context);

                OnHoldScenarios();
                // Run Federated Scenarios 
                var bob = new AnonymousCustomerBob();
                _orderId = Scenarios.SimplePhysicalDigitalItemsFederatedPayment.Run(bob.Context).Result;
                _orderId.Should().NotBeNull();
                HoldOrder(_orderId);

                OrderFulfillments();

                SetFulfillment_Digital();
                SetFulfillment_ShipToMe();
                SetFulfillment_Split();

                OrderPayments();
                VoidPayment_Federated();
                AddPayment_Federated(64.23M);

                RefundPayment_Federated();

                _orderId = Scenarios.SimplePhysical2Items.Run(jeff.Context).Result;
                _orderId.Should().NotBeNull();
                HoldOrder(_orderId);

                AddPayment_GiftCard();

                UndoOnHoldOrder();
            }
            catch (Exception ex)
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Exception in Scenario 'OrdersUX' (${ex.Message}) : Stack={ex.StackTrace}");
            }

            watch.Stop();

            Console.WriteLine($"End PromotionsUX :{watch.ElapsedMilliseconds} ms");
        }
        
        public static string HoldOrder(string orderId)
        {
            Console.WriteLine($"Hold order: '{orderId}'");

            var jeff = new AnonymousCustomerJeff();

            var view = Proxy.GetEntityView(jeff.Context.ShopsContainer(), orderId, "Details", "HoldOrder", string.Empty);
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            var result = jeff.Context.ShopsContainer().DoAction(view).GetValue();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            
            var temporaryCartId = result.Models.OfType<TemporaryCartCreated>().FirstOrDefault()?.TemporaryCartId;
            temporaryCartId.Should().NotBeNullOrEmpty();
            
            var temporaryCart = Carts.GetCart(temporaryCartId);
            temporaryCart.Should().NotBeNull();

            return temporaryCartId;
        }

        public static void CommitOnHoldOrder(string orderId)
        {
            Console.WriteLine($"Commit On-Hold Order: '{orderId}'");

            var jeff = new AnonymousCustomerJeff();

            var view = Proxy.GetEntityView(jeff.Context.ShopsContainer(), orderId, "Details", "CommitOnHoldOrder", string.Empty);
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            var result = jeff.Context.ShopsContainer().DoAction(view).GetValue();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void OnHoldScenarios()
        {
            var jeff = new AnonymousCustomerJeff();
            var container = jeff.Context.ShopsContainer();

            var orderId = Scenarios.SimplePhysical2Items.Run(jeff.Context).Result;
            orderId.Should().NotBeNull();

            // hold order
            HoldOrder(orderId);

            // Get an Order Master view
            var orderMaster = Proxy.GetEntityView(container, orderId, "Master", string.Empty, string.Empty);

            // Adjustments
            var adjustments = orderMaster.ChildViews.FirstOrDefault(p => p.Name == "Adjustments") as EntityView;
            adjustments.Should().NotBeNull();
            adjustments.ChildViews.Count.Should().NotBe(0);
            var adjustment = adjustments.ChildViews[0] as EntityView;
            adjustment.DisplayName.Should().Be("Adjustment");
            adjustment.Properties.FirstOrDefault(p => p.Name == "Adjustment")?.Value.Should().NotBeEmpty();
            adjustment.Properties.FirstOrDefault(p => p.Name == "Type")?.Value.Should().NotBeEmpty();

            // Messages
            var messages = orderMaster.ChildViews.FirstOrDefault(p => p.Name == "Messages") as EntityView;
            messages.Should().NotBeNull();
            messages.ChildViews.Count.Should().NotBe(0);
            var message = messages.ChildViews[0] as EntityView;
            message.DisplayName.Should().Be("Message");
            message.Properties.FirstOrDefault(p => p.Name == "Code")?.Value.Should().NotBeEmpty();
            message.Properties.FirstOrDefault(p => p.Name == "Text")?.Value.Should().NotBeEmpty();

            // Edit line
            var lines = orderMaster.ChildViews.FirstOrDefault(p => p.Name == "Lines") as EntityView;
            lines?.Policies.OfType<ActionsPolicy>().First().Actions.First(p => p.Name == "AddLineItem").IsEnabled.Should().Be(true);
            var line = lines?.ChildViews.First() as EntityView;
            var itemId = line?.Properties.FirstOrDefault(p => p.Name == "ItemId")?.Value;
            var editLineItemRequest = Proxy.GetEntityView(container, orderId, "EditLineItem", "EditLineItem", itemId);
            editLineItemRequest.Properties.First(p => p.Name == "Quantity").Value = "2";
            var actionResult = Proxy.GetValue(container.DoAction(editLineItemRequest));
            actionResult.ResponseCode.Should().Be("Ok");
            var totals = actionResult.Models.OfType<Sitecore.Commerce.Plugin.Carts.Totals>().FirstOrDefault();
            totals.Should().NotBeNull();

            // add payment
            AddPayment_Federated(totals.GrandTotal.Amount, orderId);

            // Commit the OnHold Order
            CommitOnHoldOrder(orderId);
           
            var order = Orders.GetOrder(container, orderId);
            order.Status.Should().Be("Pending");

            orderId = Scenarios.SimplePhysical2Items.Run(jeff.Context).Result;
            orderId.Should().NotBeNull();

            // hold order
            HoldOrder(orderId);

            order = Proxy.GetValue(container.Orders.ByKey(orderId).Expand("Components"));
            totals = order.Totals;
          
            // Void payment
            VoidPayment_Federated(orderId);

            // add payment           
            AddPayment_Federated(totals.GrandTotal.Amount, orderId);

            // Commit the OnHold Order
            CommitOnHoldOrder(orderId);

            order = Orders.GetOrder(container, orderId);
            order.Status.Should().Be("Pending");
        }

        private static void OrderFulfillments()
        {
            Console.WriteLine("Begin OrderFulfillments View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_orderId, "OrderFulfillments", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Policies.Should().BeEmpty();
                childView.Properties.Should().NotBeEmpty();
                childView.ChildViews.Should().BeEmpty();
            }
        }

        private static void SetFulfillment_Digital()
        {
            Console.WriteLine("Begin SetFulfillmentUX_Digital");
            
            var orderId = Scenarios.BuyGiftCards.Run(new AnonymousCustomerJeff().Context).Result;
            orderId.Should().NotBeNull();
            HoldOrder(orderId);

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(orderId, "OrderFulfillmentDetails", "SelectFulfillmentOption", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "Digital";

            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view?.ChildViews.Should().BeEmpty();
            view?.Action.Should().Be("SetFulfillment");
            view.Properties.FirstOrDefault(p => p.Name.Equals("EmailAddress")).Value = "jane@doe.com";
            view.Properties.FirstOrDefault(p => p.Name.Equals("EmailContent")).Value = "this is the content of my email.";

            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void SetFulfillment_ShipToMe()
        {
            Console.WriteLine("Begin SetFulfillmentUX_ShipToMe");

            var orderId = Scenarios.Buy3Items.Run(new AnonymousCustomerJeff().Context).Result;
            orderId.Should().NotBeNull();
            HoldOrder(orderId);

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(orderId, "OrderFulfillmentDetails", "SelectFulfillmentOption", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "ShipToMe";

            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Country")).Value = "CA";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("AddressName")).Value = "Home";
            view.Properties.FirstOrDefault(p => p.Name.Equals("State")).Value = "ON";          
            view.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = "first name";
            view.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = "last name";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address1")).Value = "123 street";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address2")).Value = string.Empty;
            view.Properties.FirstOrDefault(p => p.Name.Equals("City")).Value = "city";
            view.Properties.FirstOrDefault(p => p.Name.Equals("ZipPostalCode")).Value = "postalCode";
            view.Properties.FirstOrDefault(p => p.Name.Equals("PhoneNumber")).Value = "phoneNumber";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("FulfillmentOptionName")).Value = "B146622D-DC86-48A3-B72A-05EE8FFD187A";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void SetFulfillment_Split()
        {
            Console.WriteLine("Begin SetFulfillmentUX_Split");
            
            //// GET FULFILLMENT OPTIONS FOR THE ORDER
            var orderView = Proxy.GetValue(ShopsContainer.GetEntityView(_orderId, "OrderFulfillmentDetails", "SelectFulfillmentOption", string.Empty));
            orderView.Should().NotBeNull();
            orderView.Policies.Should().BeEmpty();
            orderView.Properties.Should().NotBeEmpty();
            orderView.ChildViews.Should().BeEmpty();
            orderView.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "SplitShipping";
            var orderAction = Proxy.DoCommand(ShopsContainer.DoAction(orderView));
            orderAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            //// SELECTING DIGITAL AS FULFILLMENT OPTION FOR LINE 1
            var line1View = orderAction.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(orderView.Name));
            line1View.Should().NotBeNull();
            line1View?.Policies.Should().BeEmpty();
            line1View?.Properties.Should().NotBeEmpty();
            line1View?.ChildViews.Should().BeEmpty();
            line1View.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "Digital";
            var line1Action = Proxy.DoCommand(ShopsContainer.DoAction(line1View));
            line1Action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            //// SETTING DIGITAL PROPERTIES FOR LINE 1 AND SAVING
            line1View = line1Action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(line1View.Name));
            line1View.Should().NotBeNull();
            line1View?.Policies.Should().BeEmpty();
            line1View?.Properties.Should().NotBeEmpty();
            line1View?.ChildViews.Should().BeEmpty();
            line1View?.Action.Should().Be("SetFulfillment");
            line1View.Properties.FirstOrDefault(p => p.Name.Equals("EmailAddress")).Value = "jane@doe.com";
            line1View.Properties.FirstOrDefault(p => p.Name.Equals("EmailContent")).Value = "this is the content of my email.";
            line1Action = Proxy.DoCommand(ShopsContainer.DoAction(line1View));
            line1Action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            //// SELECTING SHIPTOME AS FULFILLMENT OPTION FOR LINE 2
            var line2View = line1Action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(line1View.Name));
            line2View.Should().NotBeNull();
            line2View?.Policies.Should().BeEmpty();
            line2View?.Properties.Should().NotBeEmpty();
            line2View?.ChildViews.Should().BeEmpty();
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "ShipToMe";
            var line2Action = Proxy.DoCommand(ShopsContainer.DoAction(line2View));
            line2Action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            //// SELECTING COUNTRY FOR FULFILLMENT ADDRESS FOR LINE 2
            line2View = line2Action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(line2View.Name));
            line2View.Should().NotBeNull();
            line2View?.Policies.Should().BeEmpty();
            line2View?.Properties.Should().NotBeEmpty();
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("Country")).Value = "CA";
            line2Action = Proxy.DoCommand(ShopsContainer.DoAction(line2View));
            line2Action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            //// SELECTING STATE AND THE REST OF THE FULFILLMENT ADDRESS INFORMATION FOR LINE 2
            line2View = line2Action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(line2View.Name));
            line2View.Should().NotBeNull();
            line2View?.Policies.Should().BeEmpty();
            line2View?.Properties.Should().NotBeEmpty();
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("AddressName")).Value = "Home";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("State")).Value = "ON";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = "first name";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = "last name";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("Address1")).Value = "123 street";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("Address2")).Value = string.Empty;
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("City")).Value = "city";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("ZipPostalCode")).Value = "postalCode";
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("PhoneNumber")).Value = "phoneNumber";
            line2Action = Proxy.DoCommand(ShopsContainer.DoAction(line2View));
            line2Action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            //// SELECTING GROUND FULFILLMENT METHOD FOR LINE 2 AND SAVING
            line2View = line2Action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(line2View.Name));
            line2View.Should().NotBeNull();
            line2View?.Policies.Should().BeEmpty();
            line2View?.Properties.Should().NotBeEmpty();
            line2View.Properties.FirstOrDefault(p => p.Name.Equals("FulfillmentOptionName")).Value = "B146622D-DC86-48A3-B72A-05EE8FFD187A";
            line2Action = Proxy.DoCommand(ShopsContainer.DoAction(line2View));
            line2Action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void OrderPayments()
        {
            Console.WriteLine("Begin OrderPayments View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_orderId, "OrderPayments", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Policies.Should().BeEmpty();
                childView.Properties.Should().NotBeEmpty();
                childView.ChildViews.Should().BeEmpty();
            }
        }
        
        private static void VoidPayment_Federated(string orderId = "")
        {
            Console.WriteLine("Begin DeleteFederatedPaymentUX");

            if (string.IsNullOrEmpty(orderId))
            {
                orderId = _orderId;
            }

            var container = new AnonymousCustomerBob().Context.ShopsContainer();
            var order = Orders.GetOrder(container, orderId);
            var paymentId = order.Components.OfType<Sitecore.Commerce.Plugin.Payments.FederatedPaymentComponent>().FirstOrDefault()?.Id;

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(orderId, "OrderPayments", "VoidPayment", paymentId));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            // Get the OrderPayments View and validate that the payment is now gone
            var paymentsView = Proxy.GetValue(ShopsContainer.GetEntityView(orderId, "OrderPayments", string.Empty, string.Empty));            
            paymentsView.ChildViews.Count().Should().Be(0);
        }

        private static void RefundPayment_Federated()
        {
            Console.WriteLine("Begin RefundFederatedPaymentUX");

            var context = new AnonymousCustomerBob().Context;
            var container = context.ShopsContainer();
            var orderid = Scenarios.SimplePhysical2Items.Run(new AnonymousCustomerBob().Context).Result;
            var order = Orders.GetOrder(container, orderid);
            var paymentId = order.Components.OfType<Sitecore.Commerce.Plugin.Payments.FederatedPaymentComponent>().FirstOrDefault()?.Id;

            Orders.RunPendingOrdersMinion(context);
            Orders.RunReleasedOrdersMinion(context);
            Orders.RunSettleSalesActivitiesMinion(context);
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(orderid, "RefundPayment", "RefundPayment", paymentId));

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));

            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var updatedOrderView = Proxy.GetValue(ShopsContainer.GetEntityView(orderid, "Master", string.Empty, string.Empty));

            var salesActivities = updatedOrderView.ChildViews.First(p => p.Name == "SalesActivities") as EntityView;

            //There should be 2 Sales Activities now.  The original one and the one for the refund.
            salesActivities.ChildViews.Count.Should().Be(2);

            // Get the OrderPayments View and validate that the payment is now gone
            var paymentsView = Proxy.GetValue(ShopsContainer.GetEntityView(orderid, "OrderPayments", string.Empty, string.Empty));
            paymentsView.ChildViews.Count().Should().Be(0);
        }

        private static void AddPayment_Federated(decimal amount, string orderId = "")
        {
            Console.WriteLine("Begin AddPaymentUX_Federated");

            if (string.IsNullOrEmpty(orderId))
            {
                orderId = _orderId;
            }

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(orderId, "OrderPaymentDetails", "SelectPaymentOption", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "Federated";
            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("ClientToken")).Value.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("PaymentMethodNonce")).Value = "fake-valid-nonce";           
            view.Properties.FirstOrDefault(p => p.Name.Equals("Amount")).Value = amount.ToString();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Country")).Value = "CA";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("AddressName")).Value = "Home";
            view.Properties.FirstOrDefault(p => p.Name.Equals("State")).Value = "ON";
            view.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = "first name";
            view.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = "last name";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address1")).Value = "123 street";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address2")).Value = string.Empty;
            view.Properties.FirstOrDefault(p => p.Name.Equals("City")).Value = "city";
            view.Properties.FirstOrDefault(p => p.Name.Equals("ZipPostalCode")).Value = "12345";
            view.Properties.FirstOrDefault(p => p.Name.Equals("PhoneNumber")).Value = "phoneNumber";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void AddPayment_GiftCard()
        {
            Console.WriteLine("Begin AddPaymentUX_GiftCard");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_orderId, "OrderPaymentDetails", "SelectPaymentOption", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Option")).Value = "GiftCard";
            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Amount")).Value = "64.23";
            view.Properties.FirstOrDefault(p => p.Name.Equals("GiftCardCode")).Value = "GC1000000";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void UndoOnHoldOrder()
        {
            Console.WriteLine("Begin UndoOnHoldOrderUX");

            var view = Proxy.GetEntityView(ShopsContainer, _orderId, "Details", "UndoOnHoldOrder", string.Empty);
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
        
        private static void OrderEntitlements(ShopperContext context)
        {
            Console.WriteLine("Begin OrderEntitlements View");

            var orderId = Scenarios.Simple2PhysicalDigitalItems.Run(context).Result;
            orderId.Should().NotBeNull();

            Orders.RunPendingOrdersMinion(context);
            Orders.RunSettleSalesActivitiesMinion(context);
            Orders.RunReleasedOrdersMinion(context);

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(orderId, "OrderEntitlements", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
            foreach (var childView in result.ChildViews.Cast<EntityView>())
            {
                childView.Should().NotBeNull();
                childView.Policies.Should().BeEmpty();
                childView.Properties.Should().NotBeEmpty();
                childView.Properties.All(p => !string.IsNullOrEmpty(p.Value)).Should().BeTrue();
                childView.ChildViews.Should().BeEmpty();
            }
        }
    }
}
