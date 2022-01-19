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

        string token = "sl.BAZHMe7tvwk1BsmHDBbCD7TQRoj9PFj0Izj7z6KsHLqg5s9Q6JRPzoTBVmtS_kslcA-HEf1RBhkrdwvbQP9W7ORn9Ythtdl9xYgOn1j3cmzdI6LMhFdlB72O22_BTbDDcXibVtU";

        public async Task<IActionResult> Index()
        {
           await DownloadFolder("Profile", @"D:\Nayeem\Project_Dropbox\DropBoxTest\DropBoxTest\wwwroot\DownLoad");
            return View();
        }




        public async Task<bool> DownloadFolder(string svcUri, string localFilePath)
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

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> GetFolderSharedLink(string svcUri)
        {
            try
            {
                dropboxApi.Sharing.ListSharedLinksResult result = null;

                using (var client = new dropboxApi.DropboxClient(token))
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
            catch (Exception)
            {
                return null;
            }
        }

    }
}
