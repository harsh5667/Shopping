using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineShopping.Data;
using OnlineShopping.Models;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopping.Controllers
{
	public class ProductController : Controller
	{
		private readonly ShoppingDbContext _db;
		private readonly IWebHostEnvironment _hostEnvironment;

		public ProductController(ShoppingDbContext db, IWebHostEnvironment hostEnvironment)
		{
			_db = db;
			_hostEnvironment = hostEnvironment;

		}

		public IActionResult Index()
		{
			
			List<Product> productsList  = _db.Products.Include(p => p.Category).ToList();
			return View(productsList);

			
			
		}

		public IActionResult Create()
		{
			IEnumerable<SelectListItem> categoryList = _db.Categories.Select(c => new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString()
			});
			Console.WriteLine(categoryList);
			ViewBag.CategoryList = categoryList;
			return View();
		}

		[HttpPost]
		public IActionResult Create(Product obj)
		{
			if (ModelState.IsValid)

			{
				if (obj.ImageFile != null && obj.ImageFile.Length > 0)
				{
					string uniqueFileName = Guid.NewGuid().ToString() + "_" + obj.ImageFile.FileName;
					string uploadDirectory = Path.Combine(_hostEnvironment.WebRootPath, "images");
					string imagePath = Path.Combine(uploadDirectory, uniqueFileName);

					if (!Directory.Exists(uploadDirectory))
					{
						Directory.CreateDirectory(uploadDirectory);
					}

					using (var fileStream = new FileStream(imagePath, FileMode.Create))
					{
						obj.ImageFile.CopyTo(fileStream);
					}

					obj.ImageUrl = "/images/" + uniqueFileName;
				}


				_db.Products.Add(obj);
				_db.SaveChanges();
				TempData["Success"] = "Data Created Successfully!";
				return RedirectToAction("Index");
			}
			else
			{
				IEnumerable<SelectListItem> categoryList = _db.Categories.Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString()
				});

				ViewBag.CategoryList = categoryList;
				return View(obj);
			}
		}


		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? productFromDb = _db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

			if (productFromDb == null)
			{
				return NotFound();
			}

			IEnumerable<SelectListItem> categoryList = _db.Categories.Select(c => new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString(),
				Selected = c.Id == productFromDb.CategoryId
			});

			ViewBag.CategoryList = categoryList;

			return View(productFromDb);
		}

		[HttpPost]
		public IActionResult Edit(Product model)
		{
			if (ModelState.IsValid)
			{
				Product? product = _db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == model.Id);

				if (product == null)
				{
					return NotFound();
				}

				product.Title = model.Title;
				product.Description = model.Description;
				product.ISBN = model.ISBN;
				product.Author = model.Author;
				product.ListPrice = model.ListPrice;
				product.Price = model.Price;
				product.Price50 = model.Price50;
				product.Price100 = model.Price100;
				product.CategoryId = model.CategoryId;

				if (model.ImageFile != null && model.ImageFile.Length > 0)
				{
					string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
					string uploadDirectory = Path.Combine(_hostEnvironment.WebRootPath, "images");
					string imagePath = Path.Combine(uploadDirectory, uniqueFileName);

					if (!Directory.Exists(uploadDirectory))
					{
						Directory.CreateDirectory(uploadDirectory);
					}

					string existingImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(existingImagePath))
					{
						System.IO.File.Delete(existingImagePath);
					}

					using (var fileStream = new FileStream(imagePath, FileMode.Create))
					{
						model.ImageFile.CopyTo(fileStream);
					}

					product.ImageUrl = "/images/" + uniqueFileName;
				}

				_db.SaveChanges();
				TempData["Success"] = "Data Updated Successfully!";
				return RedirectToAction("Index");
			}

			IEnumerable<SelectListItem> categoryList = _db.Categories.Select(c => new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString()
			});

			ViewBag.CategoryList = categoryList;
			return View(model);
		}


		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? productFromDb = _db.Products.Find(id);

			if (productFromDb == null)
			{
				return NotFound();
			}

			return View(productFromDb);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Product? obj = _db.Products.Find(id);

			if (obj == null)
			{
				return NotFound();
			}

			_db.Products.Remove(obj);
			_db.SaveChanges();
			TempData["Success"] = "Data Deleted Successfully!";
			return RedirectToAction("Index");
		}
	}
}
