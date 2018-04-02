using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.Plugin.Carts;
using Plugin.Demo.HabitatHome.Wishlists.Pipelines;
using Plugin.Demo.HabitatHome.Wishlists.Components;
using Plugin.Demo.HabitatHome.Wishlists.Entities;

namespace Plugin.Demo.HabitatHome.Wishlists.Commands
{
    public class AddWishListLineItemCommand : CommerceCommand
    {

        private readonly IAddWishListLineItemPipeline _addWishListLineItemPipeli; 
        private readonly IFindEntityPipeline _getPipeline;

        public AddWishListLineItemCommand(IFindEntityPipeline getCartPipeline, IAddWishListLineItemPipeline addToCartpipeline, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            this._getPipeline = getCartPipeline;
            this._addWishListLineItemPipeli = addToCartpipeline;
        }

        public virtual async Task<Cart> Process(CommerceContext commerceContext, string wishlistId, CartLineComponent line)
        {
            AddWishListLineItemCommand addCartLineCommand = this;
            Cart result = (Cart)null;            

            using (CommandActivity.Start(commerceContext, (CommerceCommand)addCartLineCommand))
            {
                Func<Task> func = await addCartLineCommand.PerformTransaction(commerceContext, (Func<Task>)(async () =>
                {
                    FindEntityArgument findEntityArgument = new FindEntityArgument(typeof(Cart), wishlistId, true);
                    var context = commerceContext.GetPipelineContextOptions();

                    Cart cart = await this._getPipeline.Run(findEntityArgument, (IPipelineExecutionContextOptions)commerceContext.GetPipelineContextOptions()).ConfigureAwait(false) as Cart;
                    if (cart == null)
                    {
                        
                        string str = await context.CommerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().ValidationError, "EntityNotFound", new object[1]
                        {
                            (object) wishlistId
                        }, string.Format("Entity {0} was not found.", (object)wishlistId));
                    }
                    else
                    {
                        if (!cart.IsPersisted)
                        {
                          
                            cart.Id = wishlistId;                            
                            cart.Name = wishlistId;                            
                            cart.ShopName = commerceContext.CurrentShopName();
                            cart.SetComponent((Component)new ListMembershipsComponent()
                            {
                                Memberships = (IList<string>)new List<string>()
                                    {
                                      CommerceEntity.ListName<Cart>()
                                    }
                            });

                            cart.SetComponent(new CartTypeComponent() { CartType = CartTypeEnum.Wishlist.ToString() });                           
                        }
                       

                        result = await this._addWishListLineItemPipeli.Run(new CartLineArgument(cart, line), (IPipelineExecutionContextOptions)context);
                    }
                }));
            }

            return result;

        }
    }
}
