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

    public static class SimplePhysical2Items
    {
        public static string ScenarioName = "SimplePhysical2Items";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW188 06|19", 1));

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW188 06|20", 1));

                // Add Cart Level Physical Fulfillment
                var commandResponse = Proxy.DoCommand(
                    container.SetCartFulfillment(
                        cartId,
                    context.Components.OfType<PhysicalFulfillmentComponent>().First()));
                var totals = commandResponse.Models.OfType<Totals>().First();
                totals.AdjustmentsTotal.Amount.Should().Be(19M);
                totals.GrandTotal.Amount.Should().Be(209M);

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount - totals.PaymentsTotal.Amount);

                // Add a Payment
                commandResponse = Proxy.DoCommand(
                    container.AddFederatedPayment(cartId, paymentComponent));
                totals = commandResponse.Models.OfType<Totals>().First();

                totals.PaymentsTotal.Amount.Should().Be(209M);

                // Get the cart one last time before creating the order
                var cart = Carts.GetCart(cartId, context);
                cart.Version.Should().Be(4);

                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(209M);

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
