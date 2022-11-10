using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Vidyanjali.Areas.Admin.Models.Seo
{
    public class SeoModel
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly DateTime _indianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
        private readonly DateTime _minValueDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.ParseExact("01/01/1900", "dd/MM/yyyy", CultureInfo.InvariantCulture), IndianZone);

        public SeoModel()
        {
            CreatedOn = _indianDateTime;
            UpdatedOn = _minValueDateTime;
            ReviewedOn = _minValueDateTime;
            //UpdatedOn = DateTime.MinValue;
            //ReviewedOn = DateTime.MinValue;
        }

        [StringLength(200), DataType(DataType.Text)]
        public string MetaTitle { get; set; }

        [StringLength(200)]
        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [StringLength(800)]
        [Display(Name = "Meta Keywords")]
        public string MetaKeyword { get; set; }

        [Required, DataType(DataType.Text)]
        [Remote("CheckDuplicate", "Common", ErrorMessage = "Url has already taken", AdditionalFields = "initialUrl, modelName")]
        public string Url { get; set; }

        [ScaffoldColumn(false), DataType(DataType.Url)]
        public string RelativeUrl { get; set; }

        [Display(Name = "Created On"), ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Updated On"), ScaffoldColumn(false)]
        public DateTime UpdatedOn { get; set; }

        [Display(Name = "Reviewed On"), ScaffoldColumn(false)]
        public DateTime ReviewedOn { get; set; }


        [Display(Name = "Reviewed On"), ScaffoldColumn(false)]
        public string AuthorizationStatus
        {
            get
            {
                return (CreatedOn > ReviewedOn || UpdatedOn > ReviewedOn) ? AuthorizationStatuses.Pending.ToString() : AuthorizationStatuses.Authrorized.ToString();

            }
        }
    }

    public enum AuthorizationStatuses
    {
        Pending,
        Authrorized
    }
}