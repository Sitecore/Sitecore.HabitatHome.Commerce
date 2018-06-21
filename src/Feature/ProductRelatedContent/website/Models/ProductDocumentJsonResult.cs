using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductRelatedContent.Models
{
    public class ProductDocumentJsonResult
    {
        public string DocumentName { get; set; }
        public string ImageUrl { get; set; }
        public string DocumentUrl { get; set; }
        public string Description { get; set; }
        public string DocumentType { get; set; }
    }
}