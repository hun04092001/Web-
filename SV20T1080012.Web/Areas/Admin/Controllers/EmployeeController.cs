using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SV20T1080012.BusinessLayers;
using SV20T1080012.DomainModels;
using SV20T1080012.Web.AppCodes;
using SV20T1080012.Web.Models;
using System.Reflection;

namespace SV20T1080012.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    [Area("Admin")]
    public class EmployeeController : Controller
        
    {
        private const int PAGE_SIZE = 6;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchEmployee()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;

            string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
            ViewBag.SuccessMessage = successMessage;

            string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
            ViewBag.AddSuccessMessage = addsuccessMessage;
            return View(model);
        }
        // create nhân viên
        public IActionResult Create()
        {
            var model = new Employee()
            {
                EmployeeID = 0
            };
            ViewBag.Title = "Bổ sung nhân viên";
            return View(model);
        }
        // Edit nhân viên
        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật nhân viên";
            return View("Create", model);
        }
        // delete nhân viên 
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteEmployee(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhân viên này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Changepass()
        {

            return View();
        }
        public IActionResult Save(Employee data, string birthday,IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật nhân viên";
            // xử lý ngày sinh
            DateTime? dBirthDate = Converter.StringToDateTime(birthday);
            if (dBirthDate == null)
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ");
            else
                data.BirthDate = dBirthDate.Value;
            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\Employee", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
              data.Photo = fileName;
            }
            // kiểm soát dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được rỗng");

            if (string.IsNullOrWhiteSpace(data.BirthDate.ToString("dd,MM,yyyy")))
            {
                ModelState.AddModelError(nameof(data.BirthDate), "* Ngày sinh không được để trống!");
            }

            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            if (data.EmployeeID == 0)
            {
                int employeeId = CommonDataService.AddEmployee(data);
                if (employeeId > 0)
                {
                    TempData["AddSuccessMessage"] = "Bổ sung thành công !";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Không bổ sung được dữ liệu";
                    return View("Create", data);
                }
            }
            else
            {
                if (data != null)
                {
                    CommonDataService.UpdateEmployee(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
           
        }
     

    }
}
