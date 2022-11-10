using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Models;

namespace Vidyanjali.Areas.Admin.Controllers.Topic
{
    public class AdmissionController : Controller
    {
        private readonly CoreContext db = new CoreContext();
        // GET: /Admin/Admission/

        public ActionResult Index()
        {
            var admission = db.Admissions.OrderByDescending(e => e.EnquiryDate);
            return View(admission.ToList());
        }

        //
        // GET: /EMS/Enquiry/Details/5

        public ActionResult Details(int id = 0)
        {
            Admission admission = db.Admissions.Find(id);
            if (admission == null)
            {
                return HttpNotFound();
            }
            return View(admission);
        }
    }
}
