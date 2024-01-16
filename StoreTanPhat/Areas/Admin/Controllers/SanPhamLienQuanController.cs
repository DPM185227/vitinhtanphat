using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreTanPhat.Areas.Admin.Controllers;
using StoreTanPhat.Models;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class SanPhamLienQuanController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/SanPhamLienQuan
        public ActionResult Index(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Slider"); 
            }
            var sanPhamLienQuan = db.SanPhamLienQuan.Include(s => s.SanPham).Include(s => s.Slider).Where(m=>m.ID_Slider == id);
            Session["ID_Slider"] = id;
            return View(sanPhamLienQuan.ToList());
        }

        // GET: Admin/SanPhamLienQuan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPhamLienQuan sanPhamLienQuan = db.SanPhamLienQuan.Find(id);
            if (sanPhamLienQuan == null)
            {
                return HttpNotFound();
            }
            return View(sanPhamLienQuan);
        }

        // GET: Admin/SanPhamLienQuan/Create
        public ActionResult Create()
        {
            if(Session["ID_Slider"] == null)
            {
                return RedirectToAction("Index", "Slider");
            }
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham");
            ViewBag.ID_Slider = new SelectList(db.Slider, "Id_Slider", "TenSilder");
            return View();
        }

        // POST: Admin/SanPhamLienQuan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_SPLQ,ID_Slider,ID_SanPham")] SanPhamLienQuan sanPhamLienQuan)
        {
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", sanPhamLienQuan.ID_SanPham);
            ViewBag.ID_Slider = new SelectList(db.Slider, "Id_Slider", "TenSilder", sanPhamLienQuan.ID_Slider);
            if (ModelState.IsValid)
            {
                int ID_Silder = (int)Session["ID_Slider"];
                foreach(var item in db.SanPhamLienQuan.Where(m=>m.ID_Slider == ID_Silder).ToList())
                {
                    if(item.ID_SanPham == sanPhamLienQuan.ID_SanPham)
                    {
                        ViewBag.Error = "Sản phẩm này đã tồn tại";
                        return View(sanPhamLienQuan);
                    }
                }

                if(sanPhamLienQuan.SanPham.TinhTrangHienThi == 0)
                {
                    return RedirectToAction("Index");
                }
                sanPhamLienQuan.ID_Slider = (int)Session["ID_Slider"];
                db.SanPhamLienQuan.Add(sanPhamLienQuan);
                db.SaveChanges();
                return RedirectToAction("Index",new { id = Session["ID_Slider"] });
            }
            return View(sanPhamLienQuan);
        }

        // GET: Admin/SanPhamLienQuan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPhamLienQuan sanPhamLienQuan = db.SanPhamLienQuan.Find(id);
            if (sanPhamLienQuan == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", sanPhamLienQuan.ID_SanPham);
            ViewBag.ID_Slider = new SelectList(db.Slider, "Id_Slider", "TenSilder", sanPhamLienQuan.ID_Slider);
            return View(sanPhamLienQuan);
        }

        // POST: Admin/SanPhamLienQuan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_SPLQ,ID_Slider,ID_SanPham")] SanPhamLienQuan sanPhamLienQuan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPhamLienQuan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", sanPhamLienQuan.ID_SanPham);
            ViewBag.ID_Slider = new SelectList(db.Slider, "Id_Slider", "TenSilder", sanPhamLienQuan.ID_Slider);
            return View(sanPhamLienQuan);
        }

        // GET: Admin/SanPhamLienQuan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["ID_Slider"] == null)
            {
                return RedirectToAction("Index", "Slider");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SanPhamLienQuan sanPhamLienQuan = db.SanPhamLienQuan.Find(id);
                if (sanPhamLienQuan == null)
                {
                    return HttpNotFound();
                }
                return View(sanPhamLienQuan);
            }
        }

        // POST: Admin/SanPhamLienQuan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPhamLienQuan sanPhamLienQuan = db.SanPhamLienQuan.Find(id);
            db.SanPhamLienQuan.Remove(sanPhamLienQuan);
            db.SaveChanges();
            Session.Remove("ID_Slider");
            return RedirectToAction("Index", new { id = Session["ID_Slider"] });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
