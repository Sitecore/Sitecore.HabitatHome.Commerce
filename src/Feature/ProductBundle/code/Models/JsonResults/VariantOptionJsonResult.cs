using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.ProductBundle.Models.JsonResults
{
    public class VariantOptionJsonResult
    {
        public string Label { get; set; }       
        public List<RelatedProductVariantJsonResult> Options { get; set; }
    }
}