namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Core;
    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class SimplePhysicalRtrn15Coupon
    {
        public static string ScenarioName = "SimplePhysicalRtrn15Coupon";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                // Add Cart Line with Variant
                Proxy.DoCommand(
                    container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));

                // Add Cart Line without Variant
                Proxy.DoCommand(
                    container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                Proxy.DoCommand(container.AddCouponToCart(cartId, "RTRNC15P"));

                var commandResult = Proxy.DoCommand(
                    container.SetCartFulfillment(
                        cartId,
                    context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);
                commandResult = Proxy.DoCommand(
                    container.AddFederatedPayment(
                        cartId,
                    paymentComponent));

                totals = commandResult.Models.OfType<Totals>().First();
                totals.Should().NotBeNull();

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                var orderSummary = Proxy.GetValue(container.GetEntityView(order.Id, "Master", string.Empty, string.Empty));

                // Show that the coupon used is part of the view when a business user looks at an order
                orderSummary.ChildViews.OfType<EntityView>().First(p => p.Name == "Summary").Properties.FirstOrDefault(p => p.Name == "CouponUsed").Should().NotBeNull();

                watch.Stop();
                    
                order.Totals.GrandTotal.Amount.Should().Be(155.80M);

                Console.WriteLine($"End {ScenarioName} (${order.Totals.GrandTotal.Amount}):{watch.ElapsedMilliseconds} ms");

                return Task.FromResult(order.Id);
            }
            catch (Exception ex)
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Exception in Scenario {ScenarioName} (${ex.Message}) : Stack={ex.StackTrace}");
                return null;
            }
        }
    }
}
