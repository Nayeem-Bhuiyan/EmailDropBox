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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DropBoxTest.Helper;
using System.IO.Compression;
using System.Threading;


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


        string token = "sl.BAUmNo_NZAkfgDdmkLMUAaNKzQS4oaAbnZuO2OaEKe8rsCiXDWHVcNhDtvF8RsdB0TC6XHmTlwqJ2SKDpCsLJLJkpHgPYrq56WUAHr8qi4u47zGwiNxEt5zpaGLEhm6tVo3fI-Q";

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
                    string srcFile = @"https://www.dropbox.com/home" + item.AsFile.PathLower;



                    //await FileDownload(urlFile);

                    //string downloadPath = Path.Combine(this._environment.WebRootPath, "DownLoad");
                    //using (var response = await client.Files.DownloadAsync(srcFile))
                    //{
                    //    using (var fileTodownload = new FileStream(downloadPath, FileMode.Open))
                    //    {
                    //        (await response.GetContentAsStreamAsync()).CopyTo(fileTodownload);
                    //    }
                    //}

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



        //public async Task<IActionResult> HttpDownloadFile()
        //{

        //    string ToDownloadPath = @"D:\Nayeem\Project_Dropbox\DropBoxTest\DropBoxTest\wwwroot\DownLoad\";
        //    var client = new DropboxClient(token);
        //    var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);

        //    List<string> SourceUrlList = new List<string>();
        //    foreach (var item in dataList.Entries.Where(i => i.IsFile))
        //    {
        //        string srcFile = @"https://www.dropbox.com/home" + item.AsFile.PathLower;

        //        SourceUrlList.Add(srcFile);
        //    }

        //    foreach (var FromDownloadPath in SourceUrlList)
        //    {
        //        HttpClient httpClient = new HttpClient();
        //        using HttpResponseMessage response = await httpClient.GetAsync(FromDownloadPath, HttpCompletionOption.ResponseHeadersRead);
        //        using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
        //        //using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
        //        var streamToWriteTo = new FileStream(ToDownloadPath, FileMode.Create);
        //       await streamToReadFrom.CopyToAsync(streamToWriteTo);
        //    }

        //}
        public async Task<IActionResult> DownloadZip()
        {
            var client = new DropboxClient(token);
            var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);

            List<string> SourceUrlList = new List<string>();
            foreach (var item in dataList.Entries.Where(i => i.IsFile))
            {
                string srcFile = @"https://www.dropbox.com/home" + item.AsFile.PathLower;

                SourceUrlList.Add(srcFile);



            }
            string ToDownloadUrl = @"D:\Nayeem\Project_Dropbox\DropBoxTest\DropBoxTest\wwwroot\DownLoad";

            foreach (var fromDownloadUrl in SourceUrlList)
            {
                Downloader.Download(fromDownloadUrl, ToDownloadUrl,20);
            }

            //using (var memoryStream = new MemoryStream())
            //{
            //    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            //    {

            //        for (int i = 0; i < SourceUrlList.Count; i++)
            //        {
            //            byte[] myByte = System.Text.ASCIIEncoding.Default.GetBytes(SourceUrlList[i]);
            //            var fileentry = ziparchive.CreateEntry(SourceUrlList[i], CompressionLevel.Fastest);
            //            using (var zipStream = fileentry.Open()) zipStream.Write(myByte, 0, myByte.Length);
            //        }
            //    }
            //    var data = File(memoryStream.ToArray(), "application/zip", "Archeive" + Guid.NewGuid() + ".zip");
            //    return data;
            //}
            return View();

        }



        public async Task FileDownload()
        {
            var client = new DropboxClient(token);
            var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);

            foreach (var item in dataList.Entries.Where(i => i.IsFile))
            {
                string srcFile = @"https://www.dropbox.com/home" + item.AsFile.PathLower;

                await MakeDownload(srcFile);
            }

        }


        public async Task<IActionResult> MakeDownload(string path)
        {
            //var path = @"C:\Vetrivel\winforms.png";
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return File(memory, GetMimeTypes()[ext], Path.GetFileName(path));


        }




        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
                        {
                            {".txt", "text/plain"},
                            {".pdf", "application/pdf"},
                            {".doc", "application/vnd.ms-word"},
                            {".docx", "application/vnd.ms-word"},
                            {".png", "image/png"},
                            {".jpg", "image/jpeg"}
                        };
        }






        public async Task<IActionResult> DownloadLargeFile()
        {

            var client = new DropboxClient(token);
            var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);

            List<string> SourceUrlList = new List<string>();
            foreach (var item in dataList.Entries.Where(i => i.IsFile))
            {
                string srcFile = @"https://www.dropbox.com/home" + item.AsFile.PathLower;
                SourceUrlList.Add(srcFile);
            }


            return new PushStreamResult(
                 async (outputStream) =>
                 {
                     using (var zip = new ZipArchive(new WriteOnlyStreamWrapper(outputStream),
                         ZipArchiveMode.Create))
                     {
                         foreach (var f in SourceUrlList)
                         {
                             //var filePathTemp = "path from you will read the file";
                             var filePathTemp = @"D:\Nayeem\Project_Dropbox\DropBoxTest\DropBoxTest\wwwroot\DownLoad";
                             using (var res = System.IO.File.OpenRead(filePathTemp))
                             {
                                 var entry = zip.CreateEntry(Path.GetFileName(f));
                                 using (var entryStream = entry.Open())
                                 {
                                     await res.CopyToAsync(entryStream);
                                 }
                                 res.Close();
                             }
                         }
                     }
                 }, "application/octet-stream", "file_download_name.zip");

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