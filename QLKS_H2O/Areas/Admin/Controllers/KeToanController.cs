using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QLKS_H2O.Areas.Admin.Models;
using QLKS_H2O.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QLKS_H2O.Areas.Admin.Controllers
{
    public class KeToanController : BaseController
    {
        private QLKS_H2OEntities db = new QLKS_H2OEntities();

        private List<HoaDonModel> FindHoaDon(string billid, string datecreate, double summin, double summax)
        {
            var phieuThues = db.PHIEU_THUEPHONG
                .ToList()
                .Where(phieu => (phieu.DATRAPHONG == true) &&
                    (phieu.SO_PHIEU == billid || billid == "")
                    &&
                    (datecreate == "" || phieu.NGAYDI.CompareTo(DateTime.Parse(datecreate)) == 0)
                );

            List<HoaDonModel> hoaDons = new List<HoaDonModel>();

            phieuThues.ForEach(phieu =>
            {
                HoaDonModel hoaDon = new HoaDonModel();
                hoaDon.maPhieu = phieu.SO_PHIEU;
                hoaDon.ngayDen = phieu.NGAYDEN;
                hoaDon.ngayDi = phieu.NGAYDI;

                decimal tp = 0;
                double soNgay = (phieu.NGAYDI - phieu.NGAYDEN).Ticks / 864e9;

                phieu.CHITIET_THUEPHONG.ForEach(ct =>
                {
                    tp += (decimal)soNgay * (decimal)ct.GIAPHONG;
                });

                hoaDon.tienPhong = tp;

                decimal tdv = 0;
                phieu.CHITIET_THUEDICHVU.ForEach(ct =>
                {
                    tdv += ct.SOLUONG * (decimal)ct.GIA_DICHVU;
                });

                hoaDon.tienDichVu = tdv;

                hoaDon.tongTien = tp + tdv;

                hoaDons.Add(hoaDon);
            });

            return hoaDons
                .AsEnumerable()
                .Where(hoaDon => (hoaDon.tongTien >= (decimal)summin) &&
                    ((double)hoaDon.tongTien <= summax)
                ).ToList();
        }

        // GET: Admin/KeToan
        public ActionResult Index(string billid = "", string datecreate = "", double summin = 0, double summax = Double.MaxValue)
        {
            List<HoaDonModel> hoaDons = FindHoaDon(billid, datecreate, summin, summax);

            ViewBag.billid = billid;
            ViewBag.summin = summin;
            ViewBag.summax = summax;
            ViewBag.datecreate = datecreate;
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(hoaDons);
        }

        public void DownloadHoaDon(string billid = "", string datecreate = "", double summin = 0, double summax = Double.MaxValue)
        {
            List<HoaDonModel> hoaDons = FindHoaDon(billid, datecreate, summin, summax);

            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("HoaDon");

            for(int i = 1; i <= 7; i++)
            {
                sheet.Column(i).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
            sheet.Column(5).Style.Numberformat.Format = "#,##0.00";
            sheet.Column(6).Style.Numberformat.Format = "#,##0.00";
            sheet.Column(7).Style.Numberformat.Format = "#,##0.00";

            sheet.Row(1).Style.Font.Bold = true;
            sheet.Cells["A1:G1"].Merge = true;
            sheet.Cells["A1"].Value = "DANH SÁCH HÓA ĐƠN";

            sheet.Cells["A2"].Value = "STT";
            sheet.Cells["B2"].Value = "Mã phiếu";
            sheet.Cells["C2"].Value = "Ngày đến";
            sheet.Cells["D2"].Value = "Ngày đi";
            sheet.Cells["E2"].Value = "Tổng tiền phòng";
            sheet.Cells["F2"].Value = "Tổng tiền dịch vụ";
            sheet.Cells["G2"].Value = "Tổng tiền";
            sheet.Row(2).Style.Font.Bold = true;

            int row = 3;
            hoaDons.ForEach(hoaDon =>
            {
                sheet.Cells[$"A{row}"].Value = row - 2;
                sheet.Cells[$"B{row}"].Value = hoaDon.maPhieu;
                sheet.Cells[$"C{row}"].Value = hoaDon.ngayDen.ToString("dd/MM/yyyy");
                sheet.Cells[$"D{row}"].Value = hoaDon.ngayDi.ToString("dd/MM/yyyy");
                sheet.Cells[$"E{row}"].Value = hoaDon.tienPhong;
                sheet.Cells[$"F{row}"].Value = hoaDon.tienDichVu;
                sheet.Cells[$"G{row}"].Value = hoaDon.tongTien;
                row++;
            });

            sheet.Row(row).Style.Font.Bold = true;
            sheet.Cells[$"A{row}:D{row}"].Merge = true;
            sheet.Cells[$"A{row}"].Value = "Tổng cộng";
            sheet.Cells[$"E{row}"].Formula = $"SUM(E{3}:E{row - 1})";
            sheet.Cells[$"F{row}"].Formula = $"SUM(F{3}:F{row - 1})";
            sheet.Cells[$"G{row}"].Formula = $"SUM(G{3}:G{row - 1})";


            var range = sheet.Cells[$"A1:G{row}"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[$"A1:G{row}"].AutoFitColumns();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();
        }

        public ActionResult HoaDon(string id)
        {
            var phieuThue = db.PHIEU_THUEPHONG.Find(id);
            return View(phieuThue);
        }

        private DoanhThuModel CalDoanhThuNgay(string date)
        {
            DoanhThuModel doanhThu = new DoanhThuModel();

            // Tính doanh thu của từng phòng
            // Khởi tạo doanh thu = 0 cho từng phòng
            var phongs = db.PHONGs.ToList();
            List<DoanhThuPhongModel> doanhThuPhongs = new List<DoanhThuPhongModel>();
            phongs.ForEach(phong =>
            {
                DoanhThuPhongModel doanhThuPhong = new DoanhThuPhongModel();
                doanhThuPhong.maPhong = phong.MA_PHONG;
                doanhThuPhong.doanhThu = 0;
                doanhThuPhongs.Add(doanhThuPhong);
            });

            // chuyển ngày cần tính daonh thu sang DateTime
            DateTime dateNeed = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            // Lọc các phiếu thêu phòng thỏa ngày cần tính doanh thu
            // -> cộng doanh thu cho phòng tương ứng khi được thuê (có trong bảng CT thuê phòng)
            var phieuThuePhongs = db.PHIEU_THUEPHONG.ToList();
            phieuThuePhongs
                .Where(phieu => {
                    return DateTime.Compare(phieu.NGAYDEN, dateNeed) <= 0 && DateTime.Compare(phieu.NGAYDI, dateNeed) >= 0;
                })
                .ToList()
                .ForEach(phieu => {
                    var ctPhieu = phieu.CHITIET_THUEPHONG.ToList();
                    ctPhieu.ForEach(ct =>
                    {
                        doanhThuPhongs.Find(dt => dt.maPhong == ct.MAPHONG).doanhThu += (decimal)ct.GIAPHONG;
                    });
                });
            doanhThu.doanhThuPhongs = doanhThuPhongs;

            // Tính doanh thu thuê dịch vụ
            decimal doanhThuDichVu = 0;
            phieuThuePhongs
                .Where(phieu => DateTime.Compare(phieu.NGAYDI, dateNeed) == 0)
                .ToList()
                .ForEach(phieu => {
                    var ctPhieu = phieu.CHITIET_THUEDICHVU.ToList();
                    ctPhieu.ForEach(ct =>
                    {
                        doanhThuDichVu += (decimal)ct.GIA_DICHVU * ct.SOLUONG;
                    });
                });
            doanhThu.doanhThuDichVu = doanhThuDichVu;
            
            return doanhThu;
        }

        public ActionResult DoanhThuNgay(string date)
        {
            if (date == null)
            {
                ViewBag.username = ((LoginSessionModel)Session["session"]).name;
                return View(new DoanhThuModel());
            }

            DoanhThuModel doanhThu = CalDoanhThuNgay(date);

            ViewBag.date = date;
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(doanhThu);
        }

        private DoanhThuModel CalDoanhThuThang(string monthAndYear)
        {
            string[] my = monthAndYear.Split(new[] { '-' });
            DoanhThuModel doanhThu = new DoanhThuModel();
            int yearNeed = Int32.Parse(my[0]);
            int monthNeed = Int32.Parse(my[1]);

            DateTime dateMin = new DateTime(yearNeed, monthNeed, 1);
            DateTime dateMax = new DateTime(yearNeed, monthNeed + 1, 1);
            dateMax = dateMax.AddDays(-1);

            // Tính doanh thu của từng phòng
            // Khởi tạo doanh thu = 0 cho từng phòng
            var phongs = db.PHONGs.ToList();
            List<DoanhThuPhongModel> doanhThuPhongs = new List<DoanhThuPhongModel>();
            phongs.ForEach(phong =>
            {
                DoanhThuPhongModel doanhThuPhong = new DoanhThuPhongModel();
                doanhThuPhong.maPhong = phong.MA_PHONG;
                doanhThuPhong.doanhThu = 0;
                doanhThuPhongs.Add(doanhThuPhong);
            });

            // Lọc các phiếu thêu phòng thỏa ngày cần tính doanh thu

            var phieuThuePhongs = db.PHIEU_THUEPHONG.ToList();


            // cộng doanh thu cho phòng tương ứng khi được thuê (có trong bảng CT thuê phòng)
            phieuThuePhongs
                .Where(phieu => {
                    return DateTime.Compare(phieu.NGAYDEN, dateMax) <= 0 && DateTime.Compare(phieu.NGAYDI, dateMin) >= 0;
                })
                .ToList()
                .ForEach(phieu => {
                    var ctPhieu = phieu.CHITIET_THUEPHONG.ToList();

                    if (DateTime.Compare(phieu.NGAYDEN, dateMin) >= 0)
                    {
                        if (DateTime.Compare(phieu.NGAYDI, dateMax) <= 0)
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((phieu.NGAYDI - phieu.NGAYDEN).Ticks / 864e9);
                            });
                        }
                        else
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((dateMax - phieu.NGAYDEN).Ticks / 864e9);
                            });
                        }
                    }
                    else
                    {
                        if (DateTime.Compare(phieu.NGAYDI, dateMax) <= 0)
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((phieu.NGAYDI - dateMin).Ticks / 864e9);
                            });
                        }
                        else
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((dateMax - dateMin).Ticks / 864e9);
                            });
                        }
                    }
                });

            doanhThu.doanhThuPhongs = doanhThuPhongs;

            // Tính doanh thu thuê dịch vụ
            decimal doanhThuDichVu = 0;
            phieuThuePhongs
                .Where(phieu => phieu.NGAYDI.Month == monthNeed && phieu.NGAYDI.Year == yearNeed)
                .ToList()
                .ForEach(phieu =>
                {
                    var ctPhieu = phieu.CHITIET_THUEDICHVU.ToList();
                    ctPhieu.ForEach(ct =>
                    {
                        doanhThuDichVu += (decimal)ct.GIA_DICHVU * ct.SOLUONG;
                    });
                });
            doanhThu.doanhThuDichVu = doanhThuDichVu;

            return doanhThu;
        }

        public ActionResult DoanhThuThang(string monthAndYear)
        {
            if (monthAndYear == null)
            {
                ViewBag.username = ((LoginSessionModel)Session["session"]).name;
                return View(new DoanhThuModel());
            }

            DoanhThuModel doanhThu = CalDoanhThuThang(monthAndYear);

            ViewBag.monthandyear = monthAndYear;
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(doanhThu);
        }

        private DoanhThuModel CalDoanhThuNam(string year)
        {
            DoanhThuModel doanhThu = new DoanhThuModel();
            int yearNeed = Int32.Parse(year);

            DateTime dateMin = new DateTime(yearNeed, 1, 1);
            DateTime dateMax = new DateTime(yearNeed, 12, 31);

            // Tính doanh thu của từng phòng
            // Khởi tạo doanh thu = 0 cho từng phòng
            var phongs = db.PHONGs.ToList();
            List<DoanhThuPhongModel> doanhThuPhongs = new List<DoanhThuPhongModel>();
            phongs.ForEach(phong =>
            {
                DoanhThuPhongModel doanhThuPhong = new DoanhThuPhongModel();
                doanhThuPhong.maPhong = phong.MA_PHONG;
                doanhThuPhong.doanhThu = 0;
                doanhThuPhongs.Add(doanhThuPhong);
            });

            // Lọc các phiếu thêu phòng thỏa ngày cần tính doanh thu

            var phieuThuePhongs = db.PHIEU_THUEPHONG.ToList();


            // cộng doanh thu cho phòng tương ứng khi được thuê (có trong bảng CT thuê phòng)
            phieuThuePhongs
                .Where(phieu => {
                    return DateTime.Compare(phieu.NGAYDEN, dateMax) <= 0 && DateTime.Compare(phieu.NGAYDI, dateMin) >= 0;
                })
                .ToList()
                .ForEach(phieu => {
                    var ctPhieu = phieu.CHITIET_THUEPHONG.ToList();

                    if (DateTime.Compare(phieu.NGAYDEN, dateMin) >= 0)
                    {
                        if (DateTime.Compare(phieu.NGAYDI, dateMax) <= 0)
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((phieu.NGAYDI - phieu.NGAYDEN).Ticks / 864e9);
                            });
                        }
                        else
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((dateMax - phieu.NGAYDEN).Ticks / 864e9);
                            });
                        }
                    }
                    else
                    {
                        if (DateTime.Compare(phieu.NGAYDI, dateMax) <= 0)
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((phieu.NGAYDI - dateMin).Ticks / 864e9);
                            });
                        }
                        else
                        {
                            ctPhieu.ForEach((ct) => {
                                doanhThuPhongs
                                    .Find(dt => dt.maPhong == ct.MAPHONG)
                                    .doanhThu += (decimal)ct.GIAPHONG * (decimal)((dateMax - dateMin).Ticks / 864e9);
                            });
                        }
                    }
                });

            doanhThu.doanhThuPhongs = doanhThuPhongs;

            // Tính doanh thu thuê dịch vụ
            decimal doanhThuDichVu = 0;
            phieuThuePhongs
                .Where(phieu => phieu.NGAYDI.Year == yearNeed)
                .ToList()
                .ForEach(phieu =>
                {
                    var ctPhieu = phieu.CHITIET_THUEDICHVU.ToList();
                    ctPhieu.ForEach(ct =>
                    {
                        doanhThuDichVu += (decimal)ct.GIA_DICHVU * ct.SOLUONG;
                    });
                });
            doanhThu.doanhThuDichVu = doanhThuDichVu;
            return doanhThu;
        }

        public ActionResult DoanhThuNam(string year)
        {
            if (year == null)
            {
                ViewBag.username = ((LoginSessionModel)Session["session"]).name;
                return View(new DoanhThuModel());
            }

            DoanhThuModel doanhThu = CalDoanhThuNam(year);

            ViewBag.year = year;
            ViewBag.username = ((LoginSessionModel)Session["session"]).name;
            return View(doanhThu);
        }

        public void DoanhThuNgayDownload(string date)
        {
            DoanhThuModel doanhThu = CalDoanhThuNgay(date);
            string titleSheet = $"DOANH THU NGÀY {date}";
            string nameSheet = date;

            ExcelPackage ep = CreateExcelDoanhThu(doanhThu, titleSheet, nameSheet);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();
        }

        public void DoanhThuThangDownload(string monthAndYear)
        {
            string[] my = monthAndYear.Split(new[] { '-' });
            DoanhThuModel doanhThu = CalDoanhThuThang(monthAndYear);
            string titleSheet = $"DOANH THU THÁNG {my[1]} NĂM {my[0]}";
            string nameSheet = $"{my[0]}-{my[1]}";

            ExcelPackage ep = CreateExcelDoanhThu(doanhThu, titleSheet, nameSheet);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();
        }

        public void DoanhThuNamDownload(string year)
        {
            DoanhThuModel doanhThu = CalDoanhThuNam(year);
            string titleSheet = $"DOANH THU  NĂM {year}";
            string nameSheet = year;

            ExcelPackage ep = CreateExcelDoanhThu(doanhThu, titleSheet, nameSheet);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();
        }

        private static ExcelPackage CreateExcelDoanhThu(DoanhThuModel doanhThu, string titleSheet, string nameSheet)
        {
            // create excel file
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add(nameSheet);

            sheet.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Column(2).Width = 24.85;
            sheet.Column(3).Style.Numberformat.Format = "#,##0.00";

            sheet.Row(1).Style.Font.Bold = true;
            //sheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            //sheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(180, 198, 231));
            sheet.Cells["A1:C1"].Merge = true;
            sheet.Cells["A1"].Value = titleSheet;

            sheet.Cells["A2"].Value = "STT";
            sheet.Cells["B2"].Value = "Phòng";
            sheet.Cells["C2"].Value = "Tổng tiền phòng";
            sheet.Cells["C2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Row(2).Style.Font.Bold = true;

            int row = 3;
            doanhThu.doanhThuPhongs.ForEach(phong =>
            {
                sheet.Cells[$"A{row}"].Value = row - 2;
                sheet.Cells[$"B{row}"].Value = phong.maPhong;
                sheet.Cells[$"C{row}"].Value = phong.doanhThu;
                row++;
            });

            sheet.Row(row).Style.Font.Bold = true;
            sheet.Cells[$"A{row}:B{row}"].Merge = true;
            sheet.Cells[$"A{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{row}"].Value = "Tổng doanh thu tiền thuê phòng";
            sheet.Cells[$"C{row}"].Formula = $"SUM(C{3}:C{row - 1})";

            row++;
            sheet.Row(row).Style.Font.Bold = true;
            sheet.Cells[$"A{row}:B{row}"].Merge = true;
            sheet.Cells[$"A{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{row}"].Value = "Tổng doanh thu tiền thuê dịch vụ";
            sheet.Cells[$"C{row}"].Value = doanhThu.doanhThuDichVu;

            row++;
            sheet.Row(row).Style.Font.Bold = true;
            sheet.Cells[$"A{row}:B{row}"].Merge = true;
            sheet.Cells[$"A{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{row}"].Value = "Tổng doanh thu";
            sheet.Cells[$"C{row}"].Formula = $"SUM(C{row - 2}, C{row - 1})";

            var range = sheet.Cells[$"A1:C{row}"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Column(3).AutoFit();

            return ep;
        }
    }
}