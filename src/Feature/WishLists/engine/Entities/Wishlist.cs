using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities
{
    public class WishList : CommerceEntity
    {
        public WishList()
        {
            this.Lines = (IList<CartLineComponent>)new List<CartLineComponent>();
            this.Components = (IList<Component>)new List<Component>();
            //this.Adjustments = (IList<AwardedAdjustment>)new List<AwardedAdjustment>();
            //this.Totals = new Totals()
            //{
            //    SubTotal = new Money(Decimal.Zero),
            //    AdjustmentsTotal = new Money(Decimal.Zero),
            //    GrandTotal = new Money(Decimal.Zero)
            //};
        }

        public WishList(string id)
          : this()
        {
            this.Id = id;
        }

        [StringLength(50)]
        public string ShopName { get; set; }
        public bool IsFavorite { get; set; }
        public string CustomerId { get; set; }

        public int ItemCount { get; set; }        

        [Contained]
        public IList<CartLineComponent> Lines { get; set; }

        //public Totals Totals { get; set; }

        //[Contained]
        //public IList<AwardedAdjustment> Adjustments { get; set; }
    }
}
