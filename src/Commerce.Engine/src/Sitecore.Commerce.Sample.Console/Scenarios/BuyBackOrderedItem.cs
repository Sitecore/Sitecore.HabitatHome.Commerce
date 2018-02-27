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
    using Plugin.Catalog;
    using Plugin.Fulfillment;
    using Plugin.Payments;

    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class BuyBackOrderedItem
    {
        public static string ScenarioName = "BuyBackOrderedItem";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                //Request the SellableItem
                Proxy.GetValue(container.SellableItems.ByKey("Adventure Works Catalog,AW210 12,4").Expand("Components($expand=ChildComponents($expand=ChildComponents($expand=ChildComponents)))"));

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW210 12|4", 1));

                var commandResult = Proxy.DoCommand(
                    container.SetCartFulfillment(cartId,
                    context.Components.OfType<PhysicalFulfillmentComponent>().First())
                );

                var totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);
                commandResult = Proxy.DoCommand(
                    container.AddFederatedPayment(cartId, 
                    paymentComponent)
                );
                totals = commandResult.Models.OfType<Totals>().First();

                totals.PaymentsTotal.Amount.Should().Be(totals.GrandTotal.Amount);

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                order.Totals.GrandTotal.Amount.Should().Be(233.20M);

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
