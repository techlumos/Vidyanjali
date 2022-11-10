using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Vidyanjali.Areas.FileManagement.Models;

namespace Vidyanjali.Areas.FileManagement.Controllers
{
    public class ImageManagerController : Controller
    {
        private readonly IFileRepository _repository;

        public ImageManagerController()
            : this(new FileRepository())
        {
        }

        private ImageManagerController(IFileRepository repository)
        {
            _repository = repository;
        }


        public ActionResult UploadFile(string group, string section, int referenceId)
        {
            ViewBag.ReferenceID = referenceId;
            ViewBag.Group = group;
            ViewBag.Section = section;
            return View();
        }

        // Clear Temporary folder before uploading anything.
        public void ClearTemp()
        {
            var directory = new DirectoryInfo(Server.MapPath("~/Areas/FileManagement/Content/Temporary"));
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
        }

        public string CropTempImage(string x, string y, string x1, string y1, string fileAbsPath)
        {
            var image = new WebImage(Server.MapPath(fileAbsPath));
            var height = image.Height;
            var width = image.Width;
            image.Crop(Convert.ToInt32(y), Convert.ToInt32(x), height - Convert.ToInt32(y1), width - Convert.ToInt32(x1));
            image.Save(Server.MapPath(fileAbsPath));
            return fileAbsPath;
        }


        //Save all the uploaded files into Temporary folder for Processing. 
        [HttpPost]
        public ActionResult SaveToTemp(HttpPostedFileBase file)
        {
            var status = _repository.SaveToTemp(file);
            return Content(status);
        }

        // Move all the Images from Temporary folder to Actual Destination after adding
        // Title, caption, description etc.
        // Actual Location, Size and other desired attributes can be fetched from 'FileConfig.xml' file based on
        // File Group and Section.
        [HttpPost]
        public ActionResult Upload(FormCollection frm)
        {
            TempData["Success"] = _repository.ProcessFile(frm);
            //var fileList = _repository.GetImages(frm["uploadGroup"], frm["uploadSection"],
            //                                     Convert.ToInt32(frm["hdRerefenceId"]));
            //return PartialView("~/Areas/FileManagement/Views/Shared/_ImageFileList.cshtml", fileList);
            return
                Redirect(string.Format("/FileManagement/ImageManager?group={0}&section={1}&referenceId={2}",
                                       frm["uploadGroup"], frm["uploadSection"], Convert.ToInt32(frm["hdRerefenceId"])));
        }

        // Disply Uploaded files list basedd on Group, Section and ReferenceID
        public PartialViewResult DisplayList(string group, string section, int referenceId)
        {
            var fileList = _repository.GetImages(group, section, referenceId);
            return PartialView("~/Areas/FileManagement/Views/Shared/_ImageFileList.cshtml", fileList);
        }

        //Get File Types (i.e. Image or Document)
        public string GetTypes(string selected = "")
        {
            return _repository.GetTypes(selected);
        }

        // Get file sections (i.e. PageImage, Gallery, HomeBlock etc.)
        public string GetSections(string selectedType, string selected = "", string groupName = "")
        {
            return _repository.GetSections(selectedType, selected, groupName);
        }

        // Get file sections (i.e. PageImage, Gallery, HomeBlock etc.)
        public string GetFileSections()
        {
            return _repository.GetFileSections();
        }

        // Set the selected file as Default file.
        public string SetDefault(string type, string groupName, string section, int id, int referenceId)
        {
            return _repository.SetFileAsDefault(type, groupName, section, id, referenceId);
        }

        public PartialViewResult Edit(int id)
        {
            ImageFile imageFile = _repository.GetFileDetails(id);
            return PartialView("~/Areas/FileManagement/Views/Shared/_EditFileInfo.cshtml", imageFile);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ImageFile imageFile)
        {
            if (_repository.EditFile(imageFile))
            {
                TempData["Success"] = "File details have been updated!";
            }
            else
            {
                TempData["Error"] = "Error";
            }
            return
                Redirect(string.Format("/FileManagement/ImageManager?group={0}&section={1}&referenceId={2}",
                                       imageFile.Group, imageFile.Section, imageFile.ReferenceId));
        }

        public ActionResult Delete(int id)
        {
            ImageFile imageFile = _repository.GetFileDetails(id);
            if (_repository.DeleteFile(id))
            {
                TempData["Success"] = "File has been Deleted";
            }
            else
            {
                TempData["Error"] = "Error";
            }
            return
                Redirect(string.Format("/FileManagement/ImageManager?group={0}&section={1}&referenceId={2}",
                                       imageFile.Group, imageFile.Section, imageFile.ReferenceId));
        }

        public ActionResult FileManager()
        {
            return View();
        }

        public ActionResult GetFiles(string group = "", string section = "")
        {
            if (!string.IsNullOrEmpty(group) && !string.IsNullOrEmpty(section))
            {
                return PartialView("_ImageLibrary", _repository.GetImages(group, section));
            }
            return PartialView("_ImageLibrary", _repository.GetImages());
        }
    }
}
