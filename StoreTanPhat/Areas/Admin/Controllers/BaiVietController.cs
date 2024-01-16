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
    public class BaiVietController :AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/BaiViet
        public ActionResult Index()
        {
            var baiViet = db.BaiViet.Include(b => b.NhanVien);
            return View(baiViet.ToList());
        }

        // GET: Admin/BaiViet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiViet baiViet = db.BaiViet.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            return View(baiViet);
        }

        // GET: Admin/BaiViet/Create
        public ActionResult Create()
        {
            ViewBag.Id_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien");
            return View();
        }

        // POST: Admin/BaiViet/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id_BaiViet,Id_NhanVien,TieuDe,MieuTaNgan,LuotXem,NoiDung,HinhAnh,DuLieuHinhAnh,XetDuyet")] BaiViet baiViet)
        {
            if (ModelState.IsValid)
            {
                var result = UploadImage(baiViet.DuLieuHinhAnh);
                if (baiViet.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(baiViet);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(baiViet);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(baiViet);
                }
                baiViet.LuotXem = 0;
                baiViet.HinhAnh = result;
                baiViet.XetDuyet = 1;
                baiViet.Id_NhanVien = (int) Session["ID_Staff"];
                db.BaiViet.Add(baiViet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(baiViet);
        }

        // GET: Admin/BaiViet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiViet baiViet = db.BaiViet.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", baiViet.Id_NhanVien);
            return View(baiViet);
        }

        // POST: Admin/BaiViet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldImage,[Bind(Include = "Id_BaiViet,Id_NhanVien,TieuDe,MieuTaNgan,LuotXem,NoiDung,DuLieuHinhAnh,HinhAnh,XetDuyet")] BaiViet baiViet)
        {
            if (ModelState.IsValid)
            {
                if(baiViet.DuLieuHinhAnh == null)
                {
                    baiViet.HinhAnh = OldImage;
                }
                else
                {
                    var result = UploadImage(baiViet.DuLieuHinhAnh);
                    if (baiViet.DuLieuHinhAnh == null)
                    {
                        ViewBag.Error = "Vui lòng chọn hình ảnh";
                        return View(baiViet);
                    }
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        return View(baiViet);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        return View(baiViet);
                    }
                    baiViet.HinhAnh = result;
                }
                db.Entry(baiViet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", baiViet.Id_NhanVien);
            return View(baiViet);
        }

        // GET: Admin/BaiViet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaiViet baiViet = db.BaiViet.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            return View(baiViet);
        }

        // POST: Admin/BaiViet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BaiViet baiViet = db.BaiViet.Find(id);
            DeleteImage(baiViet.HinhAnh);
            db.BaiViet.Remove(baiViet);
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
            string folder = "Images/BaiViet/";
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
