using Dropbox.Api;
using DropBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Dropbox.Api.Common;
using Dropbox.Api.Files;
using Dropbox.Api.Team;

namespace DropBoxTest.Areas.DropboxInfo.Controllers
{
    [Area("DropboxInfo")]
    public class DropboxController : Controller
    {
        public async Task<IActionResult> FolderList()
        {
            List<FolderDetails> listFolder = new List<FolderDetails>();
            string token = "sl.A_-b6-CkQgOwqnM71H5Ik79wl5XZ4JxW06VN8eL6Mm8bo6V3MLrm8CGgnLXf-DYi_I5WKtSM4JIK36y75wddpjWQcDufnOcc68p47F6N8GEh3tEgfMK9t-phhuEDZ-sQi6k_3Eo";
            using (var dbx = new DropboxClient(token))
            {
                var list = await dbx.Files.ListFolderAsync(string.Empty, true);
                var files = list.Entries.Where(x => x.IsFile);

                foreach (var file in files)
                {

                    var currentPath = Path.GetFullPath(file.Name);
                    currentPath = Directory.GetParent(currentPath).FullName;


                    FolderDetails data = new FolderDetails
                    {
                        folderName = file.Name,
                        folderPath = file.PathDisplay,
                        //downloadLink= "https://www.dropbox.com/home"+file.PathDisplay
                        downloadLink= "https://www.dropbox.com/home" + currentPath
                    };
                    listFolder.Add(data);
                }
            }
            return View(listFolder);

        }


            public async Task<IActionResult> DropboxUserInfo()
            {
                string token = "sl.A_-b6-CkQgOwqnM71H5Ik79wl5XZ4JxW06VN8eL6Mm8bo6V3MLrm8CGgnLXf-DYi_I5WKtSM4JIK36y75wddpjWQcDufnOcc68p47F6N8GEh3tEgfMK9t-phhuEDZ-sQi6k_3Eo";
                UserDetails data = new UserDetails();
                using (var dbx = new DropboxClient(token))
                {
                    var user = await dbx.Users.GetCurrentAccountAsync();
                    data = new UserDetails
                    {
                        userName = user.Name?.DisplayName,
                        email = user.Email,
                        country = user.Country,
                        profileImageUrl = user.ProfilePhotoUrl
                    };
                }
                return View(data);
            }
    }
}
