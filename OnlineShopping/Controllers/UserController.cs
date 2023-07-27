using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineShopping.Data;
using OnlineShopping.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineShopping.Controllers
{
	public class UserController : Controller
	{
		private readonly ShoppingDbContext _db;

		public UserController(ShoppingDbContext db)
		{
			_db = db;
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]

		public IActionResult Register(User user)
		{
			if (ModelState.IsValid)
			{
				user.Password = HashPassword(user.Password);
				_db.Users.Add(user);
				_db.SaveChanges();
				return RedirectToAction("Login");
			}
			return View(user);
		}

		public IActionResult Login()
		{
			ClaimsPrincipal claimUser = HttpContext.User;
			if (claimUser.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> LoginAsync(User userObj)
		{
			if (ModelState.IsValid)
			{

				String hashedPassword = HashPassword(userObj.Password);
				var user = _db.Users.SingleOrDefault(u => u.Username == userObj.Username && u.Password == hashedPassword);
				if (user != null)
				{
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, userObj.Username),
						new Claim("OtherProperties","Example Role")
						// Add any other claims if needed
					};

					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);
					AuthenticationProperties properties = new AuthenticationProperties()
					{
						AllowRefresh = true,
						IsPersistent = userObj.KeepLoggedIn
					};

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
						new ClaimsPrincipal(identity),properties);
					// Redirect to a secured page or dashboard after successful login.

					return RedirectToAction("Index", "Home");
				}

			}
			ModelState.AddModelError("", "Invalid username or password");
			return View();
		}

		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
			}
		}
	}
}
