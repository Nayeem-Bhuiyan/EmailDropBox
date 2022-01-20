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
        string token = "sl.BAd1K5QvO5HZe1MPXqw76INFqzhaN0JkS7Gtk_OoxnSgQtDO9pM9jwBlkuv1cdWN39T4pjbYVtpUJTYy-iosjbOnCh6quBJOOzbABcDs9gqSy8iM4FbntRlngJZVIRznXzeVOrE";
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
