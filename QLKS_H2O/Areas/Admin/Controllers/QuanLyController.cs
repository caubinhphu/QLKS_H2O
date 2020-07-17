using QLKS_H2O.Areas.Admin.Models;
using QLKS_H2O.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QLKS_H2O.Areas.Admin.Controllers
{
    public class QuanLyController : BaseController
    {
        private QLKS_H2OEntities db = new QLKS_H2OEntities();

        // GET: LOAIPHONGs
        public ActionResult LoaiPhongIndex(string maLP = "", string tenLP = "")
        {
            ViewBag.tag = "loaiphong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.maLP = maLP;
            ViewBag.tenLP = tenLP;
            return View(
                db.LOAIPHONGs
                    .Where(lp => (lp.MA_LOAIPHONG == maLP || maLP == "") && 
                        (lp.TEN_LOAIPHONG.Contains(tenLP) || tenLP == ""))
            );
        }

        // GET: LOAIPHONGs/Details/5
        //public ActionResult LoaiPhongDetails(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LOAIPHONG lOAIPHONG = db.LOAIPHONGs.Find(id);
        //    if (lOAIPHONG == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(lOAIPHONG);
        //}

        // GET: LOAIPHONGs/Create
        public ActionResult LoaiPhongCreate()
        {
            ViewBag.tag = "loaiphong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        // POST: LOAIPHONGs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoaiPhongCreate([Bind(Include = "MA_LOAIPHONG,TEN_LOAIPHONG,KICHTHUOC,MAMAU,ANH")] LOAIPHONG lOAIPHONG)
        {
            var img = Request.Files["ANH"];
            string postedFileName = System.IO.Path.GetFileName(img.FileName);
            var path = Server.MapPath("/Images/" + postedFileName);
            img.SaveAs(path);

            if (ModelState.IsValid)
            {
                int count = db.LOAIPHONGs.Where(phong => phong.MA_LOAIPHONG == lOAIPHONG.MA_LOAIPHONG).Count();

                if (count == 0)
                {
                    lOAIPHONG.ANH = postedFileName;
                    db.LOAIPHONGs.Add(lOAIPHONG);
                    db.SaveChanges();
                    return RedirectToAction("LoaiPhongIndex");
                }
                else
                {
                    ModelState.AddModelError("", "Loại phòng đã tồn tại");
                }
            }
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.tag = "loaiphong";
            return View(lOAIPHONG);
        }

        // GET: LOAIPHONGs/Edit/5
        public ActionResult LoaiPhongEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAIPHONG lOAIPHONG = db.LOAIPHONGs.Find(id);
            if (lOAIPHONG == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "loaiphong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(lOAIPHONG);
        }

        // POST: LOAIPHONGs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoaiPhongEdit([Bind(Include = "MA_LOAIPHONG,TEN_LOAIPHONG,KICHTHUOC,MAMAU,ANH")] LOAIPHONG lOAIPHONG)
        {
            if (ModelState.IsValid)
            {
                var img = Request.Files["ANH"];
                if (img != null)
                {
                    string postedFileName = System.IO.Path.GetFileName(img.FileName);
                    var path = Server.MapPath("/Images/" + postedFileName);
                    img.SaveAs(path);
                    lOAIPHONG.ANH = postedFileName;
                }
                
                db.Entry(lOAIPHONG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LoaiPhongIndex");
            }
            ViewBag.tag = "loaiphong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(lOAIPHONG);
        }

        // GET: LOAIPHONGs/Delete/5
        public ActionResult LoaiPhongDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAIPHONG lOAIPHONG = db.LOAIPHONGs.Find(id);
            if (lOAIPHONG == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "loaiphong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(lOAIPHONG);
        }

        // POST: LOAIPHONGs/Delete/5
        [HttpPost, ActionName("LoaiPhongDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LoaiPhongDeleteConfirmed(string id)
        {
            LOAIPHONG lOAIPHONG = db.LOAIPHONGs.Find(id);
            db.LOAIPHONGs.Remove(lOAIPHONG);
            db.SaveChanges();
            return RedirectToAction("LoaiPhongIndex");
        }


        // Phòng
        // GET: PHONG_QL
        public ActionResult PhongIndex(string maP = "")
        {
            var pHONGs = db.PHONGs.Include(p => p.LOAIPHONG).Include(p => p.TRANGTHAI_PHONG);
            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.maP = maP;
            return View(pHONGs.Where(p => p.MA_PHONG == maP || maP == "").ToList());
        }

        // GET: PHONG_QL/Details/5
        //public ActionResult PhongDetails(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PHONG pHONG = db.PHONGs.Find(id);
        //    if (pHONG == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(pHONG);
        //}

        // GET: PHONG_QL/Create
        public ActionResult PhongCreate()
        {
            ViewBag.MA_LOAIPHONG = new SelectList(db.LOAIPHONGs, "MA_LOAIPHONG", "TEN_LOAIPHONG");
            ViewBag.MA_TRANGTHAI = new SelectList(db.TRANGTHAI_PHONG, "MA_TRANGTHAI", "TEN_TRANGTHAI");
            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        // POST: PHONG_QL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhongCreate([Bind(Include = "MA_PHONG,MA_LOAIPHONG,GIAPHONG,MA_TRANGTHAI")] PHONG pHONG)
        {
            if (ModelState.IsValid)
            {
                int count = db.PHONGs.Where(phong => phong.MA_PHONG == pHONG.MA_PHONG).Count();

                if (count == 0)
                {
                    db.PHONGs.Add(pHONG);
                    db.SaveChanges();
                    return RedirectToAction("PhongIndex");
                }
                else
                {
                    ModelState.AddModelError("", "Phòng đã tồn tại");
                }
            }

            ViewBag.MA_LOAIPHONG = new SelectList(db.LOAIPHONGs, "MA_LOAIPHONG", "TEN_LOAIPHONG", pHONG.MA_LOAIPHONG);
            ViewBag.MA_TRANGTHAI = new SelectList(db.TRANGTHAI_PHONG, "MA_TRANGTHAI", "TEN_TRANGTHAI", pHONG.MA_TRANGTHAI);
            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(pHONG);
        }

        // GET: PHONG_QL/Edit/5
        public ActionResult PhongEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONG pHONG = db.PHONGs.Find(id);
            if (pHONG == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_LOAIPHONG = new SelectList(db.LOAIPHONGs, "MA_LOAIPHONG", "TEN_LOAIPHONG", pHONG.MA_LOAIPHONG);
            ViewBag.MA_TRANGTHAI = new SelectList(db.TRANGTHAI_PHONG, "MA_TRANGTHAI", "TEN_TRANGTHAI", pHONG.MA_TRANGTHAI);
            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(pHONG);
        }

        // POST: PHONG_QL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhongEdit([Bind(Include = "MA_PHONG,MA_LOAIPHONG,GIAPHONG,MA_TRANGTHAI")] PHONG pHONG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pHONG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PhongIndex");
            }
            ViewBag.MA_LOAIPHONG = new SelectList(db.LOAIPHONGs, "MA_LOAIPHONG", "TEN_LOAIPHONG", pHONG.MA_LOAIPHONG);
            ViewBag.MA_TRANGTHAI = new SelectList(db.TRANGTHAI_PHONG, "MA_TRANGTHAI", "TEN_TRANGTHAI", pHONG.MA_TRANGTHAI);
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(pHONG);
        }

        // GET: PHONG_QL/Delete/5
        public ActionResult PhongDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONG pHONG = db.PHONGs.Find(id);
            if (pHONG == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "phong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.tag = "phong";
            return View(pHONG);
        }

        // POST: PHONG_QL/Delete/5
        [HttpPost, ActionName("PhongDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PhongDeleteConfirmed(string id)
        {
            PHONG pHONG = db.PHONGs.Find(id);
            db.PHONGs.Remove(pHONG);
            db.SaveChanges();
            return RedirectToAction("PhongIndex");
        }


        // Dịch vụ
        // GET: DICHVU_QL
        public ActionResult DichVuIndex(string maDV = "", string tenDV = "")
        {
            ViewBag.tag = "dichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.maDV = maDV;
            ViewBag.tenDV = tenDV;
            return View(db.DICHVUs.Where(dv => (dv.MA_DICHVU == maDV || maDV == "") && (dv.TEN_DICHVU.Contains(tenDV) || tenDV == "")).ToList());
        }

        // GET: DICHVU_QL/Details/5
        //public ActionResult DichVuDetails(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DICHVU dICHVU = db.DICHVUs.Find(id);
        //    if (dICHVU == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dICHVU);
        //}

        // GET: DICHVU_QL/Create
        public ActionResult DichVuCreate()
        {
            ViewBag.tag = "dichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        // POST: DICHVU_QL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DichVuCreate([Bind(Include = "MA_DICHVU,TEN_DICHVU,GIA_DICHVU")] DICHVU dICHVU)
        {
            if (ModelState.IsValid)
            {
                int count = db.DICHVUs.Where(dv => dv.MA_DICHVU == dICHVU.MA_DICHVU).Count();
                if (count == 0)
                {
                    db.DICHVUs.Add(dICHVU);
                    db.SaveChanges();
                    return RedirectToAction("DichVuIndex");
                }
                else
                {
                    ModelState.AddModelError("", "Dịch vụ đã tồn tại");
                }
            }
            ViewBag.tag = "dichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(dICHVU);
        }

        // GET: DICHVU_QL/Edit/5
        public ActionResult DichVuEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DICHVU dICHVU = db.DICHVUs.Find(id);
            if (dICHVU == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "dichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(dICHVU);
        }

        // POST: DICHVU_QL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DichVuEdit([Bind(Include = "MA_DICHVU,TEN_DICHVU,GIA_DICHVU")] DICHVU dICHVU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dICHVU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DichVuIndex");
            }
            ViewBag.tag = "dichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(dICHVU);
        }

        // GET: DICHVU_QL/Delete/5
        public ActionResult DichVuDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DICHVU dICHVU = db.DICHVUs.Find(id);
            if (dICHVU == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "dichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(dICHVU);
        }

        // POST: DICHVU_QL/Delete/5
        [HttpPost, ActionName("DichVuDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DichVuDeleteConfirmed(string id)
        {
            DICHVU dICHVU = db.DICHVUs.Find(id);
            db.DICHVUs.Remove(dICHVU);
            db.SaveChanges();
            return RedirectToAction("DichVuIndex");
        }

        // thống ke thuê phòng
        // GET: ThongKe_QL
        public ActionResult ThongKeThuePhong()
        {
            var thongKe = db.Database.SqlQuery<THONGKE_PHONG_Result>("exec THONGKE_PHONG").ToList();
            ViewBag.tag = "tkphong";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(thongKe);
        }


        // thống kê thuê dịch vụ 
        // GET: ThongKe_DicVu
        public ActionResult ThongKeThueDichVu()
        {
            var thongKe = db.Database.SqlQuery<THONGKE_DICHVU_Result>("exec THONGKE_DICHVU").ToList();
            ViewBag.tag = "tkdichvu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(thongKe);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}