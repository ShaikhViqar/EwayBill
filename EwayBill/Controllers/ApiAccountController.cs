using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using EwayBill.Models;
using EwayBill.Services;
using EwayBill.Utils;
using Microsoft.IdentityModel.Tokens;

namespace EwayBill.Controllers
{
    public class ApiAccountController : ApiController
    {
        private readonly UserService _userService = new UserService();
        private readonly GenerateToken _generateToken = new GenerateToken();
        ResponseWrapper resp = new ResponseWrapper();

        [Route("~/api/v1/accounts/Login")]
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult Login(Login login)
        {
            try
            {
                User user = _userService.ValidateUser(login);
                if (user == null)
                {
                    resp.Message = "Invalid credentials.";
                    resp.Status = false;
                    return Content(HttpStatusCode.Unauthorized, resp);
                }
                else
                {
                    var token = _generateToken.GenerateJwtToken(user);
                    resp.Data = new { user, token };
                    //resp.Data = user;
                    resp.Message = "Login succeeded.";
                    resp.Status = true;
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = false;
                return Content(HttpStatusCode.InternalServerError, resp);
            }
        }
    }
}
