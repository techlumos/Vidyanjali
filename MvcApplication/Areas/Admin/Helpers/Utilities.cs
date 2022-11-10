using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
//using Vidyanjali.Areas.Admin.Models.Catalog;
using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Areas.Admin.Models.Stakeholder;
using Vidyanjali.Models;

namespace Vidyanjali.Areas.Admin.Helpers
{
    public class Utilities : IDisposable
    {
        private readonly CoreContext _db = new CoreContext();
        static CoreContext _Sdb = new CoreContext();
        private readonly string _urlWord;
        private WebPage _currentPage;

        public Utilities()
        {
            _urlWord = Convert.ToString(HttpContext.Current.Request.RequestContext.RouteData.Values["url"]);
            _currentPage = _db.WebPages.AsNoTracking().FirstOrDefault(a => a.Url.Equals(_urlWord, StringComparison.CurrentCultureIgnoreCase) && a.IsPublished);
        }

        public bool IsUserAthorized(AdminUser auser, string url)
        {
            if (auser.Role.RoleMenus.Any(r => r.Menu.Url == url))
            {
                return true;
            }
            return false;
        }

        public AdminUserValidateModel IsUserAuchenticatedAndAthorized(string userEmail, string url)
        {
            AdminUserValidateModel adminvalidator = new AdminUserValidateModel();
            url = url.EndsWith("/")?url.Trim().Remove(url.Length - 1) : url;
            var auser = _db.AdminUsers.FirstOrDefault(p => p.Email == userEmail && p.IsActive);
            if (auser == null) return adminvalidator;
            adminvalidator.IsAdminUser = auser != null ? true : false;
            adminvalidator.IsAuthorizedUser = (auser.Role.RoleMenus.Any(r => r.Menu.Url == url)) == true ? true : false;
            adminvalidator.AdminUser = auser;
            return adminvalidator;
        }

        public bool IsAdminUser(AdminUser auser, string url)
        {
            if (auser.Role.RoleMenus.Any(r => r.Menu.Url == url))
            {
                return true;
            }
            return false;
        }

        #region Password Encode, Decode
        public static string EncodePassword(string plainPassword)
        {
            try
            {
                var getEncodeByte = Encoding.UTF8.GetBytes(plainPassword);
                return Convert.ToBase64String(getEncodeByte);
            }
            catch
            {
                return plainPassword;
            }
        }

        public static string DecodePassword(string encodedPassword)
        {
            try
            {
                var objEncoding = new UTF8Encoding();
                Decoder objDecoder = objEncoding.GetDecoder();

                byte[] getDecodeByte = Convert.FromBase64String(encodedPassword);
                int charCount = objDecoder.GetCharCount(getDecodeByte, 0, getDecodeByte.Length);
                var getDecodedChar = new char[charCount];
                objDecoder.GetChars(getDecodeByte, 0, getDecodeByte.Length, getDecodedChar, 0);

                var result = new String(getDecodedChar);
                return result;
            }
            catch
            {
                return encodedPassword;
            }
        }
        #endregion

        #region URL Generation

        public static string GetUrl(string routeName, string userFriendlyVirtualUrl = "", string referenceCode = "")
        {
            switch (routeName)
            {
                //    //Commented by Sunil 15May17
                //case "Category":
                //    if (!string.IsNullOrEmpty(referenceCode))
                //    {
                //        var category = _db.Categories.AsNoTracking().FirstOrDefault(p => p.ReferenceCode == referenceCode);
                //        return category != null ? MapUrl(routeName, new { curl = category.Url }) : "#";
                //    }
                //    return !string.IsNullOrEmpty(userFriendlyVirtualUrl)
                //               ? MapUrl(routeName, new { curl = userFriendlyVirtualUrl })
                //               : "#";
                default:
                    if (!string.IsNullOrEmpty(referenceCode))
                    {
                        var page = _Sdb.WebPages.AsNoTracking().FirstOrDefault(p => p.ReferenceCode == referenceCode);
                        return page != null ? MapUrl(routeName, new { url = page.Url }) : "#";
                    }
                    return !string.IsNullOrEmpty(userFriendlyVirtualUrl)
                               ? MapUrl(routeName, new { url = userFriendlyVirtualUrl })
                               : "#";
            }
        }

        private static string MapUrl(string routeName, object routeParameters)
        {
            var directory = new RouteValueDictionary(routeParameters);
            VirtualPathData pathData = RouteTable.Routes.GetVirtualPath(HttpContext.Current.Request.RequestContext, routeName, directory);
            return pathData != null ? pathData.VirtualPath.ToLower() : "#";
        }

        #endregion

        private static Dictionary<string, MvcHtmlString> DicPrimaryMenu = new Dictionary<string, MvcHtmlString>();

