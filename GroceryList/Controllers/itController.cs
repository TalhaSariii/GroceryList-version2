using Microsoft.AspNetCore.Mvc;
using GroceryList.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.Web.Common;

namespace GroceryList.Controllers
{
    public class itController : Controller
    {
        Context c = new Context();
        public IActionResult Index(int page=1)
        {
            var values = c.Items.Where(x => x.IsDeleted == false).ToList();
            return View(values.ToPagedList(page,5));
        }
        [HttpGet]
        public IActionResult newItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult newItem(Item d)
        {

            c.Items.Add(d);
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteItem(int id)

        {
            var it = c.Items.Find(id);
            it.IsDeleted = true;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditItem(int id)
        {
            var item = c.Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.IsEditing = true;
                c.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditItem(Item d)
        {
            var it = c.Items.Find(d.Id);
            it.Name = d.Name;
            it.Type = d.Type;
            it.Amount = d.Amount;
            it.Price = d.Price;
            it.ShopName = d.ShopName;
            it.ModifiedDate = d.ModifiedDate;

            
            it.IsActive = Request.Form["IsActive"] == "on";

            it.IsEditing = false;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
    }

        



    }

