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
    using Plugin.GiftCards;
    using Plugin.Payments;

    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class BuyWithGiftCard
    {
        public static string ScenarioName = "BuyWithGiftCard";

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
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));

                // Add Cart Line without Variant
                var commandResult = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                var totals = commandResult.Models.OfType<Totals>().First();

                Proxy.DoCommand(container.AddCouponToCart(cartId, "RTRNC15P"));

                commandResult = Proxy.DoCommand(container.SetCartFulfillment(cartId,context.Components.OfType<PhysicalFulfillmentComponent>().First()));
                totals = commandResult.Models.OfType<Totals>().First();

                var giftCardToUse = context.GiftCards.First();
                
                commandResult = Proxy.DoCommand(
                    container.AddGiftCardPayment(cartId, new GiftCardPaymentComponent
                    {
                        PaymentMethod = new EntityReference { EntityTarget = "B5E5464E-C851-4C3C-8086-A4A874DD2DB0", Name = "GiftCard" },
                        GiftCardCode = giftCardToUse,
                        Amount = Money.CreateMoney(50),
                    }));

                totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount - totals.PaymentsTotal.Amount);
                commandResult = Proxy.DoCommand(
                    container.AddFederatedPayment(cartId, 
                    paymentComponent));

                totals = commandResult.Models.OfType<Totals>().First();

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

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
