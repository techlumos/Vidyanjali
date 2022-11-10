using System;
using System.ComponentModel.DataAnnotations;

namespace Vidyanjali.Areas.FileManagement.Models
{
    public class ImageFile
    {
        public ImageFile()
        {
            Id = 0;
            Group = string.Empty;
            Section = string.Empty;
            ReferenceId = 0;
            FileName = string.Empty;
            Caption = string.Empty;
            Description = string.Empty;
            Url = string.Empty;
            Height = 0;
            Width = 0;
            Size = string.Empty;
            FileOrder = 0;
            CreatedOn = DateTime.Now;
            IsDefault = false;
            Tag = string.Empty;
            Eurl = string.Empty;
        }


        public ImageFile(int id, string type, string groupName, string sectionName, int referenceId, string fileName, string caption, string description, string url, int height, int width, string size, int fileOrder, DateTime fileCreatedDate, bool isDefault, string tag, string eurl)
        {
            Id = id;
            Type = type;
            Group = groupName;
            Section = sectionName;
            ReferenceId = referenceId;
            FileName = fileName;
            Caption = caption;
            Description = description;
            Url = url;
            Height = height;
            Width = width;
            Size = size;
            FileOrder = fileOrder;
            CreatedOn = fileCreatedDate;
            IsDefault = isDefault;
            Tag = tag;
            Eurl = eurl;
        }


        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        public string Description { get; set; }

        [Display(Name = "Alternate Text")]
        public string Caption { get; set; }

        public string Size { get; set; }

        public string Extension { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string Type { get; set; }

        public string Section { get; set; }

        public string Group { get; set; }

        public int ReferenceId { get; set; }

        public int FileOrder { get; set; }

        public string Url { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDefault { get; set; }

        public string Tag { get; set; }

        public string Eurl { get; set; }
    }
}