using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.Plugin.Carts;
using Plugin.Demo.HabitatHome.Wishlists.Pipelines;
namespace Plugin.Demo.HabitatHome.Wishlists.Commands
{
    public class RemoveWishListLineCommand : CommerceCommand
    {
        private readonly IRemoveWishListLinePipeline _pipeline;
        private readonly IFindEntityPipeline _getPipeline;

        public RemoveWishListLineCommand(IFindEntityPipeline getCartPipeline, IRemoveWishListLinePipeline pipeline, IServiceProvider serviceProvider)
      : base(serviceProvider)
        {
            this._getPipeline = getCartPipeline;
            this._pipeline = pipeline;
        }

        public virtual async Task<Cart> Process(CommerceContext commerceContext, string wishlistId, string cartLineId)
        {
            CommerceContext commerceContext1 = commerceContext;            
            CartLineComponent line = new CartLineComponent();            
            line.Id = cartLineId;
            return await this.Process(commerceContext1, wishlistId, line).ConfigureAwait(false);
        }

        public virtual async Task<Cart> Process(CommerceContext commerceContext, string wishlistId, CartLineComponent line)
        {
            RemoveWishListLineCommand removeCartLineCommand = this;
            Activity activity;
           
            
            Cart result = (Cart)null;
            activity = CommandActivity.Start(commerceContext, (CommerceCommand)removeCartLineCommand);
            
            try
            {
                CommercePipelineExecutionContextOptions context = commerceContext.GetPipelineContextOptions();
                FindEntityArgument findEntityArgument = new FindEntityArgument(typeof(Cart), wishlistId, false);
                Cart cart = await removeCartLineCommand._getPipeline.Run(findEntityArgument, (IPipelineExecutionContextOptions)context).ConfigureAwait(false) as Cart;
                if (cart == null)
                {
                    string str = await context.CommerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().ValidationError, "EntityNotFound", new object[1]
                    {
                    (object) wishlistId
                    }, string.Format("Entity {0} was not found.", (object)wishlistId));
                    
                    return null;
                }
                if (cart.Lines.FirstOrDefault<CartLineComponent>((Func<CartLineComponent, bool>)(c => c.Id == line.Id)) == null)
                {
                    string str = await context.CommerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().ValidationError, "CartLineNotFound", new object[1]
                    {
                        (object) line.Id
                    }, string.Format("Wishlist line {0} was not found", (object)line.Id));
                    return cart;
                }
               
                Func<Task> func = await removeCartLineCommand.PerformTransaction(commerceContext, (Func<Task>)(async () =>
                {

                    var cartResult = await this._pipeline.Run(new CartLineArgument(cart, line), (IPipelineExecutionContextOptions)context).ConfigureAwait(false);
       
                    result = cartResult;
                }));
                return result;
            }
            finally
            {
                if (activity != null)
                    activity.Dispose();
            }
        }

    }
}
