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
using dropboxApi = global::Dropbox.Api;
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
        private const string ACCESS_TOKEN = "sl.BAd1K5QvO5HZe1MPXqw76INFqzhaN0JkS7Gtk_OoxnSgQtDO9pM9jwBlkuv1cdWN39T4pjbYVtpUJTYy-iosjbOnCh6quBJOOzbABcDs9gqSy8iM4FbntRlngJZVIRznXzeVOrE"; // Set your access token here (it is quite long string)
        private const string APP_ROOT_URI = "/Documents";
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


       

        public async Task<IActionResult> DownloadZip()
        {
            

            var client = new DropboxClient(token);
            var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);
            CreateFolderViewModel model = new CreateFolderViewModel();
            List<string> SourceUrlList = new List<string>();
            foreach (var item in dataList.Entries.Where(i => i.IsFile))
            {
                string srcFile = @"https://www.dropbox.com/home" + item.AsFile.PathLower;

                SourceUrlList.Add(srcFile);



            }
            model.imageUrlList = SourceUrlList;

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {

                    for (int i = 0; i < model.imageUrlList.Count; i++)
                    {
                        byte[] myByte = System.Text.ASCIIEncoding.Default.GetBytes(SourceUrlList[i]);
                        var fileentry = ziparchive.CreateEntry(SourceUrlList[i], CompressionLevel.Fastest);
                        using (var zipStream = fileentry.Open()) zipStream.Write(myByte, 0, myByte.Length);
                    }
                }
                var data = File(memoryStream.ToArray(), "application/zip", "Archeive" + Guid.NewGuid() + ".zip");
                return data;
            }


        }


        [HttpGet]
        public async Task<IActionResult> FileDownload()
        {
            var client = new DropboxClient(token);
            var dataList = await client.Files.ListFolderAsync(string.Empty, recursive: true);
            CreateFolderViewModel model = new CreateFolderViewModel();
            foreach (var item in dataList.Entries.Where(i => i.IsFile))
            {
                model.imageUrlList.Add(@"https://www.dropbox.com/home" + item.AsFile.PathLower);
            }
             await ZipDownloadHelper.GetZip(model.imageUrlList);
            return Json(true);
        }



        private async Task DownloadDropboxFile(string downloadDropboxFileUrl)
        {
            var client = new DropboxClient(token);
            var response = await client.Files.DownloadAsync(downloadDropboxFileUrl);
            ulong fileSize = response.Response.Size;
            const int bufferSize = 1024 * 1024;

            var buffer = new byte[bufferSize];

            using (var stream = await response.GetContentAsStreamAsync())
            {
                using (var file = new FileStream(@"D:\Nayeem\Project_Dropbox\DropBoxTest\DropBoxTest\wwwroot\DownLoad\", FileMode.OpenOrCreate))
                {
                    var length = stream.Read(buffer, 0, bufferSize);

                    while (length > 0)
                    {
                        file.Write(buffer, 0, length);
                        var percentage = 100 * (ulong)file.Length / fileSize;
                        // Update progress bar with the percentage.
                        // progressBar.Value = (int)percentage
                        Console.WriteLine(percentage);

                        length = stream.Read(buffer, 0, bufferSize);
                    }
                }
            }
        }





        private async Task DownloadData(DropboxClient dbx,string fromDownloadUrl, string localFilePath)
        {
            using (var response = await dbx.Files.DownloadAsync(fromDownloadUrl))
            {
                using (var fileStream =new FileStream(localFilePath,FileMode.OpenOrCreate))
                {
                    (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                }
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
            //return File(memory, GetMimeTypes()[ext], Path.GetFileName(path));
            return File(memory, GetMIMEType(path), Path.GetFileName(path));
        }








        //private Dictionary<string, string> GetMimeTypes()
        //{
        //    return new Dictionary<string, string>{
        //        {".txt", "text/plain"},
        //        {".pdf", "application/pdf"},
        //        {".doc", "application/vnd.ms-word"},
        //        {".docx", "application/vnd.ms-word"},
        //        {".png", "image/png"},
        //        {".jpg", "image/jpeg"},
        //        {".zip", "application/zip"},
        //        {".rar", "application/x-rar-compressed, application/octet-stream"},
        //    };
        //}

   private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
  {
    {"ai", "application/postscript"},
    {"aif", "audio/x-aiff"},
    {"aifc", "audio/x-aiff"},
    {"aiff", "audio/x-aiff"},
    {"asc", "text/plain"},
    {"atom", "application/atom+xml"},
    {"au", "audio/basic"},
    {"avi", "video/x-msvideo"},
    {"bcpio", "application/x-bcpio"},
    {"bin", "application/octet-stream"},
    {"bmp", "image/bmp"},
    {"cdf", "application/x-netcdf"},
    {"cgm", "image/cgm"},
    {"class", "application/octet-stream"},
    {"cpio", "application/x-cpio"},
    {"cpt", "application/mac-compactpro"},
    {"csh", "application/x-csh"},
    {"css", "text/css"},
    {"dcr", "application/x-director"},
    {"dif", "video/x-dv"},
    {"dir", "application/x-director"},
    {"djv", "image/vnd.djvu"},
    {"djvu", "image/vnd.djvu"},
    {"dll", "application/octet-stream"},
    {"dmg", "application/octet-stream"},
    {"dms", "application/octet-stream"},
    {"doc", "application/msword"},
    {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
    {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
    {"docm","application/vnd.ms-word.document.macroEnabled.12"},
    {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
    {"dtd", "application/xml-dtd"},
    {"dv", "video/x-dv"},
    {"dvi", "application/x-dvi"},
    {"dxr", "application/x-director"},
    {"eps", "application/postscript"},
    {"etx", "text/x-setext"},
    {"exe", "application/octet-stream"},
    {"ez", "application/andrew-inset"},
    {"gif", "image/gif"},
    {"gram", "application/srgs"},
    {"grxml", "application/srgs+xml"},
    {"gtar", "application/x-gtar"},
    {"hdf", "application/x-hdf"},
    {"hqx", "application/mac-binhex40"},
    {"htm", "text/html"},
    {"html", "text/html"},
    {"ice", "x-conference/x-cooltalk"},
    {"ico", "image/x-icon"},
    {"ics", "text/calendar"},
    {"ief", "image/ief"},
    {"ifb", "text/calendar"},
    {"iges", "model/iges"},
    {"igs", "model/iges"},
    {"jnlp", "application/x-java-jnlp-file"},
    {"jp2", "image/jp2"},
    {"jpe", "image/jpeg"},
    {"jpeg", "image/jpeg"},
    {"jpg", "image/jpeg"},
    {"js", "application/x-javascript"},
    {"kar", "audio/midi"},
    {"latex", "application/x-latex"},
    {"lha", "application/octet-stream"},
    {"lzh", "application/octet-stream"},
    {"m3u", "audio/x-mpegurl"},
    {"m4a", "audio/mp4a-latm"},
    {"m4b", "audio/mp4a-latm"},
    {"m4p", "audio/mp4a-latm"},
    {"m4u", "video/vnd.mpegurl"},
    {"m4v", "video/x-m4v"},
    {"mac", "image/x-macpaint"},
    {"man", "application/x-troff-man"},
    {"mathml", "application/mathml+xml"},
    {"me", "application/x-troff-me"},
    {"mesh", "model/mesh"},
    {"mid", "audio/midi"},
    {"midi", "audio/midi"},
    {"mif", "application/vnd.mif"},
    {"mov", "video/quicktime"},
    {"movie", "video/x-sgi-movie"},
    {"mp2", "audio/mpeg"},
    {"mp3", "audio/mpeg"},
    {"mp4", "video/mp4"},
    {"mpe", "video/mpeg"},
    {"mpeg", "video/mpeg"},
    {"mpg", "video/mpeg"},
    {"mpga", "audio/mpeg"},
    {"ms", "application/x-troff-ms"},
    {"msh", "model/mesh"},
    {"mxu", "video/vnd.mpegurl"},
    {"nc", "application/x-netcdf"},
    {"oda", "application/oda"},
    {"ogg", "application/ogg"},
    {"pbm", "image/x-portable-bitmap"},
    {"pct", "image/pict"},
    {"pdb", "chemical/x-pdb"},
    {"pdf", "application/pdf"},
    {"pgm", "image/x-portable-graymap"},
    {"pgn", "application/x-chess-pgn"},
    {"pic", "image/pict"},
    {"pict", "image/pict"},
    {"png", "image/png"},
    {"pnm", "image/x-portable-anymap"},
    {"pnt", "image/x-macpaint"},
    {"pntg", "image/x-macpaint"},
    {"ppm", "image/x-portable-pixmap"},
    {"ppt", "application/vnd.ms-powerpoint"},
    {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
    {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
    {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
    {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
    {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
    {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
    {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
    {"ps", "application/postscript"},
    {"qt", "video/quicktime"},
    {"qti", "image/x-quicktime"},
    {"qtif", "image/x-quicktime"},
    {"ra", "audio/x-pn-realaudio"},
    {"ram", "audio/x-pn-realaudio"},
    {"ras", "image/x-cmu-raster"},
    {"rdf", "application/rdf+xml"},
    {"rgb", "image/x-rgb"},
    {"rm", "application/vnd.rn-realmedia"},
    {"roff", "application/x-troff"},
    {"rtf", "text/rtf"},
    {"rtx", "text/richtext"},
    {"sgm", "text/sgml"},
    {"sgml", "text/sgml"},
    {"sh", "application/x-sh"},
    {"shar", "application/x-shar"},
    {"silo", "model/mesh"},
    {"sit", "application/x-stuffit"},
    {"skd", "application/x-koan"},
    {"skm", "application/x-koan"},
    {"skp", "application/x-koan"},
    {"skt", "application/x-koan"},
    {"smi", "application/smil"},
    {"smil", "application/smil"},
    {"snd", "audio/basic"},
    {"so", "application/octet-stream"},
    {"spl", "application/x-futuresplash"},
    {"src", "application/x-wais-source"},
    {"sv4cpio", "application/x-sv4cpio"},
    {"sv4crc", "application/x-sv4crc"},
    {"svg", "image/svg+xml"},
    {"swf", "application/x-shockwave-flash"},
    {"t", "application/x-troff"},
    {"tar", "application/x-tar"},
    {"tcl", "application/x-tcl"},
    {"tex", "application/x-tex"},
    {"texi", "application/x-texinfo"},
    {"texinfo", "application/x-texinfo"},
    {"tif", "image/tiff"},
    {"tiff", "image/tiff"},
    {"tr", "application/x-troff"},
    {"tsv", "text/tab-separated-values"},
    {"txt", "text/plain"},
    {"ustar", "application/x-ustar"},
    {"vcd", "application/x-cdlink"},
    {"vrml", "model/vrml"},
    {"vxml", "application/voicexml+xml"},
    {"wav", "audio/x-wav"},
    {"wbmp", "image/vnd.wap.wbmp"},
    {"wbmxl", "application/vnd.wap.wbxml"},
    {"wml", "text/vnd.wap.wml"},
    {"wmlc", "application/vnd.wap.wmlc"},
    {"wmls", "text/vnd.wap.wmlscript"},
    {"wmlsc", "application/vnd.wap.wmlscriptc"},
    {"wrl", "model/vrml"},
    {"xbm", "image/x-xbitmap"},
    {"xht", "application/xhtml+xml"},
    {"xhtml", "application/xhtml+xml"},
    {"xls", "application/vnd.ms-excel"},
    {"xml", "application/xml"},
    {"xpm", "image/x-xpixmap"},
    {"xsl", "application/xml"},
    {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
    {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
    {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
    {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
    {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
    {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
    {"xslt", "application/xslt+xml"},
    {"xul", "application/vnd.mozilla.xul+xml"},
    {"xwd", "image/x-xwindowdump"},
    {"xyz", "chemical/x-xyz"},
    {"zip", "application/zip"}
  };

        public static string GetMIMEType(string fileName)
        {
            //get file extension
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension.Length > 0 &&
                MIMETypesDictionary.ContainsKey(extension.Remove(0, 1)))
            {
                return MIMETypesDictionary[extension.Remove(0, 1)];
            }
            return "unknown/unknown";
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







        //[HttpGet]
        //public async Task<IActionResult> GetBlobDownload()
        //{

        //    var client = new DropboxClient(token);
        //    var dataList = await client.Files.ListFolderAsync(string.Empty,recursive:true);
        //    CreateFolderViewModel model = new CreateFolderViewModel();
        //    List<string> listUrl = new List<string>();
        //    foreach (var item in dataList.Entries.Where(i => i.IsFile))
        //    {
                
        //        listUrl.Add((item.AsFile.PathLower).ToString());
        //    }
        //    model.imageUrlList=listUrl;
        //    var filePathTemp= Path.Combine(_environment.WebRootPath, @"DownLoad");
        //            foreach (var fromUrl in model.imageUrlList)
        //            {
        //                await DownloadData(client, fromUrl, filePathTemp);
        //            }
        //}






















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
            model.imageUrlList = GetImageUrlList(model);
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
            DeleteUploadedFileFromFolder();
            return Json(model);
        }



        //public async Task<List<dropboxApi.Files.FileMetadata>> UploadFiles(string svcPath, string localDirPath, List<string> filePaths)
        //{
        //    var uploadResults = new List<dropboxApi.Files.FileMetadata>();

        //    try
        //    {
        //        if (filePaths == null || filePaths.Count() == 0)
        //        {
        //            throw new ArgumentNullException("UploadFiles: files list was empty");
        //        }

        //        foreach (var relativePath in filePaths)
        //        {
        //            string fullLocalDir = Path.Combine(localDirPath, relativePath);
        //            string fullSvcUri = svcPath.ToUri() + relativePath.ToUri();

        //            if (System.IO.File.Exists(fullLocalDir))
        //            {
        //                using (var client = new dropboxApi.DropboxClient(AccessToken))
        //                using (Stream fileStream = System.IO.File.OpenRead(fullLocalDir))
        //                {
        //                    var result = await client.Files.UploadAsync(fullSvcUri, body: fileStream);
        //                    uploadResults.Add(result);
        //                }
        //            }
        //            else
        //            {
        //                throw new FileNotFoundException($"DropboxManager: File with name: {relativePath} does not exists!");
        //            }
        //        }

        //        return uploadResults;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        public List<string> GetImageUrlList(CreateFolderViewModel model)
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


        public int DeleteUploadedFileFromFolder()
        {
            int response =0;
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\DownLoad"));

            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Exists)
                {
                    file.Delete();
                    response += 1;
                }

            }

            return response;
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


        //private async Task Download(string foldername)
        //{
           
        //    var dbx = new DropboxClient(token);
        //    using (var response = await dbx.Files.DownloadZipAsync("/" + foldername))
        //    {
        //        Stream ms = await response.GetContentAsStreamAsync();
        //        Int16 bufferSize = 1024;
        //        byte[] buffer = new byte[bufferSize + 1];
        //        Response.Clear();
        //        Response.ContentType = "application/x-zip-compressed";
        //        Response.AddHeader("content-disposition", "attachment; filename=myfolder.zip");
        //        Response.BufferOutput = false;
        //        int count = ms.Read(buffer, 0, bufferSize);
        //        while (count > 0)
        //        {
        //            Response.OutputStream.Write(buffer, 0, count);
        //            count = ms.Read(buffer, 0, bufferSize);
        //        }
        //    }
        //}




        //private async Task Download(string filePathFromDropbox)
        //{
        //    var dropBoxclient = new DropboxClient(token);
        //    var response = await dropBoxclient.Files.DownloadAsync(filePathFromDropbox);
        //    var expirationTimeTask = Task.Delay(TimeSpan.FromSeconds(30));
        //    var strm = response.GetContentAsStreamAsync();
        //    var finishedTask = await Task.WhenAny(expirationTimeTask, strm);
        //    if (finishedTask == expirationTimeTask)
        //    {
        //        //timeout error processing (does not get here)
        //    }
        //}



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

    }
}