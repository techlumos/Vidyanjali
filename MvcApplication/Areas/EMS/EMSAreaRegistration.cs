using System.Web.Mvc;

namespace Vidyanjali.Areas.EMS
{
    public class EMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "EMS",
                "EMS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
