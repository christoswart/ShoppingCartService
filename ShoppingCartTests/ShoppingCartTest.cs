using ShoppingCart.Services;
using ShoppingCart.Models;
using ShoppingCart.Interfaces;

namespace ShoppingCartTests
{
    [TestClass]
    public class ShoppingCartTest
    {
        [TestMethod]
        public void CanAddProductToShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            Assert.IsTrue(myService.GetProducts().Contains(newProduct), "Item not added to shopping cart.");
        }

        [TestMethod]
        public void CanRemoveProductFromShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            myService.RemoveItem(newProduct);

            Assert.IsTrue(!myService.GetProducts().Contains(newProduct), "Item not removed from shopping cart.");
        }


        [TestMethod]
        public void CanUpdateProductQuantityInShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            Product newProductQuantity = new()
            {
                Id = 123,
                Name = "Test",
                Description = "Test this updated Product",
                Quantity = 12,
                Category = "Fruit",
                UnitPrice = 19.95M,
                UnitPriceExclVat = 17.35M
            };

            myService.UpdateCartItemQuantity(newProductQuantity);

            Assert.IsTrue(myService.GetProducts().Single().Quantity == 12, "Item Quantity is incorrect.");
        }

        [TestMethod]
        public void CanRetrieveProductsInShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            Assert.IsTrue(myService.GetProducts().Any(), "No products returned from shopping cart.");
        }

        [TestMethod]
        public void CanGetTotalOfShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            Assert.AreEqual(19.95M, myService.CalculateCartTotal(), "Total incorrect for shopping cart.");
        }

        [TestMethod]
        public void CanGetTotalExclVatOfShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            Assert.AreEqual(17.35M, myService.CalculateCartTotalExclVat(), "Total Excl Vat incorrect for shopping cart.");
        }

        [TestMethod]
        public void CanGetTotalVATOfShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            Assert.AreEqual(2.60M, myService.CalculateCartTotalVAT(), "Total VAT incorrect for shopping cart.");
        }

        [TestMethod]
        public void CanClearProductsInShoppingCart()
        {
            IShoppingCartService myService = new ShoppingCartService();

            Product newProduct = SetupNewTestProduct();

            myService.AddItem(newProduct);

            myService.ClearCart();

            Assert.IsTrue(!myService.GetProducts().Any(), "Items not cleared from shopping cart.");
        }

        private Product SetupNewTestProduct()
        {
            return new()
            {
                Id = 123,
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