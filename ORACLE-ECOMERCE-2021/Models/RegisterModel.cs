using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ORACLE_ECOMERCE_2021.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Làm ơn không để trống")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = "Nguyễn Văn Hữu";

        [Required(ErrorMessage = "Làm ơn không để trống")]
        [MinLength(3, ErrorMessage = "Tên đăng nhập phải từ 6 kí tự - 40 kí tự")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Làm ơn không để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 kí tự trở lên")]
        [DataType("Password")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = "123123123";

        [Required(ErrorMessage = "Làm ơn không để trống")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không trùng với mật khẩu")]
        [DataType("Password")]
        [Display(Name = "Xác nhận lại mật khẩu")]
        public string ConfirmPassword { get; set; } = "123123123";

        [Display(Name = "Tự động đăng nhập")]
        public bool RememberMe { get; set; } = true;

        [Required(ErrorMessage = "Làm ơn không để trống")]
        [MinLength(8, ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = "0867415712";
    }
}
