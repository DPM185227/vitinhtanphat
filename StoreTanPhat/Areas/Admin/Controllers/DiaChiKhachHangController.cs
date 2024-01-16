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
    public class DiaChiKhachHangController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/DiaChiKhachHang
        public ActionResult Index()
        {
            var diaChiKhachHang = db.DiaChiKhachHang.Include(d => d.Huyen).Include(d => d.KhachHang).Include(d => d.Tinh).Include(d => d.Xa);
            return View(diaChiKhachHang.ToList());
        }

        // GET: Admin/DiaChiKhachHang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiaChiKhachHang diaChiKhachHang = db.DiaChiKhachHang.Find(id);
            if (diaChiKhachHang == null)
            {
                return HttpNotFound();
            }
            return View(diaChiKhachHang);
        }

        // GET: Admin/DiaChiKhachHang/Create
        public ActionResult Create()
        {
            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name");
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen");
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name");
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name");
            return View();
        }

        // POST: Admin/DiaChiKhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DiaChi,ID_KhachHang,ID_Tinh,ID_Huyen,ID_Xa,DiaChi,TrangThai")] DiaChiKhachHang diaChiKhachHang)
        {
            if (ModelState.IsValid)
            {
                db.DiaChiKhachHang.Add(diaChiKhachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name", diaChiKhachHang.ID_Huyen);
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen", diaChiKhachHang.ID_KhachHang);
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name", diaChiKhachHang.ID_Tinh);
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name", diaChiKhachHang.ID_Xa);
            return View(diaChiKhachHang);
        }

        // GET: Admin/DiaChiKhachHang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiaChiKhachHang diaChiKhachHang = db.DiaChiKhachHang.Find(id);
            if (diaChiKhachHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name", diaChiKhachHang.ID_Huyen);
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen", diaChiKhachHang.ID_KhachHang);
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name", diaChiKhachHang.ID_Tinh);
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name", diaChiKhachHang.ID_Xa);
            return View(diaChiKhachHang);
        }

        // POST: Admin/DiaChiKhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DiaChi,ID_KhachHang,ID_Tinh,ID_Huyen,ID_Xa,DiaChi,TrangThai")] DiaChiKhachHang diaChiKhachHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diaChiKhachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name", diaChiKhachHang.ID_Huyen);
            ViewBag.ID_KhachHang = new SelectList(db.KhachHang, "ID_KhachHang", "HoTen", diaChiKhachHang.ID_KhachHang);
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name", diaChiKhachHang.ID_Tinh);
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name", diaChiKhachHang.ID_Xa);
            return View(diaChiKhachHang);
        }

        // GET: Admin/DiaChiKhachHang/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiaChiKhachHang diaChiKhachHang = db.DiaChiKhachHang.Find(id);
            if (diaChiKhachHang == null)
            {
                return HttpNotFound();
            }
            return View(diaChiKhachHang);
        }

        // POST: Admin/DiaChiKhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DiaChiKhachHang diaChiKhachHang = db.DiaChiKhachHang.Find(id);
            db.DiaChiKhachHang.Remove(diaChiKhachHang);
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
    }
}
