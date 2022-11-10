using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vidyanjali.Areas.Admin.Models.Stakeholder
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string ReferenceCode { get; set; }

        [Required, StringLength(50), Display(Name = "Name of the Role")]
        public string Name { get; set; }

        [Required, StringLength(300), Display(Name = "Short info about the Role")]
        public string Description { get; set; }

        [Required, StringLength(50), Display(Name = "Landing URL after Login")]
        public string LandingUrl { get; set; }

        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
    }

    public class Menu
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        readonly DateTime _indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public Menu()
        {
            NeedAuthentication = true;
            CreatedOn = _indianTime;
        }

        [Required, Key]
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "Menu Name")]
        public string DisplayName { get; set; }

        [StringLength(100)]
        [Display(Name = "Menu Group")]
        public string GroupName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Action Name")]
        public string ActionName { get; set; }

        [Display(Name = "Need Authentication")]
        public bool NeedAuthentication { get; set; }

        [Display(Name = "Display on Menu")]
        public bool DisplayOnMenu { get; set; }

        [Display(Name = "Menu URL"), StringLength(200)]
        public string Url { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }

    public class RoleMenu
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        readonly DateTime _indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        public RoleMenu()
        {
            CreatedOn = _indianTime;
        }

        [Required, Key]
        public int Id { get; set; }

        [Display(Name = "Role ID")]
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }

        [Display(Name = "Menu ID")]
        public int? MenuId { get; set; }

        public virtual Menu Menu { get; set; }

        [Required, DataType(DataType.DateTime)]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }

    public class RoleAction
    {
        [Required, Key]
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "Action Name")]
        public string ActionName { get; set; }

        [Display(Name = "Role ID")]
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
    }

    public enum MenuGroup
    {
        Cms,
        Product,
        Order,
        Others
        //Features,
        //Utilities,
        //Others
    }

}