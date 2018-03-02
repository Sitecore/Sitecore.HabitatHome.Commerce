namespace Sitecore.Commerce.Sample.Console
{
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Promotions
    {
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public  static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Begin PromotionsBooksAndCards");

            GetPromotionBook();
            GetBookAssociatedCatalogs();

            GetPromotion();

            watch.Stop();

            System.Console.WriteLine($"End PromotionsBooksAndCards :{watch.ElapsedMilliseconds} ms");
        }
        
        public static Promotion GetPromotion(string promotionFriendlyId = "")
        {
            System.Console.WriteLine("Begin GetPromotion");

            var friendlyId = string.IsNullOrEmpty(promotionFriendlyId)
                                 ? "AdventureWorksPromotionBook-CartFreeShippingPromotion"
                                 : promotionFriendlyId;

            var result = Proxy.GetValue(ShopsContainer.Promotions.ByKey(friendlyId).Expand("Components"));
            result.Should().NotBeNull();
            result.Components.Should().NotBeEmpty();
            result.Components.OfType<ApprovalComponent>().Any().Should().BeTrue();

            return result;
        }

        private static void GetPromotionBook()
        {
            System.Console.WriteLine("Begin GetPromotionBook");

            var result = Proxy.GetValue(ShopsContainer.PromotionBooks.ByKey("AdventureWorksPromotionBook").Expand("Components"));
            result.Should().NotBeNull();
        }
        
        private static void GetBookAssociatedCatalogs()
        {
            System.Console.WriteLine("Begin GetBookAssociatedCatalogs");

            var result = Proxy.Execute(ShopsContainer.GetPromotionBookAssociatedCatalogs("AdventureWorksPromotionBook"));
            result.Should().NotBeNull();
        }
    }
}
