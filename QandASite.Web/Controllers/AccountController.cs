using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QandASite.Data;
using System.Security.Claims;

namespace QandASite.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString;
        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new QuestionsRepository(_connectionString);
            repo.AddUser(user, password);
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new QuestionsRepository(_connectionString);
            var user = repo.Login(email, password);
            if(user == null)
            {
                return RedirectToAction("Login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role")))
                .Wait();

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
    }
}
