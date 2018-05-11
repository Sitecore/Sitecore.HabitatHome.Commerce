using Sitecore.Commerce.XA.Feature.Catalog.Models;   
using System.Linq;                
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Providers;          
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Data.Items;
using System.Collections.Generic;

namespace Sitecore.Feature.Catalog.Models
{
    public class PurchasableProductSummaryViewModel : ProductSummaryViewModel
    {
        public PurchasableProductSummaryViewModel(IStorefrontContext storefrontContext, IItemTypeProvider itemTypeProvider, IModelProvider modelProvider, IVariantDefinitionProvider variantDefinitionProvider, ISiteContext siteContext) : base(storefrontContext, itemTypeProvider, modelProvider, variantDefinitionProvider, siteContext)
        {
        }

        public string VariantId { get; set; }

        public bool IsPurchasable { get; set; } 

        public new void Initialize(ProductEntity product, bool initializeAsMock = false)
        {
            base.Initialize(product, initializeAsMock);

            List<VariantEntity> variants = new List<VariantEntity>();
            if (product.Item.HasChildren)
            {
                foreach (Item item in product.Item.Children)
                {
                    VariantEntity model = this.ModelProvider.GetModel<VariantEntity>();
                    model.Initialize(item);
                    variants.Add(model);
                }
            }    
            else
            {
                // no variants - product is purchasable as self
                IsPurchasable = true;
            }

            if(variants.Count == 1)
            {
                // max 1 variant, so this product is purchasable through its only variant        
                VariantId = variants.Select(a => a.VariantId).FirstOrDefault();
                IsPurchasable = true;
            }   
            else
            {
                VariantId = null;
                IsPurchasable = false;
            }
        }

    }
}