using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingCart.Interfaces;

namespace ShoppingCart.Models
{
    public class Product : IProduct
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; }
        public required string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceExclVat { get; set; }
    }
}
