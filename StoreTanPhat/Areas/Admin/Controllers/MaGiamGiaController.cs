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
    public class MaGiamGiaController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/MaGiamGia
        public ActionResult Index()
        {
            return View(db.MaGiamGia.ToList());
        }

        // GET: Admin/MaGiamGia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaGiamGia maGiamGia = db.MaGiamGia.Find(id);
            if (maGiamGia == null)
            {
                return HttpNotFound();
            }
            return View(maGiamGia);
        }

        // GET: Admin/MaGiamGia/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/MaGiamGia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_MaGiamGia,KeyGiamGia,TienGiam,NgayBatDau,NgayKetThuc,TinhTrangHienThi")] MaGiamGia maGiamGia)
        {
            if (ModelState.IsValid)
            {
                db.MaGiamGia.Add(maGiamGia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(maGiamGia);
        }

        // GET: Admin/MaGiamGia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaGiamGia maGiamGia = db.MaGiamGia.Find(id);
            if (maGiamGia == null)
            {
                return HttpNotFound();
            }
            return View(maGiamGia);
        }

        // POST: Admin/MaGiamGia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_MaGiamGia,KeyGiamGia,TienGiam,NgayBatDau,NgayKetThuc,TinhTrangHienThi")] MaGiamGia maGiamGia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maGiamGia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(maGiamGia);
        }

        // GET: Admin/MaGiamGia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaGiamGia maGiamGia = db.MaGiamGia.Find(id);
            if (maGiamGia == null)
            {
                return HttpNotFound();
            }
            return View(maGiamGia);
        }

        // POST: Admin/MaGiamGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaGiamGia maGiamGia = db.MaGiamGia.Find(id);
            db.MaGiamGia.Remove(maGiamGia);
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
