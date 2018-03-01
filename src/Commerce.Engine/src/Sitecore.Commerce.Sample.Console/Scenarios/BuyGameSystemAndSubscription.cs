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

    public static class BuyGameSystemAndSubscription
    {
        public static string ScenarioName = "BuyGameSystemAndSubscription";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");

                //Habitat NextCube-V Game Cube 1TB 
                var gameSystemLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042432|56042432", 1));

                //Habitat NextCube Now  6 month On-Demand Multigame Subscription
                var subscriptionLine = Proxy.DoCommand(container.AddCartLine(cartId, "Habitat_Master|6042456|56042456", 1));

                var gameSystemLineFulfillmentResponse = Proxy.DoCommand(container.SetCartLineFulfillment(cartId,
                    gameSystemLine.Models.OfType<LineAdded>().FirstOrDefault().LineId, 
                    context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                Proxy.DoCommand(
                    container.SetCartLineFulfillment(
                        cartId,
                        subscriptionLine.Models.OfType<LineAdded>().FirstOrDefault().LineId,
                        context.Components.OfType<ElectronicFulfillmentComponent>().First()
                        )
                );

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
