namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;
    using FluentAssertions;
    using Plugin.Fulfillment;
    using Plugin.Payments;
    using Console;
    using ServiceProxy;
    using Plugin.Carts;

    public static class BuyPhone
    {
        public static string ScenarioName = "BuyPhone";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                // Habitat Republic 32GB 4G LTE 
                var phoneLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042323|56042324", 1));

                // Habitat Shark Waterproof Smartphone Case
                var caseLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042360|56042360", 1));
              
                Proxy.DoCommand(container.SetCartLineFulfillment(
                    cartId,
                    phoneLine.Models.OfType<LineAdded>().FirstOrDefault().LineId, 
                    context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                Proxy.DoCommand(container.SetCartLineFulfillment(
                    cartId,
                    caseLine.Models.OfType<LineAdded>().FirstOrDefault().LineId, 
                    context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var cart = Carts.GetCart(cartId, context);
                cart.Should().NotBeNull();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = cart.Totals.GrandTotal;
                Proxy.DoCommand(container.AddFederatedPayment(cartId, paymentComponent));
                
                var order = Orders.CreateAndValidateOrder(container, cartId, context);
                order.Status.Should().NotBe("Problem");
                order.Totals.GrandTotal.Amount.Should().Be(cart.Totals.GrandTotal.Amount);

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
