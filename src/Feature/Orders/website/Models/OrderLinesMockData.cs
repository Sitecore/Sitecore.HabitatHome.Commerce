using System;
using System.Collections.Generic;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using Sitecore.Commerce.Entities.Prices;
using Sitecore.Data;

namespace Sitecore.HabitatHome.Feature.Orders.Models
{
    internal static class OrderLinesMockData
    {
        public static readonly ID MockProductId = new ID("{6568ABD5-1A23-4AC8-9F1D-9A8CDAF9E104}");
        internal const string PartyId = "1";

        internal static OrderLinesViewModel InitializeMockData(OrderLinesViewModel model)
        {
            Order mockOrder = OrderLinesMockData.CreateMockOrder();
            model.Initialize(mockOrder);
            return model;
        }

        internal static Order CreateMockOrder()
        {
            Order order = new Order();
            order.OrderDate = DateTime.MinValue;
            order.OrderID = "####";
            order.Lines = OrderLinesMockData.CreateMockLines();
            order.Parties = new List<Party>((IEnumerable<Party>)new Party[1]
            {
        OrderLinesMockData.CreateMockAddress()
            });
            order.Shipping = new List<ShippingInfo>((IEnumerable<ShippingInfo>)new ShippingInfo[1]
            {
        OrderLinesMockData.CreateMockShippingInfo()
            });
            return order;
        }

        internal static List<CartLine> CreateMockLines()
        {
            List<CartLine> cartLineList = new List<CartLine>();
            for (int index = 1; index < 6; ++index)
            {
                CartProduct cartProduct = new CartProduct()
                {
                    ProductName = "Lorem Ipsum",
                    ProductId = "12345",
                    SitecoreProductItemId = OrderLinesMockData.MockProductId.ToGuid(),
                    Price = new Price(new Decimal(0, 0, 0, false, (byte)2), "USD")
                };
                cartProduct.SetPropertyValue("Color", (object)"dolor sit amet");
                cartProduct.SetPropertyValue("Size", (object)"dolor sit amet");
                cartProduct.SetPropertyValue("Style", (object)"dolor sit amet");
                CartLine cartLine1 = new CartLine();
                cartLine1.Product = cartProduct;
                cartLine1.ExternalCartLineId = index.ToString((IFormatProvider)Context.Culture);
                cartLine1.LineNumber = index;
                cartLine1.Quantity = Decimal.One;
                cartLine1.Total = new Total()
                {
                    Amount = new Decimal(0, 0, 0, false, (byte)2),
                    CurrencyCode = "USD"
                };
                cartLine1.Adjustments = new List<CartAdjustment>((IEnumerable<CartAdjustment>)new CartAdjustment[1]
                {
          new CartAdjustment()
          {
            Amount = new Decimal(1, 0, 0, false, (byte) 2),
            LineNumber = index,
            Description = "Curabitur venenatis"
          }
                });
                CartLine cartLine2 = cartLine1;
                cartLine2.SetPropertyValue("_product_Images", (object)OrderLinesMockData.MockProductId);
                cartLineList.Add(cartLine2);
            }
            return new List<CartLine>((IEnumerable<CartLine>)cartLineList);
        }

        internal static Party CreateMockAddress()
        {
            Party party = new Party();
            party.PartyId = "1";
            party.ExternalId = "1";
            party.FirstName = "Aenean";
            party.LastName = "blandit";
            party.Address1 = "Mauris eget lacus sed dolor viverra";
            party.State = "In gravida";
            party.City = "Etiam";
            party.Country = "Nam pulvinar";
            party.ZipPostalCode = "99999";
            return party;
        }

        internal static ShippingInfo CreateMockShippingInfo()
        {
            ShippingInfo shippingInfo1 = new ShippingInfo();
            shippingInfo1.LineIDs = new List<string>((IEnumerable<string>)new string[5]
            {
        "1",
        "2",
        "3",
        "4",
        "5"
            });
            shippingInfo1.ExternalId = "1";
            shippingInfo1.PartyID = "1";
            ShippingInfo shippingInfo2 = shippingInfo1;
            shippingInfo2.SetPropertyValue("ShippingMethodName", (object)"Etiam rhoncus");
            return shippingInfo2;
        }
    }
}