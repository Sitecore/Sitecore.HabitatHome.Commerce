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

    public static class AddRemoveCartLine
    {
        public static string ScenarioName = "AddRemoveCartLine";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var cartId = Guid.NewGuid().ToString("B");
                
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1));
                
                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                Proxy.DoCommand(container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1));

                var updatedCart = Proxy.GetValue(
                    container.Carts.ByKey(cartId).Expand("Lines"));

                var cartLineComponent = updatedCart.Lines.FirstOrDefault(l => l.ItemId.Equals("Adventure Works Catalog|AW475 14|"));
                if (cartLineComponent != null)
                {
                    Proxy.DoCommand(container.RemoveCartLine(cartId, cartLineComponent.Id));
                }

                var commandResponse = Proxy.DoCommand(
                    container.SetCartFulfillment(
                        cartId,
                        context.Components.OfType<PhysicalFulfillmentComponent>().First()));

                var totals = commandResponse.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);

                commandResponse = Proxy.DoCommand(
                    container.AddFederatedPayment(
                        cartId,
                        paymentComponent));

                totals = commandResponse.Models.OfType<Totals>().First();

                totals.PaymentsTotal.Amount.Should().Be(totals.GrandTotal.Amount);

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                order.Totals.GrandTotal.Amount.Should().Be(115.50M);

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
