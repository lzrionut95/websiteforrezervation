using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebsiteForReservation;
using WebsiteForReservation.Classes;

namespace WebsiteForReservation.Controllers
{
    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();


        public ActionResult Home()
        {

            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            return View();
        }

        public bool existSession()
        {
            if (string.IsNullOrEmpty((string)Session["UserId"]))
            {
                return false;
            }
            return true;

        }

        public ActionResult Settings()
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }
            Tools tool = new Tools();           
            int usrId = Convert.ToInt32(Session["UserId"].ToString());
            User user = db.Users.Single(usr => usr.UserId == usrId);
            oldPassword = tool.Encrypt(oldPassword);
            if (oldPassword == user.Password && oldPassword != newPassword && newPassword == confirmNewPassword)
            {
                user.Email = user.Email.Replace(" ", "");
                user.Password = tool.Encrypt(newPassword);
                db.SaveChanges();
                return RedirectToAction("Home", "Home", new { area = "Home" });
            }
            else
            {
                return RedirectToAction("Settings", "Home", new { area = "Home" });
            }


        }

        public ActionResult Logout()
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }
            Session.Abandon();
            return RedirectToAction("Login", "Login", new { area = "Login" });
        }
        [HttpGet]
        public ActionResult Reservation()
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            DateTime startDate = new DateTime().AddHours(8);
            DateTime endDate = new DateTime().AddHours(16);
            List<string> listAllHours= new List<string>();
            DateTime[] vector = new DateTime[50];
            int i = 0;

             while (startDate.AddMinutes(15) <= endDate)
             {
             vector[i] = startDate;
             startDate = startDate.AddMinutes(15);
                listAllHours.Add(vector[i].Hour.ToString()+" : "+ vector[i].Minute.ToString());
                i++;
             }

            ViewBag.Hours = new SelectList(listAllHours, "hourSelected");
            return View();
        }


        [HttpPost]
        public ActionResult DoReservation(string daySelected, string hourSelected)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            Reservation reservation = new Reservation();
            daySelected = daySelected.Replace("/", " ");
            hourSelected = hourSelected.Replace(" : ", " ");
            String[] elements = Regex.Split(daySelected + " " + hourSelected, @"\s");
            reservation.Date = Convert.ToDateTime(elements[0] + "-" + elements[1] + "-" + elements[2] + " " + elements[3] + ":" + elements[4] + ":" + "00");

            if (reservation.Date <= DateTime.Now)
            {
                return RedirectToAction("Reservation", "Home", reservation);
            }

            reservation.Status = true;
            reservation.UserId= Convert.ToInt32(Session["UserId"].ToString());

            db.Reservations.Add(reservation);
            db.SaveChanges();

            return RedirectToAction("Home", "Home");
        }

    }
}
