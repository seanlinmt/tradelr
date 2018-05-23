using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.DBML;
using tradelr.Models.coupons;

namespace tradelr.Models.liquid.models.Cart
{
    public class Cart : Drop
    {
        public int item_count { get; set; }
        public decimal subtotal_price { get; set; }
        public List<CartItem> items { get; set; }
        public string note { get; set; }
        public string coupon_code { get; set; }
        public decimal discount_amount { get; set; }
        public decimal total_price { get; set; }

        public Cart()
        {
            discount_amount = 0;
        }
    }

    public static class CartHelper
    {
        /// <summary>
        /// don't forget to call UpdateDiscountPrice
        /// </summary>
        /// <param name="row"></param>
        /// <param name="trackInventory"></param>
        /// <param name="viewerid"></param>
        /// <returns></returns>
        public static Cart ToLiquidModel(this cart row, long? viewerid)
        {
            var cart = new Cart();
            cart.items = row.cartitems.ToLiquidModel(viewerid).ToList();
            cart.item_count = cart.items.Count;
            cart.subtotal_price = cart.items.Sum(x => x.total_price);
            cart.note = row.note;
            cart.coupon_code = row.coupon;
            if (!string.IsNullOrEmpty(row.coupon))
            {
                cart.discount_amount = row.ToDiscountAmount(cart.subtotal_price);
                cart.total_price = cart.subtotal_price - cart.discount_amount;
            }
            else
            {
                cart.total_price = cart.subtotal_price;
            }

            return cart;
        }
    }
}