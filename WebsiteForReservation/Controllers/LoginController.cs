using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteForReservation;
using WebsiteForReservation.Classes;

namespace WebsiteForReservation.Controllers
{
    public class LoginController : Controller
    {
        private DBEntities db = new DBEntities();
        private Tools tool = new Tools();

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
                
                user.Password = tool.Encrypt(user.Password);
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
            string password = tool.Encrypt(user.Password);
            try
            {
                
                
                var admin = db.Admins.SingleOrDefault(u => u.Email == user.Email && u.Password == password);
                if (admin != null)
                {
                    Session["AdminId"] = admin.AdminId.ToString();
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });

                }
            }
            catch ( Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {               
                var usr = db.Users.SingleOrDefault(u => u.Email == user.Email && u.Password == password);                         
            if (usr != null)
            {
                Session["UserId"] = usr.UserId.ToString();
                Session["Email"] = usr.Email.ToString();
                return RedirectToAction("Home", "Home", new { area = "Home" });
                
            }
            else
            {
                    return Content("<script>alert('Login Failed');</script>");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View();
        }

    }
}
