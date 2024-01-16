using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreTanPhat.Models;
using StoreTanPhat.Libary;
using StoreTanPhat.Areas.Admin.Controllers;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class NhanVienController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/NhanVien
        public ActionResult Index()
        {
            ViewBag.SanPham = db.SanPham.ToList();
            ViewBag.PhanHoiBinhLuan = db.PhanHoiBinhLuan.ToList();
            ViewBag.BaiViet = db.BaiViet.ToList();
            return View(db.NhanVien.ToList());
        }

        // GET: Admin/NhanVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanvien = db.NhanVien.Find(id);
            if (nhanvien == null)
            {
                return HttpNotFound();
            }
            return View(nhanvien);
        }

        // GET: Admin/NhanVien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_NhanVien,TenNhanVien,DiaChiNhanVien,NamSinhNhanVien,GioiTinh,GmailNhanVien,TenDangNhapNV,MatKhauNV,DuLieuHinhAnh,HinhAnhNV,QuyenNhanVien")] NhanVien nhanvien)
        {
            if (ModelState.IsValid)
            {
                var result = UploadImage(nhanvien.DuLieuHinhAnh);
                if (nhanvien.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(nhanvien);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(nhanvien);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(nhanvien);
                }
                nhanvien.HinhAnhNV = result;
                nhanvien.MatKhauNV = MaHoaSHA1.SHA1(nhanvien.MatKhauNV);
                db.NhanVien.Add(nhanvien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nhanvien);
        }

        // GET: Admin/NhanVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanvien = db.NhanVien.Find(id);
            if (nhanvien == null)
            {
                return HttpNotFound();
            }
            return View(nhanvien);
        }

        // POST: Admin/NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldPass, string OldImage, DateTime OldYear, [Bind(Include = "ID_NhanVien,TenNhanVien,DiaChiNhanVien,NamSinhNhanVien,GioiTinh,GmailNhanVien,TenDangNhapNV,MatKhauNV,DuLieuHinhAnh,HinhAnhNV,QuyenNhanVien")] NhanVien nhanvien)
        {
            if (ModelState.IsValid)
            {
                if (nhanvien.DuLieuHinhAnh != null && nhanvien.NamSinhNhanVien == null && nhanvien.MatKhauNV == null)
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
                    nhanvien.MatKhauNV = OldPass;
                }
                else if (nhanvien.MatKhauNV != null && nhanvien.DuLieuHinhAnh == null && nhanvien.NamSinhNhanVien == null)
                {
                    nhanvien.MatKhauNV = MaHoaSHA1.SHA1(nhanvien.MatKhauNV);
                    nhanvien.NamSinhNhanVien = OldYear;
                    nhanvien.HinhAnhNV = OldImage;
                }
                else if (nhanvien.NamSinhNhanVien != null && nhanvien.MatKhauNV == null && nhanvien.DuLieuHinhAnh == null)
                {
                    nhanvien.NamSinhNhanVien = nhanvien.NamSinhNhanVien;
                    nhanvien.MatKhauNV = OldPass;
                    nhanvien.HinhAnhNV = OldImage;
                }
                else if (nhanvien.NamSinhNhanVien != null && nhanvien.DuLieuHinhAnh != null && nhanvien.MatKhauNV != null)
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
                    nhanvien.MatKhauNV = MaHoaSHA1.SHA1(nhanvien.MatKhauNV);
                    nhanvien.NamSinhNhanVien = nhanvien.NamSinhNhanVien;
                }
                else if (nhanvien.NamSinhNhanVien != null && nhanvien.DuLieuHinhAnh != null && nhanvien.MatKhauNV == null)
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
                    nhanvien.MatKhauNV = OldPass;
                    nhanvien.NamSinhNhanVien = nhanvien.NamSinhNhanVien;
                }
                else if (nhanvien.NamSinhNhanVien == null && nhanvien.DuLieuHinhAnh != null && nhanvien.MatKhauNV != null)
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
                    nhanvien.MatKhauNV = MaHoaSHA1.SHA1(nhanvien.MatKhauNV);
                    nhanvien.NamSinhNhanVien = OldYear;
                }
                else if (nhanvien.NamSinhNhanVien != null && nhanvien.DuLieuHinhAnh == null && nhanvien.MatKhauNV != null)
                {
                    nhanvien.HinhAnhNV = OldImage;
                    nhanvien.MatKhauNV = MaHoaSHA1.SHA1(nhanvien.MatKhauNV);
                    nhanvien.NamSinhNhanVien = nhanvien.NamSinhNhanVien;
                }
                else
                {
                    nhanvien.NamSinhNhanVien = OldYear;
                    nhanvien.MatKhauNV = OldPass;
                    nhanvien.HinhAnhNV = OldImage;
                }
                db.Entry(nhanvien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhanvien);
        }

        // GET: Admin/NhanVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanvien = db.NhanVien.Find(id);
            if (nhanvien == null)
            {
                return HttpNotFound();
            }
            return View(nhanvien);
        }

        // POST: Admin/NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NhanVien nhanvien = db.NhanVien.Find(id);
            DeleteImage(nhanvien.HinhAnhNV);
            db.NhanVien.Remove(nhanvien);
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
    }
}
