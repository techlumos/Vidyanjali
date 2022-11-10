using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Areas.Admin.Helpers;
using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Areas.EMS.Models;
using Vidyanjali.Comman;
using Vidyanjali.Models;
using System.Data.Entity.Validation;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace Vidyanjali.Controllers
{
    public class HomeController : Controller
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        readonly CoreContext _db = new CoreContext();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        //TODO: To be Improved
        public ActionResult InnerPage(string url)
        {
            var requestedPage = _db.WebPages.FirstOrDefault(p => p.Url == url && p.IsPublished);
            if (requestedPage == null)
            {
                //return View("Index");
                return RedirectToAction("Index");
            }
            //else if (requestedPage.ReferenceCode == "ContactUs7")
            //{
            //    ViewBag.title = requestedPage.Heading;
            //    return View("_ContactUs");
            //}
           // ViewBag.Id = requestedPage.Id;
            ViewBag.title = requestedPage.SubHeading;
            return View(requestedPage);
        }

        public ActionResult BlogPage(string url)
        {
            var requestedBlog = _db.Blogs.FirstOrDefault(p => p.Url == url && p.IsPublished);
            if (requestedBlog == null)
            {
                //return View("Index");
                return Redirect(new Helper.Utilities().GetUrl("Pages", "Blogs"));
            }
            //else if (requestedPage.ReferenceCode == "ContactUs7")
            //{
            //    ViewBag.title = requestedPage.Heading;
            //    return View("_ContactUs");
            //}
            // ViewBag.Id = requestedPage.Id;
            ViewBag.title = requestedBlog.Heading;
            return View(requestedBlog);
        }

        [HttpPost]
        public ActionResult Enquiry(Enquiry enquiry)
        {
            var cookiee = Request.Cookies["SRP.Customer"];
            try
            {
                if (ModelState.IsValid)
                {
                    if (Session["Captcha"] == null || Session["Captcha"].ToString() != enquiry.Captcha)
                    {
                        TempData["Message"] = "Wrong Captcha, please try again.";
                        // ModelState.AddModelError("Captcha", "Wrong value of sum, please try again.");
                        return Redirect(new Helper.Utilities().GetUrl("Pages", "Contact-Us"));

                    }
                    else
                    {
                        try
                        {
                            Visitor visitor = _db.Visitors.Find(Convert.ToInt32(cookiee.Value)); 
                            Enquiry objEnq = new Enquiry();
                            objEnq.Name = enquiry.Name;
                            objEnq.Email = enquiry.Email;
                            objEnq.Message = enquiry.Message;
                            objEnq.Phone = enquiry.Phone;
                            objEnq.Subject = enquiry.Subject;
                            objEnq.VisitorID = visitor.ID;
                            objEnq.ReferenceId = visitor.ID;
                            _db.Enquiries.Add(objEnq);
                            _db.SaveChanges();
                            new MailController().EnquiryMailtoAdmin(enquiry).Deliver();
                            new MailController().ThankyouforEnquiry(enquiry).Deliver();
                            return Redirect(new Helper.Utilities().GetUrl("Pages", "Thank-You"));
                        }
                        catch (DbUpdateException ex)
                        {
                            UpdateException updateException = (UpdateException)ex.InnerException;
                            SqlException sqlException = (SqlException)updateException.InnerException;

                            foreach (SqlError error in sqlException.Errors)
                            {
                                // TODO: Do something with your errors
                            }
                        }
                    }
                }
            }
            catch { }
            return Redirect(new Helper.Utilities().GetUrl("Pages", "Contact-Us"));

        }

        [HttpPost]
        public ActionResult AdmissionEnquiry(FormCollection formCollection)
        {
            var cookiee = Request.Cookies["SRP.Customer"];
            try
            {
                //if (ModelState.IsValid)
                //{
                if (Session["Captcha"] == null || Session["Captcha"].ToString() != formCollection["Captcha"])
                {
                    TempData["Message"] = "Wrong Captcha, please try again.";
                    ModelState.AddModelError("Captcha", "Wrong value of sum, please try again.");
                    return Redirect(new Helper.Utilities().GetUrl("Pages", "Admission-Enquiry"));

                }
                else
                {
                    try
                    {
                        Visitor visitor = _db.Visitors.Find(Convert.ToInt32(cookiee.Value));
                        Admission objEnq = new Admission();
                        //objEnq.StudentName = enquiry.StudentName;
                        //objEnq.FatherName = enquiry.FatherName;
                        //objEnq.MotherName = enquiry.MotherName;
                        //objEnq.Email = enquiry.Email;
                        //objEnq.Message = enquiry.Message;
                        //objEnq.Phone = enquiry.Phone;
                        //objEnq.DOB = enquiry.DOB;
                        //objEnq.Gender = enquiry.Gender;
                        //objEnq.Class = enquiry.Class;
                        //objEnq.AboutUs = enquiry.AboutUs;
                        objEnq.StudentName = formCollection["StudentName"];
                        objEnq.FatherName = formCollection["FatherName"];
                    objEnq.MotherName = formCollection["MotherName"];
                    objEnq.Email = formCollection["Email"];
                    objEnq.Message = formCollection["Message"];
                    objEnq.Phone = formCollection["Phone"];
                    objEnq.DOB = formCollection["DOB"];
                    objEnq.Gender = formCollection["Gender"];
                    objEnq.Class = formCollection["Class"];
                    objEnq.AboutUs = formCollection["Aboutus"];
                    objEnq.VisitorID = visitor.ID;
                        objEnq.ReferenceId = visitor.ID;
                        _db.Admissions.Add(objEnq);
                        _db.SaveChanges();
                        new MailController().AdmissionMailtoAdmin(objEnq).Deliver();
                        new MailController().ThankyouforAdmission(objEnq).Deliver();
                        return Redirect(new Helper.Utilities().GetUrl("Pages", "Thank-You"));
                    }
                    catch (DbUpdateException ex)
                    {
                        UpdateException updateException = (UpdateException)ex.InnerException;
                        SqlException sqlException = (SqlException)updateException.InnerException;

                        foreach (SqlError error in sqlException.Errors)
                        {
                            // TODO: Do something with your errors
                        }
                    }
                   
                }
            }
            catch (DbUpdateException ex)
            {
                UpdateException updateException = (UpdateException)ex.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;

                foreach (SqlError error in sqlException.Errors)
                {
                    // TODO: Do something with your errors
                }
            }
            return Redirect(new Helper.Utilities().GetUrl("Pages", "Admission-Enquiry"));

        }

        [HttpGet]
        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            //generate new question 
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);

            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers;

            characters += alphabets + small_alphabets + numbers;
            int length = 6;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }

            var captcha = string.Format("{0}", otp);

            //store answer 
            //Session["Captcha"] = a + b;
            Session["Captcha" + prefix] = otp;

            //image stream 
            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise 
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, (x - r), (y - r), r, r);
                    }
                }

                //add question 
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg 
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }
    }
}
