using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingCart.Interfaces;

namespace ShoppingCart.Models
{
    public class Cart : ICart // Changed from internal to public
    {
        public long Id { get; set; }
        public List<Product> Products { get; set; }

        public Cart()
        {
            Products = new List<Product>();
        }
    }
}
