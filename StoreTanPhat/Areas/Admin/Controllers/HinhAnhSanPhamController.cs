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

    public class HinhAnhSanPhamController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/HinhAnhSanPham
        public ActionResult Index()
        {
            var hinhanhsanphams = db.HinhAnhSanPham.Include(h => h.SanPham);
            return View(hinhanhsanphams.ToList());
        }

        public ActionResult ListImage(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "SanPham");
            }
            var hinhanhsanphams = db.HinhAnhSanPham.Include(h => h.SanPham).Where(m => m.ID_SanPham == id).ToList();
            Session["ID_SanPham"] = id;
            return View(hinhanhsanphams);
        }

        // GET: Admin/HinhAnhSanPham/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HinhAnhSanPham hinhanhsanpham = db.HinhAnhSanPham.Find(id);
            if (hinhanhsanpham == null)
            {
                return HttpNotFound();
            }
            return View(hinhanhsanpham);
        }

        // GET: Admin/HinhAnhSanPham/Create
        public ActionResult Create()
        {
            if (Session["ID_SanPham"] == null)
            {
                return RedirectToAction("Index", "SanPham");
            }
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham");
            return View();
        }

        // POST: Admin/HinhAnhSanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_HinhAnhSP,ID_SanPham,HinhAnh,DuLieuHinhAnh")] HinhAnhSanPham hinhanhsanpham)
        {
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", hinhanhsanpham.ID_SanPham);
            if (ModelState.IsValid)
            {
                hinhanhsanpham.ID_SanPham = (int)Session["ID_SanPham"];
                if (hinhanhsanpham.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(hinhanhsanpham);
                }
                else
                {
                    foreach (var item in hinhanhsanpham.DuLieuHinhAnh)
                    {
                        var result = UploadImage(item);
                        if (result == "ErrorType")
                        {
                            ViewBag.Error = "Vui lòng chọn đúng kiểu";
                            return View(hinhanhsanpham);
                        }
                        if (result == "ErrorSize")
                        {
                            ViewBag.Error = "Vui lòng chọn đúng kích thước";
                            return View(hinhanhsanpham);
                        }
                        hinhanhsanpham.HinhAnh = result;
                        hinhanhsanpham.ID_SanPham = hinhanhsanpham.ID_SanPham;
                        db.HinhAnhSanPham.Add(hinhanhsanpham);
                        db.SaveChanges();
                    }
                }
                Session.Remove("ID_SanPham");
                return RedirectToAction("Index", "SanPham");
            }
            return View(hinhanhsanpham);
        }

        // GET: Admin/HinhAnhSanPham/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HinhAnhSanPham hinhanhsanpham = db.HinhAnhSanPham.Find(id);
            if (hinhanhsanpham == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", hinhanhsanpham.ID_SanPham);
            return View(hinhanhsanpham);
        }

        // POST: Admin/HinhAnhSanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_HinhAnhSP,ID_SanPham,HinhAnh")] HinhAnhSanPham hinhanhsanpham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hinhanhsanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", hinhanhsanpham.ID_SanPham);
            return View(hinhanhsanpham);
        }

        // GET: Admin/HinhAnhSanPham/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HinhAnhSanPham hinhanhsanpham = db.HinhAnhSanPham.Find(id);
            if (hinhanhsanpham == null)
            {
                return HttpNotFound();
            }
            return View(hinhanhsanpham);
        }

        // POST: Admin/HinhAnhSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HinhAnhSanPham hinhanhsanpham = db.HinhAnhSanPham.Find(id);
            DeleteImage(hinhanhsanpham.HinhAnh);
            db.HinhAnhSanPham.Remove(hinhanhsanpham);
            db.SaveChanges();
            return RedirectToAction("Index", "SanPham");
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
            string folder = "Images/HinhAnhReview/";
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
