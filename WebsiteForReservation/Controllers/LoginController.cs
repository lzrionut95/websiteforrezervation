using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
        private Service service = new Service();

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
                
                user.Password = service.Encrypt(user.Password);
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
            string password = service.Encrypt(user.Password);
            try
            {
                
                
                var admin = db.Admins.SingleOrDefault(u => u.Email == user.Email && u.Password == "admin");
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
                    return RedirectToAction("ErrorLogin", "Login", new { area = "Login" });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View();
        }

        
        public ActionResult Photos()
        {
            /*List<User> user = new List<User>();
            List<string> pass = new List<string>();
            user.AddRange(db.Users);
            foreach (User usr in user) { 

            pass.Add(usr.Email+"    "+  tool.Decrypt(usr.Password));
            }*/





            List< Image > images= new List<Image>();
            images.AddRange(db.Images);

            return View(images);
        }

        public ActionResult ErrorLogin()
        {
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoResetPassword(string userEmail)
        {
            Email email = new Email();
            email.IsValidEmail(userEmail);
            Random rand = new Random();
            string pass=rand.Next(1000000, 9999999).ToString();        
            try { 
                User user=db.Users.SingleOrDefault(u => u.Email == userEmail);              
                user.Password = service.Encrypt(pass);                
                db.SaveChanges();
                email.sendEmail(userEmail, pass);
            }
            catch(Exception)
            {
                return Content("<script>alert('Wrong Email!');</script>");
            }
            Content("<script>alert('Email was send!');</script>");
            return RedirectToAction("Login", "Login", new { area = "Login" });
        }

    }
}
