using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using EwayBill.Models;
using EwayBill.Services;
using EwayBill.Utils;
using Microsoft.IdentityModel.Tokens;

namespace EwayBill.Controllers
{
    public class ApiEwayBillController : ApiController
    {
        private readonly EwayBillService _ewayBillService = new EwayBillService();

        ResponseWrapper resp = new ResponseWrapper();
        
        [Route("api/v1/ewaybill/generate")]
        [HttpPost]
        public async Task<IHttpActionResult> Generate(EwayBillRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EwayBillResponse ewayBillResponse =await _ewayBillService.GenerateEwayBill(model);
                    if (ewayBillResponse.status_cd == "1")
                    {
                        resp.Data = new { ewayBillResponse.Data, ewayBillResponse.RequestData };
                        resp.Message = "E-Way Bill generated successfully.";
                        resp.Status = true;
                        return Ok(resp);
                    }
                    else
                    {
                        resp.Message = "Failed to generate E-Way Bill.";
                        resp.Status = false;
                        return Content(HttpStatusCode.BadRequest, resp);
                    }
                }
                else
                {
                    return Content(HttpStatusCode.InternalServerError, resp);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
