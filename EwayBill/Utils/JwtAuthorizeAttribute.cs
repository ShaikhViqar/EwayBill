using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EwayBill.Utils
{
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var token = filterContext.HttpContext.Request.Cookies["AuthToken"]?.Value;

            if (token != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(secretKey);

                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;

                    var userId = jwtToken.Claims.First(x => x.Type == "UserID").Value;
                    var userName = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserName")?.Value;
                    var email = jwtToken.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
                    var role = jwtToken.Claims.FirstOrDefault(x => x.Type == "Role")?.Value;

                    // Set user identity if needed
                    filterContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
                }
                catch (Exception)
                {
                    // Token validation failed, redirect to login
                    filterContext.Result = new RedirectResult("~/Account/Login");
                }
            }
            else
            {
                // No token found, redirect to login
                filterContext.Result = new RedirectResult("~/Account/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}