using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreTanPhat.Areas.Admin.Controllers;
using StoreTanPhat.Libary;
using StoreTanPhat.Models;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class KhachHangController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/KhachHang
        public ActionResult Index()
        {
            ViewBag.DiaChi = db.DiaChiKhachHang.ToList();
            ViewBag.YeuThich = db.YeuThich.ToList();
            ViewBag.DatHang = db.DatHang.ToList();
            ViewBag.PhanHoiBinhLuan = db.PhanHoiBinhLuan.ToList();
            ViewBag.GioHang = db.GioHang.ToList();
            return View(db.KhachHang.ToList());
        }

        // GET: Admin/KhachHang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachhang = db.KhachHang.Find(id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }
            return View(khachhang);
        }

        // GET: Admin/KhachHang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_KhachHang,HoTen,SoDienThoai,GioiTinh,NamSinh,Gmail,TenDangNhap,MatKhau,DuLieuHinhAnh,HinhAnh")] KhachHang khachhang)
        {
            if (ModelState.IsValid)
            {
                var result = UploadImage(khachhang.DuLieuHinhAnh);
                if (khachhang.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(khachhang);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(khachhang);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(khachhang);
                }
                khachhang.HinhAnh = result;
                string passSHA1 = MaHoaSHA1.SHA1(khachhang.MatKhau);
                khachhang.MatKhau = passSHA1;
                db.KhachHang.Add(khachhang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khachhang);
        }

        // GET: Admin/KhachHang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachhang = db.KhachHang.Find(id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }
            return View(khachhang);
        }

        // POST: Admin/KhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldPass, string OldImage, DateTime OldYear, [Bind(Include = "ID_KhachHang,HoTen,SoDienThoai,GioiTinh,NamSinh,Gmail,TenDangNhap,MatKhau,HinhAnh,DuLieuHinhAnh")] KhachHang khachhang)
        {
            if (ModelState.IsValid)
            {
                if (khachhang.DuLieuHinhAnh != null && khachhang.NamSinh == null && khachhang.MatKhau == null)
                {
                    var result = UploadImage(khachhang.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    DeleteImage(OldImage);
                    khachhang.HinhAnh = result;
                    khachhang.NamSinh = OldYear;
                    khachhang.MatKhau = OldPass;
                }
                else if (khachhang.MatKhau != null && khachhang.DuLieuHinhAnh == null && khachhang.NamSinh == null)
                {
                    khachhang.MatKhau = MaHoaSHA1.SHA1(khachhang.MatKhau);
                    khachhang.NamSinh = OldYear;
                    khachhang.HinhAnh = OldImage;
                }
                else if (khachhang.NamSinh != null && khachhang.MatKhau == null && khachhang.DuLieuHinhAnh == null)
                {
                    khachhang.NamSinh = khachhang.NamSinh;
                    khachhang.MatKhau = OldPass;
                    khachhang.HinhAnh = OldImage;
                }
                else if (khachhang.NamSinh != null && khachhang.DuLieuHinhAnh != null && khachhang.MatKhau != null)
                {
                    var result = UploadImage(khachhang.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    DeleteImage(OldImage);
                    khachhang.HinhAnh = result;
                    khachhang.MatKhau = MaHoaSHA1.SHA1(khachhang.MatKhau);
                    khachhang.NamSinh = khachhang.NamSinh;
                }
                else if (khachhang.NamSinh != null && khachhang.DuLieuHinhAnh != null && khachhang.MatKhau == null)
                {
                    var result = UploadImage(khachhang.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    DeleteImage(OldImage);
                    khachhang.HinhAnh = result;
                    khachhang.MatKhau = OldPass;
                    khachhang.NamSinh = khachhang.NamSinh;
                }
                else if (khachhang.NamSinh == null && khachhang.DuLieuHinhAnh != null && khachhang.MatKhau != null)
                {
                    var result = UploadImage(khachhang.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    DeleteImage(OldImage);
                    khachhang.HinhAnh = result;
                    khachhang.MatKhau = MaHoaSHA1.SHA1(khachhang.MatKhau);
                    khachhang.NamSinh = OldYear;
                }
                else if (khachhang.NamSinh != null && khachhang.DuLieuHinhAnh == null && khachhang.MatKhau != null)
                {
                    khachhang.HinhAnh = OldImage;
                    khachhang.MatKhau = MaHoaSHA1.SHA1(khachhang.MatKhau);
                    khachhang.NamSinh = khachhang.NamSinh;
                }
                else
                {
                    khachhang.NamSinh = OldYear;
                    khachhang.MatKhau = OldPass;
                    khachhang.HinhAnh = OldImage;
                }
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachhang);
        }

        // GET: Admin/KhachHang/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachhang = db.KhachHang.Find(id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }
            return View(khachhang);
        }

        // POST: Admin/KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhachHang khachhang = db.KhachHang.Find(id);
            DeleteImage(khachhang.HinhAnh);
            db.KhachHang.Remove(khachhang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string UploadImage(HttpPostedFileBase file)
        {
            string folder = "Images/Customers/";
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
    }
}
