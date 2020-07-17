using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLKS_H2O.Areas.Admin.Models
{
    public class PhongGroupModel
    {
        public string tang { get; set; }
        public List<PhongModel> phongs { get; set; }
    }
}