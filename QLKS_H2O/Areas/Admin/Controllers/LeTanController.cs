using QLKS_H2O.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLKS_H2O.Areas.Admin.Models;
using System.Net;
using System.Data.Entity;
using SelectPdf;
using Microsoft.Ajax.Utilities;

namespace QLKS_H2O.Areas.Admin.Controllers
{
    public class LeTanController : BaseController
    {
        private QLKS_H2OEntities db = new QLKS_H2OEntities();

        // GET: Admin/LeTan
        // Danh sách phòng + tìm kiếm phòng theo tiêu chí
        public ActionResult Index(string maphong = "", string tang = "", string trangthai = "", string loaiphong = "",
            double giatu = 0, double giaden = Double.MaxValue)
        {
            char soTang = tang != "" ? tang.ToCharArray()[0] : '_';
            List<PhongModel> phongs = db.PHONGs
                .Where(phong => (phong.MA_PHONG == maphong || maphong == "")
                            && (phong.MA_TRANGTHAI == trangthai || trangthai == "")
                            && (phong.MA_LOAIPHONG == loaiphong || loaiphong == "")
                            && ((double)phong.GIAPHONG >= giatu)
                            && ((double)phong.GIAPHONG <= giaden)
                            && (phong.MA_PHONG.Substring(0, 1) == tang || tang == "")
                            )
                .Select(phong => new PhongModel()
                {
                    maPhong = phong.MA_PHONG,
                    mauLoaiPhong = phong.LOAIPHONG.MAMAU,
                    mauTrangThai = phong.TRANGTHAI_PHONG.MAMAU,
                    giaPhong = (decimal)phong.GIAPHONG
                })
                .ToList();

            var groupPhongs = phongs.Aggregate(new List<PhongGroupModel>(), (acc, cur) =>
            {
                PhongGroupModel t = acc.Find(group => group.tang == cur.maPhong[0].ToString());
                if (t == null)
                {
                    acc.Add(new PhongGroupModel()
                    {
                        tang = cur.maPhong[0].ToString(),
                        phongs = new List<PhongModel>() { cur }
                    });
                }
                else
                {
                    t.phongs.Add(cur);
                }
                return acc;
            });

            List<LOAIPHONG> loaiPhongs = db.LOAIPHONGs.ToList();
            ViewBag.loaiPhongs = loaiPhongs;

            List<TRANGTHAI_PHONG> trangThais = db.TRANGTHAI_PHONG.ToList();
            ViewBag.trangThais = trangThais;

            ViewBag.maphong = maphong;
            ViewBag.tang = tang;
            ViewBag.trangthai = trangthai;
            ViewBag.loaiphong = loaiphong;
            ViewBag.giatu = giatu;
            ViewBag.giaden = giaden;

            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(groupPhongs);
        }

        // GET: /Admin/LaTan/Phong/:idPhong
        // Hiển thị thông tin chi tiết của 1 phòng
        public ActionResult Phong(string id)
        {
            PHONG phong = db.PHONGs.Find(id);
            var phieus = db.CHITIET_THUEPHONG.Where(ct => ct.MAPHONG == id && ct.PHIEU_THUEPHONG.DATRAPHONG == false).ToList();
            if (phieus.Count() > 0)
            {
                ViewBag.phieuThue = phieus[0].PHIEU_THUEPHONG;
            }
            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(phong);
        }

        // GET: /Admin/LaTan/DanhSachPhieuThue
        // Danh sách phiếu thuê phòng
        public ActionResult DanhSachPhieuThue(string soPhieu = "", string tenK = "")
        {
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.soPhieu = soPhieu;
            ViewBag.tenK = tenK;
            return View(
                db.PHIEU_THUEPHONG
                    .Where(ph => (ph.SO_PHIEU == soPhieu || soPhieu == "") && 
                        (ph.KHACH.HOTEN_KHACH.Contains(tenK) || tenK == ""))
                    .OrderBy(phieu => phieu.DATRAPHONG)
                    .ThenByDescending(phieu => phieu.NGAYDEN)
            );
        }

