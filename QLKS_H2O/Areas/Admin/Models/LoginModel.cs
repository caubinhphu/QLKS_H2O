using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLKS_H2O.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Chưa nhập tài khoản")]
        [DisplayName("Mã nhân viên")]
        public string username { get; set; }

        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DisplayName("Mật khẩu")]
        public string passwrord { get; set; }

    }
}