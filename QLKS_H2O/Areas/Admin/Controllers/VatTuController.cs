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
    public class VatTuController : BaseController
    {
        private QLKS_H2OEntities db = new QLKS_H2OEntities();

        // GET: VatTu
        public ActionResult Index(string maVT = "", string tenVT = "")
        {
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.maVT = maVT;
            ViewBag.tenVT = tenVT;
            return View(db.VATTUs.Where(vt => (vt.MA_VATTU == maVT || maVT == "") && (vt.TEN_VATTU.Contains(tenVT) || tenVT == "")).ToList());
        }

        // GET: VatTu/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VATTU vATTU = db.VATTUs.Find(id);
            if (vATTU == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(vATTU);
        }

        // GET: VatTu/Create
        public ActionResult Create()
        {
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View();
        }

        // POST: VatTu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MA_VATTU,TEN_VATTU")] VATTU vATTU)
        {
            if (ModelState.IsValid)
            {
                int count = db.VATTUs.Where(dv => dv.MA_VATTU == vATTU.MA_VATTU).Count();
                if (count == 0)
                {
                    db.VATTUs.Add(vATTU);

                    db.LOAIPHONGs.ToList().ForEach(lp =>
                    {
                        db.VATTU_LOAIPHONG.Add(new VATTU_LOAIPHONG()
                        {
                            MA_LOAIPHONG = lp.MA_LOAIPHONG,
                            MA_VATTU = vATTU.MA_VATTU,
                            SOLUONG = 0
                        });
                    });

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Vật tư đã tồn tại");
                }
            }
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(vATTU);
        }

        // GET: VatTu/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VATTU vATTU = db.VATTUs.Find(id);
            if (vATTU == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(vATTU);
        }

        // POST: VatTu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MA_VATTU,TEN_VATTU")] VATTU vATTU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vATTU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(vATTU);
        }

        // GET: VatTu/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VATTU vATTU = db.VATTUs.Find(id);
            if (vATTU == null)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "vattu";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(vATTU);
        }

        // POST: VatTu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            VATTU vATTU = db.VATTUs.Find(id);
            db.VATTUs.Remove(vATTU);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ThongKeVatTu(string loaiPhong = "abcxyz")
        {
            var thongKe = db.VATTU_LOAIPHONG.Where(ct => ct.MA_LOAIPHONG == loaiPhong).ToList();

            ViewBag.loaiPhong = new SelectList(db.LOAIPHONGs, "MA_LOAIPHONG", "TEN_LOAIPHONG", loaiPhong);

            ViewBag.tag = "thongke";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(thongKe);
        }

        public ActionResult EditChiTiet(string lp, string vt)
        {
            if (lp == null || vt == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chiTiets = db.VATTU_LOAIPHONG.Where(ct => ct.MA_LOAIPHONG == lp && ct.MA_VATTU == vt);
            if (chiTiets.Count() <= 0)
            {
                return HttpNotFound();
            }
            ViewBag.tag = "thongke";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(chiTiets.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChiTiet([Bind(Include = "MA_VATTU,MA_LOAIPHONG,SOLUONG")] VATTU_LOAIPHONG ct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ThongKeVatTu", new { loaiPhong = ct.MA_LOAIPHONG });
            }

            return View(ct);
        }

        public ActionResult TrangThaiPhong(string maP = "")
        {
            ViewBag.tag = "trangthai";
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(db.PHONGs.Where(p => p.MA_PHONG == maP || maP == "").ToList());
        }

        public ActionResult CapNhatTrangThaiPhong(string maP, string maTT)
        {
            ViewBag.maP = maP;
            ViewBag.maTT = new SelectList(db.TRANGTHAI_PHONG, "MA_TRANGTHAI", "TEN_TRANGTHAI", maTT);
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            ViewBag.tag = "trangthai";
            return View();
        }

        [HttpPost]
        public ActionResult CapNhatTrangThaiPhong()
        {
            string maP = Request["maP"];
            string maTT = Request["maTT"];

            db.PHONGs.Find(maP).MA_TRANGTHAI = maTT;
            db.SaveChanges();

            return RedirectToAction("TrangThaiPhong");
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