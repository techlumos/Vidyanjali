using System;
using System.ComponentModel.DataAnnotations;

namespace Vidyanjali.Areas.Admin.Models.Stakeholder
{
    public class AdminUser
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly DateTime _indianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
        public AdminUser()
        {
            RegisteredOn = _indianDateTime;
            LastSeen = _indianDateTime;
            RoleId = 1;
        }
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "email is Required"), StringLength(100), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required"), StringLength(20), DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "User Role is Required")]
        [Display(Name = "Role ID")]
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        [Required]
        public DateTime LastSeen { get; set; }
    }

    public class AdminUserValidateModel
    {
        public AdminUserValidateModel()
        {
            IsAdminUser = false;
            IsAuthorizedUser = false;
        }
        public bool IsAdminUser { get; set; }
        public bool IsAuthorizedUser { get; set; }
        public AdminUser AdminUser { get; set; }
    }



}