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
using StoreTanPhat.Models;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class ThuongHieuController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/ThuongHieu
        public ActionResult Index()
        {
            ViewBag.SanPham = db.SanPham.ToList();
            return View(db.ThuongHieu.ToList());
        }

        // GET: Admin/ThuongHieu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            if (thuongHieu == null)
            {
                return HttpNotFound();
            }
            return View(thuongHieu);
        }

        // GET: Admin/ThuongHieu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ThuongHieu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_ThuongHieu,TenThuongHieu,TinhTrangHienThi,DuLieuHinhAnh,HinhAnh")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                // gọi ra 3 lần nếu file thỏa thì nó sẻ update 3 lần
                // tạo biến result lưu kết quả sao lần gọi
                // kiểm tra kết quả khi gọi 
                var result = UploadImage(thuongHieu.DuLieuHinhAnh);
                if (thuongHieu.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(thuongHieu);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(thuongHieu);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(thuongHieu);
                }
                thuongHieu.HinhAnh = result;
                db.ThuongHieu.Add(thuongHieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuongHieu);
        }

        // GET: Admin/ThuongHieu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            if (thuongHieu == null)
            {
                return HttpNotFound();
            }
            return View(thuongHieu);
        }

        // POST: Admin/ThuongHieu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldImage, [Bind(Include = "ID_ThuongHieu,TenThuongHieu,TinhTrangHienThi,DuLieuHinhAnh,HinhAnh")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                if (thuongHieu.DuLieuHinhAnh == null)
                {
                    thuongHieu.HinhAnh = OldImage;
                }
                else
                {
                    var result = UploadImage(thuongHieu.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        thuongHieu.HinhAnh = OldImage;
                        return View(thuongHieu);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        thuongHieu.HinhAnh = OldImage;
                        return View(thuongHieu);
                    }
                    DeleteImage(OldImage);
                    thuongHieu.HinhAnh = result;
                }
                db.Entry(thuongHieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuongHieu);
        }

        // GET: Admin/ThuongHieu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            if (thuongHieu == null)
            {
                return HttpNotFound();
            }
            return View(thuongHieu);
        }

        // POST: Admin/ThuongHieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThuongHieu thuongHieu = db.ThuongHieu.Find(id);
            DeleteImage(thuongHieu.HinhAnh);
            db.ThuongHieu.Remove(thuongHieu);
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
            string folder = "Images/ThuongHieu/";
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
