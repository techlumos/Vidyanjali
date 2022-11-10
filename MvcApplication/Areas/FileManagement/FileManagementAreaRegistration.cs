using System.Web.Mvc;

namespace Vidyanjali.Areas.FileManagement
{
    public class FileManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FileManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FileManagement",
                "FileManagement/{controller}/{action}/{id}",
                new { action = "UploadFile", id = UrlParameter.Optional }
            );
        }
    }
}
