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
            string token = "sl.A_8TrrTIdb5xcIFbbJjetCg6va35Xp_Eqv-aYrPQ5ZwMwgepwbAnkBHEqDhmI36Xcvm1R7j_odN5KjU1VeZ7kdG0wQd2KuIsAd3-ljwg903mGU6hsbMnzQ3fnRw7aeISqyfRpvg";
            using (var dbx = new DropboxClient(token))
            {
                var list = await dbx.Files.ListFolderAsync(string.Empty,true);
                var folders = list.Entries.Where(x=>x.IsFolder);
                foreach (var folder in folders)
                {
                    
                    FolderDetails data = new FolderDetails
                    {
                        folderName = folder.Name,
                        folderPath = folder.PathDisplay
                    };
                    listFolder.Add(data);
                }
            }
            return View(listFolder.);
        }


        //public async Task<IActionResult> DownloadFile()
        //{
        //    string token = "sl.A_8TrrTIdb5xcIFbbJjetCg6va35Xp_Eqv-aYrPQ5ZwMwgepwbAnkBHEqDhmI36Xcvm1R7j_odN5KjU1VeZ7kdG0wQd2KuIsAd3-ljwg903mGU6hsbMnzQ3fnRw7aeISqyfRpvg";
        //    var client = new DropboxClient(token);

        //    var Path = "/Homework/math/Prime_Numbers.txt";
        //    using (var response = await client.Files.DownloadAsync("/Homework/math/Prime_Numbers.txt"))
        //    {
        //        using (var fileStream = File.Create("Prime_Numbers.txt"))
        //        {
        //            (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
        //        }

        //    }

        //}


        public async Task<IActionResult> DropboxUserInfo()
        {
            string token = "sl.A_8TrrTIdb5xcIFbbJjetCg6va35Xp_Eqv-aYrPQ5ZwMwgepwbAnkBHEqDhmI36Xcvm1R7j_odN5KjU1VeZ7kdG0wQd2KuIsAd3-ljwg903mGU6hsbMnzQ3fnRw7aeISqyfRpvg";
            UserDetails data = new UserDetails();
            using (var dbx = new DropboxClient(token))
            {
                var id = await dbx.Users.GetCurrentAccountAsync();
                data = new UserDetails
                {
                    userName = id.Name.DisplayName,
                    email = id.Email,
                    country = id.Country,
                    profileImageUrl = id.ProfilePhotoUrl
                };
            }
            return View(data);
        }








        //private async Task<string> GetAccessToken()
        //{

        //    var ApiKey = "0s67ujhwzppd67w";

        //     var accessToken = Settings.Default.AccessToken;

        //    if (string.IsNullOrEmpty(accessToken))
        //    {
        //        try
        //        {
        //            Console.WriteLine("Waiting for credentials.");
        //            var state = Guid.NewGuid().ToString("N");
        //            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ApiKey, RedirectUri, state: state);
        //            var http = new HttpListener();
        //            http.Prefixes.Add(LoopbackHost);

        //            http.Start();

        //            System.Diagnostics.Process.Start(authorizeUri.ToString());

        //            // Handle OAuth redirect and send URL fragment to local server using JS.
        //            await HandleOAuth2Redirect(http);

        //            // Handle redirect from JS and process OAuth response.
        //            var result = await HandleJSRedirect(http);

        //            if (result.State != state)
        //            {
        //                // The state in the response doesn't match the state in the request.
        //                return null;
        //            }

        //            Console.WriteLine("and back...");

        //            // Bring console window to the front.
        //            SetForegroundWindow(GetConsoleWindow());

        //            accessToken = result.AccessToken;
        //            var uid = result.Uid;
        //            Console.WriteLine("Uid: {0}", uid);

        //            Settings.Default.AccessToken = accessToken;
        //            Settings.Default.Uid = uid;

        //            Settings.Default.Save();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Error: {0}", e.Message);
        //            return null;
        //        }
        //    }

        //    return accessToken;
        //}












    }
}
