using BethanyPieShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Models
{
    public class OrderDetail //contains OrderdId to which this OrderDetail is linked
        //when you place an order, each shopping cart item will result in an OrderDetail being created
        //linked to an Order that we are going to work with 
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int PieId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public Pie Pie { get; set; }
        public Order Order { get; set; }
    }
}
