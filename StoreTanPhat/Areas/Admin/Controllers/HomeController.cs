using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreTanPhat.Libary;
using StoreTanPhat.Models;
using System.Data;
using System.Data.Entity;
using StoreTanPhat.Areas.Admin.Controllers;
using System.IO;
using Newtonsoft.Json;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {

        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        public ActionResult Index()
        {
            if(Session["ID_Staff"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.SanPham = db.SanPham.ToList();
            ViewBag.DonHang = db.DatHang.ToList();
            ViewBag.DatHangChiTiet = db.DatHangChiTiet.ToList();
            ViewBag.BinhLuan = db.BinhLuan.ToList();
            ViewBag.YeuThich = db.YeuThich.ToList();
            ViewBag.NguoiDung = db.KhachHang.ToList();
            //ViewBag.OrderToday = db.DatHang.Where(m => m.NgayLap.Equals(DateTime.Now.Date)).Sum(m => m.TongTien);
            var today = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            var ListOrder = db.DatHang.ToList();
            var sumToday = ListOrder.Where(x => x.NgayLap == today).Sum(m => m.TongTien);
            var sumDonHangToDay = ListOrder.Where(x => x.NgayLap == today).Count();
            ViewBag.DoanhThuNgay = sumToday;
            ViewBag.sumDonHangToDay = sumDonHangToDay;
            // THỐNG KÊ THEO TUẦN
            var startWeek = DateTime.Now.AddDays(-2);
            var endWeek = startWeek.AddDays(7);
            ViewBag.SumWeek = ListOrder.Where(m => m.NgayLap >= startWeek && m.NgayLap <= endWeek).Sum(m => m.TongTien);
            ViewBag.sumDonHangToWeek = ListOrder.Where(m => m.NgayLap >= startWeek && m.NgayLap <= endWeek).Count();
            // CHART TODAY
            List<DataPoint> data = new List<DataPoint>();
            foreach(var item in ListOrder.Where(x=>x.NgayLap == today))
            {
                data.Add(new DataPoint(item.NgayLap.ToString(), item.TongTien));
            }
            ViewBag.DataPoints = JsonConvert.SerializeObject(data);

            // chart all
            List<DataPoint> dataPoint = new List<DataPoint>();
            foreach (var item in db.DatHang.ToList())
            {
                dataPoint.Add(new DataPoint(item.NgayLap.ToShortDateString(), item.TongTien));
            }
            ViewBag.Data = JsonConvert.SerializeObject(dataPoint);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginNhanVien nhanvien)
        {
            string PassSHA1 = MaHoaSHA1.SHA1(nhanvien.MatKhauNV);
            var staff = db.NhanVien.Where(m => m.TenDangNhapNV == nhanvien.TenDangNhapNV && m.MatKhauNV == PassSHA1).FirstOrDefault();
            if (staff == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View(nhanvien);
            }
            else
            {
                Session["ID_Staff"] = staff.ID_NhanVien;
                Session["HoTen"] = staff.TenNhanVien;
                Session["HinhAnh"] = staff.HinhAnhNV;
                Session["Quyen"] = staff.QuyenNhanVien;
                return RedirectToAction("Index");
            }
        }


        public ActionResult ConfirmOrder(int id)
        {
            var order = db.DatHang.Where(m => m.ID_DatHang == id).FirstOrDefault();
            order.TinhTrangDonHang = 1;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Profiles()
        {
            if(Session["ID_Staff"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int id_Staff = (int)Session["ID_Staff"];
            var profile = db.NhanVien.Where(m => m.ID_NhanVien == id_Staff).FirstOrDefault();
            return View(profile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profiles(string OldImage, DateTime OldYear, [Bind(Include = "ID_NhanVien,TenNhanVien,DiaChiNhanVien,NamSinhNhanVien,GioiTinh,GmailNhanVien,TenDangNhapNV,MatKhauNV,DuLieuHinhAnh,HinhAnhNV,QuyenNhanVien")] NhanVien nhanvien)
        {
            if (ModelState.IsValid)
            {
                if (nhanvien.DuLieuHinhAnh != null && nhanvien.NamSinhNhanVien == null)
                {
                    var result = UploadImage(nhanvien.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        nhanvien.HinhAnhNV = OldImage;
                        return View(nhanvien);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        nhanvien.HinhAnhNV = OldImage;
                        return View(nhanvien);
                    }
                    DeleteImage(OldImage);
                    nhanvien.HinhAnhNV = result;
                    nhanvien.NamSinhNhanVien = OldYear;
                }
                else if (nhanvien.NamSinhNhanVien != null && nhanvien.DuLieuHinhAnh == null)
                {
                    nhanvien.NamSinhNhanVien = nhanvien.NamSinhNhanVien;
                    nhanvien.HinhAnhNV = OldImage;
                }
                else if (nhanvien.DuLieuHinhAnh != null)
                {
                    var result = UploadImage(nhanvien.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        nhanvien.HinhAnhNV = OldImage;
                        return View(nhanvien);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        nhanvien.HinhAnhNV = OldImage;
                        return View(nhanvien);
                    }
                    DeleteImage(OldImage);
                    nhanvien.HinhAnhNV = result;
                    nhanvien.NamSinhNhanVien = OldYear;
                }
                else if (nhanvien.NamSinhNhanVien != null)
                {
                    nhanvien.NamSinhNhanVien = nhanvien.NamSinhNhanVien;
                    nhanvien.HinhAnhNV = OldImage;
                }
                else
                {
                    nhanvien.NamSinhNhanVien = OldYear;
                    nhanvien.HinhAnhNV = OldImage;
                }
                db.Entry(nhanvien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DangXuat","Home");
            }
            return View(nhanvien);
        }

        public string UploadImage(HttpPostedFileBase file)
        {
            string folder = "Images/Staff/";
            string fileName = "";
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                var fileTypeSupported = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!fileTypeSupported.Contains(fileExtension))
                {
                    return "ErrorType";
                }
                if (file.ContentLength > 2 * 1024 * 1024)
                {
                    return "ErrorSize";
                }
                else
                {
                    fileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(Server.MapPath("~/" + folder), fileName);
                    file.SaveAs(filePath);
                }
            }
            return folder + fileName;
        }

        public void DeleteImage(string path)
        {
            string oldFilePath = Server.MapPath("~/" + path);
            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
        }


        public ActionResult ThongKe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Chart(string StartDay, string EndDay)
        {
            if(StartDay == "" || EndDay == "" || Convert.ToDateTime(StartDay) > Convert.ToDateTime(EndDay))
            {
                return RedirectToAction("ThongKe");
            }   
            else
            {
                var after = db.DatHang.Include(m => m.DatHangChiTiet).ToList();
                var Startday = Convert.ToDateTime(StartDay);
                var Endday = Convert.ToDateTime(EndDay);
                var thongKe = after.Where(m => m.NgayCapNhat >= Startday && m.NgayLap <= Endday);
                ViewBag.DoanhThu = thongKe.Sum(m => m.TongTien);
                ViewBag.SoLuongSanPhamDaBan = thongKe.Sum(m => m.DatHangChiTiet.Sum(r => r.SoLuong));
                ViewBag.SoDonHang = thongKe.Count();
                List<DataPoint> datas = new List<DataPoint>();
                foreach(var item in thongKe.Where(m=>m.TinhTrangDonHang != 2))
                {
                    datas.Add(new DataPoint(item.NgayLap.ToShortDateString(), item.TongTien));
                }
                ViewBag.DataPoints = JsonConvert.SerializeObject(datas);
                return View();
            }

        }
    }
}