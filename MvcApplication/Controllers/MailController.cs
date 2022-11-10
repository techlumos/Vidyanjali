using System.Configuration;
using ActionMailer.Net.Mvc;
using Vidyanjali.Areas.Admin.Models.Stakeholder;
using Vidyanjali.Models;
using Vidyanjali.Areas.EMS.Models;
using Vidyanjali.Areas.Admin.Models.Topic;

namespace Vidyanjali.Controllers
{
    public class MailController : MailerBase
    {
        public EmailResult EnquiryMailtoAdmin(Enquiry enquiry)
        {
            To.Add("info@vidyanjali.in");
            //CC.Add("yatheesh@lumos.in");

            //From = string.Format("Customer Care <{0}>", ConfigurationManager.AppSettings["SendFrom"]);
            From = "info@vidyanjali.in";
            Subject = string.Format("[{0}] - You have new Enquiry", "Vidyanjali");
            return Email("EnquiryMailtoAdmin", enquiry);
        }
        public EmailResult ThankyouforEnquiry(Enquiry enquiry)
        {
            To.Add(enquiry.Email);
            //CC.Add("suma@lumos.in");

           // From = string.Format("Customer Care <{0}>", ConfigurationManager.AppSettings["SendFrom"]);
            From = "info@vidyanjali.in";
            Subject = string.Format("[{0}] - Thanks for your Enquiry", "Vidyanjali");
           return Email("ThankyouforEnquiry", enquiry);
       }
        public EmailResult AdmissionMailtoAdmin(Admission enquiry)
        {
            To.Add("info@vidyanjali.in");
            //CC.Add("yatheesh@lumos.in");

           // From = string.Format("Customer Care <{0}>", ConfigurationManager.AppSettings["SendFrom"]);
            From = "info@vidyanjali.in";
            Subject = string.Format("[{0}] - You have new Admission Enquiry", "Vidyanjali");
            return Email("AdmissionMailtoAdmin", enquiry);
        }
        public EmailResult ThankyouforAdmission(Admission enquiry)
        {
            To.Add(enquiry.Email);
            //CC.Add("suma@lumos.in");

            //From = string.Format("Customer Care <{0}>", ConfigurationManager.AppSettings["SendFrom"]);
            From = "info@vidyanjali.in";
            Subject = string.Format("[{0}] - Thanks for your Admission Enquiry", "Vidyanjali");
            return Email("ThankyouforAdmission", enquiry);
        }
    }
}
