using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vidyanjali.Areas.Admin.Controllers.Topic
{
    [Authorize(Roles = "Admin User,Authorizer,Super User")]
    public class BugleController : Controller
    {
        private CoreContext _db = new CoreContext();
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        //
        // GET: /Admin/WebPage/

        public ActionResult Index()
        {
            var about = _db.Bugles.OrderBy(a => a.Id).ToList();
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
        public ActionResult Create(Bugle bugle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Bugles.Add(bugle);
                    _db.SaveChanges();
                    TempData["Success"] = "New about  has been Created";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            return View(bugle);
        }



        public ActionResult Edit(int id = 0)
        {
            Bugle bugle = _db.Bugles.Find(id);
            if (bugle == null)
            {
                return HttpNotFound();
            }
            return View(bugle);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Bugle bugle)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _db.Entry(bugle).State = EntityState.Modified;
                    _db.SaveChanges();

                    TempData["Success"] = "All the changes have been saved";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            return View(bugle);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