        #region Menus
       // with dropdown menus
        //public MvcHtmlString PrimaryMenu(string tags)
        //{
        //    var pages = _db.WebPages.Where(p => p.Tags.Contains(tags) && p.IsPublished).OrderBy(p => p.Priority);
        //    //  var  pages = _db.Groups.Where(p => p.Tags.Contains(tags) && p.IsPublished).OrderBy(p => p.Priority);
        //    var sb = new StringBuilder();
        //    sb.Clear();
        //    // If requested page is null (i.e. No inner page requested), Then set (_currentPage = Home)
        //    if (_currentPage == null)
        //    {
        //        _currentPage = _db.WebPages.FirstOrDefault(i => i.Id == 1);
        //    }
        //    if (pages.Any())
        //    {
        //        sb.AppendFormat("<ul class=\"nav navbar-nav navbar-right navbar-dropdown\">");
        //        foreach (var page in pages)
        //        {
        //            var group = _db.Groups.FirstOrDefault(c => c.ReferenceCode == page.ReferenceCode);
        //            var childPages =
        //        _db.Categories.Where(c => c.ParentCategoryId == null && c.IsPublished  && group.Id== c.GroupId  ).OrderBy(o => o.Id).ToList();

        //           // var childPages = page.ChildPages.Where(p => p.IsPublished).OrderBy(p => p.Priority);
        //            //  var childPages = page.Groups.Where(p => p.IsPublished).OrderBy(p => p.Priority);
        //            if (childPages.Any())
        //            {
        //                sb.AppendFormat(
        //                    "<li class=\"lidropdown\"><a href=\"{0}\" class=\"dropdown-toggle\" data-hover=\"dropdown\">{1} <span class=\"glyphicon glyphicon-chevron-down glyphicon-chevron-dropdown\"></span></a>", page.RelativeUrl,page.Heading);
        //                    //GetUrl("Pages", page.RelativeUrl), page.Heading);
        //                sb.AppendFormat("<ul class=\"dropdown-menu dropdown-menu-left pull-left\">");
        //                foreach (var childPage in childPages)
        //                {
        //                    var productslist = _db.Products.Where(c => c.IsPublished && c.CategoryId == childPage.Id).OrderBy(o => o.Id).ToList();
        //                    var products = _db.Products.Where(p =>p.GroupId == childPage.GroupId && p.IsPublished);
        //                    sb.AppendFormat("<li class =\"product\"><a href=\"{0}\">{1}</a>", childPage.RelativeUrl, childPage.Name);
        //                    if (products.Any())
        //                    {

        //                        sb.AppendFormat("<ul class=\"dropdown-menu1 dropdown-menu-left\" >");
        //                        foreach (var product in productslist)

        //                            sb.AppendFormat("<li class=\"liproductdropdown\"style ='margin:0px;position:relative;'><a href=\"{0}\">{1}</a></li>", product.RelativeUrl, product.Name);
        //                        sb.AppendFormat("</ul>");
        //                    }

        //                    sb.AppendFormat("</li>");

        //                }
        //                sb.Append("</li></ul>");
        //            }
        //            else
        //            {
        //                sb.AppendFormat(
        //                    "<li><a href=\"{0}\">{1} <span class=\"glyphicon glyphicon-chevron-down glyphicon-chevron-dropdown\"></span></a></li>",
        //                    page.RelativeUrl, page.Heading);
        //            }
        //        }
        //        sb.Append("</ul>");
        //    }
        //    return new MvcHtmlString(sb.ToString());
        //}


        public MvcHtmlString PrimaryMenu(string tags)
        {
            //WebPage requestedPage = _db.WebPages.FirstOrDefault(a => a.ReferenceCode!=null);
            var pages = _db.WebPages.Where(p => p.Tags.Contains(tags) && p.IsPublished).OrderBy(p => p.Priority);
            var requestedPage = _currentPage;
            var sb = new StringBuilder();
            sb.Clear();
            // If requested page is null (i.e. No inner page requested), Then set (_currentPage = Home)
            if (_currentPage == null || requestedPage == null)
            {
                _currentPage = _db.WebPages.FirstOrDefault(i => i.Id == 1);

            }
            if (pages.Any())
            {
                sb.AppendFormat("<ul class=\"right_separate clear_fix\">");
                foreach (var page in pages)
                {
                    var childPages = page.ChildPages.Where(p => p.IsPublished).OrderBy(p => p.Priority);

                    sb.AppendFormat(
                       _currentPage != null && (_currentPage.ParentPage != null && ((page.ReferenceCode == _currentPage.ReferenceCode || page.ReferenceCode == _currentPage.ParentPage.ReferenceCode)))

                                 ? "<li><a  href=\"{0}\">{1}</a></li>"
                                 : "<li><a   href=\"{0}\">{1}</a></li>",
                            page.RelativeUrl, page.Heading);

                  
                }


               
                sb.Append("</ul>");
            }
            return new MvcHtmlString(sb.ToString());
        }

