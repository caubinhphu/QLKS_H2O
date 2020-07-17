using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLKS_H2O.Areas.Admin.Models
{
    public class DoanhThuPhongModel
    {
        public string maPhong { set; get; }

        public decimal doanhThu { set; get; }
    }
}