namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Extensions;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Returns;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Returns
    {
        private static ShopperContext _context;

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Returns");

            var steve = new AnonymousCustomerSteve();
            _context = steve.Context;

            var order = CreateCompletedOrder();
            var lineId = order.Lines.FirstOrDefault()?.Id;
            RequestRmaLineValidation(order.Id, "invalidlineid", 1); // invalid line id
            RequestRmaLineValidation(order.Id, lineId, -1); // returning -1 out of 1 available
            RequestRmaLineValidation(order.Id, lineId, 0); // returning 0 out of 1 available
            var rmaFriendlyId = RequestRma(order.Id, lineId, 1);
            RequestRmaLineValidation(order.Id, lineId, 3); // returning 3 out of 2 available

            ReturnedItemReceivedValidation("invalidrmaid"); // invalid rma id
            ReturnedItemReceived(_context.ShopsContainer(), rmaFriendlyId, order.Id, lineId);
            ReturnedItemReceivedValidation(rmaFriendlyId); // rma invalid status

            WriteListCounts(_context.ShopsContainer());
            RunRefundRmasMinion(_context.ShopsContainer(), rmaFriendlyId, "HabitatMinions");
            WriteListCounts(_context.ShopsContainer());

            rmaFriendlyId = RequestRma(order.Id, lineId, 2);

            RequestRmaLineValidation(order.Id, order.Lines.FirstOrDefault()?.Id, 1); // returning 1 out of 0 available
            ReturnedItemReceived(_context.ShopsContainer(), rmaFriendlyId, order.Id, lineId);

            RequestDigitalRma();

            watch.Stop();

            Console.WriteLine($"End Returns:{watch.ElapsedMilliseconds} ms");
        }

        private static void RequestRmaLineValidation(string orderId, string lineId, decimal quantity)
        {
            Console.WriteLine("Begin RequestRmaLineValidation");
            
            var result =
                Proxy.DoCommand(
                    _context.ShopsContainer().RequestRma(
                        orderId,
                        "ConsoleWrongItem",
                        new List<RmaLineComponent>
                            {
                                new RmaLineComponent
                                {
                                    LineId = lineId,
                                    Quantity = quantity
                                }
                }));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            result.Models.OfType<RmaAdded>().Any().Should().BeFalse();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        private static string RequestRma(string orderId, string lineId, decimal quantity)
        {
            Console.WriteLine("Begin RequestRma");

            var result =
                Proxy.DoCommand(
                    _context.ShopsContainer().RequestRma(
                        orderId,
                        "ConsoleWrongItem",
                        new List<RmaLineComponent>
                            {
                                new RmaLineComponent
                                {
                                    LineId = lineId,
                                    Quantity = quantity
                                }
                }));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.Should().NotBeEmpty();
            result.Models.OfType<RmaAdded>().FirstOrDefault().Should().NotBeNull();
            var rmaFriendlyId = result.Models.OfType<RmaAdded>().FirstOrDefault()?.RmaId;
            rmaFriendlyId.Should().NotBeNullOrEmpty();

            var rma = GetRma(_context.ShopsContainer(), rmaFriendlyId);
            rma.Status.Should().Be("Pending");
            rma.Lines.Should().Contain(l => l.LineId.Equals(lineId));
            rma.Order.EntityTarget.Should().Be(orderId);
            rma.ItemsReturnedDate.Should().Be(DateTimeOffset.MinValue);
            rma.RefundPaymentId.Should().NotBeEmpty();

            var order = Orders.GetOrder(_context.ShopsContainer(), orderId);
            order.Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault().Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.Should().NotBeEmpty();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id)).Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id))?.Lines.Should().NotBeEmpty();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id))?.Lines.Count.Should().Be(rma.Lines.Count);
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id))?.Lines.Should().Contain(rma.Lines.Select(l => l.LineId));

            WriteListCounts(_context.ShopsContainer());

            return rmaFriendlyId;
        }

        private static ReturnMerchandiseAuthorization GetRma(Engine.Container container, string rmaFriendlyId)
        {
            var result = Proxy.GetValue(container.ReturnMerchandiseAuthorizations.ByKey(rmaFriendlyId).Expand("Lines($expand=ChildComponents),Components"));
            result.Should().NotBeNull();
            result.Status.Should().NotBeNullOrEmpty();
            result.Lines.Should().NotBeEmpty();
            result.Order.EntityTarget.Should().NotBeNullOrEmpty();
            result.ReturnReason.Should().NotBeNullOrEmpty();

            var result2 = Proxy.GetValue(container.ReturnMerchandiseAuthorizations.ByKey($"Entity-ReturnMerchandiseAuthorization-{rmaFriendlyId}").Expand("Lines($expand=ChildComponents),Components"));
            result2.Should().NotBeNull();

            return result;
        }

        private static void ReturnedItemReceivedValidation(string rmaFriendlyId)
        {
            var result = Proxy.DoCommand(_context.ShopsContainer().ReturnedItemReceived(rmaFriendlyId));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase) || m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");
        }

        public static void ReturnedItemReceived(Engine.Container container, string rmaFriendlyId, string orderId, string lineId)
        {
            var result = Proxy.DoCommand(container.ReturnedItemReceived(rmaFriendlyId));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var rma = GetRma(container, rmaFriendlyId);
            rma.Status.Should().Be("RefundPending");
            rma.ItemsReturnedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 2000);

            var order = Orders.GetOrder(container, orderId);
            order.Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault().Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.Should().NotBeEmpty();
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId)).Should().NotBeNull();
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId))?.CartLineComponents.OfType<ItemReturnedComponent>().Any().Should().BeTrue();
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId))?.CartLineComponents.OfType<ItemReturnedComponent>().FirstOrDefault()?.Returns.Should().NotBeEmpty();
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId))?.CartLineComponents.OfType<ItemReturnedComponent>().FirstOrDefault()?.Returns.Count.Should().Be(order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.Count);
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId))?.CartLineComponents.OfType<ItemReturnedComponent>().FirstOrDefault()?.Returns.OrderByDescending(r => r.ReturnedDate).FirstOrDefault().Should().NotBeNull();
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId))?.CartLineComponents.OfType<ItemReturnedComponent>().FirstOrDefault()?.Returns.OrderByDescending(r => r.ReturnedDate).FirstOrDefault()?.Quantity.Should().Be(rma.Lines.FirstOrDefault()?.Quantity);
            order.Lines.FirstOrDefault(l => l.Id.Equals(lineId))?.CartLineComponents.OfType<ItemReturnedComponent>().FirstOrDefault()?.Returns.OrderByDescending(r => r.ReturnedDate).FirstOrDefault()?.ReturnedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 2000);

            WriteListCounts(container);
        }

        public static void RunRefundRmasMinion(Engine.Container container, string rmaFriendlyId, string environmentName = "AdventureWorksMinions")
        {
            Console.WriteLine("Begin RunRefundRmasMinion");

            var policies =
                new Collection<CommerceOps.Sitecore.Commerce.Core.Policy>
                    {
                        new CommerceOps.Sitecore.Commerce.Core.RunMinionPolicy { RunChildren = false }
                    };
            var result = Proxy.GetValue(
                new MinionRunner().Context.MinionsContainer()
                .RunMinion("Sitecore.Commerce.Plugin.Returns.RefundRmasMinion, Sitecore.Commerce.Plugin.Returns", environmentName, policies));
            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            WriteListCounts(container);

            var rma = GetRma(container, rmaFriendlyId);
            rma.Status.Should().Be("Completed");
        }

        private static void WriteListCounts(Engine.Container container)
        {
            var backupColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("---------------------------------------------------");
            var rmasMetadata = container.GetListMetadata("Rmas").GetValue();
            Console.WriteLine($"List:{rmasMetadata.ListName} Count:{rmasMetadata.Count}");

            var pendingRmasMetadata = container.GetListMetadata("PendingRmas").GetValue();
            Console.WriteLine($"List:{pendingRmasMetadata.ListName} Count:{pendingRmasMetadata.Count}");
            
            var refundPendingRmasMetadata = container.GetListMetadata("RefundPendingRmas").GetValue();
            Console.WriteLine($"List:{refundPendingRmasMetadata.ListName} Count:{refundPendingRmasMetadata.Count}");

            var completedRmasMetadata = container.GetListMetadata("CompletedRmas").GetValue();
            Console.WriteLine($"List:{completedRmasMetadata.ListName} Count:{completedRmasMetadata.Count}");

            var problemRmasMetadata = container.GetListMetadata("ProblemRmas").GetValue();
            Console.WriteLine($"List:{problemRmasMetadata.ListName} Count:{problemRmasMetadata.Count}");

            Console.WriteLine("---------------------------------------------------");

            Console.ForegroundColor = backupColor;
        }
        
        private static Order CreateCompletedOrder()
        {
            var orderId = Scenarios.Buy1ItemMultipleQuantity.Run(_context).Result;
            orderId.Should().NotBeNullOrEmpty();

            var order = Orders.GetOrder(_context.ShopsContainer(), orderId);
            order.Should().NotBeNull();
            order.Lines.Should().NotBeEmpty();
            order.Lines.FirstOrDefault().Should().NotBeNull();

            var lineId = order.Lines.FirstOrDefault()?.Id;
            lineId.Should().NotBeNullOrEmpty();

            var paymentId = order.Components.OfType<Sitecore.Commerce.Plugin.Payments.PaymentComponent>().FirstOrDefault()?.Id;
            paymentId.Should().NotBeNullOrEmpty();

            Orders.RunPendingOrdersMinion(_context);
            Orders.RunSettleSalesActivitiesMinion(_context);
            Orders.RunReleasedOrdersMinion(_context);
            WriteListCounts(_context.ShopsContainer());

            return order;
        }

        private static void RequestDigitalRma()
        {
            var order = CreateDigitalCompletedOrder();
            var orderId = order.Id;
            var lineId = order.Lines.FirstOrDefault()?.Id;

            var result =
                Proxy.DoCommand(
                    _context.ShopsContainer().RequestRma(
                        orderId,
                        "ConsoleWrongItem",
                        new List<RmaLineComponent>
                        {
                            new RmaLineComponent
                            {
                                LineId = lineId,
                                Quantity = 1
                            }
                        }));

            result.Should().NotBeNull();
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.Should().NotBeEmpty();
            result.Models.OfType<RmaAdded>().FirstOrDefault().Should().NotBeNull();
            var rmaFriendlyId = result.Models.OfType<RmaAdded>().FirstOrDefault()?.RmaId;
            rmaFriendlyId.Should().NotBeNullOrEmpty();

            var rma = GetRma(_context.ShopsContainer(), rmaFriendlyId);
            rma.Status.Should().Be("RefundPending");
            rma.Lines.Should().Contain(l => l.LineId.Equals(lineId));
            rma.Order.EntityTarget.Should().Be(orderId);
            rma.ItemsReturnedDate.Should().NotBe(DateTimeOffset.MinValue);

            order = Orders.GetOrder(_context.ShopsContainer(), orderId);
            order.Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault().Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.Should().NotBeEmpty();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id)).Should().NotBeNull();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id))?.Lines.Should().NotBeEmpty();
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id))?.Lines.Count.Should().Be(rma.Lines.Count);
            order.Components.OfType<OrderRmasComponent>().FirstOrDefault()?.Returns.FirstOrDefault(r => r.Rma.EntityTarget.Equals(rma.Id))?.Lines.Should().Contain(rma.Lines.Select(l => l.LineId));

            WriteListCounts(_context.ShopsContainer());
        }

        private static Order CreateDigitalCompletedOrder()
        {
            var orderId = Scenarios.BuyWarranty.Run(_context, 2).Result;
            orderId.Should().NotBeNullOrEmpty();

            var order = Orders.GetOrder(_context.ShopsContainer(), orderId);
            order.Should().NotBeNull();

            Orders.RunPendingOrdersMinion(_context);
            Orders.RunSettleSalesActivitiesMinion(_context);
            Orders.RunReleasedOrdersMinion(_context);

            return order;
        }
    }
}
