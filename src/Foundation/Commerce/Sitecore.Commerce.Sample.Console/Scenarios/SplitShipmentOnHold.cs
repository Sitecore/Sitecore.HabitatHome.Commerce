namespace Sitecore.Commerce.Sample.Scenarios
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Extensions;
    using FluentAssertions;

    using Sitecore.Commerce.Sample.Console;

    public static class SplitShipmentOnHold
    {
        public static string ScenarioName = "SplitShipmentOnHold";

        public static Task<string> Run(ShopperContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var container = context.ShopsContainer();

                Console.WriteLine($"Begin {ScenarioName}");

                var orderId = BuySplitShipment.Run(context).Result;
                if (!string.IsNullOrEmpty(orderId))
                {
                    OrdersUX.HoldOrder(orderId);
                }

                var journalizedOrdersMetadata = container.GetListMetadata($"JournalEntries-ByEntity-{orderId}").GetValue();
                Console.WriteLine($"List:{journalizedOrdersMetadata.ListName} Count:{journalizedOrdersMetadata.Count}");


                var journalizedOrdersList = container.GetList($"JournalEntries-ByEntity-{orderId}",
                    "Sitecore.Commerce.Plugin.Journaling.JournalEntry, Sitecore.Commerce.Plugin.Journaling", 0, 10).Expand("Items").GetValue();

                journalizedOrdersList.TotalItemCount.Should().Be(2);

                watch.Stop();

                Console.WriteLine($"End {ScenarioName} - {watch.ElapsedMilliseconds} ms");

                return Task.FromResult(orderId);
            }
            catch (Exception ex)
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Exception in Scenario {ScenarioName} (${ex.Message}) : Stack={ex.StackTrace}");
                return null;
            }
        }

    }
}
