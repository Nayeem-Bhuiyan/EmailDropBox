using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using DropBoxTest.Settings;
using DropBoxTest.Services;
using DropBoxTest.Areas.EmailInfo.Models;

namespace DropBoxTest.Areas.EmailInfo.Controllers
{
    [Area("EmailInfo")]
    public class EmailController : Controller
    {
        private readonly IMailService _mailService;
        public EmailController(IMailService mailService)
        {
            this._mailService = mailService;
        }


        [HttpGet]
        public IActionResult emailDataSend()
        {
            MailRequest model = new MailRequest();
           
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> emailDataSend([FromForm] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return Json("Send Email Successfully done");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
