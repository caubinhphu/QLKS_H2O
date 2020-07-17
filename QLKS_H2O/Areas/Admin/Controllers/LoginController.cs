using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using QLKS_H2O.Areas.Admin.Models;
using QLKS_H2O.Models;

namespace QLKS_H2O.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private QLKS_H2OEntities db = new QLKS_H2OEntities();
        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                // check login
                var user = db.NHANVIENs.Find(login.username);
                if (user != null)
                {
                    if (user.PASSWORD == login.passwrord)
                    {
                        LoginSessionModel session = new LoginSessionModel();
                        session.username = user.MA_NHANVIEN;
                        session.name = user.HOTEN_NHANVIEN;

                        Session.Add("session", session);

                        switch(user.BOPHAN)
                        {
                            case "LỄ TÂN": return RedirectToAction("Index", "LeTan");
                            case "KẾ TOÁN": return RedirectToAction("Index", "KeToan");
                            case "QUẢN LÝ": return RedirectToAction("ThongKeThuePhong", "QuanLy");
                            case "VẬT TƯ": return RedirectToAction("Index", "VatTu");
                        }
                    } else
                    {
                        ModelState.AddModelError("", "Mật khẩu không đúng");
                    }
                } else
                {
                    ModelState.AddModelError("", "Nhân viên không tồn tại");
                }


            }
            return View(login);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}