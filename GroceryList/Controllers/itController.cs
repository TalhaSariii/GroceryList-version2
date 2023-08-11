using Microsoft.AspNetCore.Mvc;
using GroceryList.Models;
using System.Linq;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BCrypt.Net;


namespace GroceryList.Controllers
{
    public class itController : Controller
    {
        private readonly IConfiguration _configuration;


        private readonly IWebHostEnvironment _webHostEnvironment;

        public itController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }


        private bool IsAdminLogin(string userName, string password)
        {
            var adminUserName = _configuration["AdminCredentials:UserName"];
            var adminPassword = _configuration["AdminCredentials:Password"];

            return userName == adminUserName && password == adminPassword;
        }

        private Users GetUserById(int userId)
        {
          
            var user = c.Users.FirstOrDefault(u => u.UserId == userId);

            

            return user;
        }
        private readonly Context c = new Context();


        public IActionResult Index(int? page, int userId, int pageSize = 5)
        {
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            int pageNumber = page ?? 1;
            int validPageSize = (pageSize > 0) ? pageSize : 5;

            Users currentUser = GetUserById(userId);

            if (currentUser == null)
            {
                // Kullanıcı bulunamadı, hata durumunda yönlendir
                return RedirectToAction("Login");
            }

            ViewBag.ProfilePictureURL = currentUser.ProfilePictureURL;

            var values = c.Items
                .Where(x => x.IsDeleted == false && x.UserId == userId)
                .ToList();

            ViewBag.UserId = userId;

            var pagedList = values.ToPagedList(pageNumber, validPageSize);
            return View(pagedList);
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Users newUser)
        {
            // Kullanıcı adının alınmadığından emin olun
            bool isUsernameTaken = c.Users.Any(u => u.UserName == newUser.UserName);

            if (isUsernameTaken)
            {
                TempData["ErrorMessage"] = "Bu kullanıcı adı zaten alınmış. Lütfen başka bir kullanıcı adı seçin.";
                return RedirectToAction("Register");
            }

            // Şifreyi BCrypt ile hashleyin
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            c.Users.Add(newUser);
            c.SaveChanges();

            int newUserId = newUser.UserId;

            // Oturum yönetimini yapın (örneğin Session)
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
                return RedirectToAction("AdminPage");
            }

          
            var existingUser = c.Users.FirstOrDefault(x => x.UserName == userName);

            if (existingUser != null)
            {
       
                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, existingUser.Password);

                if (isPasswordCorrect)
                {
                   
                    HttpContext.Session.SetInt32("UserId", existingUser.UserId);
                    return RedirectToAction("Index", new { userId = existingUser.UserId, success = "true" });
                }
            }

        
            return RedirectToAction("Login", new { success = "false" });
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
            return RedirectToAction("EditItem");
        }
        
        [HttpGet]
        public IActionResult EditItem(int Id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                
                return RedirectToAction("Login", "it"); 
            }

            var item = c.Items.FirstOrDefault(x => x.Id == Id && x.UserId == userId.Value);
            if (item != null)
            {
                item.IsEditing = true;
                c.SaveChanges();
            }
            return RedirectToAction("Index", new { userId = userId, editing = true });
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

               
                if (updatedUser.Password != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
                }

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
               
                c.Users.Remove(user);
                c.SaveChanges();
            }

            return RedirectToAction("AdminPage");
        }   

        [HttpGet]
        public IActionResult AdminTable(int userId, int? page)
        {
            const int pageSize = 10; 
            var userItems = c.Items.Where(x => x.UserId == userId && x.IsDeleted == false)
                                   .ToPagedList(page ?? 1, pageSize);
            return View(userItems);
        }


        [HttpGet]
        public IActionResult EditAdminItem(int Id, int UserId)
        {
            if (UserId <= 0)
            {
                // UserId is not valid or not authenticated, redirect to Login
                return RedirectToAction("Login", "it");
            }

            var item = c.Items.FirstOrDefault(x => x.Id == Id && x.UserId == UserId);
            if (item != null)
            {
                item.IsEditing = true;
                c.SaveChanges();
            }
            return RedirectToAction("AdminTable", new { userId = UserId, editing = true });
        }

       

        [HttpPost]
        public IActionResult EditAdminItem(Item d)
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
            return RedirectToAction("AdminTable", new { userId = d.UserId });
        }
        private readonly IWebHostEnvironment _hostingEnvironment;


        [HttpPost]
        public IActionResult EditProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
              
                string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profilePicture.CopyTo(fileStream);
                }

                string profilePictureURL = "/uploads/" + uniqueFileName;

          
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    var currentUser = GetUserById(userId.Value);
                    if (currentUser != null)
                    {
                        currentUser.ProfilePictureURL = profilePictureURL;
                        c.SaveChanges();

                     
                        TempData["UserId"] = userId.Value;
                    }
                }

              
                return RedirectToAction("EditItem");
            }

           
            return RedirectToAction("Index");
        }





    }
}