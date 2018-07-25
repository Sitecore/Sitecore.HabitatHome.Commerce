using System.Collections.Generic;
using System.Web.Script.Serialization;
using Sitecore.Commerce.Entities.WishLists;   
using Sitecore.Commerce.XA.Foundation.Common.Context;            
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Diagnostics;

namespace Sitecore.HabitatHome.Feature.WishLists.Models.JsonResults
{
    public class WishListJsonResult : BaseJsonResult
    {
        public WishListJsonResult(IContext context, IStorefrontContext storefrontContext, IModelProvider modelProvider)
      : base(context, storefrontContext)
        {
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            this.ModelProvider = modelProvider;
        }
        [ScriptIgnore]
        public IModelProvider ModelProvider { get; set; }

        public string ShopName { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public bool IsFavorite { get; set; }
        public List<WishListLineJsonResult> Lines { get; set; }     
        public virtual void Initialize(WishList wishList)
        {
            this.ShopName = wishList.ShopName;
            this.Name = wishList.Name;
            this.CustomerId = wishList.CustomerId;
            this.UserId = wishList.UserId;
            this.IsFavorite = wishList.IsFavorite;
            this.Lines = new List<WishListLineJsonResult>();
            foreach (WishListLine line in wishList.Lines)
            {
                WishListLineJsonResult model = this.ModelProvider.GetModel<WishListLineJsonResult>();
                model.Initialize(line);
                this.Lines.Add(model);
            }            
        }
    }
}