        // GET: /Admin/LaTan/ThemPhieuThuePhong
        // View thuê phòng
        public ActionResult ThemPhieuThuePhong()
        {
            ViewBag.phong = db.PHONGs.Where(phong => phong.MA_TRANGTHAI == "VC").ToList();
            ViewBag.dichVus = new SelectList(db.DICHVUs, "MA_DICHVU", "TEN_DICHVU");

            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        // POST: /Admin/LaTan/ThemPhieuThuePhong
        // thuê phòng
        [HttpPost]
        public ActionResult ThemPhieuThuePhong(FormCollection form)
        {
            string tenKhach = form["tenkhach"];
            string cmnd = form["cmnd"];
            string ngaySinh = form["ngaysinh"];
            string gioiTinh = form["gioitinh"];
            string dienThoai = form["dienthoai"];
            string quocTich = form["quoctich"];
            string ngayDi = form["ngaydi"];
            string phongs = form["phong"];
            string soNguois = form["songuoi"];
            string dichVus = form["dichvu"];
            string soLuongs = form["soluong"];

            string maKhach = Math.Round(((new Random()).NextDouble() * 1e9)).ToString().PadLeft(9, '0');
                string soPhieu = Math.Round(((new Random()).NextDouble() * 1e9)).ToString().PadLeft(9, '0');

                List<String> phongList = phongs.Split(new[] { ',' }).ToList();
                List<String> soNguoiList = soNguois.Split(new[] { ',' }).ToList();
                List<String> dichVuList = dichVus != null ? dichVus.Split(new[] { ',' }).ToList() : new List<string>();
                List<String> soLuongList = soLuongs != null ? soLuongs.Split(new[] { ',' }).ToList() : new List<string>();

                PHIEU_THUEPHONG phieu = new PHIEU_THUEPHONG()
                {
                    SO_PHIEU = soPhieu,
                    MAKHACH = maKhach,
                    KHACH = new KHACH()
                    {
                        MA_KHACH = maKhach,
                        CMND = cmnd,
                        DIENTHOAI = dienThoai,
                        HOTEN_KHACH = tenKhach,
                        QUOCTICH = quocTich,
                        GIOITINH = gioiTinh == "1" ? true : false,
                        NGAYSINH = DateTime.Parse(ngaySinh)
                    },
                    DATRAPHONG = false,
                    NGAYDEN = DateTime.Now,
                    NGAYLAP_PHIEU = DateTime.Now,
                    NGAYDI = DateTime.Parse(ngayDi),
                    MA_NHANVIEN = ((LoginSessionModel)Session["session"]).username,
                    CHITIET_THUEPHONG = phongList.Select((p, index) => new CHITIET_THUEPHONG()
                    {
                        SO_PHIEU = soPhieu,
                        MAPHONG = p,
                        SONGUOI = Byte.Parse(soNguoiList[index]),
                        GIAPHONG = db.PHONGs.Find(p).GIAPHONG,
                    }).ToList(),
                    CHITIET_THUEDICHVU = dichVuList.Select((dv, index) => new CHITIET_THUEDICHVU()
                    {
                        SO_PHIEU = soPhieu,
                        MA_DICHVU = dv,
                        SOLUONG = Byte.Parse(soLuongList[index]),
                        GIA_DICHVU = db.DICHVUs.Find(dv).GIA_DICHVU,
                    }).ToList(),
                };

                db.PHIEU_THUEPHONG.Add(phieu);

                phongList.ForEach(p => db.PHONGs.Find(p).MA_TRANGTHAI = "OC");

                db.SaveChanges();

                return RedirectToAction("DanhSachPhieuThue");
            

            //ViewBag.phong = db.PHONGs.Where(p => p.MA_TRANGTHAI == "VC").ToList();
            //ViewBag.dichVus = new SelectList(db.DICHVUs, "MA_DICHVU", "TEN_DICHVU");

            //ViewBag.tag = "thuephong";
            //ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            //return View();
        }

        // GET: /Admin/LeTan/PhieuThuePhong/:id
        // Xem thông tin phiếu thuê phòng
        public ActionResult PhieuThuePhong(string id)
        {
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(db.PHIEU_THUEPHONG.Find(id));
        }

        // GET: /Admin/LeTan/XoaPhieuThuePhong/:id
        // Xác nhận xóa phiếu thuê phòng
        public ActionResult XoaPhieuThuePhong(string id)
        {
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(db.PHIEU_THUEPHONG.Find(id));
        }

        // POST: /Admin/LeTan/XoaPhieuThuePhong/:id
        [HttpPost, ActionName("XoaPhieuThuePhong")]
        public ActionResult XoaPhieuThuePhongConfirmed(string id)
        {
            PHIEU_THUEPHONG phieu = db.PHIEU_THUEPHONG.Find(id);
            db.PHIEU_THUEPHONG.Remove(phieu);
            db.SaveChanges();
            return RedirectToAction("DanhSachPhieuThue");
        }

        // GET: /Admin/LeTan/SuaPhieuThuePhong/:id
        // View cập nhật phiếu thuê phòng
        public ActionResult SuaPhieuThuePhong(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHIEU_THUEPHONG phieu = db.PHIEU_THUEPHONG.Find(id);
            if (phieu == null)
            {
                return HttpNotFound();
            }

            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(phieu);
        }

        // POST: /Admin/LeTan/SuaPhieuThuePhong
        // Cập nhật phiếu thuê phòng
        [HttpPost]
        public ActionResult SuaPhieuThuePhong([Bind(Include = "SO_PHIEU,MAKHACH,MA_NHANVIEN,NGAYLAP_PHIEU,NGAYDEN,NGAYDI,DATRAPHONG")] PHIEU_THUEPHONG phieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachPhieuThue");
            }
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(phieu);
        }

