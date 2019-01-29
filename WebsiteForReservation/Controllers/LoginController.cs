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
        public ActionResult Register([Bind(Include = "UserId,Email,Password,FirstName,LastName")] User user,string confirmPassword,string confirmEmail)
        {
            if (ModelState.IsValid && confirmPassword==user.Password)
            {
                if (confirmEmail == user.Email) { 
                db.Users.Add(user);
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
            ViewBag.Message = user.FirstName + " " + user.LastName + " successfully registered";
            return View(user);
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                var admin = db.Admins.Single(u => u.Email == user.Email && u.Password == user.Password);
                if (admin != null)
                {
                    Session["AdminId"] = admin.AdminId.ToString();
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });

                }
            }
            catch ( Exception e)
            {

            }
            try
            {

                var usr = db.Users.Single(u => u.Email == user.Email && u.Password == user.Password);
            
            if (usr != null)
            {
                Session["UserId"] = usr.UserId.ToString();
                Session["Email"] = usr.Email.ToString();
                return RedirectToAction("Home", "Home", new { area = "Home" });
                
            }
            else
            {
                    // ModelState.AddModelError("", "Email or password is wrong!");
                    //return RedirectToAction("Login", "Login", new { area = "Login" });
                    return Content("<script>alert('Welcome To All');</script>");
                }
            }
            catch (Exception e)
            {

            }
            return View();
        }

    }
}
