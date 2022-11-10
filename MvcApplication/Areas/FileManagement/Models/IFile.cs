using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Simple.ImageResizer;

namespace Vidyanjali.Areas.FileManagement.Models
{
    interface IFileRepository
    {
        IEnumerable<ImageFile> GetImages();

        IEnumerable<ImageFile> GetImages(string group);

        IEnumerable<ImageFile> GetImages(string group, string section);

        IEnumerable<ImageFile> GetImages(string group, string section, int referenceId, bool getDefault = false);

        bool InsertFile(ImageFile imageFile);

        string ProcessFile(FormCollection frm);

        string GetTypes(string selected);

        string GetFileSections();

        string GetSections(string selectedType, string selected, string groupName);

        string SaveToTemp(HttpPostedFileBase file);

        string SetFileAsDefault(string type, string groupName, string section, int id, int referenceId);

        ImageFile GetFileDetails(int id);

        bool EditFile(ImageFile imageFile);

        bool DeleteFile(int id);

        IEnumerable<ImageFile> GetThumbs(string groupName, string sectionName, int referenceId,
                                         string thumbSize, bool random = false);
    }

    public class FileRepository : IFileRepository
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        readonly DateTime _indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndianZone);

        private readonly List<ImageFile> _allFiles;
        private readonly IEnumerable<XElement> _allElementsFromConfig;

        private readonly XDocument _xDocument, _documentConfig;

        public FileRepository()
        {
            _allFiles = new List<ImageFile>();
            _xDocument = XDocument.Load(HttpContext.Current.Server.MapPath("~/Areas/FileManagement/Xml/Files.xml"));
            var files = from elements in _xDocument.Descendants("File")
                        let xId = elements.Attribute("Id")
                        where xId != null
                        let xType = elements.Attribute("Type")
                        where xType != null
                        let xgroup = elements.Attribute("Group")
                        where xgroup != null
                        let xSection = elements.Attribute("Section")
                        where xSection != null
                        let xreferenceId = elements.Attribute("ReferenceId")
                        where xreferenceId != null
                        let xFileName = elements.Element("Name")
                        where xFileName != null
                        let xCaption = elements.Element("Caption")
                        where xCaption != null
                        let xDescription = elements.Element("Description")
                        where xDescription != null
                        let xUrl = elements.Element("Url")
                        where xUrl != null
                        let xHeight = elements.Element("Height")
                        where xHeight != null
                        let xWidth = elements.Element("Width")
                        where xWidth != null
                        let xSize = elements.Element("Size")
                        where xSize != null
                        let xFileOrder = elements.Element("Order")
                        where xFileOrder != null
                        let xFileDate = elements.Element("Date")
                        where xFileDate != null
                        let xIsDefault = elements.Element("IsDefault")
                        where xIsDefault != null
                        let xTag = elements.Element("Tags")
                        where xTag != null
                        let xEUrl = elements.Element("Eurl")
                        where xEUrl != null
                        select new ImageFile(
                            int.Parse(xId.Value),
                            xType.Value,
                            xgroup.Value,
                            xSection.Value,
                            int.Parse(xreferenceId.Value),
                            xFileName.Value,
                            xCaption.Value,
                            xDescription.Value,
                            xUrl.Value,
                            int.Parse(xHeight.Value),
                            int.Parse(xWidth.Value),
                            xSize.Value,
                            int.Parse(xFileOrder.Value),
                            DateTime.Parse(xFileDate.Value),
                            bool.Parse(xIsDefault.Value),
                            xTag.Value,
                            xEUrl.Value
                            );
            _allFiles.AddRange(files.ToList().OrderBy(o => o.FileOrder));


            string strFileName = HttpContext.Current.Server.MapPath("~/Areas/FileManagement/Xml/FileConfig.xml");
            _documentConfig = XDocument.Load(strFileName);
            _allElementsFromConfig = _documentConfig.Descendants("Files").Descendants();
        }


        public IEnumerable<ImageFile> GetImages()
        {
            return _allFiles;
        }

        public IEnumerable<ImageFile> GetImages(string groupName)
        {
            return _allFiles.Where(f => f.Group == groupName);
        }

        public IEnumerable<ImageFile> GetImages(string groupName, string sectionName)
        {
            return _allFiles.Where(f => f.Group == groupName && f.Section == sectionName);
        }

        public IEnumerable<ImageFile> GetImages(string groupName, string sectionName, int referenceId, bool getDefault = false)
        {
            if (!getDefault)
                return _allFiles.Where(
                    f => f.Group == groupName && f.Section == sectionName && f.ReferenceId == referenceId);
            return _allFiles.Where(
                    f =>
                    f.Group == groupName && f.Section == sectionName && f.ReferenceId == referenceId &&
                    f.IsDefault);
        }


        public IEnumerable<ImageFile> GetThumbs(string groupName, string sectionName, int referenceId, string thumbSize, bool random = false)
        {
            var files =
                   _allFiles.Where(
                       f => f.Group == groupName && f.Section == sectionName && f.ReferenceId == referenceId &&
                            f.IsDefault).ToList();
            foreach (var imageFile in files)
            {
                var absPath = imageFile.Url.Remove(imageFile.Url.LastIndexOf("/", StringComparison.Ordinal) + 1);
                var fileName = thumbSize + "_" + imageFile.FileName;
                imageFile.Url = absPath + fileName;
            }
            return files;
        }

        public bool InsertFile(ImageFile imgFile)
        {
            bool isInserted;

            try
            {
                var imageFiles = _xDocument.Descendants("File").OrderBy(nodes => (int)nodes.Attribute("Id"));
                int imageId = 1;
                int order = 1;

                if (imageFiles.Any())
                {
                    imageId = imageFiles.Max(i => (int)i.Attribute("Id")) + 1;

                    var imageCategory =
                        imageFiles.Where(
                            i =>
                            (string)i.Attribute("Section") == imgFile.Section &&
                            (string)i.Attribute("Group") == imgFile.Group &&
                            (int)i.Attribute("ReferenceId") == imgFile.ReferenceId).ToList();
                    order = (!imageCategory.Any()) ? 1 : imageCategory.Max(i => (int)i.Element("Order")) + 1;
                }

                if (_xDocument.Root != null)
                    _xDocument.Root.Add(
                        new XElement("File",
                                     new XAttribute("Id", imageId),
                                     new XAttribute("Type", imgFile.Type),
                                     new XAttribute("Group", imgFile.Group),
                                     new XAttribute("Section", imgFile.Section),
                                     new XAttribute("ReferenceId", imgFile.ReferenceId),
                                     new XElement("Name", imgFile.FileName),
                                     new XElement("Caption",
                                                  string.IsNullOrEmpty(imgFile.Caption)
                                                      ? imgFile.FileName.Replace("_", " ").Replace("-", " ")
                                                      : imgFile.Caption),
                                     new XElement("Description",
                                                  string.IsNullOrEmpty(imgFile.Description)
                                                      ? imgFile.FileName.Replace("_", " ").Replace("-", " ")
                                                      : imgFile.Description),
                                     new XElement("Url", imgFile.Url),
                                     new XElement("Height", imgFile.Height),
                                     new XElement("Width", imgFile.Width),
                                     new XElement("Size", imgFile.Size),
                                     new XElement("Order", order),
                                     new XElement("IsDefault", false),
                                     new XElement("Tags", imgFile.Tag),
                                     new XElement("Eurl", imgFile.Eurl),
                                     new XElement("Date", imgFile.CreatedOn)
                            ));
                _xDocument.Save(HttpContext.Current.Server.MapPath("/Areas/FileManagement/Xml/Files.xml"));
                isInserted = true;
            }
            catch
            {
                isInserted = false;
            }
            return isInserted;
        }

        public string ProcessFile(FormCollection frm)
        {
            int successCount = 0;

            FileInfo[] tempFiles = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Areas/FileManagement/Content/Temporary/")).GetFiles();
            foreach (var file in tempFiles)
            {
                int fileIndex = Array.IndexOf(frm["hdImageName"].Split(','), file.Name);
                ImageFile imgFile = GetFileInstence(frm, fileIndex, file);

                if (imgFile != null)
                {
                    string pathToSave = GetFolderPathFromXml(imgFile.Group, imgFile.Section, Convert.ToString(imgFile.ReferenceId));
                    if (!File.Exists(string.Format("{0}{1}", pathToSave, file.Name)))
                    {
                        imgFile.Url = string.Format("{0}{1}", pathToSave.Replace("\\", "/"), file.Name);
                        imgFile.CreatedOn = _indianTime;

                        if (CheckSizeAndMoveFile(imgFile, file, pathToSave))
                        {
                            if (InsertFile(imgFile))
                            {
                                successCount++;
                            }
                        }
                    }
                }
                file.Delete();
            }

            return
                string.Format(
                    "Info: {0} out of {1} file(s) have been uploaded, rest couldn't meet the config criteria or ended with errors!",
                    successCount, tempFiles.Count());
        }

        private bool CheckSizeAndMoveFile(ImageFile imgFile, FileInfo file, string pathToSave)
        {
            var isMovedProperly = false;

            try
            {
                var firstOrDefault =
                    _allElementsFromConfig.Descendants().FirstOrDefault(
                        f =>
                        (string)f.Attribute("Section") == imgFile.Section &&
                        (string)f.Attribute("Group") == imgFile.Group);
                if (firstOrDefault != null)
                {
                    var xThumbs = firstOrDefault.Element("Thumbs");

                    var source = Path.Combine(file.DirectoryName, file.Name);
                    var destination = Path.Combine(HttpContext.Current.Server.MapPath(pathToSave),
                                                   Path.GetFileName(file.Name));

                    using (var img = Image.FromFile(Path.Combine(file.DirectoryName, file.Name)))
                    {
                        if (imgFile.Width > 0 && imgFile.Height > 0)
                        {
                            var iw = img.Width; // Width of image to be uploaded
                            var ih = img.Height; // Height of Image to be uploaded
                            var cw = int.Parse(Convert.ToString(imgFile.Width)); // Config Width for Such type
                            var ch = int.Parse(Convert.ToString(imgFile.Height)); // Config Height for Such type

                            // Uploaded image Resolution is Larger than Config values, So Resize it
                            if (iw < cw && ih < ch)
                            {
                                //Cant upload since the size doesn't match with config data.
                                return false;
                            }
                            if ((iw > cw && ih > ch) || (iw == cw && ih > ch) || (iw > cw && ih == ch))
                            {
                                using (var imgResizer = new ImageResizer(source))
                                {
                                    imgResizer.Resize(cw, ch, true, ImageEncoding.Png);
                                    imgResizer.SaveToFile(destination);
                                }
                                imgFile.Width = cw;
                                imgFile.Height = ch;
                            }
                            else if (img.Width == cw && img.Height == ch)
                            {
                                File.Copy(source, destination, true);
                            }

                            // Generate thumbs
                            if (xThumbs != null && xThumbs.Value != "None" && xThumbs.Value != "0")
                            {
                                // Get thumbsizes
                                string[] sizes = xThumbs.Value.Split(',');
                                foreach (var size in sizes)
                                {
                                    string[] resolution = size.Split('x');
                                    var width = resolution[0];
                                    var height = resolution[1];
                                    var fileName = string.Format("{0}_{1}", size, file.Name);

                                    var tSource = file.FullName;
                                    var tDestination = Path.Combine(HttpContext.Current.Server.MapPath(pathToSave),
                                                                    fileName);

                                    using (var imgResizer = new ImageResizer(tSource))
                                    {
                                        imgResizer.Resize(int.Parse(width), int.Parse(height), ImageEncoding.Png);
                                        imgResizer.SaveToFile(tDestination);
                                    }
                                }
                            }

                            isMovedProperly = true;
                        }
                    }
                }
            }
            catch
            {
                isMovedProperly = false;
            }
            return isMovedProperly;
        }

        private ImageFile GetFileInstence(FormCollection frm, int fileIndex, FileInfo file)
        {
            //using (var img = Image.FromFile(Path.Combine(file.DirectoryName, file.Name)))
            //{
            var imageFile = new ImageFile
                                {
                                    Section = frm["uploadSection"],
                                    FileName = file.Name,
                                    Caption = frm["txtTitle"].Split(',')[fileIndex],
                                    Description = frm["txtAreaDescription"].Split(',')[fileIndex],
                                    Group = frm["uploadGroup"],
                                    ReferenceId = Convert.ToInt32(frm["hdRerefenceId"]),
                                    Type = frm["uploadType"],
                                    Width = Convert.ToInt32(frm["txtWidth"]),
                                    Height = Convert.ToInt32(frm["txtHeight"]),
                                    Size = GetSize(file.Length),
                                    Tag = frm["txtTag"],
                                    Eurl = frm["txtEurl"]
                                };
            //}
            return imageFile;
        }

        private string GetSize(long byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount + " Bytes";
            return size;
        }


        private string GetFolderPathFromXml(string group, string section, string referenceId)
        {
            var absUrl = string.Empty;

            var firstOrDefault = _allElementsFromConfig.Descendants().FirstOrDefault(file => (string)file.Attribute("Section") == section && (string)file.Attribute("Group") == @group);
            if (firstOrDefault != null)
            {
                var xElement = firstOrDefault.Element("Directory");
                if (xElement != null)
                {
                    var directoryPath = xElement.Value;
                    absUrl = string.Format("{0}/{1}/", directoryPath, referenceId);

                    string pathToCreate = HttpContext.Current.Server.MapPath(string.Format("{0}/{1}/", directoryPath, referenceId));
                    if (!Directory.Exists(pathToCreate))
                    {
                        Directory.CreateDirectory(pathToCreate);
                    }
                }
            }
            return absUrl;
        }

        public string GetTypes(string selected)
        {
            List<ImageFile> types = (from elements in _allElementsFromConfig
                                     let xType = elements.Attribute("Type")
                                     let xSize = elements.Attribute("Size")
                                     let xAllowed = elements.Attribute("Allowed")
                                     where xType != null && xSize != null
                                     select new ImageFile
                                         {
                                             Size = xSize.Value,
                                             Type = xType.Value,
                                             Extension = xAllowed.Value,
                                         }).ToList();

            var sb = new StringBuilder();
            sb.Clear();
            sb.Append("<select name=\"uploadType\" id=\"selectType\" data-val=\"true\" data-val-required=\"Type is required.\">");
            sb.AppendFormat("<option value=\"\" data=\"\">--Select Type--</option>");
            foreach (var item in types)
            {
                if (!string.IsNullOrEmpty(selected) && item.Type == selected)
                {
                    sb.AppendFormat("<option selected=\"selected\" data=\"{1}|{2}\" value=\"{0}\">{3}</option>",
                                    item.Type, item.Size,
                                    item.Extension, Regex.Replace(item.Type, "(\\B[A-Z])", " $1"));
                }
                else
                {
                    /*storing the Image type and Allowed extensions in Title
                     * So that, it can be access via seleted type for Uplodify object
                     */
                    sb.AppendFormat("<option data=\"{1}|{2}\" value=\"{0}\">{3}</option>", item.Type, item.Size,
                                    item.Extension, Regex.Replace(item.Type, "(\\B[A-Z])", " $1"));
                }
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public string GetFileSections()
        {
            List<ImageFile> sections = (from elements in _allElementsFromConfig.Descendants()
                                        let parent = elements.Parent
                                        where parent != null
                                        let eleType = parent.Attribute("Type")
                                        let eleSection = elements.Attribute("Section")
                                        let eleGroup = elements.Attribute("Group")
                                        let eleDescription = elements.Element("Description")
                                        let eleWidth = elements.Element("Width")
                                        let eleHeight = elements.Element("Height")

                                        where eleType != null && eleSection != null
                                        select new ImageFile
                                        {
                                            Section = (string)eleSection,
                                            Type = (string)eleType,
                                            Description = eleDescription.Value,
                                            Width = eleWidth != null ? Convert.ToInt16(eleWidth.Value) : 0,
                                            Height = eleHeight != null ? Convert.ToInt16(eleHeight.Value) : 0,
                                            Group = (string)eleGroup
                                        }).ToList();

            var sb = new StringBuilder();
            sb.Clear();
            sb.Append("<ul id=\"listSection\">");
            foreach (var item in sections)
            {
                sb.AppendFormat("<li class=\"sectionItem\" data=\"{0}|{2}\">{1}</li>", item.Section,
                                Regex.Replace(item.Section, "(\\B[A-Z])", " $1"), item.Group);
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        public string GetSections(string selectedType, string selected, string groupName)
        {
            List<ImageFile> sections = (from elements in _allElementsFromConfig.Descendants()
                                        let parent = elements.Parent
                                        where parent != null
                                        let eleType = parent.Attribute("Type")
                                        let eleSection = elements.Attribute("Section")
                                        let eleGroup = elements.Attribute("Group")
                                        let eleDescription = elements.Element("Description")
                                        let eleWidth = elements.Element("Width")
                                        let eleHeight = elements.Element("Height")

                                        where eleType != null && (eleSection != null && eleType.Value == selectedType && eleGroup.Value == groupName)
                                        select new ImageFile
                                                   {
                                                       Section = (string)eleSection,
                                                       Type = (string)eleType,
                                                       Description = eleDescription.Value,
                                                       Width = eleWidth != null ? Convert.ToInt16(eleWidth.Value) : 0,
                                                       Height = eleHeight != null ? Convert.ToInt16(eleHeight.Value) : 0
                                                   }).ToList();

            var sb = new StringBuilder();
            sb.Clear();
            sb.Append("<select name=\"uploadSection\" id=\"selectSection\"  data-val=\"true\" data-val-required=\"Section is required.\">");
            sb.AppendFormat("<option value=\"\" data=\"\">--Select Section--</option>");
            foreach (var item in sections)
            {
                if (!string.IsNullOrEmpty(selected) && item.Section == selected)
                {
                    sb.AppendFormat("<option selected=\"selected\" value=\"{0}\" data=\"{2}|{3}\">{1}</option>",
                                    item.Section, Regex.Replace(item.Section, "(\\B[A-Z])", " $1"), item.Description,
                                    string.Format("{0}x{1}", item.Width, item.Height));
                }
                else
                {
                    sb.AppendFormat("<option value=\"{0}\" data=\"{2}|{3}\">{1}</option>", item.Section,
                                    Regex.Replace(item.Section, "(\\B[A-Z])", " $1"), item.Description,
                                    string.Format("{0}x{1}", item.Width, item.Height));
                }
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public string SaveToTemp(HttpPostedFileBase file)
        {
            var dir =
                new DirectoryInfo(
                    HttpContext.Current.Server.MapPath(string.Format("~/Areas/FileManagement/Content/Temporary/")));
            if (!dir.Exists)
            {
                dir.Create();
            }

            var path = Path.Combine(dir.FullName, Path.GetFileName(file.FileName));
            try
            {
                file.SaveAs(path);
                return path;
            }
            catch
            {
                return "Error in uploading file into Temporary location!";
            }
        }

        public string SetFileAsDefault(string type, string groupName, string section, int id, int referenceId)
        {
            bool isProcessed = false;
            if (_xDocument.Root != null)
            {
                try
                {
                    // Make all others files Default 'false'
                    var files = _xDocument.Root.Elements("File").Where(
                        f => (string)f.Attribute("Type") == type &&
                             (string)f.Attribute("Group") == groupName &&
                             (string)f.Attribute("Section") == section &&
                             (int)f.Attribute("ReferenceId") == referenceId
                        ).ToList();

                    foreach (var item in files)
                    {
                        item.SetElementValue("IsDefault", false);
                    }

                    var fileToBeProcessed = files.FirstOrDefault(f => (int)f.Attribute("Id") == id);
                    if (fileToBeProcessed != null)
                    {
                        fileToBeProcessed.SetElementValue("IsDefault", true);
                    }
                    _xDocument.Save(HttpContext.Current.Server.MapPath("~/Areas/FileManagement/Xml/Files.xml"));
                    isProcessed = true;
                }
                catch
                {
                    isProcessed = false;
                }
            }
            return isProcessed ? "Selected file has been set as Default" : "Error in setting Default, Please try again..";
        }

        public ImageFile GetFileDetails(int id)
        {
            return _allFiles.Find(f => f.Id == id);
        }

        public bool EditFile(ImageFile imageFile)
        {
            bool isProcessed;

            try
            {
                if (_xDocument.Root != null)
                {
                    XElement node =
                        _xDocument.Root.Elements("File").FirstOrDefault(i => (int)i.Attribute("Id") == imageFile.Id);
                    if (node != null)
                    {
                        node.SetElementValue("Caption", imageFile.Caption);
                        node.SetElementValue("Description", imageFile.Description);
                        node.SetElementValue("Order", imageFile.FileOrder);
                        node.SetElementValue("Tags", imageFile.Tag);
                        node.SetElementValue("Eurl", imageFile.Eurl);
                    }
                }
                _xDocument.Save(HttpContext.Current.Server.MapPath("~/Areas/FileManagement/Xml/Files.xml"));
                isProcessed = true;
            }
            catch
            {
                isProcessed = false;
            }
            return isProcessed;
        }

        public bool DeleteFile(int id)
        {
            bool isProcessed;
            try
            {
                if (_xDocument.Root != null)
                {
                    _xDocument.Root.Elements("File").Where(i => (int)i.Attribute("Id") == id).Remove();
                }
                _xDocument.Save(HttpContext.Current.Server.MapPath("~/Areas/FileManagement/Xml/Files.xml"));
                isProcessed = true;
            }
            catch
            {
                isProcessed = false;
            }
            return isProcessed;
        }
    }
}
