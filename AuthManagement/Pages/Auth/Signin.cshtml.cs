using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthManagement.DbUtil.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthManagement.Pages.Auth
{
    public class SigninModel : PageModel
    {
        private readonly AuthDbContext _context;
        public SigninModel(AuthDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            throw new Exception("test()()()77777......");
        }

        public async Task OnPost()
        {
            string userAcc = Request.Form["acc"];
            string userPwd = Request.Form["pwd"];
            string rememberMe = Request.Form["rememberMe"];

            if (string.IsNullOrWhiteSpace(userAcc) || string.IsNullOrWhiteSpace(userPwd))
            {
                Response.Redirect("/Auth/Signin");
                return;
            }

            bool isRemember = rememberMe == "1" ? true : false;
            TUser user = _context.TUsers.FirstOrDefault<TUser>(user => user.SigninAcc == userAcc && user.SigninPwd == user.SigninPwd && user.IsValid == 1);
            if (user == null || user.UserId < 1)
            {
                Response.Redirect("/Auth/Signin");
                return;
            }

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name,user.SigninAcc),
                new Claim("UserId",user.UserId.ToString()),
                new Claim("UserName",user.UserName.ToString()),
                new Claim("DeptId",user.DeptId.ToString()),
                new Claim("DeptName",user.DeptName.ToString())
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            string returnUrl = Request.Query["ReturnUrl"];
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                IsPersistent = isRemember,
                RedirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/Index" : returnUrl,
                ExpiresUtc = DateTime.UtcNow.AddMonths(1)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        }
    }
}
