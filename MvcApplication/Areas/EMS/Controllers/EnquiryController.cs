using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Areas.EMS.Models;
using Vidyanjali.Models;

namespace Vidyanjali.Areas.EMS.Controllers
{
    
    public class EnquiryController : Controller
    {
        private CoreContext db = new CoreContext();

        //
        // GET: /EMS/Enquiry/

        public ActionResult Index()
        {
            var enquiries = db.Enquiries.Include(e => e.Visitor).OrderByDescending(e => e.EnquiryDate);
            return View(enquiries.ToList());
        }

        //
        // GET: /EMS/Enquiry/Details/5

        public ActionResult Details(int id = 0)
        {
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            return View(enquiry);
        }

        ////
        //// GET: /EMS/Enquiry/Create

        //public ActionResult Create()
        //{
        //    ViewBag.VisitorID = new SelectList(db.Visitors, "ID", "Host");
        //    return View();
        //}

        ////
        //// POST: /EMS/Enquiry/Create

        //[HttpPost]
        //public ActionResult Create(Enquiry enquiry)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Enquiries.Add(enquiry);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.VisitorID = new SelectList(db.Visitors, "ID", "Host", enquiry.VisitorID);
        //    return View(enquiry);
        //}

        ////
        //// GET: /EMS/Enquiry/Edit/5

        //public ActionResult Edit(int id = 0)
        //{
        //    Enquiry enquiry = db.Enquiries.Find(id);
        //    if (enquiry == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.VisitorID = new SelectList(db.Visitors, "ID", "Host", enquiry.VisitorID);
        //    return View(enquiry);
        //}

        ////
        //// POST: /EMS/Enquiry/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Enquiry enquiry)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(enquiry).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.VisitorID = new SelectList(db.Visitors, "ID", "Host", enquiry.VisitorID);
        //    return View(enquiry);
        //}

        //
        // GET: /EMS/Enquiry/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Enquiry enquiry = db.Enquiries.Find(id);
            if (enquiry == null)
            {
                return HttpNotFound();
            }
            return View(enquiry);
        }

        //
        // POST: /EMS/Enquiry/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Enquiry enquiry = db.Enquiries.Find(id);
            db.Enquiries.Remove(enquiry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}