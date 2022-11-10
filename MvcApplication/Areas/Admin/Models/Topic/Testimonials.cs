using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class Testimonials
    {
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public Testimonials()
        {
            IsPublished = true;
            CreatedOn = IndianDateTime;
        }
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Name is required")]
        public string Name { get; set; }

        [MaxLength, Required, DataType(DataType.Html)]
        public string Description { get; set; }

        [Required]
        public int Priority { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}