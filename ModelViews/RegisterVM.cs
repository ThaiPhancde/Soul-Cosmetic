using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPhamSoul.ModelViews
{
    public class RegisterVM
    {
        [Key]
        public int AccountId { get; set; }
        [Display(Name = "Họ Và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
        public string TenKhachHang { get; set; }
        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        public DateTime NgaySinh { get; set; }
        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public Boolean GioiTinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }
        [MaxLength(11)]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string SoDienThoai { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string Password { get; set; }
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Vui lòng nhập mật khẩu giống nhau")]

        public string ConfirmPassword { get; set; }
    }
}
