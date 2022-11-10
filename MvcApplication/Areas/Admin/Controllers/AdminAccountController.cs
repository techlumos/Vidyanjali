using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Vidyanjali.Areas.Admin.Helpers;
using Vidyanjali.Areas.Admin.Models.Stakeholder;
using Vidyanjali.Models;
using Vidyanjali.Comman;//TODO: Analyze this
//using DotNetOpenAuth.OpenId.RelyingParty;
using System.Data;

namespace Vidyanjali.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin User,Authorizer,Super User")]
    public class AdminAccountController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly DateTime _indianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
        private CoreContext db = new CoreContext();

        //
        // GET: /Admin/AdminAccount/logon
        [AllowAnonymous]
        public ActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var requestedurl = HttpContext.Request.RawUrl;
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string path = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            string host = System.Web.HttpContext.Current.Request.Url.Host;
            var adminuser = db.AdminUsers.FirstOrDefault(p => p.Email == User.Identity.Name && p.IsActive);
            if (User.Identity.IsAuthenticated && adminuser != null)
            {
                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }

            }
            return View();
        }

        // /Admin/AdminAccount/logon
        [AllowAnonymous]
        [HttpPost, ValidateInput(false)]
        public ActionResult LogOn(AdminUser model, string returnUrl)
        {

            using (var db = new CoreContext())
            {
                string encodedPassword = Utilities.EncodePassword(model.Password);

                var user = db.AdminUsers.FirstOrDefault(p => p.Email == model.Email && p.Password == encodedPassword && p.IsActive == true);
                if (ModelState.IsValid)
                {

                    if (user != null)
                    {
                        var authTicket = new FormsAuthenticationTicket(
                                            1,                             // version
                                            model.Email,                      // user name
                                            DateTime.Now,                  // created
                                            DateTime.Now.AddMinutes(30),   // expires
                                            false,                    // persistent?
                                            user.Role.Name            // can be used to store roles
                                            );

                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                        //FormsAuthentication.SetAuthCookie(model.Email, false);

                        user.LastSeen = DateTime.Now;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            this.ShowMessage(Extension.MessageType.Success, string.Format("Welcome {0}", user.Email), true);
                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                }
                else
                {
                    foreach (var modelerror in ViewData.ModelState.Values)
                    {
                        foreach (var error in modelerror.Errors)
                        {
                            var errorMessage = error.ErrorMessage;
                            var exceptionMessage = error.Exception;
                        }
                    }
                }
                this.ShowMessage(Extension.MessageType.Error, "The user name or password provided is incorrect.", true);
                ModelState.AddModelError("LogOnError", "The user name or password provided is incorrect.");
                //return RedirectToAction("LogOn", "AdminAccount");
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return Redirect("/Admin/AdminAccount/logon");
        }

        public ActionResult Index()
        {
            if (!User.IsInRole("Super User")) return RedirectToAction("NotAuthorized", "AdminAccount", new { area = "Admin" });
            var adminusers = db.AdminUsers;
            return View(adminusers.ToList());
        }

        public ActionResult Create()
        {
            if (!User.IsInRole("Super User")) return RedirectToAction("NotAuthorized", "AdminAccount", new { area = "Admin" });
            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.Name != "Customer") , "Id", "Name");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(AdminUser adminuser)
        {
            if (!User.IsInRole("Super User")) return RedirectToAction("NotAuthorized", "AdminAccount", new { area = "Admin" });
            if (ModelState.IsValid)
            {
                adminuser.Password = Utilities.EncodePassword(adminuser.Password);
                db.AdminUsers.Add(adminuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.Name != "Customer"), "Id", "Name", adminuser.RoleId);
            return View(adminuser);
        }

        public ActionResult Edit(int id = 0)
        {
            if (!User.IsInRole("Super User")) return RedirectToAction("NotAuthorized", "AdminAccount", new { area = "Admin" });
            AdminUser adminuser = db.AdminUsers.Find(id);
            adminuser.Password = Utilities.DecodePassword(adminuser.Password);
            if (adminuser == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.Name != "Customer"), "Id", "Name", adminuser.RoleId);
            return View(adminuser);
        }

        [HttpPost]
        public ActionResult Edit(AdminUser adminuser)
        {
            if (!User.IsInRole("Super User")) return RedirectToAction("NotAuthorized", "AdminAccount", new { area = "Admin" });
            if (ModelState.IsValid)
            {
                adminuser.Password = Utilities.EncodePassword(adminuser.Password);
                db.Entry(adminuser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.Name != "Customer"), "Id", "Name", adminuser.RoleId);
            return View(adminuser);
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
