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
using StoreTanPhat.Areas.Admin.Controllers;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class DanhMucSanPhamConController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/DanhMucSanPhamCon
        public ActionResult Index(int ?id)
        {
            var danhmucsanphamcons = db.DanhMucSanPhamCon.Include(d => d.DanhMucSanPham).Where(m=>m.ID_DanhMucSanPham == id);
            Session["MaDanhMuc"] = id;
            ViewBag.Product = db.SanPham.ToList();
            return View(danhmucsanphamcons.ToList());
        }

        // GET: Admin/DanhMucSanPhamCon/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var danhmucsanphamcon = db.DanhMucSanPham.Find(id);
            if (danhmucsanphamcon == null)
            {
                return HttpNotFound();
            }
            return View(danhmucsanphamcon);
        }

        // GET: Admin/DanhMucSanPhamCon/Create
        public ActionResult Create()
        {
            ViewBag.ID_DanhMucSanPham = new SelectList(db.DanhMucSanPham.Where(m=>m.TinhTrangHienThi == 1).ToList(), "ID_DanhMuc", "TenDanhMuc");
            return View();
        }

        // POST: Admin/DanhMucSanPhamCon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DMSPC,ID_DanhMucSanPham,TenDMSPC,TinhTrangHienThi,DuLieuHinhAnh,HinhAnh")] DanhMucSanPhamCon danhmucsanphamcon)
        {
            ViewBag.ID_DanhMucSanPham = new SelectList(db.DanhMucSanPham, "ID_DanhMuc", "TenDanhMuc", danhmucsanphamcon.ID_DanhMucSanPham);
            if (ModelState.IsValid)
            {
                var result = UploadImage(danhmucsanphamcon.DuLieuHinhAnh);
                if (danhmucsanphamcon.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(danhmucsanphamcon);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(danhmucsanphamcon);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(danhmucsanphamcon);
                }
                int ID_DanhMuc = (int)Session["MaDanhMuc"];
                danhmucsanphamcon.ID_DanhMucSanPham = ID_DanhMuc;
                danhmucsanphamcon.HinhAnh = result;
                db.DanhMucSanPhamCon.Add(danhmucsanphamcon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(danhmucsanphamcon);
        }

        // GET: Admin/DanhMucSanPhamCon/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucSanPhamCon danhmucsanphamcon = db.DanhMucSanPhamCon.Find(id);
            if (danhmucsanphamcon == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DanhMucSanPham = new SelectList(db.DanhMucSanPham, "ID_DanhMuc", "TenDanhMuc", danhmucsanphamcon.ID_DanhMucSanPham);
            return View(danhmucsanphamcon);
        }

        // POST: Admin/DanhMucSanPhamCon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldImage, [Bind(Include = "ID_DMSPC,ID_DanhMucSanPham,TenDMSPC,TinhTrangHienThi,DuLieuHinhAnh,HinhAnh")] DanhMucSanPhamCon danhmucsanphamcon)
        {
            ViewBag.ID_DanhMucSanPham = new SelectList(db.DanhMucSanPham, "ID_DanhMuc", "TenDanhMuc", danhmucsanphamcon.ID_DanhMucSanPham);
            if (ModelState.IsValid)
            {
                if (danhmucsanphamcon.DuLieuHinhAnh == null)
                {
                    danhmucsanphamcon.HinhAnh = OldImage;
                }
                else
                {
                    var result = UploadImage(danhmucsanphamcon.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        danhmucsanphamcon.HinhAnh = OldImage;
                        return View(danhmucsanphamcon);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        danhmucsanphamcon.HinhAnh = OldImage;
                        return View(danhmucsanphamcon);
                    }
                    DeleteImage(OldImage);
                    danhmucsanphamcon.HinhAnh = result;
                }
                db.Entry(danhmucsanphamcon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(danhmucsanphamcon);
        }

        // GET: Admin/DanhMucSanPhamCon/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucSanPhamCon danhmucsanphamcon = db.DanhMucSanPhamCon.Find(id);
            if (danhmucsanphamcon == null)
            {
                return HttpNotFound();
            }
            return View(danhmucsanphamcon);
        }

        // POST: Admin/DanhMucSanPhamCon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMucSanPhamCon danhmucsanphamcon = db.DanhMucSanPhamCon.Find(id);
            DeleteImage(danhmucsanphamcon.HinhAnh);
            db.DanhMucSanPhamCon.Remove(danhmucsanphamcon);
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
            string folder = "Images/CateChild/";
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
