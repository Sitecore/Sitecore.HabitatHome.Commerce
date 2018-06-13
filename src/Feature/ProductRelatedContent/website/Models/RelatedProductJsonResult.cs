using Sitecore.Commerce.XA.Feature.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductRelatedContent.Models
{
    public class RelatedProductJsonResult
    {
        public string ProductName { get; set; }
        public string ImageSrc { get; set; }
        public string ProductUrl { get; set; }
        public string ListPrice { get; set; }
    }
}