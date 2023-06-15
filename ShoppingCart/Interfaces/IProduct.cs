namespace ShoppingCart.Interfaces
{
    public interface IProduct
    {
        string Description { get; set; }
        long Id { get; set; }
        string Name { get; set; }
        int Quantity { get; set; }
    }
}