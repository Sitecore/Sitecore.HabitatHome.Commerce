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

    public static class Simple2PhysicalDigitalItems
    {
        public static string ScenarioName = "Simple2PhysicalDigitalItems";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                var commandResponse = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|22565422120|100", 1));
                var firstLineId = commandResponse.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;

                commandResponse = Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW188 06|19", 1));
                var secondLineId = commandResponse.Models.OfType<LineAdded>().FirstOrDefault()?.LineId;
                
                Proxy.DoCommand(container.SetCartLineFulfillment(cartId, firstLineId, context.Components.OfType<ElectronicFulfillmentComponent>().First()));
                commandResponse = Proxy.DoCommand(container.SetCartLineFulfillment(cartId, secondLineId, context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var totals = commandResponse.Models.OfType<Totals>().First();
                totals.AdjustmentsTotal.Amount.Should().Be(0M);
                totals.GrandTotal.Amount.Should().Be(229M);

                // Add a Payment
                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount - totals.PaymentsTotal.Amount);
                commandResponse = Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));
                totals = commandResponse.Models.OfType<Totals>().First();
                totals.PaymentsTotal.Amount.Should().Be(229M);

                // Get the cart one last time before creating the order
                var cart = Carts.GetCart(cartId, context);
                cart.Version.Should().Be(5);
                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(229.0M);

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
