using System.Configuration;
using System.Data;
using System.Windows;

namespace MariEMontiRandomPizza
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //cart
        public static Dictionary<Pizza, int> Cart = new Dictionary<Pizza, int>();
    }

    public static class CartExtensions
    {
        public static void AddToCart(this Dictionary<Pizza, int> cart, Pizza pizza)
        {
            if (cart.ContainsKey(pizza))
            {
                cart[pizza]++;
            }
            else
            {
                cart[pizza] = 1;
            }
        }
    }

}
