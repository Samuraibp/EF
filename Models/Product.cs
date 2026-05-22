using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp5.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Supplier { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal CostPrice { get; set; }

        public DateTime SupplyDate { get; set; }
    }
}
