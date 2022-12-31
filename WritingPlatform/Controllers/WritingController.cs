using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WritingPlatform.Models;

namespace WritingPlatform.Controllers
{
    public class WritingController : Controller
    {
        // GET: Writing
        private WritingPlatformEntities db = new WritingPlatformEntities();
        public List<String> genres = new List<string>();
        public List<String> title_writings = new List<string>();
        //public List<Writing> l_writing  = new List<Writing>();
        public List<WritingDTO> l_writingDTO = new List<WritingDTO>();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard(int? id)
        {
            if (null != Request.Form.GetValues("btn"))
            {
                String[] btn = Request.Form.GetValues("btn");
                System.Diagnostics.Debug.WriteLine("btn 0: " + btn[0]);
                if (btn[0] == "Upload")
                {
                    return RedirectToAction("AddPublishWriting");
                }
                else if (btn[0] == "Compose")
                {
                    return RedirectToAction("PublishWriting");
                }
                return View();
            }
            else
            {
                User user = (User)Session["user"];
                if (null == user)
                {
                    WritingDTO wDTO;
                    foreach (Writing w in db.Writings)
                    {
                        wDTO = new WritingDTO();
                        wDTO.Id = w.Id;
                        wDTO.Username = w.User.Username;
                        wDTO.Title = w.Title;
                        wDTO.Type = w.Genre.Type;
                        wDTO.Date = w.Date;
                        wDTO.Path = w.Content;
                        l_writingDTO.Add(wDTO);
                        title_writings.Add(w.Title);
                    }
                    ViewBag.title_writings = title_writings;
                    ViewData["l_writing"] = l_writingDTO;

                    return View();
                }
                else
                {
                    WritingDTO wDTO;
                    foreach (Writing w in db.Writings)
                    {
                        if(w.User_id == user.Id)
                        {
                            wDTO = new WritingDTO();
                            wDTO.Id = w.Id;
                            wDTO.Username = w.User.Username;
                            wDTO.Title = w.Title;
                            wDTO.Type = w.Genre.Type;
                            wDTO.Date = w.Date;
                            wDTO.Path = w.Content;
                            l_writingDTO.Add(wDTO);
                            title_writings.Add(w.Title);
                        }

                    }
                    ViewBag.title_writings = title_writings;
                    ViewData["l_writing"] = l_writingDTO;
                    ViewBag.uid = user.Id;
                    return View();
                }
            }
        }

        public ActionResult WritingDetail(int ? id)
        {
                if (id != null)
                {
                    String path = db.Writings.Find(id).Content;
                    String mapPath = path.Replace("~", String.Empty);
                    ViewBag.content = mapPath;
                }

                User user = (User)Session["user"];
                if (null == user)
                {
                    return View();
                }
                else
                {
                    ViewBag.uid = user.Id;
                    System.Diagnostics.Debug.WriteLine("user id: " + user.Id);
                    return View();
                }

            }

        public ActionResult WritingDetailAnonymous(int? id)
        {
            String path = db.Writings.Find(id).Content;
            String mapPath = path.Replace("~", String.Empty);
            ViewBag.content = mapPath;
            return View();
        }
        public ActionResult PublishWriting()
        {
            User user = (User)Session["user"];
            if (null == user)
            {
                return View();
            }
            else
            {
                ViewBag.uid = user.Id;
                System.Diagnostics.Debug.WriteLine("user id: " + user.Id);
                return View();
            }
        }

