using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebsiteForReservation;
using WebsiteForReservation.Classes;

namespace WebsiteForReservation.Controllers
{
    public class LoginController : Controller
    {
        private Entities db = new Entities();
        private Service service = new Service();

        public ActionResult Login()
        {
            return View();
            
        }
        [HttpGet]
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
                List<User> userList = db.Users.ToList();
                foreach(User usr in userList)
                {
                    if (usr.Email.Replace(" ","") == user.Email.Replace(" ", ""))
                    {
                        string errorMessage = "Account with this e-mail already exist!";
                        return RedirectToAction("ErrorLogin", new RouteValueDictionary(new { controller = "Login", message = errorMessage }));

                    }
                }
                user.Password = service.Encrypt(user.Password);
                if (confirmEmail == user.Email) {
                    user.isAdmin = false;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    string errorMessage = "Invalid E-mail!";
                    return RedirectToAction("ErrorLogin", new RouteValueDictionary(new { controller = "Login", message = errorMessage }));
                }
            }
            ViewBag.Message = user.FirstName + " " + user.LastName + " successfully registered";
            return View(user);
        }
        [HttpPost]
        public ActionResult Login(User userLoged)
        {
            string password = service.Encrypt(userLoged.Password);
            try
            {               
                User user = db.Users.SingleOrDefault(u => u.Email == userLoged.Email && u.Password == password);
                if (user != null)
                {
                    Session["UserId"] = user.UserId.ToString();
                    Session["Email"] = user.Email.ToString();
                    if (user.isAdmin == true)
                    {
                        return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Reservation", "User", new { area = "User" });
                    }
                }
                else
                {
                    string errorMessage = "E-mail or password is incorrect!";
                    return RedirectToAction("ErrorLogin", new RouteValueDictionary(new { controller = "Login", message = errorMessage }));
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
            List< Image > images= new List<Image>();
            images.AddRange(db.Images);

            return View(images);
        }

        public ActionResult ErrorLogin(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoResetPassword(string userEmail)
        {
            if(service.IsValidEmail(userEmail)==true)
            {
                Random rand = new Random();
            string pass=rand.Next(1000000, 9999999).ToString();        
            try { 
                User user=db.Users.SingleOrDefault(u => u.Email == userEmail);              
                user.Password = service.Encrypt(pass);                
                db.SaveChanges();
                String message = "Your new password is" + pass;
                service.sendEmail(userEmail, message, "Password reseted");
            }
            catch(Exception)
            {
                string errorMessage = "E-mail is inccorect!";
                return RedirectToAction("ErrorLogin", new RouteValueDictionary(new { controller = "Login", message = errorMessage }));
            }
            string sucesfullyMessage = "E-mail was send!";
            return RedirectToAction("ErrorLogin", new RouteValueDictionary(new { controller = "Login", message = sucesfullyMessage }));
            }

            return RedirectToAction("ErrorLogin", new RouteValueDictionary(new { controller = "Login", message = "E-mail is inccorect!" }));
        }



    }
}
