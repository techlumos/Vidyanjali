using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vidyanjali.Areas.EMS.Models
{
    public class Enquiry
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public Enquiry()
        {
            EnquiryDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
        }
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        [StringLength(40, ErrorMessage = "Max 40 Characters allowed")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Not a valid Email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required!")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        [RegularExpression("\\d{10,15}", ErrorMessage = "Only numbers; Minimum 10 digits!")]
        public string Phone { get; set; }

        public int? ReferenceId { get; set; }

        [Required(ErrorMessage = "Message is required!")]
        [DataType(DataType.Html)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        public virtual Visitor Visitor { get; set; }

        public int? VisitorID { get; set; }

        public DateTime EnquiryDate { get; set; }

       [NotMapped]
        public string Captcha { get; set; }

        
    }
}