namespace Sitecore.Commerce.Sample.Console
{
    using System.Diagnostics;

    using FluentAssertions;

    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Coupons
    {
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        private static readonly Sitecore.Commerce.Engine.Container CsrShopsContainer = new CsrSheila().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Begin Coupons");

            GetCoupon();
            GetPrivateCouponGroup();

            watch.Stop();

            System.Console.WriteLine($"End Coupons :{watch.ElapsedMilliseconds} ms");
        }

        #region CartHelpers

        public static void AddCouponToCart(string cartId, string coupon)
        {
            var commandResult = Proxy.DoCommand(ShopsContainer.AddCouponToCart(cartId, coupon));

            if (commandResult.ResponseCode != "Ok" && coupon != "InvalidCoupon")
            {
                System.Console.WriteLine($"AddCouponToCart_Fail:{commandResult.ResponseCode}");
            }
        }

        #endregion

        #region Coupons

        public static Coupon GetCoupon(string couponFriendlyId = "")
        {
            System.Console.WriteLine("Begin GetCoupon");

            var friendlyId = string.IsNullOrEmpty(couponFriendlyId)
                                 ? "RTRNEC5P"
                                 : couponFriendlyId;

            var result = Proxy.GetValue(ShopsContainer.Coupons.ByKey(friendlyId).Expand("Components"));
            result.Should().NotBeNull();
            result.Components.Should().NotBeEmpty();

            return result;
        }

        private static void GetPrivateCouponGroup(string groupFriendlyId = "")
        {
            System.Console.WriteLine("Begin GetPrivateCouponGroup"); 

            var friendlyId = string.IsNullOrEmpty(groupFriendlyId)
                                 ? "SPCP_-_22"
                                 : groupFriendlyId;

            var result = Proxy.GetValue(ShopsContainer.PrivateCouponGroups.ByKey(friendlyId).Expand("Components"));
            result.Should().NotBeNull();
            result.AllocatedCount.Should().Be(0);
            result.Description.Should().Be("Sample Private Coupon Promotion");
            result.DisplayName.Should().Be("Sample Private Coupon Promotion");
            result.Name.Should().Be("SamplePrivateCouponPromotion");
            result.Prefix.Should().Be("SPCP_");
            result.Suffix.Should().Be("_22");
            result.Total.Should().Be(15);
        }

        #endregion
    }
}
