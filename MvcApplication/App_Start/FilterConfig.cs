using System.Web.Mvc;

namespace Vidyanjali.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            GlobalFilters.Filters.Add(new Vidyanjali.Areas.EMS.Models.UserTrackingActionFilterAttribute());
        }
    }
}