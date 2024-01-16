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
    public class DatHangChiTietController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/DatHangChiTiet
        public ActionResult Index(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "DatHang");
            }
            var datHangChiTiet = db.DatHangChiTiet.Include(d => d.DatHang).Include(d => d.SanPham).Where(m=>m.ID_DatHang == id);
            ViewBag.DatHang = db.DatHang.Where(m => m.ID_DatHang == id).FirstOrDefault();
            Session["DatHangID"] = id;
            return View(datHangChiTiet.ToList());
        }

        // GET: Admin/DatHangChiTiet/Details/5
        public ActionResult Details(int? id)
        {
            int Order_Details = (int)Session["DatHangID"];
            if(Session["DatHangID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DatHangChiTiet datHangChiTiet = db.DatHangChiTiet.Find(id);
                if (datHangChiTiet == null)
                {
                    return HttpNotFound();
                }
                return View(datHangChiTiet);
            }
            else
            {
                return RedirectToAction("Index", "DatHang");
            }
        }

        // GET: Admin/DatHangChiTiet/Create
        public ActionResult Create()
        {
            ViewBag.ID_DatHang = new SelectList(db.DatHang, "ID_DatHang", "DiaChiGiaoHang");
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham");
            return View();
        }

        // POST: Admin/DatHangChiTiet/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DatHangChiTiet,ID_DatHang,ID_SanPham,DonGia,SoLuong,TongTien")] DatHangChiTiet datHangChiTiet)
        {
            if (ModelState.IsValid)
            {
                db.DatHangChiTiet.Add(datHangChiTiet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_DatHang = new SelectList(db.DatHang, "ID_DatHang", "DiaChiGiaoHang", datHangChiTiet.ID_DatHang);
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", datHangChiTiet.ID_SanPham);
            return View(datHangChiTiet);
        }

        // GET: Admin/DatHangChiTiet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["DatHangID"] != null)
            {
                int Order_Details = (int)Session["DatHangID"];
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DatHangChiTiet datHangChiTiet = db.DatHangChiTiet.Find(id);
                if (datHangChiTiet == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ID_DatHang = new SelectList(db.DatHang, "ID_DatHang", "DiaChiGiaoHang", datHangChiTiet.ID_DatHang);
                ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", datHangChiTiet.ID_SanPham);
                return View(datHangChiTiet);
            }
            else
            {
                return RedirectToAction("Index", "DatHang");
            }

        }

        // POST: Admin/DatHangChiTiet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DatHangChiTiet,ID_DatHang,ID_SanPham,DonGia,SoLuong,TongTien")] DatHangChiTiet datHangChiTiet)
        {
            if (ModelState.IsValid)
            {
                var old_chitiet = db.DatHangChiTiet.Where(m => m.ID_DatHangChiTiet == datHangChiTiet.ID_DatHangChiTiet).FirstOrDefault();
                if(datHangChiTiet.ID_SanPham != old_chitiet.ID_SanPham)
                {
                    if (datHangChiTiet.SoLuong > 0)
                    {
                        datHangChiTiet.ID_SanPham = datHangChiTiet.ID_SanPham;
                        datHangChiTiet.SoLuong = datHangChiTiet.SoLuong;
                        datHangChiTiet.DonGia = datHangChiTiet.SanPham.GiaBan;
                        datHangChiTiet.TongTien = datHangChiTiet.DonGia * datHangChiTiet.SoLuong;
                        datHangChiTiet.ID_DatHang = datHangChiTiet.ID_DatHang;

                        var product = db.SanPham.Where(m => m.ID_SanPham == datHangChiTiet.ID_SanPham).FirstOrDefault();
                        product.SoLuong = product.SoLuong - datHangChiTiet.SoLuong;
                        db.Entry(product).State = EntityState.Modified;
                    }
                    else
                    {
                        ViewBag.ErrorSoLuong = "Vui lòng nhập số lượng";
                        return View(datHangChiTiet);
                    }
                }
                else
                {
                    if(datHangChiTiet.SoLuong != old_chitiet.SoLuong)
                    {
                        datHangChiTiet.TongTien = datHangChiTiet.SoLuong * datHangChiTiet.DonGia;
                    }
                }
                db.Entry(datHangChiTiet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_DatHang = new SelectList(db.DatHang, "ID_DatHang", "DiaChiGiaoHang", datHangChiTiet.ID_DatHang);
            ViewBag.ID_SanPham = new SelectList(db.SanPham, "ID_SanPham", "TenSanPham", datHangChiTiet.ID_SanPham);
            return View(datHangChiTiet);
        }

        // GET: Admin/DatHangChiTiet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatHangChiTiet datHangChiTiet = db.DatHangChiTiet.Find(id);
            if (datHangChiTiet == null)
            {
                return HttpNotFound();
            }
            return View(datHangChiTiet);
        }

        // POST: Admin/DatHangChiTiet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DatHangChiTiet datHangChiTiet = db.DatHangChiTiet.Find(id);
            int Order_ID = (int) Session["DatHangID"];
            var OrderList = db.DatHangChiTiet.Where(m => m.ID_DatHang == Order_ID).ToList();
            var Order = db.DatHang.Where(m => m.ID_DatHang == datHangChiTiet.ID_DatHang).FirstOrDefault();
            if (OrderList.Count > 1)
            {
                // cập nhật  lại số lượng sản phẩm
                foreach (var item in OrderList)
                {
                    foreach (var product in db.SanPham.Where(m => m.ID_SanPham == item.ID_SanPham).ToList())
                    {
                        product.SoLuong += item.SoLuong;
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                db.DatHangChiTiet.Remove(datHangChiTiet);
                db.SaveChanges();
                // cập nhật lại tổng tiền
                Order.TongTien = (int) db.DatHangChiTiet.Where(m => m.ID_DatHang == Order.ID_DatHang).Sum(m => m.DonGia * m.SoLuong) + (int) Order.PhiShip;
            }
            else
            {
                foreach (var item in OrderList)
                {
                    foreach (var product in db.SanPham.Where(m => m.ID_SanPham == item.ID_SanPham).ToList())
                    {
                        product.SoLuong += item.SoLuong;
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                db.DatHangChiTiet.Remove(datHangChiTiet);
                db.SaveChanges();
                Order.TongTien = 0;
                Order.TinhTrangDonHang = 2;
                Order.PhiShip = 0;
            }
            db.Entry(Order).State = EntityState.Modified;
            db.SaveChanges();
            Session.Remove("DatHangID");
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
