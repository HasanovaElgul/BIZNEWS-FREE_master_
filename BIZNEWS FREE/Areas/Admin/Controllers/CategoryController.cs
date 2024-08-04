using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Mvc;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{
	[Area(nameof(Admin))]    //neyin kontrolleridi
	public class CategoryController : Controller
	{
		private readonly AppDbContext _context;
		private Category category;

		public CategoryController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var categories = _context.Categories.ToList();      //getsin bazadan melunat getirsin    (indexde list category yaradilir)
			return View(categories);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Category category)        //category table qosul
		{
			var findCategory = _context.Categories.FirstOrDefault(x => x.CategoryName == category.CategoryName);
			if (findCategory != null)
			{
				ViewBag.Error = "Category name is already exist!";        //eyni kategoriyani elave ede bilmesin
				return View(findCategory);
			}
			_context.Categories.Add(category);
			_context.SaveChanges();
			return Redirect("/admin/category");
		}
		public IActionResult Edit(int id)
		{
			var findCategory = _context.Categories.FirstOrDefault(x => x.Id == id);   //first ilk tapdigi melumeti gorsedirbosdursa error/firstORdefault null deyeri qaytarir
			if (findCategory == null)         //eger melumati tapmasa
				return NotFound("Melumat tapilmadi");
			return View(findCategory);
		}
		[HttpPost]

		public IActionResult Edit(int id, Category category)
		{
			_context.Categories.Update(category);
			_context.SaveChanges();
			return Redirect("/admin/category/index");
		}

		[HttpPost]

		public IActionResult Delete(int id)
		{
			var category = _context.Categories.Find(id);
			if (category == null)
				return NotFound();  // Возвращаем 404, если категория не найдена
			_context.Categories.Remove(category);
			_context.SaveChanges();
			return RedirectToAction("Index", "Category", new { area = "Admin" });
		}
	}


}
