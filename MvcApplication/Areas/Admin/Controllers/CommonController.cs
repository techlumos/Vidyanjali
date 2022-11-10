using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidyanjali.Models;

namespace Vidyanjali.Areas.Admin.Controllers
{
    public class CommonController : Controller
    {
        private readonly CoreContext _db = new CoreContext();

        public JsonResult CheckDuplicate(string url, string initialUrl = "", string modelName = "")
        {
            bool isExist = false;
            switch (modelName)
            {
                case "WebPage":
                    isExist = string.IsNullOrEmpty(initialUrl)
                                  ? _db.WebPages.Count(
                                      w => w.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase)) <= 0
                                  : _db.WebPages.Count(
                                      w =>
                                      w.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase) &&
                                      !w.Url.Equals(initialUrl, StringComparison.CurrentCultureIgnoreCase)) <= 0;
                    break;
            }

            return Json(isExist, JsonRequestBehavior.AllowGet);
        }

    }
}
