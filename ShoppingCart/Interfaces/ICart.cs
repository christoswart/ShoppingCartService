using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    internal interface ICart
    {
        long Id { get; set; }
        List<Product> Products { get; set; }
    }
}