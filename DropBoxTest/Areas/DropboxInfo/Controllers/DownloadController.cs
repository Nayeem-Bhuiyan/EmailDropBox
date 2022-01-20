using Dropbox.Api;
using DropBoxTest.Areas.DropboxInfo.Models;
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
        public DownloadController(IWebHostEnvironment environment)
        {

            _environment = environment;
        }

        private const string ACCESS_TOKEN = "sl.BAd1K5QvO5HZe1MPXqw76INFqzhaN0JkS7Gtk_OoxnSgQtDO9pM9jwBlkuv1cdWN39T4pjbYVtpUJTYy-iosjbOnCh6quBJOOzbABcDs9gqSy8iM4FbntRlngJZVIRznXzeVOrE"; // Set your access token here (it is quite long string)
        private const string APP_ROOT_URI = "/Documents"; // Set your application root folder name
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

        string token = "sl.BAd1K5QvO5HZe1MPXqw76INFqzhaN0JkS7Gtk_OoxnSgQtDO9pM9jwBlkuv1cdWN39T4pjbYVtpUJTYy-iosjbOnCh6quBJOOzbABcDs9gqSy8iM4FbntRlngJZVIRznXzeVOrE";
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
           
            string localDownloadPath = @"D:\Download";

            string response = "";
            var list = await new dropboxApi.DropboxClient(AccessToken).Files.ListFolderAsync(string.Empty, true);
            var folders = list.Entries.Where(x => x.IsFolder);

            foreach (var folder in folders)
            {
                response = await DownloadFolder(folder.PathLower, localDownloadPath);

            }
            //var files = list.Entries.Where(x => x.IsFile);
            //foreach (var file in files)
            //{
            //    response = await DownloadFile(file.AsFile.PathLower, localDownloadPath);
            //}


            return Ok(response);
        }








        //svcUri=dropbox folder url

        public async Task<string> DownloadFolder(string svcUri, string localFilePath)
        {
            try
            {
                string sharedFolderUrl = await this.GetFolderSharedLink(svcUri);
                string fullPath = Path.Combine(localFilePath, $"{Path.GetFileName(svcUri)}.zip");

                using (var webClient = new WebClient())
                {
                    await Task.Run(() =>
                    {
                        // dl=1 flag shows that folder will be downloaded as .zip archieve
                        webClient.DownloadFile(new Uri(sharedFolderUrl.Replace("dl=0", "dl=1")), fullPath);
                    });
                }

                return "Success fully Download";
            }
            catch (Exception ex)
            {
                return ex.Message+ " ResponseFrom : DownloadFolder";
            }
        }

        public async Task<string> DownloadFile(string svcFileUri, string localFilePath)
        {
            try
            {
                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    var result = await client.Files.DownloadAsync(svcFileUri);
                    using (Stream sourceStream = await result.GetContentAsStreamAsync())
                    //using (FileStream source =System.IO.File.Open(localFilePath, FileMode.Create))
                    //{
                    //    await sourceStream.CopyToAsync(source);
                    //}
                    using (var source =new FileStream(localFilePath, FileMode.Create))
                    {
                        await sourceStream.CopyToAsync(source);
                    }
                }

                return "Success!!DownloadFile";
            }
            catch (Exception ex)
            {
              
                return ex.Message+ " ResponseFrom : DownloadFile";
            }
        }


        public async Task<string> GetFolderSharedLink(string svcUri)
        {
            try
            {
                dropboxApi.Sharing.ListSharedLinksResult result = null;

                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Sharing.ListSharedLinksAsync(svcUri, directOnly: true);
                    if (result.Links.Count == 0)
                    {
                        var sharedLinkMeta = await client.Sharing.CreateSharedLinkWithSettingsAsync(svcUri);
                        return sharedLinkMeta.Url;
                    }
                }

                return result.Links[0].Url;
            }
            catch (Exception ex)
            {
                return ex.Message+"  ResponseFrom : GetFolderSharedLink";
            }
        }



    }
}
