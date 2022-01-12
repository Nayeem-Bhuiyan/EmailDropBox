using Dropbox.Api;
using DropBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Dropbox.Api.Common;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using System.IO;

namespace DropBoxTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

            string token = "sl.A_8TrrTIdb5xcIFbbJjetCg6va35Xp_Eqv-aYrPQ5ZwMwgepwbAnkBHEqDhmI36Xcvm1R7j_odN5KjU1VeZ7kdG0wQd2KuIsAd3-ljwg903mGU6hsbMnzQ3fnRw7aeISqyfRpvg";
            UserDetails data = new UserDetails();
            using (var dbx = new DropboxClient(token))
            {
                var id = await dbx.Users.GetCurrentAccountAsync();
                 data = new UserDetails
                {
                    userName=id.Name.DisplayName,
                    email=id.Email,
                    country=id.Country,
                    profileImageUrl=id.ProfilePhotoUrl

                };


            }

            return View(data);
        }


        //public async Task<IActionResult> Download()
        //{
        //    string token = "sl.A_7zSU1dpe5eBiEq6gXvczEg4mnR8jkXJFXJzK1rMUDkMIRySn0mKiQO_W340LbOPAEZoKr8SIx6ybp5imjqiSjgJa5GOcbY7yZYgvAsTOuwM5arU3m1mnD4-nrap9YIrW-E8v4";
        //    using (var dbx = new DropboxClient(token))
        //    {
               
        //        string folder = "";
        //        string file = "full-file-name";
        //        using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
        //        {
        //            var s = response.GetContentAsByteArrayAsync();
        //            s.Wait();
        //            var d = s.Result;
        //           //File.WriteAllBytes(file, d);

        //            //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

        //            //return File(fileBytes, "application/force-download", fileName);
        //        }
        //    }

        //}

        //public async Task<IEnumerable<UploadedImageDetails>> UploadedImageInfo(this DropboxClient client)
        //{
        //    string token = "sl.A_1H9p1YUdJlLv7Dxa17hs_AeuglCLDkI_h-7zzxbA8IS0KWTg5-yZjMzpxh1NWyzPAEFYFd8JFHkMI2bw2fgPERxlSFwQlbglzlRvmeCADlgS6_g12gF93nFd8gp5riR9OFg04";
        //    await GetArticleList(new DropboxClient(token));


        //    return View(listData);
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
