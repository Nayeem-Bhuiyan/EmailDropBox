using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropBoxTest.Areas.DropboxInfo.Models
{
    public class CreateFolderViewModel
    {
        public string folderName { get; set; }
        public string errorResponse { get; set; }
        public string successResponse { get; set; }
        public string redirectFolder { get; set; }
        public int? countFile { get; set; }
        public List<IFormFile> imageList { get; set; }
        public List<string> imageUrlList { get; set; }
    }

}
