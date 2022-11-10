using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vidyanjali.Helper
{
    public class Home
    {
        private readonly CoreContext _db = new CoreContext();
        private readonly string _urlWord;
        private WebPage _currentPage;
        private readonly string _controller, _action;

        public Home()
        {
            _urlWord = Convert.ToString(HttpContext.Current.Request.RequestContext.RouteData.Values["url"]);
            _currentPage =
                _db.WebPages.FirstOrDefault(
                    a => a.Url.Equals(_urlWord, StringComparison.CurrentCultureIgnoreCase) && a.IsPublished);

            _controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            _action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
        }

        public MvcHtmlString Menu()
        {
            //string requestedGroup = group.ToString();
            var pages = _db.WebPages.Where(p => p.Tags.Contains("Menu") && p.IsPublished && p.ParentId == 1).OrderBy(p => p.Priority);
            var sb = new StringBuilder();
            sb.Clear();

            if (_currentPage == null)
            {
                _currentPage = _db.WebPages.FirstOrDefault(i => i.Id == 1);
            }

            if (pages.Any())
            {
                 sb.AppendFormat("<ul id=\"main-menu\" class=\"sm sm-blue\">");
                foreach (var page in pages)
                {
                    if (page.ReferenceCode != "Home")
                    {
                        var childpages = _db.WebPages.Where(p => p.ParentId == page.Id && p.IsPublished).OrderBy(p => p.Priority);
                        if (childpages.Any())
                        {
                            sb.AppendFormat("<li class=\"dropdown\"> <a href=\"{0}\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" id=\"content\">{1}</a>", GetUrl("Pages", page.ReferenceCode), page.Heading.ToUpper());
                            sb.AppendFormat("<ul class=\"dropdown-menu\" id=\"sub-menu\">");
                            sb.AppendFormat("<div class=\"sub-menu-list\" >");
                            foreach (var childpage in childpages)
                            {
                                var subchildpages = _db.WebPages.Where(p => p.ParentId == childpage.Id && p.IsPublished).OrderBy(p => p.Priority);
                                if (subchildpages.Any())
                                {
                                    sb.AppendFormat("<li><a href=\"{0}\" >{1}</a>", GetUrl("Pages", childpage.ReferenceCode), childpage.Heading);
                                    sb.AppendFormat("<ul>");
                                    foreach (var subchildpage in subchildpages)
                                    {
                                        sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", GetUrl("Pages", subchildpage.ReferenceCode), subchildpage.Heading);
                                    }
                                    sb.AppendFormat("</ul>");
                                    sb.AppendFormat("</li>");
                                }
                                else { sb.AppendFormat("<li><a href=\"{0}\" >{1}</a></li>", GetUrl("Pages", childpage.ReferenceCode), childpage.Heading); }
                            }
                            sb.AppendFormat("</div>");
                            sb.AppendFormat("</ul>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li class=\"dropdown\" id=\"content-a\"> <a  href=\"{0}\" id=\"content\">{1}</a>", GetUrl("Pages", page.ReferenceCode), page.Heading.ToUpper());
                            sb.AppendFormat("</li>");
                        }
                    }

                }

                sb.Append("</ul>");
            }
            return new MvcHtmlString(sb.ToString());
        }


        public MvcHtmlString indexMenu()
        {
            //string requestedGroup = group.ToString();
            var pages = _db.WebPages.Where(p => p.Tags.Contains("Menu") && p.IsPublished && p.ParentId == 1).OrderBy(p => p.Priority);
            var sb = new StringBuilder();
            sb.Clear();

            if (_currentPage == null)
            {
                _currentPage = _db.WebPages.FirstOrDefault(i => i.Id == 1);
            }

            if (pages.Any())
            {
                sb.AppendFormat("<ul id=\"main-menu\" class=\"sm sm-blue\">");
                sb.AppendFormat("<li><a href=\"/\"><i class=\"fa fa-home\"></i></a></li>");
                foreach (var page in pages)
                {
                    if (page.ReferenceCode != "Home")
                    {
                        var childpages = _db.WebPages.Where(p => p.ParentId == page.Id && p.IsPublished).OrderBy(p => p.Priority);
                        if (childpages.Any())
                        {
                            sb.AppendFormat("<li class=\"dropdown\"> <a href=\"{0}\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" id=\"content\">{1}</a>", GetUrl("Pages", page.ReferenceCode), page.Heading.ToUpper());
                            sb.AppendFormat("<ul class=\"dropdown-menu\" id=\"sub-menu\">");
                            sb.AppendFormat("<div class=\"sub-menu-list\" >");
                            foreach (var childpage in childpages)
                            {
                                var subchildpages = _db.WebPages.Where(p => p.ParentId == childpage.Id && p.IsPublished).OrderBy(p => p.Priority);
                                if (subchildpages.Any())
                                {
                                    sb.AppendFormat("<li><a href=\"{0}\" >{1}</a>", GetUrl("Pages", childpage.ReferenceCode), childpage.Heading);
                                    sb.AppendFormat("<ul>");
                                    foreach (var subchildpage in subchildpages)
                                    {
                                        sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", GetUrl("Pages", subchildpage.ReferenceCode), subchildpage.Heading);
                                    }
                                    sb.AppendFormat("</ul>");
                                    sb.AppendFormat("</li>");
                                }
                                else { sb.AppendFormat("<li><a href=\"{0}\" >{1}</a></li>", GetUrl("Pages", childpage.ReferenceCode), childpage.Heading); }
                            }
                            sb.AppendFormat("</div>");
                            sb.AppendFormat("</ul>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li class=\"dropdown\" id=\"content-a\"> <a  href=\"{0}\" id=\"content\">{1}</a>", GetUrl("Pages", page.ReferenceCode), page.Heading.ToUpper());
                            sb.AppendFormat("</li>");
                        }
                    }

                }

                sb.Append("</ul>");
            }
            return new MvcHtmlString(sb.ToString());
        }



        public string GetUrl(string routeName, string referenceCode)
        {
            switch (routeName)
            {
                default:
                    if (!string.IsNullOrEmpty(referenceCode))
                    {
                        var page = _db.WebPages.AsNoTracking().FirstOrDefault(p => p.ReferenceCode == referenceCode);
                        return page != null ? MapUrl(routeName, new { url = page.Url }) : "#";
                    }
                    return "#";
            }
        }

        private string MapUrl(string routeName, object routeParameters)
        {
            var directory = new RouteValueDictionary(routeParameters);
            VirtualPathData pathData = RouteTable.Routes.GetVirtualPath(HttpContext.Current.Request.RequestContext,
                                                                        routeName, directory);
            return pathData != null ? pathData.VirtualPath : "#";
        }

        public MvcHtmlString PageBc(WebPage requestedPage)
        {
            var sb = new StringBuilder();
            sb.Clear();

            var page = requestedPage;
            if (requestedPage != null)
            {
                sb.Append("<ol class=\"breadcrumb\">");
                sb.Append("<li class=\"breadcrumb-item\"><a href=\"/\">Home </a></li>");
                while (page.ParentId != 1)
                {
                    page = requestedPage.ParentPage;
                    sb.AppendFormat("<li class=\"breadcrumb-item\"><a href=\"\">{1} </a></li>", page.ReferenceCode, page.SubHeading);
                }
                sb.AppendFormat("<li class=\"breadcrumb-item active\" aria-current=\"page\">{1}</li>", requestedPage.ReferenceCode, requestedPage.SubHeading);
                sb.Append("</ol>");
            }
            return new MvcHtmlString(sb.ToString());
        }
        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}