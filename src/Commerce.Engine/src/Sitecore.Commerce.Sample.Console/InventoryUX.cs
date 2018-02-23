namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Sitecore.Commerce.Engine;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Sample.Contexts;
    using Sitecore.Commerce.ServiceProxy;

    public static class InventoryUX
    {
        private const string InventorySetName =
            "ConsoleInventorySet";

        private static readonly string InventorySetId =
            $"Entity-InventorySet-{InventorySetName}";

        private static readonly Container ShopsContainer 
            = new AnonymousCustomerJeff().Context.ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin InventoryUX");
            
            AddInventorySet();
            EditInventorySet();

            watch.Stop();

            Console.WriteLine($"End InventoryUX :{watch.ElapsedMilliseconds} ms");
        }

        private static void AddInventorySet()
        {
            Console.WriteLine("Begin AddInventorySet");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "Details", "AddInventorySet", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();
            
            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "Name", Value = InventorySetName},
                new ViewProperty {Name = "DisplayName", Value = "Console UX Inventory Set"},
                new ViewProperty {Name = "Description", Value = "Console UX Inventory Set Description"}
            };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void EditInventorySet()
        {
            Console.WriteLine("Begin EditInventorySet");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(InventorySetId, "Details", "EditInventorySet", string.Empty));
            view.Should().NotBeNull();
            view.Policies.Should().BeEmpty();
            view.Properties.Should().NotBeEmpty();
            view.ChildViews.Should().BeEmpty();

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Properties = new ObservableCollection<ViewProperty>
            {
                new ViewProperty {Name = "DisplayName", Value = "Console UX Inventory Set (updated)"},
                new ViewProperty {Name = "Description", Value = "Console UX Inventory Set Description"},
                version
            };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
    }
}
