using DropBoxTest.Areas.EmailInfo.Models;
using System.Threading.Tasks;

namespace DropBoxTest.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

    }
}
