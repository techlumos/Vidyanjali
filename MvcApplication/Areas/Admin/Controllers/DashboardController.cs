using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Models;
using Vidyanjali.Areas.Admin.Helpers;
using Vidyanjali.Areas.Admin.Models;

namespace Vidyanjali.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin User,Authorizer,Super User")]
    public class DashboardController : Controller
    {
        private readonly CoreContext _db = new CoreContext();
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly DateTime _now, _today, _weekly, _monthly, _qyarterly, _yearly;
        
        public DashboardController()
        {
            _now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
            _today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, IndianZone);
            _weekly = _now.AddDays(-7);
            _monthly =_now.AddMonths(-1);
            _qyarterly = _now.AddMonths(-3);
            _yearly = _now.AddYears(-1);
        }

        public ActionResult UserManual()
        {
            return View();
        }

        //
        // GET: /Admin/Dashboard/        
        public ActionResult Index()
        {
            // Get Registered Customers
            RegisteredDashboard();
            return View();
        }

        private void RegisteredDashboard()
        {


            var enquirycount = 0;
            var enquiriestoday = 0;
            var enquiriesthisweek = 0;
            var enquiriesthismonth = 0;
            var enquiriesthisquarter = 0;
            var enquiriesthisyear = 0;
            var webpagescount = 0;
            var webpagesactive = 0;
            var webpagesinactive = 0;
            var testimonialsCount = 0;

            enquirycount = _db.Enquiries.Count();
            enquiriestoday = _db.Enquiries.Count(e => e.EnquiryDate >= _today && e.EnquiryDate <= _now);
            enquiriesthisweek = _db.Enquiries.Count(e => e.EnquiryDate >= _weekly && e.EnquiryDate <= _now);
            enquiriesthismonth = _db.Enquiries.Count(e => e.EnquiryDate >= _monthly && e.EnquiryDate <= _now);
            enquiriesthisquarter = _db.Enquiries.Count(e => e.EnquiryDate >= _qyarterly && e.EnquiryDate <= _now);
            enquiriesthisyear = _db.Enquiries.Count(e => e.EnquiryDate >= _yearly && e.EnquiryDate <= _now);

            var Admissioncount = _db.Admissions.Count();
            var admissionstoday = _db.Admissions.Count(e => e.EnquiryDate >= _today && e.EnquiryDate <= _now);
            var admissionsthisweek = _db.Admissions.Count(e => e.EnquiryDate >= _weekly && e.EnquiryDate <= _now);
            var admissionsthismonth = _db.Admissions.Count(e => e.EnquiryDate >= _monthly && e.EnquiryDate <= _now);
            var admissionsthisquarter = _db.Admissions.Count(e => e.EnquiryDate >= _qyarterly && e.EnquiryDate <= _now);
            var admissionsthisyear = _db.Admissions.Count(e => e.EnquiryDate >= _yearly && e.EnquiryDate <= _now);

            var webpages = _db.WebPages.ToList();
            if (webpages.Any())
            {
                webpagescount = webpages.Count(p => p.IsPublished);
                webpagesactive = webpages.Count(p => p.IsPublished);
                webpagesinactive = webpages.Count(p => p.IsPublished == false);
            }

            testimonialsCount = _db.Testimonials.Count();

            ViewBag.admissioncount = Admissioncount;
            ViewBag.admissiontoday = admissionstoday;
            ViewBag.admissionthisweek = admissionsthisweek;
            ViewBag.admissionthismonth = admissionsthismonth;
            ViewBag.admissionthisquarter = admissionsthisquarter;
            ViewBag.admissionthisyear = admissionsthisyear;

            ViewBag.enquirycount = enquirycount;
            ViewBag.enquiriestoday = enquiriestoday;
            ViewBag.enquiriesthisweek = enquiriesthisweek;
            ViewBag.enquiriesthismonth = enquiriesthismonth;
            ViewBag.enquiriesthisquarter = enquiriesthisquarter;
            ViewBag.enquiriesthisyear = enquiriesthisyear;

            ViewBag.webpagescount = webpagescount;
            ViewBag.webpagesactive = webpagesactive;
            ViewBag.webpagesinactive = webpagesinactive;

            ViewBag.testimonialsCount = testimonialsCount;
        }

    }
}
