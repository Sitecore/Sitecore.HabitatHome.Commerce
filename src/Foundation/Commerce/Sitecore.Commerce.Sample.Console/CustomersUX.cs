namespace Sitecore.Commerce.Sample.Console
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Core;
    using EntityViews;
    using Extensions;
    using FluentAssertions;
    using Microsoft.OData.Client;

    using Sitecore.Commerce.Engine;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.ServiceProxy;

    public static class CustomersUX
    {
        private static string _customerId;
        private static string _customerEmail;
        private static string _addressId;

        private static readonly Sitecore.Commerce.Engine.Container ShopsContainer = new ShopperContext().ShopsContainer();

        public static void RunScenarios()
        {
            var watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Begin CustomersUX");

            try
            {
                GenerateRandomEmail();
                AddCustomer();
                EditCustomer();
                AddAddress();
                EditAddress();
                RemoveAddress();
                RemoveCustomer();               
            }
            catch (Exception ex)
            {
                ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, $"Exception in Scenario 'CustomersUX' (${ex.Message}) : Stack={ex.StackTrace}");
            }

            watch.Stop();

            Console.WriteLine($"End CustomersUX :{watch.ElapsedMilliseconds} ms");
        }
        
        public static string GenerateRandomEmail()
        {
            var random = new Random().Next(10, 999);
            _customerEmail = "jane" + random + "@doe.com";

            return _customerEmail;
        }

        public static Customer GetCustomer(Container container, string customerId)
        {
            try
            {
                var customer = Proxy.GetValue(
                    container.Customers.ByKey(customerId).Expand("Components($expand=ChildComponents)"));

                return customer;
            }
            catch (DataServiceQueryException ex)
            {
                Console.WriteLine($"Exception Retrieving Customer: {ex} OrderId:{customerId}");
                return null;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"Exception Retrieving Customer: {ex} OrderId:{customerId}");
                return null;
            }
        }

        #region Customer

        public static string AddCustomer(string email = "", ShopperContext context = null)
        {
            Console.WriteLine("Begin CustomerDetails for Add View");

            var container = context != null ? context.ShopsContainer() : ShopsContainer;
            var view = Proxy.GetValue(ShopsContainer.GetEntityView(string.Empty, "Details", "AddCustomer", string.Empty));
            view.Should().NotBeNull();            
            view.Properties.Should().NotBeEmpty();

            view.Action.Should().Be("AddCustomer");
            view.Properties.Should().NotBeEmpty();
            view.Properties.FirstOrDefault(p => p.Name.Equals("Email")).Value = string.IsNullOrEmpty(email) ? _customerEmail : email;
            view.Properties.FirstOrDefault(p => p.Name.Equals("Password")).Value = "Password01";

            var action = Proxy.DoCommand(container.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties.FirstOrDefault(p => p.Name.Equals("AccountStatus")).Value = "ActiveAccount";

            action = Proxy.DoCommand(container.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase) || 
                                     m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            action.Models.OfType<CustomerAdded>().FirstOrDefault().Should().NotBeNull();
            _customerId = action.Models.OfType<CustomerAdded>().FirstOrDefault()?.CustomerId;

            return _customerId;
        }

        private static void EditCustomer()
        {
            Console.WriteLine("Begin CustomerDetails for Edit View");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_customerId, "Details", "EditCustomer", string.Empty));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            view.Action.Should().Be("EditCustomer");
            view.Properties.Should().NotBeEmpty();
            
            view.Properties.FirstOrDefault(p => p.Name.Equals("Language")).Value = "fr-FR";
            view.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = "Jane";
            view.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = "Doe";
            view.Properties.FirstOrDefault(p => p.Name.Equals("IncludedTags")).Value = "['First Tag', 'Second Tag']";

            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase) || 
                                     m.Code.Equals("validationerror", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();

            var customer = CustomersUX.GetCustomer(ShopsContainer, _customerId);
            customer.Should().NotBeNull();
            customer.Tags.Count.Should().Be(2);
            customer.Components.OfType<CustomerDetailsComponent>().Should().NotBeEmpty();
            customer.Components.OfType<CustomerDetailsComponent>().FirstOrDefault().View.ChildViews.Should().NotBeEmpty();
            customer.Components.OfType<CustomerDetailsComponent>().FirstOrDefault().View.ChildViews.FirstOrDefault().Should().BeOfType<EntityView>();
            var details = customer.Components.OfType<CustomerDetailsComponent>().FirstOrDefault().View.ChildViews.FirstOrDefault() as EntityView;
            details.Properties.Should().NotBeEmpty();
        }
        
        private static void RemoveCustomer()
        {
            Console.WriteLine("Begin RemoveCustomer");           

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_customerId, "Details", string.Empty, string.Empty));

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "RemoveCustomer";
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      version
                                  };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion

        #region Address        

        private static void AddAddress()
        {
            Console.WriteLine("Begin AddressDetails for Add");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_customerId, "AddressDetails", "SelectAddressCountry", string.Empty));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();
            view?.Policies.Should().BeEmpty();
            view.Action.Should().Be("GetCountryRegionsForCustomers");

            view.Properties.FirstOrDefault(p => p.Name.Equals("Country")).Value = "CA";
            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();           

            view = action.Models.OfType<EntityView>().FirstOrDefault(v => v.Name.Equals(view.Name));
            view.Should().NotBeNull();
            view?.Policies.Should().BeEmpty();
            view?.Properties.Should().NotBeEmpty();
            view?.Action.Should().Be("AddAddress");
            view.Properties.FirstOrDefault(p => p.Name.Equals("AddressName")).Value = "Home";
            view.Properties.FirstOrDefault(p => p.Name.Equals("State")).Value = "ON";

            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.EndsWith("error", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            ConsoleExtensions.WriteColoredLine(ConsoleColor.Yellow, "Expected error");

            view.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = "first name";
            view.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = "last name";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address1")).Value = "123 street";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address2")).Value = string.Empty;
            view.Properties.FirstOrDefault(p => p.Name.Equals("City")).Value = "city";
            view.Properties.FirstOrDefault(p => p.Name.Equals("ZipPostalCode")).Value = "postalCode";
            view.Properties.FirstOrDefault(p => p.Name.Equals("PhoneNumber")).Value = "phoneNumber";
            view.Properties.FirstOrDefault(p => p.Name.Equals("IsPrimary")).Value = "true";
            action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.EndsWith("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
            action.Models.OfType<CustomerAddressAdded>().FirstOrDefault().Should().NotBeNull();
            _addressId = action.Models.OfType<CustomerAddressAdded>().FirstOrDefault()?.AddressId;         
        }

        private static void EditAddress()
        {
            Console.WriteLine("Begin AddressDetails for Edit View");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_customerId, "AddressDetails", "EditAddress", _addressId));
            view.Should().NotBeNull();
            view.Properties.Should().NotBeEmpty();

            view.Action.Should().Be("EditAddress");
            view.Properties.FirstOrDefault(p => p.Name.Equals("AddressName")).Value = "Home";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Country")).Value = "CA";
            view.Properties.FirstOrDefault(p => p.Name.Equals("State")).Value = "ON";
            view.Properties.FirstOrDefault(p => p.Name.Equals("FirstName")).Value = "Jane";
            view.Properties.FirstOrDefault(p => p.Name.Equals("LastName")).Value = "Doe";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address1")).Value = "123 street";
            view.Properties.FirstOrDefault(p => p.Name.Equals("Address2")).Value = "apt 3";
            view.Properties.FirstOrDefault(p => p.Name.Equals("City")).Value = "city";
            view.Properties.FirstOrDefault(p => p.Name.Equals("ZipPostalCode")).Value = "postalCode";
            view.Properties.FirstOrDefault(p => p.Name.Equals("PhoneNumber")).Value = "phoneNumber";
            view.Properties.FirstOrDefault(p => p.Name.Equals("IsPrimary")).Value = "false";

            var action = Proxy.DoCommand(ShopsContainer.DoAction(view));
            action.Messages.Any(m => m.Code.EndsWith("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        private static void RemoveAddress()
        {
            Console.WriteLine("Begin RemoveAddress");

            var view = Proxy.GetValue(ShopsContainer.GetEntityView(_customerId, "Details", string.Empty, _addressId));

            var version = view.Properties.FirstOrDefault(p => p.Name.Equals("Version"));

            view.Action = "RemoveAddress";
            view.Properties = new ObservableCollection<ViewProperty>
                                  {
                                      version
                                  };

            var result = Proxy.DoCommand(ShopsContainer.DoAction(view));
            result.Messages.Any(m => m.Code.Equals("error", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }

        #endregion
    }
}
