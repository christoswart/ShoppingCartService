using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    public interface IShoppingCartService
    {
        void AddItem(Product item);
        decimal CalculateCartTotal();
        decimal CalculateCartTotalExclVat();
        decimal CalculateCartTotalVAT();
        void ClearCart();
        List<Product> GetProducts();
        void RemoveItem(Product item);
        void UpdateCartItemQuantity(Product item);
    }
}