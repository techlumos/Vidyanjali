using System.Web.Mvc;

namespace Vidyanjali.Areas.Admin.Helpers
{
    public static class GeneralHelper
    {
        public static string DisplayBoolFor(this HtmlHelper helper, bool value)
        {
            return value
                       ? "<i class=\"fa fa-check fa-1x\"></i>"
                       : "<i class=\"fa fa-times fa-1x\"></i>";
        }
    }
}