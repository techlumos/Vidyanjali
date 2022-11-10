using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Models;

namespace Vidyanjali.Areas.EMS.Models
{
    public class UserTrackingActionFilterAttribute : ActionFilterAttribute
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public override void OnResultExecuting(ResultExecutingContext resultExecutingContext)
        {
            base.OnResultExecuting(resultExecutingContext);

            HttpRequestBase request = resultExecutingContext.HttpContext.Request;
            HttpResponseBase response = resultExecutingContext.HttpContext.Response;

            // Check if visitor exist, if exist do nothing, just leave this!
            if (request.Cookies["SRP.Customer"] != null) return;
            if (request.RawUrl.Contains("Thankyou")) return;

            string sourceType = "";
            string source = "";
            string campaignName = "";
            string keywords = "";
            string host = "";
            string queryString = "";

            //if not, Proceed and add visitor details to cookie
            // Get the requested URL to decide whether it's SEM (ad posted on Adwords, facebook etc.)

            string requestedUrl = string.Empty, referrerUrl = string.Empty;
            // Just making sure that, URL is not null at this moment
            if (request.Url != null)
            {
                requestedUrl = request.Url.ToString();

                var uriToEnquiry = new Uri(requestedUrl);
                host = uriToEnquiry.Host;

                if (!string.IsNullOrEmpty(uriToEnquiry.Query))
                {
                    queryString = uriToEnquiry.Query;

                    var data = HttpUtility.ParseQueryString(uriToEnquiry.Query);
                    foreach (string item in data)
                    {
                        switch (item)
                        {
                            case "s": // Source (adwords, facebook, etc.)
                                source = data[item];
                                sourceType = "SEM";
                                break;
                            case "kw":
                                keywords = data[item];
                                break;
                            case "cname":
                                campaignName = data[item];
                                break;
                        }
                    }
                }
            }

            if (request.UrlReferrer != null)
            {
                referrerUrl = request.UrlReferrer.ToString();

                var uriToEnquiry = new Uri(referrerUrl);
                host = uriToEnquiry.Host;

                if (!string.IsNullOrEmpty(uriToEnquiry.Query))
                {
                    queryString = queryString + " | " + uriToEnquiry.Query;

                    var data = HttpUtility.ParseQueryString(uriToEnquiry.Query);
                    foreach (string item in data)
                    {
                        switch (item)
                        {
                            case "q": // Google & Bing Search Keywords
                            case "p": // Yahoo Search Keywords
                                source = string.IsNullOrEmpty(source)
                                             ? uriToEnquiry.Host
                                             : source + " | " + uriToEnquiry.Host;
                                keywords = string.IsNullOrEmpty(keywords)
                                               ? data[item]
                                               : keywords + " | " + data[item];
                                sourceType = string.IsNullOrEmpty(sourceType)
                                                 ? "SEO"
                                                 : sourceType + " | " + "SEO";
                                break;
                        }
                    }
                }
            }

            using (var db = new CoreContext())
            {
                var visitor = new Visitor
                {
                    Browser = request.ServerVariables["HTTP_USER_AGENT"],
                    CampaignName = string.IsNullOrEmpty(campaignName) ? "Direct" : campaignName,
                    Host = host,
                    IpAddress = request.ServerVariables["REMOTE_ADDR"],
                    Keywords = string.IsNullOrEmpty(keywords) ? "None" : keywords,
                    OperatingSystem = request.Browser.Platform, /*request.ServerVariables["SERVER_SOFTWARE"],*/
                    QueryString = string.IsNullOrEmpty(queryString) ? "None" : queryString,
                    ReferralUrl = string.IsNullOrEmpty(referrerUrl) ? "None" : referrerUrl,
                    LandingUrl = requestedUrl,
                    Source = string.IsNullOrEmpty(source) ? "Direct" : source,
                    SourceType = string.IsNullOrEmpty(sourceType) ? "Direct" : sourceType,
                    VisitedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone)
                };

                try
                {
                    db.Visitors.Add(visitor);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var sb = new StringBuilder();
                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }
                }

                var httpCookie = response.Cookies["SRP.Customer"];
                if (httpCookie != null)
                {
                    httpCookie.Value = Convert.ToString(visitor.ID);
                    httpCookie.Expires = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone).AddHours(5);
                }
            }
        }
    }
}

