using Dropbox.Api;
using DropBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                var list = await dbx.Files.ListFolderAsync(string.Empty);
                var folders = list.Entries.Where(x => x.IsFolder);
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
            return View(listFolder);
        }

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



    }
}
