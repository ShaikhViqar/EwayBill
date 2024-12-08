using EwayBill.Models;
using EwayBill.Services;
using EwayBill.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EwayBill.Controllers
{
    public class EwayBillController : Controller
    {
        private readonly EwayBillService _ewayBillService = new EwayBillService();

        [JwtAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authenticate()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Authenticate(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var response = await _ewayBillService.AuthenticateAsync(request);
                if (response.status_cd == "1")
                {
                    ViewBag.Message = "Authentication successful!";
                    ViewBag.ResponseData = response;
                }
                else
                {
                    ViewBag.Message = $"Authentication failed: {response.status_desc}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View(request);
        }

        //[JwtAuthorize]
        //[HttpPost]
        //public async Task<ActionResult> Generate(EwayBillRequest model)
        //{
        //    if (!string.IsNullOrEmpty(model.DocDate))
        //    {
        //        if (DateTime.TryParseExact(model.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        //        {
        //            model.DocDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("DocDate", "Invalid date format. Please use dd/mm/yyyy.");
        //        }
        //    }

        //    model.DocumentTypes = (await _ewayBillService.GetDocumentTypes()).Select(dt => new SelectListItem
        //    {
        //        Value = dt.DocumentCode,
        //        Text = dt.DocumentName
        //    }).ToList();

        //    if (ModelState.IsValid)
        //    {
        //        var response = await _ewayBillService.GenerateEwayBill(model);
        //        if (response.status_cd == "1")
        //        {
        //            //ViewBag.Message = "E-Way Bill generated successfully.";
        //            //ViewBag.ResponseData = response.Data;
        //            //return RedirectToAction("EwayBillSuccess", new { responseData = JsonConvert.SerializeObject(response) });

        //            TempData["ResponseData"] = JsonConvert.SerializeObject(response);
        //            return RedirectToAction("EwayBillSuccess");
        //        }
        //        else
        //        {
        //            //ViewBag.Message = $"Error: {response.Error}";
        //            ViewBag.Message = "Failed to generate E-Way Bill";
        //            //ViewBag.ResponseData = null;
        //        }
        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Invalid model data.";
        //        //ViewBag.ResponseData = null;
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public ActionResult Generate()
        //{
        //    var model = new EwayBillRequest
        //    {
        //    };
        //    return View(model);
        //}

        [JwtAuthorize]
        [HttpGet]
        public async Task<ActionResult> Generate()
        {
            // Fetch Document Types from the database
            var documentTypes = await _ewayBillService.GetDocumentTypes();

            // Prepare the model and pass the DocumentTypes as SelectList
            var model = new EwayBillRequest
            {
                DocumentTypes = documentTypes.Select(dt => new SelectListItem
                {
                    Value = dt.DocumentCode,
                    Text = dt.DocumentName
                }).ToList()
            };

            return View(model);
        }

        //public ActionResult EwayBillSuccess(string responseData)
        //{
        //    var response = JsonConvert.DeserializeObject<EwayBillResponse>(responseData);
        //    return View(response);
        //}

        [HttpGet]
        public ActionResult EwayBillSuccess()
        {
            // Retrieve response data from TempData
            var responseData = TempData["ResponseData"] as string;
            if (responseData == null)
            {
                // Handle case where responseData is null
                // You could redirect or return a view with an error message
                return RedirectToAction("Index");  // or a specific error page
            }
            var response = JsonConvert.DeserializeObject<EwayBillResponse>(responseData);
            // Keep TempData for further requests (such as a page refresh)
            TempData.Keep("ResponseData");
            return View(response);
        }

        [HttpGet]
        public async Task<ActionResult> EwayBillDetails(string email, string ewbNo)
        {
            try
            {
                var response = await _ewayBillService.GetEwayBillDetailsAsync(email, ewbNo);
                return View(response);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "EwayBill", "EwayBillDetails"));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewGenerateData(string DocNo = null)
        {
            var ewayBillDetails = await _ewayBillService.GetAllEwayBillDetailsAsync(DocNo);
            return View(ewayBillDetails);
        }

        //public async Task<ActionResult> ViewGenerateResponse(int pageNumber = 1, int pageSize = 10)
        //{
        //    var ewayBillResponses = await _ewayBillService.GetAllEwayBillResponseAsync(pageNumber, pageSize);

        //    ViewBag.PageNumber = pageNumber;
        //    ViewBag.PageSize = pageSize;

        //    return View(ewayBillResponses);
        //}

        public async Task<ActionResult> ViewGenerateResponse(int pageNumber = 1, int pageSize = 10)
        {
            // Get the total count of records to calculate total pages
            var totalRecords = await _ewayBillService.GetTotalEwayBillCountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Ensure pageNumber doesn't go beyond total pages or below 1
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            var ewayBillResponses = await _ewayBillService.GetAllEwayBillResponseAsync(pageNumber, pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(ewayBillResponses);
        }

        [HttpPost]
        public async Task<JsonResult> Generate(EwayBillRequest model)
        {
            if (!string.IsNullOrEmpty(model.DocDate))
            {
                if (DateTime.TryParseExact(model.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    model.DocDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    return Json(new { success = false, message = "Invalid date format. Please use dd/mm/yyyy." });
                }
            }

            var response = await _ewayBillService.GenerateEwayBill(model);

            if (response.status_cd == "1")
            {
                TempData["ResponseData"] = JsonConvert.SerializeObject(response);

                return Json(new { success = true, message = "E-Way Bill generated successfully.", data = response });
            }
            else
            {
                return Json(new { success = false, message = "Failed to generate E-Way Bill", error = response.Error });
            }
        }
    }
}