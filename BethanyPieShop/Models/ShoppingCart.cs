using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanyPieShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BethanysPieShop.Models
{
    public class ShoppingCart
    {
        private readonly AppDbContext _appDbContext;

        public string ShoppingCartId { get; set; }

        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        private ShoppingCart(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public static ShoppingCart GetCart(IServiceProvider services) //gives us access to the services
            //collection, the collection of services managed in the depedency injection container
        {
            //connected to start up services.AddHttpContextAccessor
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            //asking service for AppDbContext by assigning it to AppDbContext context variable
            var context = services.GetService<AppDbContext>();

            //our shopping cart that is storing a session
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            //checking to see if there is already an active session containing a cart ID
            //if not, creates a new Guid that will then be the cartId it has set within the session
            //( Guid.NewGuid().ToString() )

            //creating instance of shopping cart, passing in that cartID
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Pie pie, int amount)
        {
            //checking to see if shoppingcartitems Ids exist in database (DbSet<ShoppingCartItems>)
            var shoppingCartItem =
                    _appDbContext.ShoppingCartItems.SingleOrDefault(
                        theShoppingCartItem => theShoppingCartItem.Pie.PieId == pie.PieId 
                        && theShoppingCartItem.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                //if pie is not in shopping cart yet (null),
                //create one how it is shown below:

                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pie = pie,
                    Amount = 1
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                //if you do find shoppingCartItem, just increase the amount
                shoppingCartItem.Amount++;
            }
            _appDbContext.SaveChanges();
        }

        public int RemoveFromCart(Pie pie)
        {
            var shoppingCartItem =
                    _appDbContext.ShoppingCartItems.SingleOrDefault(
                        s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }

            _appDbContext.SaveChanges();

            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??
                   (ShoppingCartItems =
                       _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                           .Include(session => session.Pie)
                           .ToList());
        }

        public void ClearCart()
        {
            var cartItems = _appDbContext
                .ShoppingCartItems
                .Where(cart => cart.ShoppingCartId == ShoppingCartId);

            _appDbContext.ShoppingCartItems.RemoveRange(cartItems);

            _appDbContext.SaveChanges();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Pie.Price * c.Amount).Sum();
            return total;
        }
    }
}
