using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Common.Entities;
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Models.JsonResults
{
    public class RelatedProductJsonResult
    {
        public string ProductId { get; set; }
        public string Image { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }        
        public List<VariantDefinitionEntity> VariantDefinitionList { get; set; }
        public List<VariantOptionJsonResult> VariantOptions { get; set; }
        public RelatedProductJsonResult()
        {            
            this.VariantDefinitionList = new List<VariantDefinitionEntity>();
            this.VariantOptions = new List<VariantOptionJsonResult>();
        }

        public List<RelatedProductVariantJsonResult> GetDistinctVariantPropertyValues(List<Item> variantItems, string propertyName)
        {
            List<RelatedProductVariantJsonResult> valueList = new List<RelatedProductVariantJsonResult>();
            List<string> optionList = new List<string>();
            variantItems.ForEach(variant => optionList.Add(variant[propertyName]));
            optionList = optionList.Distinct<string>().ToList<string>();
            optionList = optionList.Where<string>((Func<string, bool>)(x => !string.IsNullOrWhiteSpace(x))).ToList<string>();
            variantItems.Where(v => !String.IsNullOrEmpty(v[propertyName])).ForEach(variant =>  valueList.Add(new RelatedProductVariantJsonResult
            {
                DisplayName = variant[propertyName],
                VariantId = variant.Name,                
            }));
            return valueList;
        }
    }
}