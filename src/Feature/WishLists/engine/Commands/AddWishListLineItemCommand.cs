using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.ManagedLists;      
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Components;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Entities;
using Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Commands
{
    public class AddWishListLineItemCommand : CommerceCommand
    {
        private readonly IAddWishListLineItemPipeline _addWishListLineItemPipeline; 
        private readonly IFindEntityPipeline _getPipeline;

        public AddWishListLineItemCommand(IFindEntityPipeline getCartPipeline, IAddWishListLineItemPipeline addToCartpipeline, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _getPipeline = getCartPipeline;
            _addWishListLineItemPipeline = addToCartpipeline;
        }

        public virtual async Task<Cart> Process(CommerceContext commerceContext, string wishlistId, CartLineComponent line)
        {
            AddWishListLineItemCommand addCartLineCommand = this;
            Cart result = null;            

            using (CommandActivity.Start(commerceContext, addCartLineCommand))
            {
                await addCartLineCommand.PerformTransaction(commerceContext, async () =>
                {
                    FindEntityArgument findEntityArgument = new FindEntityArgument(typeof(Cart), wishlistId, true);

                    Cart cart = await _getPipeline.Run(findEntityArgument, commerceContext.PipelineContextOptions).ConfigureAwait(false) as Cart;
                    if (cart == null)
                    {            
                        string str = await commerceContext.PipelineContextOptions.CommerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().ValidationError, "EntityNotFound", new object[1]
                        {
                            wishlistId
                        }, $"Entity {wishlistId} was not found.").ConfigureAwait(false);
                    }
                    else
                    {
                        if (!cart.IsPersisted)
                        {              
                            cart.Id = wishlistId;
                            cart.Name = wishlistId;
                            cart.ShopName = commerceContext.CurrentShopName();
                            cart.SetComponent(new ListMembershipsComponent()
                            {
                                Memberships = new List<string>
                                    {
                                      CommerceEntity.ListName<Cart>()
                                    }
                            });

                            cart.SetComponent(new CartTypeComponent { CartType = CartTypeEnum.Wishlist.ToString() });
                        }


                        result = await _addWishListLineItemPipeline.Run(new CartLineArgument(cart, line), commerceContext.PipelineContextOptions).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);
            }

            return result;                
        }
    }
}
