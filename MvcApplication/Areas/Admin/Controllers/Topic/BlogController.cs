using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Helper;

namespace Vidyanjali.Areas.Admin.Controllers.Topic
{
    [Authorize(Roles = "Admin User,Authorizer,Super User")]
    public class BlogController : Controller
    {
        private CoreContext _db = new CoreContext();
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        //
        // GET: /Admin/WebPage/

        public ActionResult Index()
        {
            var about = _db.Blogs.OrderBy(a=>a.Id).ToList();
            return View(about);
        }

        //
        // GET: /Admin/WebPage/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/WebPage/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Blog blogs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (Utilities utility = new Utilities())
                    {
                        blogs.RelativeUrl = utility.GetUrl("Vidyanjaliblog", blogs.Url);
                    }
                    _db.Blogs.Add(blogs);
                    _db.SaveChanges();

                    blogs.ReferenceCode = string.Format("{0}{1}",
                                                      blogs.Url.Replace("-", string.Empty).
                                                          Replace(" ", string.Empty),
                                                      Convert.ToString(blogs.Id));
                    _db.SaveChanges();

                    TempData["Success"] = "New about  has been Created";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error";
            }
            return View(blogs);
        }



        public ActionResult Edit(int id = 0)
        {
            Blog blogs = _db.Blogs.Find(id);
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View(blogs);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Blog blogs)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _db.Entry(blogs).State = EntityState.Modified;
                    _db.SaveChanges();

                    TempData["Success"] = "All the changes have been saved";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            return View(blogs);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
