using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Foundation.Promotions.Models
{
    public class Promotion
    {
        public string Id { get; set; }
        public string DisplayCartText { get; set; }
        public List<dynamic> Qualifications { get; set; }
        public List<dynamic> Benefits { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public Promotion()
        {
            this.ValidFrom = new DateTime();
            this.ValidTo = new DateTime();
            this.Qualifications = new List<dynamic>();
            this.Benefits = new List<dynamic>();
        }
    }
}