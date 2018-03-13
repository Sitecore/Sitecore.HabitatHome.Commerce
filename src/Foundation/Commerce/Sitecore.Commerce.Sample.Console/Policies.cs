namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Policies
    {
        private static Sitecore.Commerce.Engine.Container ShopsContainer = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Policies");

            AddUpdatePolicySet();
            RemovePolicy();
            GetPolicySet();
                        
            watch.Stop();

            Console.WriteLine($"End Policies :{watch.ElapsedMilliseconds} ms");
        }

        private static void AddUpdatePolicySet()
        {
            var result = Proxy.DoCommand(
              ShopsContainer.AddPolicy(
                   "Entity-PolicySet-GlobalCartPolicies",
                   "Sitecore.Commerce.Plugin.Availability.AvailabilityAlwaysPolicy, Sitecore.Commerce.Plugin.Availability",
                   new AvailabilityAlwaysPolicy
                   {
                       PolicyId = "AvailabilityAlways"
                   }));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            result.Models.OfType<PolicyAddedModel>().Any().Should().BeTrue();
        }

        private static void RemovePolicy()
        {
            var result =
                Proxy.DoCommand(
                    ShopsContainer.RemovePolicy(
                        "Entity-PolicySet-GlobalCartPolicies",
                        "Sitecore.Commerce.Plugin.Availability.AvailabilityAlwaysPolicy, Sitecore.Commerce.Plugin.Availability",
                        string.Empty));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void GetPolicySet()
        {
            var result = ShopsContainer.PolicySets.ByKey("DoesNotExist").GetValue();

            result.Should().NotBeNull();
        }
    }
}
