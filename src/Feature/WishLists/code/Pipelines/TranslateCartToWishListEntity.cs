using Sitecore.Commerce.Core;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Feature.WishLists.Pipelines.Arguments;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Data;
using Sitecore.Commerce.Plugin.Catalog;


namespace Sitecore.Feature.WishLists.Pipelines
{

    public class TranslateCartToWishListEntity // : TranslateODataEntityToEntity<TranslateCartToEntityRequest, TranslateCartToWishListEntityResult, Sitecore.Commerce.Plugin.Carts.Cart, WishList>
    {
        public TranslateCartToWishListEntity(IEntityFactory entityFactory)
          //: base(entityFactory)
        {
        }

        protected void Translate(TranslateCartToEntityRequest request, Sitecore.Commerce.Plugin.Carts.Cart source, WishList destination)
        {
            //base.Translate(request, source, destination);
            destination.ExternalId = source.Id;
            destination.Name = source.Name;
            destination.ShopName = source.ShopName;
            if (source.Components != null && source.Components.Any<Component>())
            {
                ContactComponent contactComponent = source.Components.OfType<ContactComponent>().FirstOrDefault<ContactComponent>();
                if (contactComponent != null)
                {
                    // destination.Email = contactComponent.Email ?? string.Empty;
                    destination.UserId = contactComponent.ShopperId ?? string.Empty;
                    destination.CustomerId = string.IsNullOrEmpty(contactComponent.CustomerId) ? contactComponent.ShopperId ?? string.Empty : contactComponent.CustomerId;
                }
            }
            
            this.TranslateLines(request, source, destination);            
        }

        //protected override WishList GetTranslateDestination(TranslateCartToEntityRequest request)
        //{
        //    return this.EntityFactory.Create<WishList>("Cart");
        //}

        protected virtual void TranslateLines(TranslateCartToEntityRequest request, Sitecore.Commerce.Plugin.Carts.Cart source, WishList destination)
        {
            Assert.ArgumentNotNull((object)request, nameof(request));
            Assert.ArgumentNotNull((object)source, nameof(source));
            Assert.ArgumentNotNull((object)destination, nameof(destination));
            List<WishListLine> resultWishlist = new List<WishListLine>();


            if (source.Lines != null)
            {
                foreach(var lineItem in source.Lines)
                {
                    var wishListLine = new WishListLine() { ExternalId = lineItem.Id, Product = new CartProduct() };                    

                    if (lineItem.CartLineComponents != null && !string.IsNullOrEmpty(lineItem.ItemId))
                    {
                        CartProductComponent productComponent = lineItem.CartLineComponents.OfType<CartProductComponent>().FirstOrDefault<CartProductComponent>();
                        var product = new CartProduct();
                        if (productComponent != null)
                        {
                           
                           // string[] strArray = source.ItemId.Split("|".ToCharArray());
                            //product.ProductCatalog = strArray[0];
                            product.ProductId = productComponent.Id;
                            product.ProductName = string.IsNullOrEmpty(productComponent.ProductName) ? productComponent.DisplayName : productComponent.ProductName;
                            destination.SetPropertyValue("_product_Images", productComponent.Image == null || string.IsNullOrEmpty(productComponent.Image.SitecoreId) ? (object)string.Empty : (object)productComponent.Image.SitecoreId);
                            product.SetPropertyValue("Color", string.IsNullOrEmpty(productComponent.Color) ? (object)(string)null : (object)productComponent.Color);
                            product.SetPropertyValue("Size", string.IsNullOrEmpty(productComponent.Size) ? (object)(string)null : (object)productComponent.Size);
                            product.SetPropertyValue("Style", string.IsNullOrEmpty(productComponent.Style) ? (object)(string)null : (object)productComponent.Style);
                            ID result;
                            if (!string.IsNullOrEmpty(productComponent.ExternalId) && ID.TryParse(productComponent.ExternalId, out result))
                                product.SitecoreProductItemId = result.ToGuid();
                        }

                        ItemVariationSelectedComponent selectedComponent = lineItem.CartLineComponents.OfType<ItemVariationSelectedComponent>().FirstOrDefault<ItemVariationSelectedComponent>();
                        if (selectedComponent != null)
                            product.ProductId = productComponent.Id + "|" + selectedComponent.VariationId;

                        if(lineItem.UnitListPrice != null)
                        {
                            product.Price = new Price(lineItem.UnitListPrice.Amount, lineItem.UnitListPrice.CurrencyCode);
                        }

                        wishListLine.Product = product;
                        wishListLine.Quantity = lineItem.Quantity;                        
                    }

                    resultWishlist.Add(wishListLine);
                }
            }
            
        }
       
    }
}