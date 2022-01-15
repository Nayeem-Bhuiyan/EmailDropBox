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
using DropBoxTest.Areas.DropboxInfo.Models;


namespace DropBoxTest.Areas.DropboxInfo.Controllers
{
    [Area("DropboxInfo")]
    public class DropboxController : Controller
    {
        string token = "sl.BALFmi6vEWlzknjBFsWDjgPpEUuitPqfryzhYy83akPZE6GBmHA6o1lREGOHwpbTqPb1E3gb1q45AhC61sUd1F9BnNtGsDlPQDOCWuomMwTUPoGaEnO4fVbEHeG8e3x3q6D0dfw";
        public async Task<IActionResult> FolderList()
        {
            List<FolderDetails> listFolder = new List<FolderDetails>();
            using (var client = new DropboxClient(token))
            {
                var list = await client.Files.ListFolderAsync(string.Empty, true);
                var folders = list.Entries.Where(x => x.IsFolder);
                var files = list.Entries.Where(x => x.IsFile);
                
                foreach (var folder in folders)
                {

                    FolderDetails data = new FolderDetails
                    {
                        folderName = folder.Name,
                        folderPath = folder.PathDisplay,
                        downloadLink = "https://www.dropbox.com/home" + folder.PathDisplay,
                        count = files.Count()

                    };
                    listFolder.Add(data);

                }
            }
            return View(listFolder);

        }

        public async Task<IActionResult> DropboxUserInfo()
            {
                
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

        [HttpGet]
        public IActionResult CreateFolder()
        {
            CreateFolderViewModel model = new CreateFolderViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder(CreateFolderViewModel model)
        {
        
            var dropBoxclient = new DropboxClient(token);
            var list = await dropBoxclient.Files.ListFolderAsync(string.Empty);
            
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                if (item.Name.Equals(model.folderName))
                {

                    model.errorResponse = "Sorry: folder with name "+model.folderName+" already exists!";
                    
                }
                
            }
            if (model.errorResponse!= "Sorry: folder with name " + model.folderName + " already exists!")
            {
                Dropbox.Api.Files.CreateFolderArg folderArg = new CreateFolderArg("/" + model.folderName);
                await dropBoxclient.Files.CreateFolderV2Async(folderArg);
                model.successResponse = "Successfully Created folder named  " + model.folderName;
                model.redirectFolder = "https://www.dropbox.com/home/" + model.folderName;
            }
            else
            {
                model.redirectFolder = "https://www.dropbox.com/home/" + model.folderName;
            }
           

            return View(model);
        }


        public async Task Upload(string remoteFileName, string localFilePath)
        {
            
            using (var dbx = new DropboxClient(token))
            {
                using (var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                {
                    await dbx.Files.UploadAsync($"/{remoteFileName}", WriteMode.Overwrite.Instance, body: fs);
                }
            }
        }


    }
}
