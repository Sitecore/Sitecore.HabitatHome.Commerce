namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using FluentAssertions;

    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class Search
    {
        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new CsrSheila().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin Search Indexing");
            
            var view = Sitecore.Commerce.ServiceProxy.Proxy.GetEntityView(ShopsContainer, string.Empty, "Indexes", string.Empty, string.Empty);
            view.Should().NotBeNull();
            view.ChildViews.Should().NotBeNull();
            view.ChildViews.Should().NotBeEmpty();
            foreach (var entityView in view.ChildViews.OfType<EntityView>())
            {
                view = new EntityView { Name = "Index", DisplayName = "Index", EntityId = entityView.EntityId, Action = "DeleteSearchIndex" };
                var result = Sitecore.Commerce.ServiceProxy.Proxy.DoCommand(ShopsContainer.DoAction(view));
                result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            }

            // run minions
            var policies =
                new Collection<CommerceOps.Sitecore.Commerce.Core.Policy>
                    {
                        new CommerceOps.Sitecore.Commerce.Core.RunMinionPolicy { RunChildren = false }
                    };
            var minionResult = Proxy.GetValue(new MinionRunner().Context.MinionsContainer()
                .RunMinion("Sitecore.Commerce.Plugin.Search.FullIndexMinion, Sitecore.Commerce.Plugin.Search", "AdventureWorksMinions", policies));
            minionResult.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            minionResult = Proxy.GetValue(new MinionRunner().Context.MinionsContainer()
                .RunMinion("Sitecore.Commerce.Plugin.Search.FullIndexMinion, Sitecore.Commerce.Plugin.Search", "HabitatMinions", policies));
            minionResult.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            watch.Stop();

            Console.WriteLine($"End Search Indexing :{watch.ElapsedMilliseconds} ms");
        }
    }
}

