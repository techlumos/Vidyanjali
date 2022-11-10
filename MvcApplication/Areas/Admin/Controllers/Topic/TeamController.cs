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
    public class teamController : Controller
    {
        private CoreContext _db = new CoreContext();
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        
        public ActionResult Index()
        {
            var about = _db.Teams.OrderBy(a => a.Id).ToList();
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
        public ActionResult Create(Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Teams.Add(team);
                    _db.SaveChanges();
                    TempData["Success"] = "New about  has been Created";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            return View(team);
        }



        public ActionResult Edit(int id = 0)
        {
            Team team = _db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _db.Entry(team).State = EntityState.Modified;
                    _db.SaveChanges();

                    TempData["Success"] = "All the changes have been saved";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            return View(team);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
