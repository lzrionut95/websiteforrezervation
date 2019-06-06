using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
            return View();
        }
        [HttpPost]
        public ActionResult FindReservation(string daySelected)
        {
            UserTime userTime = new UserTime();
            userTime.dateOfReservation=userTime.dateOfReservation.AddYears(Convert.ToInt32(daySelected.Substring(0, 4))-1);
            userTime.dateOfReservation=userTime.dateOfReservation.AddMonths(Convert.ToInt32(daySelected.Substring(5, 2))-1);
            userTime.dateOfReservation=userTime.dateOfReservation.AddDays (Convert.ToInt32(daySelected.Substring(8, 2))-1);
            userTime.startHour=userTime.startHour.AddHours(8);
            userTime.endHour=userTime.endHour.AddHours(16);

            List<Reservation> busyDates = db.Reservations.SqlQuery("SELECT * FROM dbo.Reservation WHERE(DATEPART(yy, Date) = @year AND    DATEPART(mm, Date) = @month AND    DATEPART(dd, Date) = @day)",
                                        new SqlParameter("@year", userTime.dateOfReservation.Year),
                                        new SqlParameter("@month", userTime.dateOfReservation.Month),
                                         new SqlParameter("@day", userTime.dateOfReservation.Day)).ToList();
            List<string> hoursBusy = new List<string>();
            for (int index = 0; index < busyDates.Count; index++)
            {
                string hour = busyDates.ElementAt(index).Date.ToString().Substring(10, 5);
                hoursBusy.Add(hour);
            }

            List<string> availableHours = new List<string>();
            if (hoursBusy.Count > 0)
            {
                while (userTime.startHour.AddMinutes(15) <= userTime.endHour)
                {
                    userTime.startHour=userTime.startHour.AddMinutes(15);
                    string reservationHour = userTime.startHour.Hour.ToString("00.##") + ":" + userTime.startHour.Minute.ToString("00.##");
                    if (!hoursBusy.Contains(reservationHour))
                    {
                        availableHours.Add(reservationHour);
                    }               
                }
            }
            else
            {
                while (userTime.startHour.AddMinutes(15) <= userTime.endHour)
                {
                    userTime.startHour=userTime.startHour.AddMinutes(15);
                    string reservationHour = userTime.startHour.Hour.ToString("00.##") + ":" + userTime.startHour.Minute.ToString("00.##");
                    availableHours.Add(reservationHour);
                }
            }
            Session["userTime"] = userTime;
            return PartialView("../PartialView/_AvailableReservation", availableHours);
        }


        [HttpGet]
        public ActionResult DoReservation(string hourSelected)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }
            
            UserTime userTime = (UserTime)Session["userTime"];

            DateTime reservationDate =  userTime.dateOfReservation;
            reservationDate=reservationDate.AddHours(Convert.ToInt32(hourSelected.Substring(0,2)));
            reservationDate=reservationDate.AddMinutes(Convert.ToInt32(hourSelected.Substring(3, 2)));
            Reservation reservation = new Reservation();
            reservation.Date = reservationDate;
            reservation.Status = true;
            reservation.UserId= Convert.ToInt32(Session["UserId"].ToString());
            db.Reservations.Add(reservation);
            db.SaveChanges();
            
            return RedirectToAction("Home", "Home");
        }

    }
}
