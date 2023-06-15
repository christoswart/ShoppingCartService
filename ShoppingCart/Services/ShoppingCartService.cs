using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.ComponentModel;

/*
 * Shopping Cart
Using your language/framework of choice to build a shopping cart. The shopping 
cart should be fully unit tested & include the following functionality:
1. As a shopping cart user, I can add a product to the shopping cart. The 
product should include fields to identify itself: name, category, code & unit 
price
a. Product: Bag of bananas @ 19.95
2. As a shopping cart user, I can add a product to the cart & specify a quantity
3. As a shopping cart user, I can change the number of a particular product 
that I have in my cart
4. As a shopping cart user, I can calculate the total value of the shopping cart
(excluding VAT)
5. As a shopping cart user, I can calculate the total VAT of the shopping cart
a. VAT: 15%
6. As a shopping cart user, I can calculate the total value of the shopping car
 */

namespace ShoppingCart.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private ICart myCart = new Cart();

        public void AddItem(Product item)
        {
            if (item == null)
                return;

            if (myCart.Products.Any(x => x.Id == item.Id))
                UpdateCartItemQuantity(item);
            else
                myCart.Products.Add(item);
        }

        public void RemoveItem(Product item)
        {
            if (item == null || !myCart.Products.Any(x => x.Id == item.Id))
                return;

            myCart.Products.Remove(item);
        }

        public void UpdateCartItemQuantity(Product item)
        {
            if (item == null)
                return;

            if (!myCart.Products.Any(x => x.Id == item.Id))
                AddItem(item);

            myCart.Products.Single<Product>(x => x.Id == item.Id).Quantity = item.Quantity;
        }

        public decimal CalculateCartTotal()
        {
            return myCart.Products.Sum(x => x.Quantity * x.UnitPrice);
        }

        public decimal CalculateCartTotalExclVat()
        {
            return myCart.Products.Sum(x => x.Quantity * x.UnitPriceExclVat);
        }

        public decimal CalculateCartTotalVAT()
        {
            return myCart.Products.Sum(x => x.Quantity * (x.UnitPrice - x.UnitPriceExclVat));
        }

        public List<Product> GetProducts()
        {
            return myCart.Products;
        }

        public void ClearCart()
        {
            myCart.Products.Clear();
        }
    }
}