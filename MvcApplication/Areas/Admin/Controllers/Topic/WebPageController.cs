using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Helper;
using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Models;
using System.Globalization;

namespace Vidyanjali.Areas.Admin.Controllers.Topic
{
    [Authorize(Roles = "Admin User,Authorizer,Super User")]
    public class WebPageController : Controller
    {
        private CoreContext _db = new CoreContext();
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        //
        // GET: /Admin/WebPage/

        public ActionResult Index()
        {
            var webPages = _db.WebPages.Where(w => w.ParentId != null).Include(w => w.ParentPage).OrderBy(o => o.Id);
            return View(webPages.ToList());
            //var webPages = _db.WebPages.Where(w => w.IsPublished && w.ParentId != null).Include(w => w.ParentPage).OrderBy(o => o.Id);
        }

        //
        // GET: /Admin/WebPage/Create

        public ActionResult Create()
        {
            ViewBag.ParentId = PrepareNestedPageDropdown();
            ViewBag.Properties = PreparePropertyDropdown();
            return View();
        }

        //
        // POST: /Admin/WebPage/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(WebPage webPage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (Utilities utility = new Utilities())
                    {
                        webPage.RelativeUrl = utility.GetUrl("Pages", webPage.Url);
                    }
                    if (User.IsInRole("Authorizer") || User.IsInRole("Super User")) { webPage.IsPublished = true; webPage.ReviewedOn = webPage.CreatedOn; }
                    else { webPage.IsPublished = true; }
                    _db.WebPages.Add(webPage);
                    _db.SaveChanges();

                    webPage.ReferenceCode = string.Format("{0}{1}",
                                                      webPage.Url.Replace("-", string.Empty).
                                                          Replace(" ", string.Empty),
                                                      Convert.ToString(webPage.Id));
                    _db.SaveChanges();

                    TempData["Success"] = "New Webpage has been Created";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            ViewBag.ParentId = PrepareNestedPageDropdown();
            ViewBag.Properties = PreparePropertyDropdown();
            return View(webPage);
        }

        //
        // GET: /Admin/WebPage/Edit/5

        public ActionResult Edit(int id = 0)
        {
            WebPage webpage = _db.WebPages.Find(id);
            if (webpage == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ParentId = PrepareNestedPageDropdown(webpage.Id);
            var selected = _db.WebPages.Include(p => p.ParentPage).Where(p => p.Id == id).FirstOrDefault();
            ViewBag.ParentId = new SelectList(_db.WebPages.Include(p => p.ParentPage), "Id", "Heading", selected.ParentId);

            return View(webpage);
        }

        //
        // POST: /Admin/WebPage/Edit/5

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(WebPage webpage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (User.IsInRole("Authorizer") || User.IsInRole("Super User")) { webpage.IsPublished = true; webpage.UpdatedOn = IndianDateTime; webpage.ReviewedOn = IndianDateTime; }
                    else { /*webpage.IsPublished = true;*/ webpage.UpdatedOn = IndianDateTime; }
                    _db.Entry(webpage).State = EntityState.Modified;
                    _db.SaveChanges();

                    TempData["Success"] = "All the changes have been saved";
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                TempData["Error"] = "Error";
            }
            ViewBag.ParentId = new SelectList(_db.WebPages.Include(w => w.ParentPage), "Id", "Path", webpage.ParentId);
            return View(webpage);
        }


        #region local functions

        private dynamic PrepareNestedPageDropdown(int? parentId = 0)
        {
            IDictionary<string, IEnumerable<SelectListItem>> allPages = new Dictionary<string, IEnumerable<SelectListItem>>();

            // Put "Home" as static under "Root" Page
            var subpageList = new List<SelectListItem>
                                  {
                                      new SelectListItem
                                          {
                                              Value = "1",
                                              Text = "Home",
                                              Selected = 1 == parentId
                                          }
                                  };
            allPages.Add("Root", subpageList);


            // And, Get all other pages dynamically form database context
            var pages = _db.WebPages.Include(w => w.ParentPage).ToList();
            foreach (var page in pages)
            {
                subpageList = new List<SelectListItem>();
                var subPages = page.ChildPages;
                if (subPages.Any())
                {
                    subpageList.AddRange(subPages.Select(
                        subPage => new SelectListItem
                        {
                            Value = subPage.Id.ToString(CultureInfo.InvariantCulture),
                            Text = subPage.Heading,
                            Selected = subPage.ParentId == parentId
                        }));
                    allPages.Add(page.Heading, subpageList);
                }
            }
            return allPages;
        }

        private dynamic PreparePropertyDropdown()
        {
            return new SelectList(Enum.GetValues(typeof(PageProperties)).Cast<PageProperties>().Select(
                p => new { ID = (int)p, Name = p.ToString() }), "ID", "Name");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}