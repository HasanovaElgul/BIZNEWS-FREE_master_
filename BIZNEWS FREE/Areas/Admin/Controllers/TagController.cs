using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Mvc;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{

    [Area(nameof(Admin))]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tags = _context.Tags.ToList();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(Tag tag)
        {
            if (tag.TagName == null)
            {

                ModelState.AddModelError("error", "Tag name is required");               //biz teyin etdiyimiz errorlar
                return View();

            }

            var checkTag = _context.Tags.FirstOrDefault(x => x.TagName == tag.TagName);
            if (checkTag != null)
            {
                ViewBag.Error = "Tag name is already exist!";
                // ModelState.AddModelError("error", "Tag name is already exist!");
                return View();
            }

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return Redirect("/admin/tag/index");
        }
    }
}
