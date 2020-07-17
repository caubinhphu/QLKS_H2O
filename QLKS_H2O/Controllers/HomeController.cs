using QLKS_H2O.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLKS_H2O.Controllers
{
    public class HomeController : Controller
    {
        QLKS_H2OEntities db = new QLKS_H2OEntities();
        // GET: Home
        public ActionResult Index()
        {
            var loaiPhongs = db.LOAIPHONGs.ToList().Select(lp =>
            {
                LoaiPhongGioiThieu loaiPhongGioiThieu = new LoaiPhongGioiThieu();
                loaiPhongGioiThieu.tenLP = lp.TEN_LOAIPHONG;
                loaiPhongGioiThieu.anh = lp.ANH;
                var giaPhongs = lp.PHONGs.Where(p => p.MA_TRANGTHAI != "HU").Select(p => (int)p.GIAPHONG);
                loaiPhongGioiThieu.giaMin = giaPhongs.Min();
                loaiPhongGioiThieu.giaMax = giaPhongs.Max();
                return loaiPhongGioiThieu;
            });
            return View(loaiPhongs.ToList());
        }
    }
}