        [HttpGet]
        public ActionResult AddPublishWriting()
        {
            User user = (User)Session["user"];
            if (null == user)
            {
                foreach (Genre g in db.Genres)
                {
                    genres.Add(g.Type);
                    System.Diagnostics.Debug.WriteLine("genre: " + g.Type);
                }
                ViewBag.genres = genres;
                return View();
            }
            else
            {
                ViewBag.uid = user.Id;
                foreach (Genre g in db.Genres)
                {
                    genres.Add(g.Type);
                    System.Diagnostics.Debug.WriteLine("genre: " + g.Type);
                }
                ViewBag.genres = genres;
                System.Diagnostics.Debug.WriteLine("user id: " + user.Id);
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddPublishWriting(HttpPostedFileBase filename, [Bind(Include = "title,date,type")] Writing writing)
        {
            String type = Request.Form["selected-type"];
            String title = Request.Form["title"];
            String date = Request.Form["date"];

            System.Diagnostics.Debug.WriteLine("type: " + type);
            System.Diagnostics.Debug.WriteLine("title: " + title);
            System.Diagnostics.Debug.WriteLine("date: " + date);

            User user = (User)Session["user"];
            string dir = Server.MapPath("~/Writing_Data/Papers/" + user.Username);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            ViewBag.filename = Path.GetFileName(filename.FileName);
            filename.SaveAs(Server.MapPath("~/Writing_Data/Papers/") + user.Username + "/" + filename.FileName);
            ViewBag.FileList = Directory.GetFiles(Server.MapPath("~/Writing_Data/Papers/") + user.Username + "/");
            System.Diagnostics.Debug.WriteLine("user name: " + user.Username);

            writing.User_id = user.Id;
            writing.Content = "~/Writing_Data/Papers/" + user.Username + "/" + filename.FileName;
            writing.Date = DateTime.Now;
            writing.Genre_id = db.Genres.Where(g => g.Type == type).FirstOrDefault().Id;
            db.Writings.Add(writing);
            db.SaveChanges();

            foreach (Genre g in db.Genres)
            {
                genres.Add(g.Type);
                System.Diagnostics.Debug.WriteLine("genre: " + g.Type);
            }
            ViewBag.genres = genres;
            return View();
        }

        public ActionResult NonUserDashboard()
        {
                WritingDTO wDTO;
                foreach (Writing w in db.Writings)
                {
                    wDTO = new WritingDTO();
                    wDTO.Id = w.Id;
                    wDTO.Username = w.User.Username;
                    wDTO.Title = w.Title;
                    wDTO.Type = w.Genre.Type;
                    wDTO.Date = w.Date;
                    wDTO.Path = w.Content;
                    l_writingDTO.Add(wDTO);
                    title_writings.Add(w.Title);
                }
                ViewBag.title_writings = title_writings;
                ViewData["l_writing"] = l_writingDTO;

                return View();

        }
        
        public ActionResult UserDashboard(int? id)
        {
            User user = (User)Session["user"];
            if (null == user)
            {
                WritingDTO wDTO;
                foreach (Writing w in db.Writings)
                {
                    
                    wDTO = new WritingDTO();
                    wDTO.Id = w.Id;
                    wDTO.Username = w.User.Username;
                    wDTO.Title = w.Title;
                    wDTO.Type = w.Genre.Type;
                    wDTO.Date = w.Date;
                    wDTO.Path = w.Content;
                    l_writingDTO.Add(wDTO);
                    title_writings.Add(w.Title);
                }
                ViewBag.title_writings = title_writings;
                ViewData["l_writing"] = l_writingDTO;
                return View();
            }
            else
            {
                WritingDTO wDTO;
                foreach (Writing w in db.Writings)
                {
                    if (w.User_id == user.Id)
                    {
                        wDTO = new WritingDTO();
                        wDTO.Id = w.Id;
                        wDTO.Username = w.User.Username;
                        wDTO.Title = w.Title;
                        wDTO.Type = w.Genre.Type;
                        wDTO.Date = w.Date;
                        wDTO.Path = w.Content;
                        l_writingDTO.Add(wDTO);
                        title_writings.Add(w.Title);
                    }
 
                }
                ViewBag.title_writings = title_writings;
                ViewData["l_writing"] = l_writingDTO;
                ViewBag.uid = user.Id;
                return View(new { id = user.Id });
            }
        }
    }

    public class WritingDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public String Username { get; set; }
        public String Type { get; set; }
        public String Path { get; set; }
    }
}