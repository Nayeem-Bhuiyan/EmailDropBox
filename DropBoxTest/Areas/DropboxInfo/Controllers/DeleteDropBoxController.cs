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
        string token = "sl.BAdkrj24ieAEBrtdqTRkScMWRFFwmjeogbMy-Q5mM8ykcpKKKe982sXtQpfEWnpv2dHJbhhsYUGanmu-WG83IvS3_sGqbP44RvKTtKIrI6aTB6MDHGCHSRfj10ljAaIBE3eIc6g";
        public async Task<IActionResult> Delete()
        {
            await DeleteFileOrFolder();
            return View();
        }

        public async Task<dropboxApi.Files.DeleteResult> DeleteFileOrFolder()
        {
            string svcUri = "/Profile";
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
