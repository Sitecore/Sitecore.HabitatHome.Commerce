using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Diagnostics;
using System;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Feature.WishLists.Pipelines.Arguments;

namespace Sitecore.Feature.WishLists.Pipelines
{
    public class WishListProcessor : PipelineProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
        }

        protected virtual bool AreLineEquals(CartLine line1, CartLine line2)
        {
            Assert.ArgumentNotNull((object)line1, nameof(line1));
            Assert.ArgumentNotNull((object)line2, nameof(line2));
            int num1 = line2.Product.ProductId == null || line1.Product.ProductId == null ? 0 : (line1.Product.ProductId.Equals(line2.Product.ProductId, StringComparison.OrdinalIgnoreCase) ? 1 : 0);
            bool flag1 = ((CommerceCartProduct)line1.Product).ProductCatalog == null || ((CommerceCartProduct)line2.Product).ProductCatalog == null || ((CommerceCartProduct)line1.Product).ProductCatalog.Equals(((CommerceCartProduct)line2.Product).ProductCatalog, StringComparison.OrdinalIgnoreCase);
            bool flag2 = ((CommerceCartProduct)line1.Product).ProductVariantId == null || ((CommerceCartProduct)line2.Product).ProductVariantId == null || ((CommerceCartProduct)line1.Product).ProductVariantId.Equals(((CommerceCartProduct)line2.Product).ProductVariantId, StringComparison.OrdinalIgnoreCase);
            int num2 = flag1 ? 1 : 0;
            return (num1 & num2 & (flag2 ? 1 : 0)) != 0;
        }

        protected virtual Sitecore.Commerce.Plugin.Carts.Cart GetWishList(string userId, string shopName, string cartId, string customerId = "", string currency = "")
        {
            return Proxy.GetValue<Sitecore.Commerce.Plugin.Carts.Cart>(this.GetContainer(shopName, userId, customerId, "", currency, new DateTime?()).Carts.ByKey(cartId).Expand("Lines($expand=CartLineComponents),Components"));
        }

        protected virtual CommerceCommand AddWishListLine(string userId, string shopName, string wishListId, string itemId, Decimal quantity, string customerId = "", string currency = "")
        {
            return Proxy.DoCommand<CommerceCommand>(this.GetContainer(shopName, userId, customerId, currency, "", new DateTime?()).AddWishListLineItem(wishListId, itemId, quantity));
        }

        
        protected virtual CommerceCommand UpdateCartLine(string userId, string shopName, string wishListId, string lineId, Decimal quantity, string customerId = "", string currency = "")
        {
            return Proxy.DoCommand<CommerceCommand>(this.GetContainer(shopName, userId, customerId, "", currency, new DateTime?()).UpdateCartLine(wishListId, lineId, quantity));
        }

        protected virtual CommerceCommand RemoveWishListLine(string userId, string shopName, string wishListId, string lineId, string customerId = "", string currency = "")
        {
            return Proxy.DoCommand<CommerceCommand>(this.GetContainer(shopName, userId, customerId, "", currency, new DateTime?()).RemoveWishListLineItem(wishListId, lineId));
        }

        protected virtual string GenerateLineItemId(WishListLine line)
        {
            CommerceCartProduct product = line.Product as CommerceCartProduct;
            if (product == null || string.IsNullOrEmpty(product.ProductCatalog) || string.IsNullOrEmpty(product.ProductId))
                return string.Empty;
            return product.ProductCatalog + "|" + product.ProductId + "|" + product.ProductVariantId;
        }

        internal WishList TranslateCartToWishListEntity(Sitecore.Commerce.Plugin.Carts.Cart cart, ServiceProviderResult currentResult)
        {
            TranslateCartToEntityRequest cartToEntityRequest = new TranslateCartToEntityRequest();
            cartToEntityRequest.TranslateSource = cart;
            TranslateCartToEntityRequest request = cartToEntityRequest;

            // TODO: Move the name of pipeline to constants
            TranslateCartToWishListEntityResult cartToEntityResult = PipelineUtility.RunConnectPipeline<TranslateCartToEntityRequest, TranslateCartToWishListEntityResult>("translate.cartToWishListEntity", request);
            this.MergeResults(currentResult, (ServiceProviderResult)cartToEntityResult);
            return cartToEntityResult.TranslatedEntity;
        }


        internal void ValidateArguments<TRequest, TResult>(ServicePipelineArgs args, out TRequest request, out TResult result) where TRequest : ServiceProviderRequest where TResult : ServiceProviderResult
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Assert.ArgumentNotNull((object)args.Request, "args.Request");
            Assert.ArgumentNotNull((object)args.Request.RequestContext, "args.Request.RequestContext");
            Assert.ArgumentNotNull((object)args.Result, "args.Result");
            request = args.Request as TRequest;
            result = args.Result as TResult;
            Assert.IsNotNull((object)request, "The parameter args.Request was not of the expected type.  Expected {0}.  Actual {1}.", new object[2]
            {
                (object) typeof (TRequest).Name,
                (object) args.Request.GetType().Name
                    });
            Assert.IsNotNull((object)result, "The parameter args.Result was not of the expected type.  Expected {0}.  Actual {1}.", new object[2]
            {
                (object) typeof (TResult).Name,
                (object) args.Result.GetType().Name
            });
        }


        internal SystemMessage CreateSystemMessage(Exception ex)
        {
            SystemMessage systemMessage1 = new SystemMessage()
            {
                Message = ex.Message
            };
            if (ex.InnerException != null && !ex.Message.Equals(ex.InnerException.Message, StringComparison.OrdinalIgnoreCase))
            {
                SystemMessage systemMessage2 = systemMessage1;
                string str = systemMessage2.Message + " - " + ex.InnerException.Message;
                systemMessage2.Message = str;
            }
            return systemMessage1;
        }

        internal SystemMessage CreateSystemMessage(string message)
        {
            return new SystemMessage() { Message = message };
        }
    }


    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.Cart as global::Sitecore.Commerce.Plugin.Carts.CartSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.Cart> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.CartSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.Cart as global::Sitecore.Commerce.Plugin.Carts.CartSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.Cart> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.CartSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.CommerceEntity to its derived type global::Sitecore.Commerce.Plugin.Carts.Cart
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartSingle CastToCart(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.CommerceEntity> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.Cart> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.Cart>();
            return new global::Sitecore.Commerce.Plugin.Carts.CartSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.CartLineComponent as global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.CartLineComponent> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.CartLineComponent as global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.CartLineComponent> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Component to its derived type global::Sitecore.Commerce.Plugin.Carts.CartLineComponent
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle CastToCartLineComponent(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Component> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.CartLineComponent> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.CartLineComponent>();
            return new global::Sitecore.Commerce.Plugin.Carts.CartLineComponentSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.CartProductComponent as global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.CartProductComponent> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.CartProductComponent as global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.CartProductComponent> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Component to its derived type global::Sitecore.Commerce.Plugin.Carts.CartProductComponent
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle CastToCartProductComponent(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Component> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.CartProductComponent> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.CartProductComponent>();
            return new global::Sitecore.Commerce.Plugin.Carts.CartProductComponentSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent as global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent as global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Component to its derived type global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle CastToTemporaryCartComponent(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Component> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponent>();
            return new global::Sitecore.Commerce.Plugin.Carts.TemporaryCartComponentSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand as global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand as global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Commands.CommerceCommand to its derived type global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle CastToAddCartLineCommand(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Commands.CommerceCommand> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommand>();
            return new global::Sitecore.Commerce.Plugin.Carts.AddCartLineCommandSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand as global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand as global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Commands.CommerceCommand to its derived type global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle CastToAddEmailToCartCommand(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Commands.CommerceCommand> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommand>();
            return new global::Sitecore.Commerce.Plugin.Carts.AddEmailToCartCommandSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.GetCartCommand as global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.GetCartCommand> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.GetCartCommand as global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.GetCartCommand> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Commands.CommerceCommand to its derived type global::Sitecore.Commerce.Plugin.Carts.GetCartCommand
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle CastToGetCartCommand(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Commands.CommerceCommand> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.GetCartCommand> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.GetCartCommand>();
            return new global::Sitecore.Commerce.Plugin.Carts.GetCartCommandSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand as global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand as global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Commands.CommerceCommand to its derived type global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle CastToMergeCartsCommand(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Commands.CommerceCommand> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommand>();
            return new global::Sitecore.Commerce.Plugin.Carts.MergeCartsCommandSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand as global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand as global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Commands.CommerceCommand to its derived type global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle CastToRemoveCartLineCommand(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Commands.CommerceCommand> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommand>();
            return new global::Sitecore.Commerce.Plugin.Carts.RemoveCartLineCommandSingle(source.Context, query.GetPath(null));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand as global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand as global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand> source,
            string id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Sitecore.Commerce.Core.Commands.CommerceCommand to its derived type global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand
        /// </summary>
        /// <param name="source">source entity</param>
        public static global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle CastToUpdateCartLineCommand(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Core.Commands.CommerceCommand> source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand> query = source.CastTo<global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommand>();
            return new global::Sitecore.Commerce.Plugin.Carts.UpdateCartLineCommandSingle(source.Context, query.GetPath(null));
        }
    }
}