using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Vidyanjali.Areas.EMS.Models;

namespace Vidyanjali.Areas.Admin.Models.Topic
{
    public class Admission
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public Admission()
        {
            EnquiryDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);
        }
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Student Name is required!")]
        [StringLength(50, ErrorMessage = "Max 50 Characters allowed")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Father Name is required!")]
        [StringLength(50, ErrorMessage = "Max 50 Characters allowed")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "Mother Name is required!")]
        [StringLength(50, ErrorMessage = "Max 50 Characters allowed")]
        public string MotherName { get; set; }

        [Required(ErrorMessage = "Gender is required!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Class is required!")]
        public string Class { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string AboutUs { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string DOB { get; set; }

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

        //[Required(ErrorMessage = "Message is required!")]
        [DataType(DataType.Html)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        public virtual Visitor Visitor { get; set; }

        public int? VisitorID { get; set; }

        public DateTime EnquiryDate { get; set; }

        [NotMapped]
        public string Captcha { get; set; }
    }
}