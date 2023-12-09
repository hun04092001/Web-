using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SV20T1080012.Web.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
        [Area("Admin")]// bat buoc phai co
        public IActionResult Index()
        {
            return View();
        }
    }
}
