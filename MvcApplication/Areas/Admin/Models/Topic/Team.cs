using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class Team
    {
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public Team()
        {
            IsPublished = true;
            CreatedOn = IndianDateTime;
        }


        [Key]
        public int Id { get; set; }

        [StringLength(200), DataType(DataType.Text)]
        public string Name { get; set; }

        [StringLength(200), DataType(DataType.Text)]
        public string Qualification { get; set; }

        [StringLength(500), DataType(DataType.Text)]
        public string Designation { get; set; }

        [ScaffoldColumn(false)]
        public bool IsPublished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
