using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WritingPlatform.Models;

namespace WritingPlatform.Controllers
{
    public class UsersController : Controller
    {
        private WritingPlatformEntities db = new WritingPlatformEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
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
        public ActionResult Details()
        {
            String[] btn = Request.Form.GetValues("btn");
            System.Diagnostics.Debug.WriteLine("btn 0: " + btn[0]);
            if (btn[0] == "Update")
            {
                User usr = (User)Session["user"];
                return RedirectToAction("Edit", new { id = usr.Id });
            }
            else if (btn[0] == "Delete")
            {
                User usr = (User)Session["user"];
                return RedirectToAction("Delete", new { id = usr.Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Users/Create
        [HttpGet]
        public ActionResult Create()
        {
            System.Diagnostics.Debug.WriteLine("User Invoke");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,Email")] User user)
        {
            System.Diagnostics.Debug.WriteLine("User: " + user.Username);

            if (ModelState.IsValid)
            {
                user.Password = GetSHA256Hash(user.Password);
                user.Role = "user";
                user.Status = true;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
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

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Email,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                User u = (User)Session["user"];
                User usr = db.Users.Find(u.Id);
                user.Password = usr.Password;
                user.Status = true;
                usr.Username = user.Username;
                usr.Email = user.Email;
                usr.Role = user.Role;
                System.Diagnostics.Debug.WriteLine("User: " + usr.Password);
                System.Diagnostics.Debug.WriteLine("User: " + user.Username);

                db.Entry(usr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = usr.Id });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();            
        }

        [HttpPost]
        public ActionResult Login(String post)
        {
            String username = Request.Form["txtUsername"];
            String password = Request.Form["pbPassword"];
            System.Diagnostics.Debug.WriteLine("username: " + username);
            System.Diagnostics.Debug.WriteLine("password: " + password);
            password = GetSHA256Hash(password);
            User user = db.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
            if (user != null)
            {
                Session["user"] = user;
                return RedirectToAction("UserDashboard", "Writing", new { id = user.Id });
            }
            else
            {
                ViewBag.Error = "Username or password is incorrect";
                return View();
            }

        }

        public string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        public string GetSHA256Hash(string input)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
    }
}
