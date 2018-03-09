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

    public static class OnSaleItem
    {
        public static string ScenarioName = "OnSaleItem";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW188 06|20", 1));
                Proxy.DoCommand(container.SetCartFulfillment(cartId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));
                
                var giftCardToUse = context.GiftCards.First();
                
                var commandResult = Proxy.DoCommand(
                    container.AddGiftCardPayment(
                        cartId,
                    new GiftCardPaymentComponent
                    {
                        PaymentMethod = new EntityReference { EntityTarget = "B5E5464E-C851-4C3C-8086-A4A874DD2DB0", Name = "GiftCard" },
                        GiftCardCode = giftCardToUse,
                        Amount = Money.CreateMoney(50),
                    }));

                var totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount - totals.PaymentsTotal.Amount);
                commandResult = Proxy.DoCommand(
                    container.AddFederatedPayment(
                        cartId,
                    paymentComponent));

                totals = commandResult.Models.OfType<Totals>().First();

                totals.PaymentsTotal.Amount.Should().Be(totals.GrandTotal.Amount);

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                order.Totals.GrandTotal.Amount.Should().Be(119.50M);

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
