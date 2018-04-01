namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Core;
    using Extensions;
    using FluentAssertions;
    using Plugin.Carts;
    using Plugin.Fulfillment;
    using Plugin.Payments;

    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    /// <summary>
    /// Try to use a limited single use coupon more than once.
    /// </summary>
    public static class UseLimitedCouponMoreThanOnce
    {
        public static string ScenarioName = "UseLimitedCouponMoreThanOnce";

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
                var commandResult = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));
                var cartLineId = commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;

                // Add Cart Line without Variant
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                // Add a valid coupon
                Proxy.DoCommand(container.AddCouponToCart(cartId, "SingleUseCouponCode"));

                Proxy.DoCommand(container.UpdateCartLine(cartId, cartLineId, 10));

                commandResult = Proxy.DoCommand(container.SetCartFulfillment(cartId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);

                Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                order.Totals.GrandTotal.Amount.Should().Be(1219.90M);

                cartId = Guid.NewGuid().ToString("B");

                // Add Cart Line with Variant
                commandResult = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));
                cartLineId = commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;

                // Add Cart Line without Variant
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                // Add a valid coupon
                commandResult = Proxy.DoCommand(container.AddCouponToCart(cartId, "SingleUseCouponCode"));
                commandResult.ResponseCode.Should().NotBe("Ok", "Expecting failure as this coupon code is single use only and has been used prior");
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected AddCouponToCart_Fail: The coupon code 'SingleUseCoupon' is not valid");

                Console.WriteLine($"End {ScenarioName}: {watch.ElapsedMilliseconds} ms");

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
