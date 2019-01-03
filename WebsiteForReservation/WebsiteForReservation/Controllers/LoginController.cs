using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteForReservation;

namespace WebsiteForReservation.Controllers
{
    public class LoginController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserId,Email,Password,FirstName,LastName")] User1 user1,string confirmPassword,string confirmEmail)
        {
            if (ModelState.IsValid && confirmPassword==user1.Password)
            {
                if (confirmEmail == user1.Email) { 
                db.User1.Add(user1);
                db.SaveChanges();
                return RedirectToAction("Login");
                }
                else
                {

                }
            }
            else
            {

            }
            ViewBag.Message = user1.FirstName + " " + user1.LastName + " successfully registered";
            return View(user1);
        }
        [HttpPost]
        public ActionResult Login(User1 user)
        {
            var usr = db.User1.Single(u => u.Email == user.Email && u.Password == user.Password);
            if (usr != null)
            {
                Session["UserId"] = usr.UserId.ToString();
                Session["Email"] = usr.Email.ToString();
                return RedirectToAction("Index", "Home", new { area = "Home" });
                
            }
            else
            {
                ModelState.AddModelError("", "Email or password is wrong!");
            }
            return View();
        }

    }
}
