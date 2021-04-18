using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanyPieShop.Models
{
    public class ShoppingCartItem
    {//make these public properties show we can set values to them
        public int ShoppingCartItemId { get; set; }
        public Pie Pie { get; set; }
        public int Amount { get; set; }
        public string ShoppingCartId { get; set; }
        //add DbSet<ShoppingCartItem> ShoppingCartItems in AppDbContext after this




    }
}
