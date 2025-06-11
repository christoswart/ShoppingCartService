using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using ShoppingCart.Data; // Added using statement
using Microsoft.EntityFrameworkCore; // Added for Include
using System.Linq; // Added for FirstOrDefault, ToList etc.
using System.Collections.Generic; // Added for List

// Retaining existing comments for context if any were outside class structure.
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
        private readonly ShoppingCartContext _context;
        private ICart _cart;

        public ShoppingCartService(ShoppingCartContext context)
        {
            _context = context;
            _cart = _context.Carts.Include(c => c.Products).FirstOrDefault();
            if (_cart == null)
            {
                _cart = new Cart { Products = new List<Product>() };
                _context.Carts.Add((Cart)_cart);
                _context.SaveChanges();
            }
        }

        public void AddItem(Product item)
        {
            if (item == null)
                return;

            Product productInCart = null;
            if (item.Id != 0)
            {
                 productInCart = _cart.Products.FirstOrDefault(p => p.Id == item.Id);
            }

            if (productInCart != null)
            {
                // Product instance already in cart, update its quantity
                productInCart.Quantity += (item.Quantity > 0 ? item.Quantity : 1);
            }
            else
            {
                // New item to the cart (either new product type Id=0, or existing type not yet in cart)
                item.Quantity = (item.Quantity > 0 ? item.Quantity : 1);
                _cart.Products.Add(item); // EF tracks 'item'. If Id=0, adds to Products table. Links to cart.
            }
            _context.SaveChanges();
        }

        public void RemoveItem(Product item)
        {
            if (item == null) return;

            var productInCart = _cart.Products.FirstOrDefault(p => p.Id == item.Id);

            if (productInCart != null)
            {
                _cart.Products.Remove(productInCart);
                _context.SaveChanges();
            }
        }

        public void UpdateCartItemQuantity(Product item)
        {
            if (item == null || item.Quantity < 0)
                return;

            var productInCart = _cart.Products.FirstOrDefault(p => p.Id == item.Id);

            if (productInCart != null)
            {
                if (item.Quantity == 0)
                {
                    _cart.Products.Remove(productInCart);
                }
                else
                {
                    productInCart.Quantity = item.Quantity;
                }
                _context.SaveChanges();
            }
            else if (item.Quantity > 0)
            {
                // If product not in cart and quantity is positive, add it.
                // AddItem will set appropriate quantity if item.Quantity is weird and call SaveChanges.
                AddItem(item);
            }
        }

        public List<Product> GetProducts()
        {
             if (_cart != null && !_context.Entry(_cart).Collection(c => c.Products).IsLoaded)
             {
                 _context.Entry(_cart).Collection(c => c.Products).Load();
             }
            return _cart?.Products.ToList() ?? new List<Product>();
        }

        public void ClearCart()
        {
            if (_cart == null || _cart.Products == null) return;

            // Clearing the collection. EF handles the join table changes.
            _cart.Products.Clear();
            _context.SaveChanges();
        }

        public decimal CalculateCartTotal()
        {
            if (_cart == null) return 0;
            return _cart.Products.Sum(s => s.UnitPrice * s.Quantity);
        }

        public decimal CalculateCartTotalExclVat()
        {
            if (_cart == null) return 0;
            return _cart.Products.Sum(s => s.UnitPriceExclVat * s.Quantity);
        }

        public decimal CalculateCartTotalVAT()
        {
            if (_cart == null) return 0;
            return _cart.Products.Sum(s => (s.UnitPrice - s.UnitPriceExclVat) * s.Quantity);
        }
    }
}
