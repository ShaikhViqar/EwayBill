using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EwayBill.Models;
using EwayBill.Services;

namespace EwayBill.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService = new EmployeeService();

        [HttpPost]
        public async Task<JsonResult> SaveEmployee(Employee model, HttpPostedFileBase[] childFiles)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Employee data is required." });
            }

            try
            {
                if (childFiles != null)
                {
                    foreach (var file in childFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("~/Uploads"), file.FileName);
                            file.SaveAs(filePath);
                            ChildFile childFile = new ChildFile
                            {
                                FileName = file.FileName
                            };
                            model.ChildFiles.Add(childFile);
                        }
                    }
                }

                var employeeId = await _employeeService.InsertEmployeeDataAsync(model);

                return Json(new { success = true, message = "Employee saved successfully.", employeeId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving the employee.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Json(new { success = true, data = employee }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}