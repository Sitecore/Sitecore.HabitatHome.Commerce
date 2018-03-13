namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Extensions;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class CouponsUX
    {
        private const string BookName = "consoleuxcouponpromotionbook";
        private const string PromotionName = "consoleuxcouponpromotion";
        private static string _bookId;
        private static string _promotionId;
        private static string _promotionFriendlyId;

        private static string _publicCouponId;

        private static string _privateCouponGroupId;

        private static Promotion _promotion;
        
        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new CsrSheila().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin CouponsUX");

            AddPromotionBook();
            AddPromotion();

            PublicCouponsView();
            PrivateCouponsView();

            AddPublicCoupon();
            AddPrivateCoupon();

            PublicCouponsViewCheck();
            PrivateCouponsViewCheck();

            NewAllocation();

            PublicCouponsViewCheck();
            PrivateCouponsViewCheck();

            // error scenarios
            AddPublicCouponAgain();
            AddPrivateCouponAgain();
            AllocateTooMuch();

            PublicCouponsViewCheck();
            PrivateCouponsViewCheck();

            watch.Stop();

            Console.WriteLine($"End CouponsUX :{watch.ElapsedMilliseconds} ms");
        }

        #region CouponPreReqs

        private static void AddPromotionBook()
        {
            Console.WriteLine("Begin AddPromotionBook CouponsUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "Details", "AddPromotionBook", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = BookName },
                                      new ViewProperty { Name = "DisplayName", Value = "displayname" },
                                      new ViewProperty { Name = "Description", Value = "description" }
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            _bookId = $"Entity-PromotionBook-{result.Models.OfType<PromotionBookAdded>().FirstOrDefault()?.PromotionBookFriendlyId}";
        }

        private static void AddPromotion()
        {
            Console.WriteLine("Begin AddPromotion CouponsUX");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_bookId, "Details", "AddPromotion", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            var fromDate = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now.AddDays(30);
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      new ViewProperty { Name = "Name", Value = PromotionName },
                                      new ViewProperty { Name = "Description", Value = "promotion's description" },
                                      new ViewProperty { Name = "DisplayName", Value = "promotion's display name" },
                                      new ViewProperty { Name = "DisplayText", Value = "promotion's text" },
                                      new ViewProperty { Name = "DisplayCartText", Value = "promotion's cart text" },
                                      new ViewProperty
                                          {
                                              Name = "ValidFrom",
                                              Value = fromDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty
                                          {
                                              Name = "ValidTo",
                                              Value = toDate.ToString(CultureInfo.InvariantCulture)
                                          },
                                      new ViewProperty { Name = "IsExclusive", Value = "true" },
                                      version
                                  };
            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PromotionAdded>().FirstOrDefault().Should().NotBeNull();
            _promotionFriendlyId = result.Models.OfType<PromotionAdded>().FirstOrDefault()?.PromotionFriendlyId;
            _promotionId = $"Entity-Promotion-{_promotionFriendlyId}";

            _promotion = Proxy.GetValue(ShopsContainer.Promotions.ByKey(_promotionFriendlyId).Expand("Components"));
            _promotion.Should().NotBeNull();
            _promotion.ValidFrom.Should().BeCloseTo(fromDate, 1000);
            _promotion.ValidTo.Should().BeCloseTo(toDate, 1000);
        }

        #endregion

        #region Coupons

        private static void PublicCouponsView()
        {
            Console.WriteLine("Begin Public Coupons View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "PublicCoupons", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void PrivateCouponsView()
        {
            Console.WriteLine("Begin Private Coupons View");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "PrivateCoupons", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().BeEmpty();
        }

        private static void PublicCouponsViewCheck()
        {
            Console.WriteLine("Begin Public Coupons View Check");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "PublicCoupons", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
            result.ChildViews.Count.Should().Be(1);
            result.Name.Should().Be("PublicCoupons");
            result.DisplayName.Should().Be("Public Coupons");
            result.UiHint.Should().Be("Table");
            result.Policies.Count.Should().Be(1);
            result.Properties.Count.Should().Be(1);
            result.ChildViews[0].Should().BeOfType(typeof(EntityView));

            var couponDetails = result.ChildViews[0] as EntityView;
            couponDetails.Should().NotBeNull();
            couponDetails.ChildViews?.Count.Should().Be(0);
            couponDetails.Action.Should().BeNullOrEmpty();
            couponDetails.DisplayName.Should().Be("Coupon Details");
            couponDetails.Name.Should().Be("CouponDetails");
            couponDetails.Properties.Count.Should().Be(2);
        }

        private static void PrivateCouponsViewCheck()
        {
            Console.WriteLine("Begin Private Coupons View Check");

            var result = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "PrivateCoupons", string.Empty, string.Empty));
            result.Should().NotBeNull();
            result.Policies.Should().NotBeEmpty();
            result.Properties.Should().NotBeEmpty();
            result.ChildViews.Should().NotBeEmpty();
            result.Name.Should().Be("PrivateCoupons");
            result.DisplayName.Should().Be("Private Coupons");
            result.UiHint.Should().Be("Table");
            result.ChildViews.Count.Should().Be(1);
            result.Policies.Count.Should().Be(1);
            result.Properties.Count.Should().Be(1);
            result.ChildViews[0].Should().BeOfType(typeof(EntityView));

            var couponDetails = result.ChildViews[0] as EntityView;
            couponDetails.Should().NotBeNull();
            couponDetails.ChildViews.Count.Should().Be(0);
            couponDetails.Action.Should().BeNullOrEmpty();
            couponDetails.DisplayName.Should().Be("Coupon Details");
            couponDetails.Name.Should().Be("CouponDetails");
            couponDetails.Properties.Count.Should().Be(4);
        }

        private static void AddPublicCoupon()
        {
            Console.WriteLine("Begin AddPublicCoupon");

            var addView = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "PublicCoupons", "AddPublicCoupon", string.Empty));
            addView.Should().NotBeNull();
            addView.Policies.Should().BeEmpty();
            addView.Properties.Should().NotBeEmpty();
            addView.Properties.Count.Should().Be(2); // Version & Code
            addView.ChildViews.Should().BeEmpty();
            var version = addView.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            addView.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "Code", Value = "PublicCoupon_ABC123" },
                    version
                };

            var addAction = Proxy.DoCommand(ShopsContainer.DoAction(addView));
            addAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            addAction.Models.OfType<PublicCouponAdded>().FirstOrDefault().Should().NotBeNull();
            _publicCouponId = addAction.Models.OfType<PublicCouponAdded>().FirstOrDefault()?.CouponFriendlyId;
            _publicCouponId.Should().NotBeNullOrEmpty();
            var message = addAction.Models.OfType<PublicCouponAdded>().FirstOrDefault()?.Name;
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Green, $"Created public coupon code: {message}");
        }

        private static void AddPublicCouponAgain()
        {
            Console.WriteLine("Begin Error Test: AddPublicCouponAgain");

            var addView = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "PublicCoupons", "AddPublicCoupon", string.Empty));
            addView.Should().NotBeNull();
            addView.Policies.Should().BeEmpty();
            addView.Properties.Should().NotBeEmpty();
            addView.Properties.Count.Should().Be(2); // Version & Code
            addView.ChildViews.Should().BeEmpty();
            var version = addView.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            addView.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "Code", Value = "PublicCoupon_ABC123" },
                    version
                };

            var addAction = Proxy.DoCommand(ShopsContainer.DoAction(addView));
            ConsoleExtensions.WriteWarningLine("Expecting Code already in use error");
            addAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            addAction.Models.OfType<PublicCouponAdded>().FirstOrDefault().Should().BeNull();
        }

        private static void AddPrivateCoupon()
        {
            Console.WriteLine("Begin AddPrivateCoupon");

            var addView = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "CouponDetails", "AddPrivateCoupon", string.Empty));
            addView.Should().NotBeNull();
            addView.Policies.Should().BeEmpty();
            addView.Properties.Should().NotBeEmpty();
            addView.Properties.Count.Should().Be(4); // Version, Prefix, Suffix & Total
            addView.ChildViews.Should().BeEmpty();
            var version = addView.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            addView.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "Prefix", Value = "before_" },
                    new ViewProperty { Name = "Suffix", Value = "_after" },
                    new ViewProperty { Name = "Total", Value = "20" },
                    version
                };

            var addAction = Proxy.DoCommand(ShopsContainer.DoAction(addView));
            addAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            addAction.Models.OfType<PrivateCouponGroupAdded>().FirstOrDefault().Should().NotBeNull();
            _privateCouponGroupId = addAction.Models.OfType<PrivateCouponGroupAdded>().FirstOrDefault()?.GroupFriendlyId;
            _privateCouponGroupId.Should().NotBeNullOrEmpty();
            var message = addAction.Messages.FirstOrDefault(m => m.Code.Equals("information", StringComparison.OrdinalIgnoreCase));
            message.Should().NotBeNull();

            ConsoleExtensions.WriteColoredLine(ConsoleColor.Green, message.Text);
        }

        private static void AddPrivateCouponAgain()
        {
            Console.WriteLine("Begin Error Test: AddPrivateCouponAgain");

            var addView = Proxy.GetValue(ShopsContainer.GetEntityView(_promotionId, "CouponDetails", "AddPrivateCoupon", string.Empty));
            addView.Should().NotBeNull();
            addView.Policies.Should().BeEmpty();
            addView.Properties.Should().NotBeEmpty();
            addView.Properties.Count.Should().Be(4); // Version, Prefix, Suffix & Total
            addView.ChildViews.Should().BeEmpty();
            var version = addView.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            addView.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "Prefix", Value = "before_" },
                    new ViewProperty { Name = "Suffix", Value = "_after" },
                    new ViewProperty { Name = "Total", Value = "20" },
                    version
                };

            var addAction = Proxy.DoCommand(ShopsContainer.DoAction(addView));
            ConsoleExtensions.WriteWarningLine("Expecting private group exists error");
            addAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            addAction.Models.OfType<PrivateCouponGroupAdded>().FirstOrDefault().Should().BeNull();
        }

        private static void NewAllocation()
        {
            Console.WriteLine("Begin NewAllocation");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PrivateCouponGroup-" + _privateCouponGroupId, "AllocationDetails", "NewAllocation", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.Properties.Count.Should().Be(2); // Version & Count
            view.ChildViews.Should().BeEmpty();
            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            view.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "Count", Value = "15" },
                    version
                };

            var newAllocationAction = Proxy.DoCommand(ShopsContainer.DoAction(view));
            newAllocationAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            newAllocationAction.Models.OfType<PrivateCouponList>().FirstOrDefault().Should().NotBeNull();
            var privateCouponList = newAllocationAction.Models.OfType<PrivateCouponList>().FirstOrDefault();
            privateCouponList.Should().NotBeNull();
            privateCouponList.GroupFriendlyId.Should().NotBeNullOrEmpty();
            privateCouponList.GroupFriendlyId.Should().Be(_privateCouponGroupId);

            ConsoleExtensions.WriteColoredLine(ConsoleColor.Green, "Allocated the following coupon codes");
            foreach (var code in privateCouponList.CouponCodes)
            {
                Console.WriteLine(code);
            }
        }

        private static void AllocateTooMuch()
        {
            Console.WriteLine("Begin Error Test: AllocateTooMuch");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView("Entity-PrivateCouponGroup-" + _privateCouponGroupId, "AllocationDetails", "NewAllocation", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.Properties.Count.Should().Be(2); // Version & Count
            view.ChildViews.Should().BeEmpty();
            var version = view?.Properties.FirstOrDefault(p => p.Name.Equals("Version"));
            view.Properties = new ObservableCollection<ViewProperty>
                {
                    new ViewProperty { Name = "Count", Value = "15" },
                    new ViewProperty { Name = "Version", Value = "2" },
                };

            var newAllocationAction = Proxy.DoCommand(ShopsContainer.DoAction(view));
            ConsoleExtensions.WriteWarningLine("Expecting total exceeds the max allocation available error");
            newAllocationAction.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            newAllocationAction.Models.OfType<PrivateCouponList>().FirstOrDefault().Should().BeNull();
        }

        #endregion
    }
}
