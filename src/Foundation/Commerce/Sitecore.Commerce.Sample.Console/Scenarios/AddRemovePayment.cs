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

    public static class AddRemovePayment
    {
        public static string ScenarioName = "AddRemovePayment";

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
                Proxy.DoCommand(
                    container.AddCartLine(cartId, "Adventure Works Catalog|AW098 04|5", 1)
                );

                //Add Cart Line without Variant
                Proxy.DoCommand(
                    container.AddCartLine(cartId, "Adventure Works Catalog|AW475 14|", 1)
                );

                var commandResult = Proxy.DoCommand(
                    container.SetCartFulfillment(cartId,
                    context.Components.OfType<PhysicalFulfillmentComponent>().First())
                );

                var totals = commandResult.Models.OfType<Totals>().First();

                var paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(totals.GrandTotal.Amount);

                Proxy.DoCommand(
                    container.AddFederatedPayment(cartId, 
                    paymentComponent)
                );

                var cart = Proxy.GetValue(container.Carts.ByKey(cartId).Expand("Lines($expand=CartLineComponents),Components"));

                var federatedPaymentComponent = cart.Components.OfType<FederatedPaymentComponent>().First();

                container.RemovePayment(cartId, federatedPaymentComponent.Id).GetValue();

                cart = Proxy.GetValue(container.Carts.ByKey(cartId).Expand("Lines($expand=CartLineComponents),Components"));

                paymentComponent = context.Components.OfType<FederatedPaymentComponent>().First();
                paymentComponent.Amount = Money.CreateMoney(cart.Totals.GrandTotal.Amount);

                Proxy.DoCommand(
                    container.AddFederatedPayment(cartId, 
                    paymentComponent)
                );

                var order = Orders.CreateAndValidateOrder(container, cartId, context);

                watch.Stop();

                order.Totals.GrandTotal.Amount.Should().Be(180.40M);

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
