using System.ComponentModel;

namespace MyPhamSoul.Models
{
    public class MailData
    {
        [DisplayName("Địa chỉ email người nhận")]
        public string ReceiverEmail { get; set; }
        [DisplayName("Tên người nhận")]
        public string ReceiverName { get; set; }
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [DisplayName("Nội dung")]
        public string Body { get; set; }

    }
}
