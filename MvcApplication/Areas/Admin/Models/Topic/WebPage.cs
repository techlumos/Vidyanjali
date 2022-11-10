using Vidyanjali.Areas.Admin.Models.Seo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class WebPage : SeoModel
    {
        public static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public readonly DateTime IndianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public WebPage()
        {
            CreatedOn = IndianDateTime;
            Properties = "Published";
            //Tags = "Page";
            RelativeUrl = string.Empty;
            IncludeInSitemap = false;
        }

        public string Path
        {
            get { return (ParentPage != null) ? ParentPage.Heading + " >> " + Heading : Heading; }
        }

        [Key]
        public int Id { get; set; }

        [StringLength(200), Required, DataType(DataType.Text)]
        public string Heading { get; set; }

        [Display(Name = "Sub Heading")]
        [StringLength(200), Required, DataType(DataType.Text)]
        public string SubHeading { get; set; }

        [MaxLength, Required, DataType(DataType.Html)]
        public string Content { get; set; }

        [Display(Name = "Reference Code"), ScaffoldColumn(false)]
        public string ReferenceCode { get; set; }

        [Required, StringLength(500)]
        public string Properties { get; set; }

        [Required, StringLength(500)]
        public string Tags { get; set; }

        [Required]
        public int Priority { get; set; }


        [ScaffoldColumn(false)]
        public bool IncludeInSitemap { get; set; }

        [ScaffoldColumn(false)]
        public bool IsPublished { get; set; }

        //Parent Page Object (Self Reference: ParentId = > Id)
        [Display(Name = "Parent Page")]
        public int? ParentId { get; set; }

        
        [ForeignKey("ParentId"), DisplayFormat(NullDisplayText = "Root")]
        public virtual WebPage ParentPage { get; set; }

        public virtual ICollection<WebPage> ChildPages { get; set; }

    }

    public enum PageProperties
    {
        Menu,
        Page,
        NoFollow,
        NoIndex,
        Archive,
        Reterive,
        NoImage,
        Published
    }
}