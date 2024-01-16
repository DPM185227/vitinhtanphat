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
    public class DanhMucSanPhamController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/DanhMucSanPham
        public ActionResult Index()
        {
            ViewBag.DanhMucCon = db.DanhMucSanPhamCon.ToList();
            return View(db.DanhMucSanPham.ToList());
        }

        // GET: Admin/DanhMucSanPham/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var DanhMucSanPham = db.DanhMucSanPham.Find(id);
            if (DanhMucSanPham == null)
            {
                return HttpNotFound();
            }
            return View(DanhMucSanPham);
        }

        // GET: Admin/DanhMucSanPham/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DanhMucSanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DanhMuc,TenDanhMuc,TinhTrangHienThi,DuLieuHinhAnh,HinhAnh")] DanhMucSanPham DanhMucSanPham)
        {
            if (ModelState.IsValid)
            {
                var result = UploadImage(DanhMucSanPham.DuLieuHinhAnh);
                if (DanhMucSanPham.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(DanhMucSanPham);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(DanhMucSanPham);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(DanhMucSanPham);
                }
                DanhMucSanPham.HinhAnh = result;
                db.DanhMucSanPham.Add(DanhMucSanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(DanhMucSanPham);
        }

        // GET: Admin/DanhMucSanPham/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucSanPham DanhMucSanPham = db.DanhMucSanPham.Find(id);
            if (DanhMucSanPham == null)
            {
                return HttpNotFound();
            }
            return View(DanhMucSanPham);
        }

        // POST: Admin/DanhMucSanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldImage, [Bind(Include = "ID_DanhMuc,TenDanhMuc,TinhTrangHienThi,DuLieuHinhAnh,HinhAnh")] DanhMucSanPham DanhMucSanPham)
        {
            if (ModelState.IsValid)
            {
                if (DanhMucSanPham.DuLieuHinhAnh == null)
                {
                    DanhMucSanPham.HinhAnh = OldImage;
                }
                else
                {
                    var result = UploadImage(DanhMucSanPham.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        DanhMucSanPham.HinhAnh = OldImage;
                        return View(DanhMucSanPham);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        DanhMucSanPham.HinhAnh = OldImage;
                        return View(DanhMucSanPham);
                    }
                    DeleteImage(OldImage);
                    DanhMucSanPham.HinhAnh = result;
                }
                db.Entry(DanhMucSanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(DanhMucSanPham);
        }

        // GET: Admin/DanhMucSanPham/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucSanPham DanhMucSanPham = db.DanhMucSanPham.Find(id);
            if (DanhMucSanPham == null)
            {
                return HttpNotFound();
            }
            return View(DanhMucSanPham);
        }

        // POST: Admin/DanhMucSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMucSanPham DanhMucSanPham = db.DanhMucSanPham.Find(id);
            DeleteImage(DanhMucSanPham.HinhAnh);
            db.DanhMucSanPham.Remove(DanhMucSanPham);
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
            string folder = "Images/Catelogy/";
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
