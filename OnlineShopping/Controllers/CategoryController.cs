using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ShoppingDbContext _db;

		public CategoryController(ShoppingDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			List<Category> CategoryList = _db.Categories.ToList();
			return View(CategoryList);
		}

		public IActionResult Create()
		{
			return View();
		}

		public Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary GetModelState()
		{
			return ModelState;
		}

		[HttpPost]
		public IActionResult Create(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("name", "DisplayOrder cannot match with Name");
			}
			if (ModelState.IsValid)
			{
				_db.Categories.Add(obj);
				_db.SaveChanges();
				TempData["Success"] = "Data Created Successfully!";
				return RedirectToAction("Index", "Category");
			}
			return View();
		}


		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Category? categoryFromDb = _db.Categories.Find(id);
			if (categoryFromDb == null)
			{
				return NotFound();
			}
			return View(categoryFromDb);
		}

		[HttpPost]
		public IActionResult Edit(Category obj)
		{

			if (ModelState.IsValid)
			{
				_db.Categories.Update(obj);
				_db.SaveChanges();
				TempData["success"] = "Data Updated Successfully!";
				return RedirectToAction("Index");
			}
			return View();
		}


		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Category? categoryFromDb = _db.Categories.Find(id);
			if (categoryFromDb == null)
			{
				return NotFound();
			}
			return View(categoryFromDb);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Category? obj = _db.Categories.Find(id);
			if (obj == null)
			{
				return NotFound();
			}
			_db.Categories.Remove(obj);
			_db.SaveChanges();
			TempData["Success"] = "Data Deleted Successfully!";
			return RedirectToAction("Index");

		}


	}
}
