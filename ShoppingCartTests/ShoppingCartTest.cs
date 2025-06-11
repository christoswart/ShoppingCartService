using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Services;
using ShoppingCart.Models;
using ShoppingCart.Interfaces;
using System.Linq;
using System.Collections.Generic; // For List<Product>

namespace ShoppingCartTests
{
    [TestClass]
    public class ShoppingCartTest
    {
        private DbContextOptions<ShoppingCartContext>? _options;
        private ShoppingCartContext? _context;
        private IShoppingCartService? _shoppingCartService;
        private Cart? _testCart; // To reference the specific cart used by the service

        [TestInitialize]
        public void TestInitialize()
        {
            _options = new DbContextOptionsBuilder<ShoppingCartContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ShoppingCartContext(_options);
            _shoppingCartService = new ShoppingCartService(_context);

            _testCart = _context.Carts.Include(c => c.Products).FirstOrDefault();
            Assert.IsNotNull(_testCart, "Cart should be initialized by ShoppingCartService constructor.");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context?.Dispose(); // Add null check before disposing
        }

        [TestMethod]
        public void CanAddProductToShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            Assert.IsNotNull(_context, "Database context should be initialized.");
            Assert.IsNotNull(_testCart, "Test cart should be initialized.");

            _shoppingCartService.AddItem(newProduct);

            var productsInCartService = _shoppingCartService.GetProducts();
            Assert.AreEqual(1, productsInCartService.Count, "Service GetProducts count incorrect.");
            Assert.AreEqual(newProduct.Name, productsInCartService.First().Name);
            Assert.IsTrue(productsInCartService.First().Id != 0, "Product Id should be set by DB.");

            var cartFromDb = _context.Carts.Include(c => c.Products).First(c => c.Id == _testCart.Id);
            Assert.AreEqual(1, cartFromDb.Products.Count, "DB context cart product count incorrect.");
            Assert.AreEqual(newProduct.Name, cartFromDb.Products.First().Name);
        }

        [TestMethod]
        public void CanRemoveProductFromShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            Assert.IsNotNull(_context, "Database context should be initialized.");
            Assert.IsNotNull(_testCart, "Test cart should be initialized.");

            _shoppingCartService.AddItem(newProduct);

            var productInCart = _shoppingCartService.GetProducts().First(p => p.Name == newProduct.Name);
            Assert.IsNotNull(productInCart, "Product was not added correctly to be removed.");

            _shoppingCartService.RemoveItem(productInCart);

            var productsInCartService = _shoppingCartService.GetProducts();
            Assert.IsFalse(productsInCartService.Any(p => p.Id == productInCart.Id), "Item not removed via service.");

            var cartFromDb = _context.Carts.Include(c => c.Products).First(c => c.Id == _testCart.Id);
            Assert.IsFalse(cartFromDb.Products.Any(p => p.Id == productInCart.Id), "Item not removed from DB context.");
        }

        [TestMethod]
        public void CanUpdateProductQuantityInShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            Assert.IsNotNull(_context, "Database context should be initialized.");
            Assert.IsNotNull(_testCart, "Test cart should be initialized.");

            _shoppingCartService.AddItem(newProduct);

            var productToUpdate = _shoppingCartService.GetProducts().First(p => p.Name == newProduct.Name);
            productToUpdate.Quantity = 12;
            _shoppingCartService.UpdateCartItemQuantity(productToUpdate);

            var updatedProductService = _shoppingCartService.GetProducts().Single(p => p.Id == productToUpdate.Id);
            Assert.AreEqual(12, updatedProductService.Quantity, "Item Quantity incorrect via service.");

            var cartFromDb = _context.Carts.Include(c => c.Products).First(c => c.Id == _testCart.Id);
            var updatedProductDb = cartFromDb.Products.Single(p => p.Id == productToUpdate.Id);
            Assert.AreEqual(12, updatedProductDb.Quantity, "Item Quantity incorrect in DB context.");
        }

        [TestMethod]
        public void CanRetrieveProductsInShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            _shoppingCartService.AddItem(newProduct);

            Assert.IsTrue(_shoppingCartService.GetProducts().Any(), "No products returned from shopping cart.");
        }

        [TestMethod]
        public void CanGetTotalOfShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            newProduct.Quantity = 1;
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            _shoppingCartService.AddItem(newProduct);

            Assert.AreEqual(19.95M, _shoppingCartService.CalculateCartTotal(), "Total incorrect for shopping cart.");
        }

        [TestMethod]
        public void CanGetTotalExclVatOfShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            newProduct.Quantity = 1;
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            _shoppingCartService.AddItem(newProduct);

            Assert.AreEqual(17.35M, _shoppingCartService.CalculateCartTotalExclVat(), "Total Excl Vat incorrect for shopping cart.");
        }

        [TestMethod]
        public void CanGetTotalVATOfShoppingCart()
        {
            Product newProduct = SetupNewTestProduct();
            newProduct.Quantity = 1;
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            _shoppingCartService.AddItem(newProduct);

            Assert.AreEqual(2.60M, _shoppingCartService.CalculateCartTotalVAT(), "Total VAT incorrect for shopping cart.");
        }

        [TestMethod]
        public void CanClearProductsInShoppingCart()
        {
            Product newProduct1 = SetupNewTestProduct();
            Product newProduct2 = new Product { Id = 0, Name = "Apples", UnitPrice = 10.0M, Quantity = 2, UnitPriceExclVat = 9.0M, Category = "Fruit", Description="Granny Smith"};
            Assert.IsNotNull(_shoppingCartService, "Shopping cart service should be initialized.");
            Assert.IsNotNull(_context, "Database context should be initialized.");
            Assert.IsNotNull(_testCart, "Test cart should be initialized.");

            _shoppingCartService.AddItem(newProduct1);
            _shoppingCartService.AddItem(newProduct2);

            Assert.AreEqual(2, _shoppingCartService.GetProducts().Count, "Should have two items before clear.");

            _shoppingCartService.ClearCart();

            Assert.IsFalse(_shoppingCartService.GetProducts().Any(), "Items not cleared via service.");

            var cartFromDb = _context.Carts.Include(c => c.Products).First(c => c.Id == _testCart.Id);
            Assert.IsFalse(cartFromDb.Products.Any(), "Items not cleared from DB context.");
        }

        private Product SetupNewTestProduct()
        {
            return new Product()
            {
                Id = 0,
                Name = "Bananas",
                Description = "Bag of Bananas",
                Quantity = 1,
                Category = "Fruit",
                UnitPrice = 19.95M,
                UnitPriceExclVat = 17.35M
            };
        }
    }
}
