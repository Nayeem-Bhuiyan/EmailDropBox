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
        string token = "sl.BAAgQXyjgasV1OS-GtnmsTwVo8HiArW2Mnp2uU_qXc7xPqoV3SQv0T5DB8w5d8BXCCh0kxdnk3EDh8karZPNiePO6zHAK5WV1iIsFoKCE1s_B_0w6LXurffviIX1JWDI54l2wgA";
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



        public async Task<IActionResult> UploadFiles(CreateFolderViewModel model)
        {
            var dropboxApiToken = token;
            var bakupFolderPath = "D:/myDropbox";
            try
            {
                var dateNowString = System.DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                var backupName = $"<YOUR_DB_NAME>_db_bak{dateNowString}.sql.gz";
                var localFilePath = Path.Combine(bakupFolderPath, backupName);

                if (string.IsNullOrEmpty(localFilePath))
                {
                    var dbs = new DropBoxService(dropboxApiToken);
                   
                    await dbs.Upload(backupName, localFilePath);
                
                    //File.Delete(localFilePath);
                }
                else
                {
                    // WARN
                }


            }
            catch (System.Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                // WARN
            }

            return View();

        }




        }

    public class DropBoxService
    {
        private string ApiToken { get; set; }
        public DropBoxService(string token)
        {
            ApiToken = token;
        }

        public async Task Upload(string remoteFileName, string localFilePath)
        {
            using (var dbx = new DropboxClient(this.ApiToken))
            {
                using (var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                {
                    await dbx.Files.UploadAsync($"/{remoteFileName}", WriteMode.Overwrite.Instance, body: fs);
                }
            }
        }
    }






}
