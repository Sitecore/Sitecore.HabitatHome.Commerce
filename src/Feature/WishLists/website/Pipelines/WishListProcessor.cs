using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Engine.Connect.DataProvider;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Commerce.Services;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Cart = Sitecore.Commerce.Plugin.Carts.Cart;

namespace Sitecore.HabitatHome.Feature.WishLists.Pipelines
{
    public class WishListProcessor : PipelineProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
        }                 

        protected virtual Cart GetWishList(string userId, string shopName, string cartId, string customerId = "", string currency = "")
        {
            return Proxy.GetValue(GetContainer(shopName, userId, customerId, "", currency, new DateTime?()).Carts.ByKey(cartId).Expand("Lines($expand=CartLineComponents),Components"));
        }

        protected virtual CommerceCommand AddWishListLine(string userId, string shopName, string wishListId, string itemId, Decimal quantity, string customerId = "", string currency = "")
        {
            return Proxy.DoCommand(GetContainer(shopName, userId, customerId, currency, "", new DateTime?()).AddWishListLineItem(wishListId, itemId, quantity));
        }             

        protected virtual CommerceCommand RemoveWishListLine(string userId, string shopName, string wishListId, string lineId, string customerId = "", string currency = "")
        {
            return Proxy.DoCommand(GetContainer(shopName, userId, customerId, "", currency, new DateTime?()).RemoveWishListLineItem(wishListId, lineId));
        }                                      

        internal WishList TranslateCartToWishListEntity(Cart cart, ServiceProviderResult currentResult)
        {                                                     
            WishList result = new WishList();
            Translate(cart, result);

            return result;
        }           

        protected void Translate(Cart source, WishList destination)
        {            
            destination.ExternalId = source.Id;
            destination.Name = source.Name;
            destination.ShopName = source.ShopName;
            if (source.Components != null && source.Components.Any())
            {
                ContactComponent contactComponent = source.Components.OfType<ContactComponent>().FirstOrDefault();
                if (contactComponent != null)
                {
                    // destination.Email = contactComponent.Email ?? string.Empty;
                    destination.UserId = contactComponent.ShopperId ?? string.Empty;
                    destination.CustomerId = string.IsNullOrEmpty(contactComponent.CustomerId) ? contactComponent.ShopperId ?? string.Empty : contactComponent.CustomerId;
                }
            }

            destination.Lines = TranslateLines(source, destination);
        }

        protected List<WishListLine> TranslateLines(Cart source, WishList destination)
        {                  
            List<WishListLine> resultWishlist = new List<WishListLine>();
                      
            if (source.Lines != null)
            {
                foreach (var lineItem in source.Lines)
                {
                    var wishListLine = new WishListLine
                        {
                            ExternalId = lineItem.Id,
                            Product = new CartProduct()
                        };

                    if (lineItem.CartLineComponents != null && !string.IsNullOrEmpty(lineItem.ItemId))
                    {
                        CartProductComponent productComponent = lineItem.CartLineComponents.OfType<CartProductComponent>().FirstOrDefault();
                        var product = new CommerceCartProduct();
                        if (productComponent != null)
                        {                                           
                            string[] array = lineItem.ItemId.Split("|".ToCharArray());
                            product.ProductCatalog = array[0];
                            product.ProductId = array[1];         
                            product.ProductName = string.IsNullOrEmpty(productComponent.ProductName) ? productComponent.DisplayName : productComponent.ProductName;
                            product.SitecoreProductItemId = GetSitecoreItemId(array[1], array[2]);
                            destination.SetPropertyValue("_product_Images", productComponent.Image == null || string.IsNullOrEmpty(productComponent.Image.SitecoreId) ? string.Empty : productComponent.Image.SitecoreId);
                            product.SetPropertyValue("Image", productComponent.Image == null || string.IsNullOrEmpty(productComponent.Image.SitecoreId) ? string.Empty : productComponent.Image.SitecoreId);
                            product.SetPropertyValue("Color", string.IsNullOrEmpty(productComponent.Color) ? null : productComponent.Color);
                            product.SetPropertyValue("Size", string.IsNullOrEmpty(productComponent.Size) ? null : productComponent.Size);
                            product.SetPropertyValue("Style", string.IsNullOrEmpty(productComponent.Style) ? null : productComponent.Style);

                            //if (!string.IsNullOrEmpty(productComponent.ExternalId) &&
                            //    ID.TryParse(productComponent.ExternalId, out var result))
                            //{
                            //    product.SitecoreProductItemId = result.ToGuid();
                            //}               

                            ItemVariationSelectedComponent selectedComponent = lineItem.CartLineComponents.OfType<ItemVariationSelectedComponent>().FirstOrDefault();
                            if (selectedComponent != null)
                            {
                                product.ProductId = productComponent.Id + "|" + selectedComponent.VariationId;
                                product.ProductVariantId = selectedComponent.VariationId;
                            }  
                        }

                        if (lineItem.UnitListPrice != null)
                        {
                            product.Price = new Price(lineItem.UnitListPrice.Amount, lineItem.UnitListPrice.CurrencyCode);
                        }

                        wishListLine.Product = product;
                        wishListLine.Quantity = lineItem.Quantity;
                    }

                    resultWishlist.Add(wishListLine);
                }
            }

            return resultWishlist;
        }                               

        internal void ValidateArguments<TRequest, TResult>(ServicePipelineArgs args, out TRequest request, out TResult result) where TRequest : ServiceProviderRequest where TResult : ServiceProviderResult
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Request.RequestContext, "args.Request.RequestContext");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            request = args.Request as TRequest;
            result = args.Result as TResult;
            Assert.IsNotNull(request, "The parameter args.Request was not of the expected type.  Expected {0}.  Actual {1}.", (object) typeof (TRequest).Name, (object) args.Request.GetType().Name);
            Assert.IsNotNull(result, "The parameter args.Result was not of the expected type.  Expected {0}.  Actual {1}.", (object) typeof (TResult).Name, (object) args.Result.GetType().Name);
        }

        protected virtual Guid GetSitecoreItemId(string productId, string variantId)
        {
            CatalogRepository catalogRepository = new CatalogRepository();
            string text = "Entity-SellableItem-" + productId;
            if (!string.IsNullOrEmpty(variantId))
            {
                text = text + "|" + variantId;
            }
            if (catalogRepository.GetSitecoreIdFromMappings(text) != null)
            {
                return Guid.Parse(catalogRepository.GetSitecoreIdFromMappings(text));
            }
            return Guid.Empty;
        }

        internal List<WishListLine> SetListLines(WishList wishList)
        {
            List<WishListLine> lines = new List<WishListLine>();
            foreach (var line in wishList.Lines)
            {
                lines.Add(line);
            }
            return lines;
        }

        internal SystemMessage CreateSystemMessage(Exception ex)
        {
            SystemMessage systemMessage1 = new SystemMessage
            {
                Message = ex.Message
            };
            if (ex.InnerException != null && !ex.Message.Equals(ex.InnerException.Message, StringComparison.OrdinalIgnoreCase))
            {
                SystemMessage systemMessage2 = systemMessage1;
                string str = systemMessage2.Message + " - " + ex.InnerException.Message;
                systemMessage2.Message = str;
            }
            return systemMessage1;
        }
    }  
}