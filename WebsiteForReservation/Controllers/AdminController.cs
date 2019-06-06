﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteForReservation;
using WebsiteForReservation.Classes;

namespace WebsiteForReservation.Controllers
{
    public class AdminController : Controller
    {
        private DBEntities db = new DBEntities();
        private Service service = new Service();
        public ActionResult Index()
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            return View();
        }
        public bool existSession()
        {
            if (string.IsNullOrEmpty((string)Session["AdminId"]))
            {
                return false;
            }
            return true;

        }

        public ActionResult AllUsers()
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            List<User> user = new List<User>();
            user = db.Users.SqlQuery(
                  "SELECT * FROM MainDB.dbo.Users").ToList();
            

            return View(user);
        }

        public ActionResult AddPhotos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPhotos(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null)
            {
                Image img = new Image();
                var fileName = Path.GetFileName(uploadFile.FileName);                
                img.ImagesPath = "~/Photos/" + fileName;
                db.Images.Add(img);
                db.SaveChanges();

                uploadFile.SaveAs(Server.MapPath(img.ImagesPath));
            }

            return RedirectToAction("Index");
        }

        public ActionResult AdminReservation(int? id)
        {
           UserTime userTime = new UserTime();
           userTime.user= db.Users.Find(id);
           Session["userTime"] = userTime;
            return View();
        }
        [HttpPost]
        public ActionResult FindAdminReservation(string daySelected)
        {
            UserTime userTime = new UserTime();
            userTime.dateOfReservation = userTime.dateOfReservation.AddYears(Convert.ToInt32(daySelected.Substring(0, 4)) - 1);
            userTime.dateOfReservation = userTime.dateOfReservation.AddMonths(Convert.ToInt32(daySelected.Substring(5, 2)) - 1);
            userTime.dateOfReservation = userTime.dateOfReservation.AddDays(Convert.ToInt32(daySelected.Substring(8, 2)) - 1);
            userTime.startHour = userTime.startHour.AddHours(8);
            userTime.endHour = userTime.endHour.AddHours(16);

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
                    userTime.startHour = userTime.startHour.AddMinutes(15);
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
                    userTime.startHour = userTime.startHour.AddMinutes(15);
                    string reservationHour = userTime.startHour.Hour.ToString("00.##") + ":" + userTime.startHour.Minute.ToString("00.##");
                    availableHours.Add(reservationHour);
                }
            }
            UserTime usrTemp = (UserTime)Session["userTime"];
            userTime.user = usrTemp.user;
            Session["userTime"] = userTime;
            return PartialView("../PartialView/_AvailableAdminReservation", availableHours);
          
        }

        [HttpGet]
        public ActionResult DoAdminReservation(string hourSelected)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            UserTime userTime = (UserTime)Session["userTime"];

            DateTime reservationDate = userTime.dateOfReservation;
            reservationDate = reservationDate.AddHours(Convert.ToInt32(hourSelected.Substring(0, 2)));
            reservationDate = reservationDate.AddMinutes(Convert.ToInt32(hourSelected.Substring(3, 2)));
            Reservation reservation = new Reservation();
            reservation.Date = reservationDate;
            reservation.Status = true;
            reservation.UserId = userTime.user.UserId;
            db.Reservations.Add(reservation);
            db.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser([Bind(Include = "UserId,Email,Password,FirstName,LastName")] User user, string confirmPassword, string confirmEmail)
        {
            if (ModelState.IsValid && confirmPassword == user.Password)
            {
                user.Password = service.Encrypt(user.Password);
                if (confirmEmail == user.Email)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    Content("<script>alert('Successfully registered');</script>");
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else
                {
                    Content("<script>alert('E-mail are not identical!');</script>");
                    return RedirectToAction("CreateUser", "CreateUser", new { area = "Admin" });
                }
            }
            else
            {
                Content("<script>alert('Password are not identical!');</script>");
                return RedirectToAction("CreateUser", "CreateUser", new { area = "Admin" });
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
            Service service = new Service();
            int adminId = Convert.ToInt32(Session["AdminId"].ToString());
            Admin admin = db.Admins.Single(a => a.AdminId == adminId);
            oldPassword = service.Encrypt(oldPassword);
            string abc = admin.Password.Replace(" ", "");
            if (oldPassword == admin.Password.Replace(" ", "") && oldPassword != newPassword && newPassword == confirmNewPassword)
            {
                admin.Email = admin.Email.Replace(" ", "");
                admin.Password = service.Encrypt(newPassword);
                db.SaveChanges();
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            else
            {
                Content("<script>alert('Error change password!');</script>");
                return RedirectToAction("Settings", "Admin", new { area = "Admin" });
            }


        }



    }
}
