using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class DashboardController : Controller
	{
		[Authorize] //adminə yalniz qeydiyyatlı daxil ola bilsin
		public IActionResult Index()
		{
			return View();
		}
	}
}