        private static Dictionary<string, MvcHtmlString> DicFooterMenu = new Dictionary<string, MvcHtmlString>();

        public MvcHtmlString FooterMenu(string parentReferenceCode)
        {
            var sb = new StringBuilder();
            sb.Clear();

            var parentPage = _db.WebPages.FirstOrDefault(p => p.ReferenceCode.Contains(parentReferenceCode) && p.IsPublished);
            if (parentPage != null)
            {
                sb.Append("<ul>");
               sb.AppendFormat("<li><a href=\"{0}\">{1}</a> </li>", parentPage.RelativeUrl, parentPage.Heading);

                var childPages = parentPage.ChildPages.Where(p => p.IsPublished).OrderBy(p => p.Priority);
                foreach (var page in childPages)
                {
                    sb.AppendFormat("<li><a href=\"{0}\">{1}</a> </li>", page.RelativeUrl, page.Heading);
                }
                sb.Append("</ul>");
            }
            return new MvcHtmlString(sb.ToString());
        }


        public MvcHtmlString PageBc(WebPage requestedPage)
        {
            if (requestedPage == null)
                return new MvcHtmlString(string.Empty);

            string key = string.Format("PageBc-{0}", requestedPage.Id);
            MvcHtmlString _PageBc = HttpContext.Current.Cache[key] as MvcHtmlString;
            if (_PageBc == null)
            {
                var sb = new StringBuilder();

                var page = requestedPage;
                if (requestedPage != null)
                {
                    sb.Append("<ul>");
                    sb.Append("<li><a href=\"/\">Home ></a></li>");
                    while (page.ParentId != 1)
                    {
                        page = requestedPage.ParentPage;
                        sb.AppendFormat("<li><a href=\"{0}\">{1} ></a></li>", page.RelativeUrl, page.Heading);
                    }
                    sb.AppendFormat("<li><a href=\"{0}\" class=\"breadcrumb-active\">{1}</a></li>", requestedPage.RelativeUrl, requestedPage.Heading);
                    sb.Append("</ul>");
                }
                _PageBc = new MvcHtmlString(sb.ToString());
                HttpContext.Current.Cache[key] = _PageBc;
            }
            return _PageBc;
        }

        ////Commented by Sunil 15May17
        //public MvcHtmlString CategoryBc(Category requestedCategory)
        //{
        //    if (requestedCategory == null)
        //        return new MvcHtmlString(string.Empty);

        //    string key = string.Format("CategoryBc-{0}", requestedCategory.Id);
        //    MvcHtmlString _CategoryBc = HttpContext.Current.Cache[key] as MvcHtmlString;
        //    if (_CategoryBc == null)
        //    {

        //        var sb = new StringBuilder();


        //        var category = requestedCategory;
        //        if (requestedCategory != null)
        //        {
        //            sb.Append("<ul>");
        //            sb.Append("<li><a href=\"/\">Home ></a></li>");
        //            while (category.ParentCategoryId != null)
        //            {
        //                category = requestedCategory.ParentCategory;
        //                sb.AppendFormat("<li><a href=\"{0}\">{1} ></a></li>", category.RelativeUrl, category.Name);
        //            }
        //            sb.AppendFormat("<li><a href=\"{0}\" class=\"breadcrumb-active\">{1}</a></li>", requestedCategory.RelativeUrl, requestedCategory.Name);
        //            sb.Append("</ul>");
        //        }
        //        _CategoryBc = new MvcHtmlString(sb.ToString());
        //        HttpContext.Current.Cache[key] = _CategoryBc;
        //    }
        //    return _CategoryBc;
        //}


        ////Commented by Sunil 15May17
        //public MvcHtmlString ProductBc(Product requestedProduct)
        //{
        //    if (requestedProduct == null)
        //        return new MvcHtmlString(string.Empty);

        //    string key = string.Format("ProductBc-{0}", requestedProduct.Id);
        //    MvcHtmlString _ProductBc = HttpContext.Current.Cache[key] as MvcHtmlString;
        //    if (_ProductBc == null)
        //    {

        //        var sb = new StringBuilder();
        //        var category = requestedProduct.Category;
        //        if (category != null)
        //        {
        //            sb.Append("<ul>");
        //            sb.Append("<li><a href=\"/\">Home ></a></li>");
        //            while (category.ParentCategoryId != null)
        //            {
        //                category = category.ParentCategory;
        //                sb.AppendFormat("<li><a href=\"{0}\">{1} ></a></li>", category.RelativeUrl, category.Name);
        //            }
        //            sb.AppendFormat("<li><a href=\"{0}\">{1} ></a></li>", category.RelativeUrl, category.Name);
        //            sb.AppendFormat("<li><a href=\"{0}\" class=\"breadcrumb-active\">{1}</a></li>", requestedProduct.RelativeUrl, requestedProduct.Name);
        //            sb.Append("</ul>");
        //        }
        //        _ProductBc = new MvcHtmlString(sb.ToString());
        //        HttpContext.Current.Cache[key] = _ProductBc;
        //    }
        //    return _ProductBc;
        //}

