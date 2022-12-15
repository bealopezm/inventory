using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1.Model
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime ExpirationDate { get; set; }

        // Type of item: food, drink, frozen food, dry food, etc.
        public string Type { get; set; }

        public double NetPrice { get; set; }


        //The weight is measured in litres o Kilograms.
        public double Weight { get; set; }

        public int Quantity { get; set; }

        public int IsDeleted { get; set; }
    }
}
