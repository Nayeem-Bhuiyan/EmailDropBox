using Dropbox.Api;
using DropBoxTest.Areas.DropboxInfo.Models;
using DropBoxTest.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using dropboxApi = global::Dropbox.Api;
namespace DropBoxTest.Areas.DropboxInfo.Controllers
{
    [Area("DropboxInfo")]
    public class DownloadController : Controller
    {
        
        private IWebHostEnvironment _environment;
        private readonly IDropboxManager _dropboxService;
        public DownloadController(IWebHostEnvironment environment, IDropboxManager dropboxService)
        {

            _environment = environment;
            _dropboxService = dropboxService;
        }

        private const string ACCESS_TOKEN = "sl.BAd1K5QvO5HZe1MPXqw76INFqzhaN0JkS7Gtk_OoxnSgQtDO9pM9jwBlkuv1cdWN39T4pjbYVtpUJTYy-iosjbOnCh6quBJOOzbABcDs9gqSy8iM4FbntRlngJZVIRznXzeVOrE"; // Set your access token here (it is quite long string)
        private const string APP_ROOT_URI = "/IngenStudioAppFolder";
        public static string AccessToken
        {
            get
            {
                return ACCESS_TOKEN;
            }
        }

        /// <summary>
        /// The root path where DwgOperations app files are stored.
        /// </summary>
        public static string AppRootUri
        {
            get
            {
                return APP_ROOT_URI;
            }
        }

       public readonly string token = "sl.BAd1K5QvO5HZe1MPXqw76INFqzhaN0JkS7Gtk_OoxnSgQtDO9pM9jwBlkuv1cdWN39T4pjbYVtpUJTYy-iosjbOnCh6quBJOOzbABcDs9gqSy8iM4FbntRlngJZVIRznXzeVOrE";
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
           
            string localDownloadPath = @"D:\Download";

            bool response = false;
            var list = await new dropboxApi.DropboxClient(AccessToken).Files.ListFolderAsync(string.Empty, true);
            var folders = list.Entries.Where(x => x.IsFolder);

            foreach (var folder in folders)
            {
                response = await _dropboxService.DownloadFolder(AppRootUri, localDownloadPath);

            }

            response = await _dropboxService.DownloadFolder(AppRootUri, localDownloadPath);

            
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> DownloadZip()
        {


            string localDownloadPath = @"D:\Download";

            bool response = false;
            var list = await new dropboxApi.DropboxClient(AccessToken).Files.ListFolderAsync(string.Empty, true);
            var folders = list.Entries.Where(x => x.IsFolder);

            //foreach (var folder in folders)
            //{
            //    response = await _dropboxService.DownloadFolder("/", localDownloadPath);

            //}
            response = await _dropboxService.DownloadFolder(AppRootUri, localDownloadPath);
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAllFiles()
        {
            string localDownloadPath = @"D:\Download";

            bool response =false;
            var list = await new dropboxApi.DropboxClient(AccessToken).Files.ListFolderAsync(string.Empty, true);

            var files = list.Entries.Where(x => x.IsFile);
            foreach (var file in files)
            {
                response = await _dropboxService.DownloadFile(file.AsFile.PathLower, localDownloadPath);
            }

            return Json(response);
        }








    }
}
