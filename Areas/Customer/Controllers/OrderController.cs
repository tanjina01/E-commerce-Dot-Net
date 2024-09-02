using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StataIT.Data;
using StataIT.Models;
using StataIT.Utility;

namespace StataIT.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET Checkout actioin method

        public IActionResult Checkout()
        {
            var order = new Order
            {
                OrderDate = DateTime.Now // Set default order date to current date
            };
            return View(order);
        }


        // POST Checkout action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order anOrder)
        {
            if (ModelState.IsValid)
            {
                List<Products> products = HttpContext.Session.Get<List<Products>>("products");
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        OrderDetails orderDetails = new OrderDetails
                        {
                            PorductId = product.Id
                        };
                        anOrder.OrderDetails.Add(orderDetails);
                    }
                }

                anOrder.OrderNo = GetOrderNo();
                _db.Orders.Add(anOrder);
                await _db.SaveChangesAsync();
                HttpContext.Session.Set("products", new List<Products>());

                return RedirectToAction("OrderConfirmation", new { id = anOrder.Id });
            }
            return View(anOrder);
        }

        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }

        public IActionResult OrderConfirmation(int id)
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == id);
            return View(order);
        }
    }
}