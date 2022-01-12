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

            string token = "sl.A_-b6-CkQgOwqnM71H5Ik79wl5XZ4JxW06VN8eL6Mm8bo6V3MLrm8CGgnLXf-DYi_I5WKtSM4JIK36y75wddpjWQcDufnOcc68p47F6N8GEh3tEgfMK9t-phhuEDZ-sQi6k_3Eo";
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
