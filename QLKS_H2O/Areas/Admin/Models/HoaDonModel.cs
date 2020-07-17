using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLKS_H2O.Areas.Admin.Models
{
    public class HoaDonModel
    {
        public string maPhieu { set; get; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ngayDen { set; get; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ngayDi { set; get; }

        [DisplayFormat(DataFormatString = "{0,1:N1}", ApplyFormatInEditMode = true)]
        public decimal tienPhong { set; get; }

        [DisplayFormat(DataFormatString = "{0,1:N1}", ApplyFormatInEditMode = true)]
        public decimal tienDichVu { set; get; }

        [DisplayFormat(DataFormatString = "{0,1:N1}", ApplyFormatInEditMode = true)]
        public decimal tongTien { set; get; }
    }
}