using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Rules;
using Sitecore.Commerce.Rules.Conditions;

namespace Sitecore.Foundation.Rules.Conditions
{
    public class SpecificProductQuantityCondition<T> : BaseCartMetricsCondition<T> where T : RuleContext
    {
        /// <summary>Gets or sets the product id.</summary>
        /// <value>The product id.</value>
        public string ProductId { get; set; }

        /// <summary>Gets or sets the product quantity.</summary>
        /// <value>The product quantity.</value>
        public decimal ProductQuantity { get; set; }

        /// <summary>Gets the cart metrics.</summary>
        /// <param name="carts">The carts.</param>
        /// <returns>The metric of the cart.</returns>
        protected override IComparable GetCartMetrics(IEnumerable<Cart> carts)
        {
            return (IComparable)carts.Where<Cart>((Func<Cart, bool>)(cart => cart != null)).Aggregate<Cart, Decimal>(Decimal.Zero, (Func<Decimal, Cart, Decimal>)((quantity, cart) => quantity + cart.Lines.Where<CartLine>((Func<CartLine, bool>)(cartLine =>
            {
                if (cartLine != null)
                    return cartLine.Product != null;
                return false;
            })).Aggregate<CartLine, Decimal>(Decimal.Zero, (Func<Decimal, CartLine, Decimal>)((partialQuantity, cartLine) => partialQuantity + cartLine.Quantity * (Decimal)System.Convert.ToByte(cartLine.Product.ProductId == this.ProductId)))));
        }

        /// <summary>Gets the predefined value.</summary>
        /// <returns>The value to compare with.</returns>
        protected override object GetPredefinedValue()
        {
            return (object)this.ProductQuantity;
        }
    }
}