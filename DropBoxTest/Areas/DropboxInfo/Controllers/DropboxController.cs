﻿using Dropbox.Api;
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
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DropBoxTest.Areas.DropboxInfo.Controllers
{
    [Area("DropboxInfo")]
    public class DropboxController : Controller
    {
        private IWebHostEnvironment _environment;
        public DropboxController(IWebHostEnvironment environment)
        {

            _environment = environment;
        }


        string token = "sl.BAQdymFj_g7Yem42gd1BI2RRj9Sfz8DeFIDZOjP__WVFIQfluB-WFsj6tmOveguAjapCMUcPmOWaXRXkZ-_Aw1etq43VpledUXrM99KpEu1V3-uNcPQgJ_lXf1TQeQVm8KoLsJU";

        public async Task<IActionResult> FolderList()
        {


            List<FolderDetails> listFolder = new List<FolderDetails>();
            using (var client = new DropboxClient(token))
            {
                var list = await client.Files.ListFolderAsync(string.Empty, true);
                var folders = list.Entries.Where(x => x.IsFolder);
                var files = list.Entries.Where(x => x.IsFile);
    
                var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);

                foreach (var item in dataList.Entries.Where(i => i.IsFile))
                {
                    string urlFile = item.AsFile.PathLower;
                    string downloadPath = Path.Combine(this._environment.WebRootPath, "DownLoad");
                    using (var response = await client.Files.DownloadAsync(urlFile))
                    {
                        using (var fileTodownload = new FileStream(downloadPath, FileMode.Open))
                        {
                            (await response.GetContentAsStreamAsync()).CopyTo(fileTodownload);
                        }
                        
                    }
                }



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
        public async Task<IActionResult> saveToFolder([FromForm] CreateFolderViewModel model)
        {

            var dropBoxclient = new DropboxClient(token);
            var list = await dropBoxclient.Files.ListFolderAsync(string.Empty);

            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                if (item.Name.Equals(model.folderName))
                {

                    model.errorResponse = "Sorry: folder with name " + model.folderName + " already exists!";

                }

            }
            if (model.errorResponse != "Sorry: folder with name " + model.folderName + " already exists!")
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

            var user = await dropBoxclient.Users.GetCurrentAccountAsync();

            var targetFolder = "/" + model.folderName + "/";
            string path = Path.Combine(this._environment.WebRootPath, "UploadedImageFolder");
            model.imageUrlList = SaveUpload(model);
            int? count = 1;
            foreach (string imageUrl in model.imageUrlList)
            {
                string targetFileName = user.Name.DisplayName + count + DateTime.Now.ToString("yymmssfff") + ".jpg";

                using (var fileToSave = new FileStream(imageUrl, FileMode.Open))
                {
                    await dropBoxclient.Files.UploadAsync(targetFolder + targetFileName, WriteMode.Overwrite.Instance, body: fileToSave);

                }
                count = count + 1;
            }



            model.countFile = count;
            return Json(model);
        }

        public List<string> SaveUpload(CreateFolderViewModel model)
        {
            string wwwPath = this._environment.WebRootPath;
            string contentPath = this._environment.ContentRootPath;

            string path = Path.Combine(this._environment.WebRootPath, "UploadedImageFolder");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> dataList = new List<string>();
            foreach (IFormFile image in model.imageList)
            {
                string fileName = Path.GetFileName(image.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    image.CopyTo(stream);
                    dataList.Add(Path.Combine(this._environment.WebRootPath, "UploadedImageFolder", fileName));

                }
            }
            model.imageUrlList = dataList;
            return model.imageUrlList;
        }


        //private async Task Upload(string localPath, string remotePath)
        //{
        //    var dropBoxclient = new DropboxClient(token);
        //    const int ChunkSize = 40096 * 1024;
        //    using (var fileStream = new FileStream(localPath, FileMode.Open))
        //    {
        //        if (fileStream.Length <= ChunkSize)
        //        {
        //            await dropBoxclient.Files.UploadAsync(remotePath, body: fileStream);
        //        }
        //        else
        //        {
        //            await this.ChunkUpload(remotePath, fileStream, (int)ChunkSize);
        //        }
        //    }
        //}


        //private async Task ChunkUpload(String path, FileStream stream, int chunkSize)
        //{
        //    var dropBoxclient = new DropboxClient(token);
        //    ulong numChunks = (ulong)Math.Ceiling((double)stream.Length / chunkSize);
        //    byte[] buffer = new byte[chunkSize];
        //    string sessionId = null;
        //    for (ulong idx = 0; idx < numChunks; idx++)
        //    {
        //        var byteRead = stream.Read(buffer, 0, chunkSize);

        //        using (var memStream = new MemoryStream(buffer, 0, byteRead))
        //        {
        //            if (idx == 0)
        //            {
        //                var result = await dropBoxclient.Files.UploadSessionStartAsync(false, memStream);
        //                sessionId = result.SessionId;
        //            }
        //            else
        //            {
        //                var cursor = new UploadSessionCursor(sessionId, (ulong)chunkSize * idx);

        //                if (idx == numChunks - 1)
        //                {
        //                    FileMetadata fileMetadata = await dropBoxclient.Files.UploadSessionFinishAsync(cursor, new CommitInfo(path), memStream);
        //                    Console.WriteLine(fileMetadata.PathDisplay);
        //                }
        //                else
        //                {
        //                    await dropBoxclient.Files.UploadSessionAppendV2Async(cursor, false, memStream);
        //                }
        //            }
        //        }
        //    }
        //}



        private async Task Download(string filePathFromDropbox)
        {
            var dropBoxclient = new DropboxClient(token);
            var response = await dropBoxclient.Files.DownloadAsync(filePathFromDropbox);
            var expirationTimeTask = Task.Delay(TimeSpan.FromSeconds(30));
            var strm = response.GetContentAsStreamAsync();
            var finishedTask = await Task.WhenAny(expirationTimeTask, strm);
            if (finishedTask == expirationTimeTask)
            {
                //timeout error processing (does not get here)
            }
        }



        public IActionResult ListTeamMembers()
        {

            return View();
        }

        private async Task<IActionResult> ListTeamMembers(DropboxTeamClient client)
        {
            var members = await client.Team.MembersListAsync();
            List<TeamMember> dataList = new List<TeamMember>();
            foreach (var member in members.Members)
            {
                TeamMember data = new TeamMember
                {
                    TeamMemberId = member.Profile.TeamMemberId,
                    Name = member.Profile.Name.ToString(),
                    Email = member.Profile.Email
                };
                dataList.Add(data);
            }

            return View(dataList);
        }





        //public async Task Upload(string remoteFileName, string localFilePath)
        //    {

        //        using (var dbx = new DropboxClient(token))
        //        {
        //            using (var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
        //            {
        //                await dbx.Files.UploadAsync($"/{remoteFileName}", WriteMode.Overwrite.Instance, body: fs);
        //            }
        //        }
        //    }

        //public IActionResult Upload()
        //{

        //    return View();
        //}
        //[HttpPost]
        //public async Task<ActionResult> Upload(string name, DateTime date, string content)
        //{
        //    var filename = string.Format(
        //        CultureInfo.InvariantCulture,
        //        "{0}.{1:yyyyMMdd}.md",
        //        name,
        //        date);

        //    var client = new DropboxClient(token);
        //    if (client == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
        //    {
        //        var upload = await client.Files.UploadAsync("/" + filename, body: mem);
        //    }

        //}





    }
}
