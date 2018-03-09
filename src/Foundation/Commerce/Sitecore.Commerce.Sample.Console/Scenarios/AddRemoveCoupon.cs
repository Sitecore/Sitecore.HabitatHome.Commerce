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

    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class AddRemoveCoupon
    {
        public static string ScenarioName = "AddRemoveCoupon";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                //Add Cart Line with Variant
                var commandResult = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));
                var cartLineId = commandResult.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;

                //Add Cart Line without Variant
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                //Show adding an invalid coupon
                commandResult = Proxy.DoCommand(container.AddCouponToCart(cartId, "InvalidCouponCode"));
                if (commandResult.ResponseCode != "Ok")
                {
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected AddCouponToCart_Fail: An unrecognized coupon was presented: InvalidCouponCode");
                }

                // Add a valid coupon, remove it and add again
                Proxy.DoCommand(container.AddCouponToCart(cartId, "RTRNC10P"));

                var cart = Carts.GetCart(cartId, context);
                cart.Components.OfType<CartCouponsComponent>().FirstOrDefault().Should().NotBeNull();

                Proxy.DoCommand(container.RemoveCouponFromCart(cartId, "RTRNC10P"));
                cart = Carts.GetCart(cartId, context);
                cart.Components.OfType<CartCouponsComponent>().FirstOrDefault().Should().BeNull();

                Proxy.DoCommand(container.AddCouponToCart(cartId, "RTRNC10P"));

                commandResult = Proxy.DoCommand(container.RemoveCartLine(cartId, "BadLineId"));
                if (commandResult.ResponseCode != "Ok")
                {
                    ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error: Cart line BadLineId was not found.");
                }

                Proxy.DoCommand(container.UpdateCartLine(cartId, cartLineId, 10));

                commandResult = Proxy.DoCommand(container.SetCartFulfillment(cartId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);

                Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                order.Totals.GrandTotal.Amount.Should().Be(1109.00M);

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
