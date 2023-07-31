using Microsoft.AspNetCore.Mvc;
using GroceryList.Models;
using System.Linq;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace GroceryList.Controllers
{
    public class itController : Controller
    {
        private readonly IConfiguration _configuration;


        public itController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private bool IsAdminLogin(string userName, string password)
        {
            var adminUserName = _configuration["AdminCredentials:UserName"];
            var adminPassword = _configuration["AdminCredentials:Password"];

            return userName == adminUserName && password == adminPassword;
        }

        private readonly Context c = new Context();

        public IActionResult Index(int? userId, int page = 1, int pageSize = 5)
        {
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var values = c.Items
                .Where(x => x.IsDeleted == false && x.UserId == userId)
                .ToList();

            ViewBag.UserId = userId; 
            return View(values.ToPagedList(page, pageSize));
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Users newUser)
        {
          
            bool isUsernameTaken = c.Users.Any(u => u.UserName == newUser.UserName);

            if (isUsernameTaken)
            {
               
                TempData["ErrorMessage"] = "Username is already taken. Please choose a different username.";
                return RedirectToAction("Register");
            }

        
            c.Users.Add(newUser);
            c.SaveChanges();

            int newUserId = newUser.UserId;

            HttpContext.Session.SetInt32("UserId", newUserId);

            return RedirectToAction("Index", new { userId = newUserId, success = "true" });
        }

        [HttpGet]
        public IActionResult login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult newItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult newItems(Item d)
        {
           
            var userId = HttpContext.Session.GetInt32("UserId");

           
            if (userId == null)
            {
                
                return RedirectToAction("login", "it");
            }

           
            d.UserId = userId.Value;

     
            c.Items.Add(d);
            c.SaveChanges();

            return RedirectToAction("Index", new { userId = userId.Value });
        }



        [HttpPost]
        public IActionResult LoginCheck(string userName, string password)
        {
            if (IsAdminLogin(userName, password))
            {
                HttpContext.Session.SetInt32("IsAdmin", 1);
                // Admin girişi başarılı, Admin sayfasına yönlendir.
                return RedirectToAction("AdminPage");
            }

            var existingUser = c.Users.FirstOrDefault(x => x.UserName == userName && x.Password == password);
            if (existingUser != null)
            {
                HttpContext.Session.SetInt32("UserId", existingUser.UserId);
                return RedirectToAction("Index", new { userId = existingUser.UserId, success = "true" });
            }
            else
            {
                // Kullanıcı adı veya şifre yanlış, Login sayfasına geri dön.
                return RedirectToAction("Login", new { success = "false" });
            }
        }
        [HttpGet]
        public IActionResult AdminPage()
        {
            var isAdmin = HttpContext.Session.GetInt32("IsAdmin");
            if (isAdmin == 1)
            {
                var users = c.Users.ToList();
                return View(users);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public IActionResult DeleteItem(int Id)

        {
            var it = c.Items.Find(Id);
            it.IsDeleted = true;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditItem(int Id)
        {
            var item = c.Items.FirstOrDefault(x => x.Id == Id && x.UserId == (int)HttpContext.Session.GetInt32("UserId"));
            if (item != null)
            {
                item.IsEditing = true;
                c.SaveChanges();
            }
            return RedirectToAction("Index", new { userId = HttpContext.Session.GetInt32("UserId"), editing = true });
        }





        [HttpPost]
        public IActionResult EditItem(Item d)
        {
            var it = c.Items.FirstOrDefault(x => x.Id == d.Id && x.UserId == d.UserId);
            if (it != null)
            {
                it.Name = d.Name;
                it.Type = d.Type;
                it.Amount = d.Amount;
                it.Price = d.Price;
                it.ShopName = d.ShopName;
                it.ModifiedDate = d.ModifiedDate;
                it.IsActive = Request.Form["IsActive"] == "on";
                it.IsEditing = false;
                c.SaveChanges();
            }
            return RedirectToAction("Index", new { userId = d.UserId });
        }

        [HttpGet]
        public IActionResult EditUser(int userId)
        {
            var user = c.Users.FirstOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("AdminPage");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateUser(Users updatedUser)
        {
            var user = c.Users.FirstOrDefault(x => x.UserId == updatedUser.UserId);
            if (user != null)
            {
                user.UserName = updatedUser.UserName;
                user.Password = updatedUser.Password;
                // Diğer kullanıcı bilgileri için burada güncelleme yapabilirsiniz.
                c.SaveChanges();
            }

            return RedirectToAction("AdminPage");
        }
        [HttpGet]
        public IActionResult DeleteUser(int userId)
        {
            var user = c.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                // Silme işlemini burada yapın
                c.Users.Remove(user);
                c.SaveChanges();
            }

            return RedirectToAction("AdminPage");
        }


    }
}