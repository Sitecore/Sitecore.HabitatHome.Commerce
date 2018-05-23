using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductKit.Models.JsonResults
{
    public class RelatedProductJsonResult
    {
        public string Image { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}