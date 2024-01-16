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
    public class PhiVanChuyenController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/PhiVanChuyen
        public ActionResult Index()
        {
            var phiVanChuyen = db.PhiVanChuyen.Include(p => p.Huyen).Include(p => p.Tinh).Include(p => p.Xa);
            return View(phiVanChuyen.ToList());
        }

        // GET: Admin/PhiVanChuyen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhiVanChuyen phiVanChuyen = db.PhiVanChuyen.Find(id);
            if (phiVanChuyen == null)
            {
                return HttpNotFound();
            }
            return View(phiVanChuyen);
        }

        // GET: Admin/PhiVanChuyen/Create
        public ActionResult Create()
        {
            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name");
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name");
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name");
            return View();
        }

        // POST: Admin/PhiVanChuyen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_PhiVanChuyen,ID_Tinh,ID_Huyen,ID_Xa,GiaVanChuyen,GhiChu")] PhiVanChuyen phiVanChuyen)
        {
            if (ModelState.IsValid)
            {
                db.PhiVanChuyen.Add(phiVanChuyen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name", phiVanChuyen.ID_Huyen);
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name", phiVanChuyen.ID_Tinh);
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name", phiVanChuyen.ID_Xa);
            return View(phiVanChuyen);
        }

        // GET: Admin/PhiVanChuyen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhiVanChuyen phiVanChuyen = db.PhiVanChuyen.Find(id);
            if (phiVanChuyen == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name", phiVanChuyen.ID_Huyen);
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name", phiVanChuyen.ID_Tinh);
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name", phiVanChuyen.ID_Xa);
            return View(phiVanChuyen);
        }

        // POST: Admin/PhiVanChuyen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_PhiVanChuyen,ID_Tinh,ID_Huyen,ID_Xa,GiaVanChuyen,GhiChu")] PhiVanChuyen phiVanChuyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phiVanChuyen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Huyen = new SelectList(db.Huyen, "maqh", "name", phiVanChuyen.ID_Huyen);
            ViewBag.ID_Tinh = new SelectList(db.Tinh, "matp", "name", phiVanChuyen.ID_Tinh);
            ViewBag.ID_Xa = new SelectList(db.Xa, "xaid", "name", phiVanChuyen.ID_Xa);
            return View(phiVanChuyen);
        }

        // GET: Admin/PhiVanChuyen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhiVanChuyen phiVanChuyen = db.PhiVanChuyen.Find(id);
            if (phiVanChuyen == null)
            {
                return HttpNotFound();
            }
            return View(phiVanChuyen);
        }

        // POST: Admin/PhiVanChuyen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhiVanChuyen phiVanChuyen = db.PhiVanChuyen.Find(id);
            db.PhiVanChuyen.Remove(phiVanChuyen);
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
