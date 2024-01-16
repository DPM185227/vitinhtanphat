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
    public class PhanHoiBinhLuanController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/PhanHoiBinhLuan
        public ActionResult Index(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "BinhLuan");
            }
            var phanHoiBinhLuan = db.PhanHoiBinhLuan.Include(p => p.BinhLuan).Include(p => p.KhachHang).Include(p => p.NhanVien);
            Session["ID_BinhLuan"] = id;
            return View(phanHoiBinhLuan.Where(m=>m.ID_BinhLuan == id).ToList());
        }

        // GET: Admin/PhanHoiBinhLuan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhanHoiBinhLuan phanHoiBinhLuan = db.PhanHoiBinhLuan.Find(id);
            if (phanHoiBinhLuan == null)
            {
                return HttpNotFound();
            }
            return View(phanHoiBinhLuan);
        }

        // GET: Admin/PhanHoiBinhLuan/Create
        public ActionResult Create()
        {
            ViewBag.ID_BinhLuan = new SelectList(db.BinhLuan, "ID_BinhLuan", "NoiDung");
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen");
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien");
            return View();
        }

        // POST: Admin/PhanHoiBinhLuan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_PhanHoiBinhLuan,ID_BinhLuan,NoiDung,ID_KhachHang,ID_NhanVien,HinhAnh,DuLieuHinhAnh,NgayBinhLuan,HienThi")] PhanHoiBinhLuan phanHoiBinhLuan)
        {
            if (ModelState.IsValid)
            {
                var result = UploadImage(phanHoiBinhLuan.DuLieuHinhAnh);
                if (phanHoiBinhLuan.DuLieuHinhAnh == null)
                {
                    phanHoiBinhLuan.HinhAnh = "/Images/Comment/No_Image.png";
                }
                else
                {
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        return View(phanHoiBinhLuan);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        return View(phanHoiBinhLuan);
                    }
                    else
                    {
                        phanHoiBinhLuan.HinhAnh = result;
                    }
                }
                phanHoiBinhLuan.ID_BinhLuan = (int)Session["ID_BinhLuan"];
                phanHoiBinhLuan.NgayBinhLuan = DateTime.Now;
                phanHoiBinhLuan.ID_NhanVien = 1;
                phanHoiBinhLuan.HienThi = 1;
                db.PhanHoiBinhLuan.Add(phanHoiBinhLuan);
                db.SaveChanges();
                return RedirectToAction("Index",new{ id = (int)Session["ID_BinhLuan"] });
            }

            ViewBag.ID_BinhLuan = new SelectList(db.BinhLuan, "ID_BinhLuan", "NoiDung", phanHoiBinhLuan.ID_BinhLuan);
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen", phanHoiBinhLuan.ID_KhachHang);
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", phanHoiBinhLuan.ID_NhanVien);
            return View(phanHoiBinhLuan);
        }

        // GET: Admin/PhanHoiBinhLuan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhanHoiBinhLuan phanHoiBinhLuan = db.PhanHoiBinhLuan.Find(id);
            if (phanHoiBinhLuan == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_BinhLuan = new SelectList(db.BinhLuan, "ID_BinhLuan", "NoiDung", phanHoiBinhLuan.ID_BinhLuan);
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen", phanHoiBinhLuan.ID_KhachHang);
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", phanHoiBinhLuan.ID_NhanVien);
            return View(phanHoiBinhLuan);
        }

        // POST: Admin/PhanHoiBinhLuan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_PhanHoiBinhLuan,ID_BinhLuan,NoiDung,ID_KhachHang,ID_NhanVien,HinhAnh,NgayBinhLuan,HienThi")] PhanHoiBinhLuan phanHoiBinhLuan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phanHoiBinhLuan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_BinhLuan = new SelectList(db.BinhLuan, "ID_BinhLuan", "NoiDung", phanHoiBinhLuan.ID_BinhLuan);
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen", phanHoiBinhLuan.ID_KhachHang);
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", phanHoiBinhLuan.ID_NhanVien);
            return View(phanHoiBinhLuan);
        }

        // GET: Admin/PhanHoiBinhLuan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhanHoiBinhLuan phanHoiBinhLuan = db.PhanHoiBinhLuan.Find(id);
            if (phanHoiBinhLuan == null)
            {
                return HttpNotFound();
            }
            return View(phanHoiBinhLuan);
        }

        // POST: Admin/PhanHoiBinhLuan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhanHoiBinhLuan phanHoiBinhLuan = db.PhanHoiBinhLuan.Find(id);
            DeleteImage(phanHoiBinhLuan.HinhAnh);
            db.PhanHoiBinhLuan.Remove(phanHoiBinhLuan);
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
            string folder = "Images/Comment/";
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
