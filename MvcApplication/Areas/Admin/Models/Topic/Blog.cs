using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class Blog
    {
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public Blog()
        {
            IsPublished = true;
            CreatedOn = IndianDateTime;
        }


        [Key]
        public int Id { get; set; }

        [StringLength(200), DataType(DataType.Text)]
        public string Heading { get; set; }

        [MaxLength,  DataType(DataType.Html)]
        public string ShortDescription { get; set; }

        [MaxLength,  DataType(DataType.Html)]
        public string Content { get; set; }

        [Display(Name = "Reference Code"), ScaffoldColumn(false)]
        public string ReferenceCode { get; set; }

        [Required, DataType(DataType.Text)]
        //[ErrorMessage = "Url has already taken", AdditionalFields = "initialUrl, modelName")]
        public string Url { get; set; }

        [ScaffoldColumn(false), DataType(DataType.Url)]
        public string RelativeUrl { get; set; }

        [Required]
        public int Priority { get; set; }

        [ScaffoldColumn(false)]
        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}