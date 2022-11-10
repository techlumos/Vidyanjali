using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class Gallery
    {
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public Gallery()
        {
            IsPublished = true;
            CreatedOn = IndianDateTime;
        }
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Name is required")]
        public string Heading { get; set; }

        [MaxLength,  DataType(DataType.Html)]
        public string Description { get; set; }

        [Required]
        public int Priority { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}