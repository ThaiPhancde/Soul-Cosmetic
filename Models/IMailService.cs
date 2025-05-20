using MyPhamSoul.Models;

namespace MyPhamSoul.Models
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }
}
