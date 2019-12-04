using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Engine.Connect.Pipelines.Carts;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Plugin.Carts;

namespace Sitecore.HabitatHome.Feature.Cart.Pipelines
{
    public class TranslateCartLineToEntity : Sitecore.Commerce.Engine.Connect.Pipelines.Carts.TranslateCartLineToEntity
    {
        public TranslateCartLineToEntity(IEntityFactory entityFactory)
            : base(entityFactory)
        {
        }

        protected override void TranslateProduct(
            TranslateCartLineToEntityRequest request,
            CartLineComponent source,
            CommerceCartLine destination,
            bool isSubLine = false)
        {
            base.TranslateProduct(request, source, destination, isSubLine);

            if (destination == null || destination.Product == null)
                return;

            if (source.CartLineComponents != null && !string.IsNullOrEmpty(source.ItemId))
            {
                CartProductComponent productComponent = source.CartLineComponents.OfType<CartProductComponent>().FirstOrDefault();
                if (productComponent != null)
                {
                    destination.SetPropertyValue("ItemType", string.IsNullOrEmpty(productComponent.ItemType) ? null : productComponent.ItemType);
                }
            }
        }
    }
}