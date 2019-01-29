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
    public class AdminController : Controller
    {
        private DBEntities db = new DBEntities();

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
            ViewData["MyData"] = user;

            return View(user);
        }
        public ActionResult CreateUser()
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser([Bind(Include = "UserId,Email,Password,FirstName,LastName")] User user)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }
            Tools tool = new Tools();
            if (ModelState.IsValid)
            {
                user.Password = tool.Encrypt(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult EditUser(int? id)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "UserId,Email,Password,FirstName,LastName")] User user)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult DeleteUser(int? id)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!existSession())
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }

            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
