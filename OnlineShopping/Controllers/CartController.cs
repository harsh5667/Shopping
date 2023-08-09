
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Controllers
{
	public class CartController : Controller
	{
		private readonly ShoppingDbContext _db;

		public CartController(ShoppingDbContext db)
		{
			_db = db;
		}
		public IActionResult AddToCart(int productId, int quantity)
		{
			var userId = User.Identity.Name; // Get the currently logged-in user's ID

			// Check if the product is already in the cart for this user
			var existingItem = _db.CartItems.FirstOrDefault(item => item.UserId == userId && item.ProductId == productId);

			if (existingItem != null)
			{
				existingItem.Quantity += quantity;
			}
			else
			{
				var newItem = new CartItems { UserId = userId, ProductId = productId, Quantity = quantity };
				_db.CartItems.Add(newItem);
			}

			_db.SaveChanges();

			return RedirectToAction("Index", "Cart");
		
		}

	
		public IActionResult Index()
		{
            var userId = User.Identity.Name;
            var map = new Dictionary<int, List<String>>();
            var products_List = _db.Products.ToList();
			foreach (var product in products_List)
			{
				map.Add(product.Id, new List<String> { product.Title,product.ImageUrl ,product.Price.ToString()});
			}
            
			

			ViewBag.ProductList=map;
            var productsList = _db.CartItems.Where(item => item.UserId == userId).ToList();
            return View(productsList);
		}

		public IActionResult UpdateQuantity(int itemId,int quantity)
		{
			var item=_db.CartItems.SingleOrDefault(c=>c.Id == itemId);
			if(item != null)
			{
				item.Quantity = quantity;
				_db.SaveChanges();
			}
			return RedirectToAction("Index", "Cart");
		}
		
        public IActionResult RemoveFromCart(int? id)
        {
            CartItems? obj = _db.CartItems.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.CartItems.Remove(obj);
            _db.SaveChanges();
            TempData["Success"] = "Item Removed Successfully!";
            return RedirectToAction("Index", "Cart");

        }
    }
}
