using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class FAQ
    {
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public FAQ()
        {
            IsPublished = true;
            CreatedOn = IndianDateTime;
        }


        [Key]
        public int Id { get; set; }

        [StringLength(200), DataType(DataType.Text)]
        public string Heading { get; set; }

        [MaxLength, Required, DataType(DataType.Html)]
        public string Content { get; set; }

        [Required]
        public int Priority { get; set; }

        [ScaffoldColumn(false)]
        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}