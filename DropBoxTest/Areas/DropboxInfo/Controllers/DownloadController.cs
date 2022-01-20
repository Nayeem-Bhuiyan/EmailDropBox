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

        string token = "sl.BAeM9Acr4tzv5k-t4VUiRHeXKXwV2ADT6gzx2dI0gltMcJRyVHU4seLoxvwiFX0Az4YD7DpKM-iSUyeOgKEHBQ1r5A-Ul0odGzuAU0eXQ1bioLBKL04IjgzT6_aRVwH2yr6QevA";

        public async Task<IActionResult> Index()
        {
            string localDownloadPath = @"D:\Nayeem\Project_Dropbox\DropBoxTest\DropBoxTest\wwwroot\DownLoad";


            var list = await new dropboxApi.DropboxClient(token).Files.ListFolderAsync(string.Empty, true);
            var folders = list.Entries.Where(x => x.IsFolder);
            var files = list.Entries.Where(x => x.IsFile);
            //foreach (var folder in folders)
            //{
            //    await DownloadFolder(folder.PathLower, localDownloadPath);
            //}

            foreach (var file in files)
            {
                await DownloadFile(file.AsFile.PathLower, localDownloadPath);
            }


            return View();
        }



        //svcUri=dropbox folder url

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

        public async Task<bool> DownloadFile(string svcFileUri, string localFilePath)
        {
            try
            {
                using (var client = new dropboxApi.DropboxClient(token))
                {
                    var result = await client.Files.DownloadAsync(svcFileUri);
                    using (Stream sourceStream = await result.GetContentAsStreamAsync())
                    using (FileStream source =System.IO.File.Open(localFilePath, FileMode.Create))
                    {
                        await sourceStream.CopyToAsync(source);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return null;
            }
        }

    }
}
