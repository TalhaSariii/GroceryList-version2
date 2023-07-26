using Microsoft.AspNetCore.Mvc;
using GroceryList.Models;
using System.Linq;
using X.PagedList;

namespace GroceryList.Controllers
{
    public class itController : Controller
    {
        private readonly Context c = new Context();

         public IActionResult Index(int userId, int page = 1, int pageSize = 5)
          {
           var values = c.Items.Where(x => x.IsDeleted == false && x.UserId == userId).ToList();

           TempData["UserId"] = userId; 

        return View(values.ToPagedList(page, pageSize));
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
            c.Items.Add(d);
            c.SaveChanges();
            return RedirectToAction("Index", new { userId = d.UserId });
        }

        [HttpPost]
        public IActionResult LoginCheck(string userName, string password)
        {
            var existingUser = c.Users.FirstOrDefault(x => x.UserName == userName && x.Password == password);
            if (existingUser != null)
            {
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
        public IActionResult EditItem(int Id, int userId)
        {
            var item = c.Items.FirstOrDefault(x => x.Id == Id && x.UserId == userId);
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

    }
}