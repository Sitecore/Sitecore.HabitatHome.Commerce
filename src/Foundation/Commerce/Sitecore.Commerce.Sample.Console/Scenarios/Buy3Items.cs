namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Sample.Console;
    using Sitecore.Commerce.ServiceProxy;

    public static class Buy3Items
    {
        public static string ScenarioName = "Buy3Items";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW029 03|54", 1));
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW029 03|64", 2));
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW032 01|37", 3));

                // Add Cart Level Physical Fulfillment
                var commandResponse = Proxy.DoCommand(
                    container.SetCartFulfillment(
                        cartId,
                    context.Components.OfType<PhysicalFulfillmentComponent>().First()));
                var totals = commandResponse.Models.OfType<Totals>().First();

                totals.AdjustmentsTotal.Amount.Should().Be(13.50M);
                totals.GrandTotal.Amount.Should().Be(148.50M);

                // Add a Payment
                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount - totals.PaymentsTotal.Amount);
                commandResponse = Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));
                totals = commandResponse.Models.OfType<Totals>().First();
                totals.PaymentsTotal.Amount.Should().Be(148.5M);

                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(148.50M);

                watch.Stop();
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
