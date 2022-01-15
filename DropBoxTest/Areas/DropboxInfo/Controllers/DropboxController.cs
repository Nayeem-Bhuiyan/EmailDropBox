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
using System.Globalization;
using System.Text;

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



            var filename = string.Format(
                   CultureInfo.InvariantCulture,
                   "upload",
                   DateTime.Now);
            var fName = Path.ChangeExtension(filename, ".txt");
            string file = @"D:\DownloadImage\download.jpg";


                var folder = "/" + model.folderName+"/";

            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(file)))
                {
                    //var updated = dropBoxclient.Files.UploadAsync(folder + "/" + filename, WriteMode.Overwrite.Instance, body: mem);
                    //updated.Wait();
                    //var tx = dropBoxclient.Sharing.CreateSharedLinkWithSettingsAsync(folder + "/" + filename);
                    //tx.Wait();
                    //url = tx.Result.Url;
                 //await dropBoxclient.Files.UploadAsync("/Profile/" + filename, body: mem);


            }

            using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(file)))
            {
                var response = await dropBoxclient.Files.UploadAsync(folder + fName, WriteMode.Overwrite.Instance, body: stream);

            }
            

            return View(model);
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
