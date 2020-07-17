using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLKS_H2O.Areas.Admin.Models
{
    public class DoanhThuModel
    {
        public List<DoanhThuPhongModel> doanhThuPhongs { set; get; }

        public decimal doanhThuDichVu { set; get; }

        public DoanhThuModel()
        {
            doanhThuPhongs = new List<DoanhThuPhongModel>();
            doanhThuDichVu = 0;
        }

    }
}