using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dropboxApi = global::Dropbox.Api;

namespace DropBoxTest.Areas.DropboxInfo.Controllers
{
    [Area("DropboxInfo")]
    public class DeleteDropBoxController : Controller
    {
        string token = "sl.BAeM9Acr4tzv5k-t4VUiRHeXKXwV2ADT6gzx2dI0gltMcJRyVHU4seLoxvwiFX0Az4YD7DpKM-iSUyeOgKEHBQ1r5A-Ul0odGzuAU0eXQ1bioLBKL04IjgzT6_aRVwH2yr6QevA";
        public async Task<IActionResult> Delete()
        {
            await DeleteFileOrFolder();
            return View();
        }

        public async Task<dropboxApi.Files.DeleteResult> DeleteFileOrFolder()
        {
            string svcUri = "/Apps/FootWear/images(1).jpg";
            try
            {
                dropboxApi.Files.DeleteResult result = null;

                using (var client = new dropboxApi.DropboxClient(token))
                {
                    result = await client.Files.DeleteV2Async(svcUri);
                    
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