        public MvcHtmlString LeftMenu()
        {
            if (_currentPage == null)
                return MvcHtmlString.Create(string.Empty);

            string key = string.Format("WebPage-{0}", _currentPage.Id);
            MvcHtmlString _leftMenu = HttpContext.Current.Cache[key] as MvcHtmlString;

            if (_leftMenu == null)
            {
                var sb = new StringBuilder();
                if (_currentPage != null)
                {
                    int? parentId = _currentPage.ParentId;
                    if (parentId == 1)
                    {
                        WebPage firstOrDefault = _currentPage.ChildPages.Where(p => p.IsPublished).OrderBy(p => p.Priority).FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            HttpContext.Current.Response.RedirectPermanent(firstOrDefault.RelativeUrl);
                        }
                    }
                    else
                    {
                        parentId = _currentPage.ParentId;
                    }
                    sb.Append("<ul>");
                    if (parentId != 1)
                    {
                        IOrderedEnumerable<WebPage> childs = _db.WebPages.Find(parentId).ChildPages.Where(p => p.IsPublished).OrderBy(p => p.Priority);
                        if (childs.Any())
                        {
                            foreach (WebPage page in childs.AsQueryable())
                            {
                                sb.AppendFormat(page.ReferenceCode == _currentPage.ReferenceCode
                                                    ? "<li><a class=\"product-category-active\" href=\"#\">{1} <span class=\"glyphicon\"></span><span class=\"glyphicon\"></span></a></li>"
                                                    : "<li><a href=\"{0}\">{1} <span class=\"glyphicon\"></span><span class=\"glyphicon\"></span></a></li>",
                                                page.RelativeUrl, page.Heading);
                            }
                        }
                    }
                    else
                    {
                        sb.AppendFormat("<li><a class=\"product-category-active\" href=\"#\">{0} <span class=\"glyphicon\"></span><span class=\"glyphicon\"></span></a></li>", _currentPage.Heading);
                    }
                    sb.Append("</ul>");
                }
                _leftMenu = new MvcHtmlString(sb.ToString());
                HttpContext.Current.Cache[key] = _leftMenu;
            }
            return _leftMenu;
        }
        #endregion

        #region Sitemap
        public MvcHtmlString Sitemap()
        {
            MvcHtmlString _SiteMap = HttpContext.Current.Cache["_SiteMap"] as MvcHtmlString;
            if (_SiteMap == null)
            {
                var sb = new StringBuilder();

                var root = _db.WebPages.AsNoTracking().FirstOrDefault(p => p.ReferenceCode == "Home");
                if (root != null)
                {
                    sb.AppendFormat("<div class=\"sitemap\"><ul>");
                    sb.AppendFormat("<li ><a style='color:#b80018;font-weight:bold;' href=\"{0}\">Home</a>", "/");

                    //string group = PageGroup.Page.ToString();
                    var child = _db.WebPages.Where(p => p.ParentId == root.Id && p.IsPublished && p.IncludeInSitemap == true);
                    if (child.Any())
                    {
                        sb.Append(GetPages(child));
                        sb.AppendFormat("</li>");
                    }
                    else
                    {
                        sb.AppendFormat("<li><a style='color:#b80018;font-weight:bold;' href=\"{0}\">Home</a></li>", "/");
                    }
                    sb.AppendFormat("</ul></div>");
                }
                _SiteMap = new MvcHtmlString(sb.ToString());
                HttpContext.Current.Cache["_SiteMap"] = _SiteMap;
            }
            return _SiteMap;

        }

        private string GetPages(IEnumerable<WebPage> childPages)
        {
            var sb = new StringBuilder();
            foreach (var page in childPages)
            {
                WebPage page1 = page;
                var child = _db.WebPages.AsNoTracking().Where(p => p.ParentId == page1.Id && p.IsPublished && p.IncludeInSitemap == true);
                if (child.Any())
                {

                    sb.AppendFormat("<li><a style='color:#b80018;font-weight:bold' href=\"{0}\">{1}</a>", GetUrl("Pages", page.Url), page.Heading);
                    sb.AppendFormat("<ul style='margin-left:37px;'>");
                    sb.Append(GetPages(child));
                    sb.AppendFormat("</li></ul>");
                }
                else
                {
                    sb.AppendFormat("<li><a style='color:#b80018;' href=\"{0}\">{1}</a></li>", GetUrl("Pages", page.Url), page.Heading);
                }
            }
            return sb.ToString();
        }
        #endregion

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