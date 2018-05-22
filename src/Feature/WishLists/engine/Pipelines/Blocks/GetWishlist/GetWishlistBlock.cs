﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Blocks.GetWishlist
{
    [PipelineDisplayName("Wishlists.GetWishListBlock")]
    public class GetWishListBlock : PipelineBlock<GetWishListArgument, WishList, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;

        public GetWishListBlock(IFindEntityPipeline findEntityPipeline)
        : base((string)null)
        {
            this._findEntityPipeline = findEntityPipeline;
        }

        public override async Task<WishList> Run(GetWishListArgument arg, CommercePipelineExecutionContext context)
        {
            GetWishListBlock getWishListBlock = this;
            
            Condition.Requires<string>(arg.WishListId).IsNotNullOrEmpty("The WishListId can not be null or empty");
            List<WishList> objects = context.CommerceContext.GetObjects<WishList>();

            if (objects.Any<WishList>() && objects.FirstOrDefault<WishList>((Func<WishList, bool>)(p => p.Id == arg.WishListId)) != null)
            {                
                context.Logger.LogWarning(string.Format("{0}.AlreadyLoaded: CartId:{1}", getWishListBlock.Name, (object)arg), Array.Empty<object>());
            }

            WishList wishlist = await getWishListBlock._findEntityPipeline.Run(new FindEntityArgument(typeof(WishList), arg.WishListId, true), context).ConfigureAwait(false) as WishList;
            if (wishlist == null || wishlist.IsPersisted)
                return wishlist;
            wishlist.Id = arg.WishListId;
            wishlist.Name = arg.WishListName;
            wishlist.ShopName = arg.ShopName;
            wishlist.CustomerId = arg.ShopperId;
            wishlist.SetComponent((Component)new ListMembershipsComponent()
            {
                Memberships = (IList<string>)new List<string>()
                {
                  CommerceEntity.ListName<WishList>()
                }
            });
            return  wishlist;
        }

    }
}
