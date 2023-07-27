using Microsoft.AspNetCore.Mvc;
using GroceryList.Models;
using System.Linq;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace GroceryList.Controllers
{
    public class itController : Controller
    {
        private readonly Context c = new Context();

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var values = c.Items.Where(x => x.IsDeleted == false && x.UserId == userId).ToList();
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
            // Check if the username already exists in the database
            bool isUsernameTaken = c.Users.Any(u => u.UserName == newUser.UserName);

            if (isUsernameTaken)
            {
                // Display an alert and redirect back to the registration page
                TempData["ErrorMessage"] = "Username is already taken. Please choose a different username.";
                return RedirectToAction("Register");
            }

            // If the username is not taken, proceed with registration
            c.Users.Add(newUser);
            c.SaveChanges();

            int newUserId = newUser.UserId;

            HttpContext.Session.SetInt32("UserId", newUserId);

            // Redirect to the index page with success message
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
            var existingUser = c.Users.FirstOrDefault(x => x.UserName == userName && x.Password == password);
            if (existingUser != null)
            {
              
                HttpContext.Session.SetInt32("UserId", existingUser.UserId);

                return RedirectToAction("Index", new { userId = existingUser.UserId, success = "true" });
            }

            return RedirectToAction("Login", new { success = "false" });
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

    }
}