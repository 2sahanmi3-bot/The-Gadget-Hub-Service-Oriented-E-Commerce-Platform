using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechworldManagementSystem
{
    internal class Product
    {

        public int Id { get; set; }


        public string GlobalId { get; set; }


        public string ItemName { get; set; }


        public decimal UnitPrice { get; set; }

        public int Inventory { get; set; }

        public string ProductDetails { get; set; }

        public string ProductCategory { get; set; }

        public string Thumbnail { get; set; } = "https://via.placeholder.com/300x200?text=TechWorld+Product";

    }
}
