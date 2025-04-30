using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariEMontiRandomPizza
{
    // Class to represent a pizza in the menu
    public class Pizza
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Ingredients { get; set; }

        public Pizza(string name, decimal price, string ingredients)
        {
            Name = name;
            Price = price;
            Ingredients = ingredients;
        }
    }
}