        // GET: /Admin/LeTan/SuaKhachHang/:id
        // View cập nhật khách hàng
        public ActionResult SuaKhachHang(string id)
        {
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(db.KHACHes.Find(id));
        }

        // POST: /Admin/LeTan/SuaKhachHang/:id
        // Cập nhật khách hàng
        [HttpPost]
        public ActionResult SuaKhachHang([Bind(Include = "MA_KHACH,HOTEN_KHACH,CMND,DIENTHOAI,QUOCTICH,GIOITINH,NGAYSINH")] KHACH khach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachPhieuThue");
            }
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(khach);
        }

        // GET: /Admin/LeTan/ThemDichVu/:id
        // View thêm dịch vụ
        public ActionResult ThemDichVu(string id)
        {
            ViewBag.MA_DICHVU = new SelectList(db.DICHVUs, "MA_DICHVU", "TEN_DICHVU");
            ViewBag.id = id;
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        // POST: /Admin/LeTan/ThemDichVu/:id
        // Thêm dịch vụ vào phiếu
        [HttpPost]
        public ActionResult ThemDichVu([Bind(Include = "SO_PHIEU,MA_DICHVU,SOLUONG")] CHITIET_THUEDICHVU chiTiet)
        {
            if (ModelState.IsValid)
            {
                var ct = db.CHITIET_THUEDICHVU.Where(c => c.SO_PHIEU == chiTiet.SO_PHIEU && c.MA_DICHVU == chiTiet.MA_DICHVU);
                if (ct.Count() > 0)
                {
                    // nếu đã có dịch vụ thì cập nhật số lượng
                    ct.First().SOLUONG += chiTiet.SOLUONG;
                }
                else
                {
                    // chưa có thì thêm mới vào chi tiết
                    chiTiet.GIA_DICHVU = db.DICHVUs.Find(chiTiet.MA_DICHVU).GIA_DICHVU;
                    db.CHITIET_THUEDICHVU.Add(chiTiet);
                }
                db.SaveChanges();
                return RedirectToAction("DanhSachPhieuThue");
            }

            ViewBag.MA_DICHVU = new SelectList(db.DICHVUs, "MA_DICHVU", "TEN_DICHVU");
            ViewBag.id = chiTiet.SO_PHIEU;
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        public ActionResult TraPhong(string id)
        {
            var phieu = db.PHIEU_THUEPHONG.Find(id);
            phieu.NGAYDI = DateTime.Now;
            phieu.DATRAPHONG = true;
            phieu.CHITIET_THUEPHONG.ToList().ForEach(ct => ct.PHONG.MA_TRANGTHAI = "VD");
            db.SaveChanges();
            ViewBag.tag = "thuephong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(phieu);
        }

        public ActionResult CreateHoaDon(string id)
        {
            return View(db.PHIEU_THUEPHONG.Find(id));
        }

        public ActionResult InHoaDon(string id)
        {
            HtmlToPdf converter = new HtmlToPdf();

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl("https://localhost:44359/Admin/Letan/CreateHoaDon/" + id);
            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "HD" + id + ".pdf";
            return fileResult;
        }
    }
}