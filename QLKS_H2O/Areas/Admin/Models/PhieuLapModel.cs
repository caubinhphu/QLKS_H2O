using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLKS_H2O.Areas.Admin.Models
{
    public class PhieuLapModel
    {
        [Required(ErrorMessage ="Chưa nhập tên khách")]
        public string tenkhach { get; set; }
        [Required(ErrorMessage = "Chưa nhập CMND/PASSPORT của khách")]
        public string cmnd { get; set; }
        [Required(ErrorMessage = "Chưa nhập ngày sinh của khách")]
        public string ngaysinh { get; set; }
        public string gioitinh { get; set; }
        [Required(ErrorMessage = "Chưa nhập điện thoại của khách")]
        public string dienthoai { get; set; }
        [Required(ErrorMessage = "Chưa nhập quốc tịch của khách")]
        public string quoctich { get; set; }
        [Required(ErrorMessage = "Chưa nhập ngày đi dự kiến")]
        public string ngaydi { get; set; }
        public string phong { get; set; }
        public string songuoi { get; set; }
        public string dichvu { get; set; }
        public string soluong { get; set; }
    }
}