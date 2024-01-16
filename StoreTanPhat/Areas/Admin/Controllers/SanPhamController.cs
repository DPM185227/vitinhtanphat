using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using PagedList;
using StoreTanPhat.Areas.Admin.Controllers;
using StoreTanPhat.Models;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class SanPhamController : AuthController
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();

        // GET: Admin/SanPham
        public ActionResult Index(int ?page)
        {
            ViewBag.hinhanhSanPham = db.HinhAnhSanPham.ToList();
            ViewBag.sanPhamLienQuan = db.SanPhamLienQuan.ToList();
            ViewBag.gioHang = db.GioHang.ToList();
            ViewBag.datHangChiTiet = db.DatHangChiTiet.ToList();
            ViewBag.yeuThich = db.YeuThich.ToList();
            ViewBag.binhLuan = db.BinhLuan.ToList();

            ViewBag.CountImage = db.HinhAnhSanPham.ToList();

            // 1. Tham số int? dùng để thể hiện null và kiểu int
            // page có thể có giá trị là null và kiểu int.

            // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;
            // 3. Tạo truy vấn, lưu ý phải sắp xếp theo trường nào đó, ví dụ OrderBy
            // theo BookID mới có thể phân trang.
            var SanPham = db.SanPham.Include(s => s.DanhMucSanPhamCon).Include(s => s.NhanVien).Include(s => s.ThuongHieu).OrderBy(m=>m.ID_SanPham);
            // 4. Tạo kích thước trang (pageSize) hay là số Link hiển thị trên 1 trang
            int pageSize = 3;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 5. Trả về các Link được phân trang theo kích thước và số trang.
            return View(SanPham.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/SanPham/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanpham = db.SanPham.Find(id);
            if (sanpham == null)
            {
                return HttpNotFound();
            }
            return View(sanpham);
        }

        // GET: Admin/SanPham/Create
        public ActionResult Create()
        {
            ViewBag.ID_DanhMucSanPhamCon = new SelectList(db.DanhMucSanPhamCon.Where(m=>m.TinhTrangHienThi == 1).ToList(), "ID_DMSPC", "TenDMSPC");
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien");
            ViewBag.ID_ThuongHieu = new SelectList(db.ThuongHieu.Where(m=>m.TinhTrangHienThi == 1).ToList(), "ID_ThuongHieu", "TenThuongHieu");
            return View();
        }

        // POST: Admin/SanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_SanPham,ID_ThuongHieu,ID_DanhMucSanPhamCon,ID_NhanVien,TenSanPham,GiaBan,GiaKhuyenMai,HinhAnhDaiDien,DuLieuHinhAnh,Motasanpham,TinhTrang,BaoHanh,TinhTrangHienThi,SoLuong,NgayCapNhat,SanPhamNoiBat,KhuyenMai")] SanPham sanpham)
        {
            ViewBag.ID_DanhMucSanPhamCon = new SelectList(db.DanhMucSanPhamCon, "ID_DMSPC", "TenDMSPC", sanpham.ID_DanhMucSanPhamCon);
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", sanpham.ID_NhanVien);
            ViewBag.ID_ThuongHieu = new SelectList(db.ThuongHieu, "ID_ThuongHieu", "TenThuongHieu", sanpham.ThuongHieu);
            if (ModelState.IsValid)
            {
                sanpham.NgayCapNhat = DateTime.Now;
                sanpham.ID_NhanVien = 1;
                var result = UploadImage(sanpham.DuLieuHinhAnh);
                if (sanpham.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(sanpham);
                }
                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(sanpham);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(sanpham);
                }
                sanpham.HinhAnhDaiDien = result;
                //sanpham.ID_NhanVien = (int)Session["ID_Staff"];
                if (sanpham.SoLuong <= 0)
                {
                    // tình trạng : 0 hết hàng 1 còn hàng 2 sắp về
                    sanpham.TinhTrang = 1;
                }
                else
                {
                    sanpham.TinhTrang = 0;
                }
                if (sanpham.SoLuong < 0)
                {
                    sanpham.TinhTrangHienThi = 0;
                }
                else sanpham.TinhTrangHienThi = 1;
                db.SanPham.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sanpham);
        }

        // GET: Admin/SanPham/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanpham = db.SanPham.Find(id);
            if (sanpham == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DanhMucSanPhamCon = new SelectList(db.DanhMucSanPhamCon.Where(m=>m.TinhTrangHienThi == 1).ToList(), "ID_DMSPC", "TenDMSPC", sanpham.ID_DanhMucSanPhamCon);
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", sanpham.ID_NhanVien);
            ViewBag.ID_ThuongHieu = new SelectList(db.ThuongHieu.Where(m=>m.TinhTrangHienThi == 1).ToList(), "ID_ThuongHieu", "TenThuongHieu", sanpham.ThuongHieu);
            return View(sanpham);
        }

        // POST: Admin/SanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OldImage, [Bind(Include = "ID_SanPham,ID_ThuongHieu,ID_DanhMucSanPhamCon,ID_NhanVien,TenSanPham,GiaBan,GiaKhuyenMai,HinhAnhDaiDien,DuLieuHinhAnh,Motasanpham,TinhTrang,BaoHanh,TinhTrangHienThi,SoLuong,NgayCapNhat,SanPhamNoiBat,KhuyenMai")] SanPham sanpham)
        {
            ViewBag.ID_DanhMucSanPhamCon = new SelectList(db.DanhMucSanPhamCon, "ID_DMSPC", "TenDMSPC", sanpham.ID_DanhMucSanPhamCon);
            ViewBag.ID_NhanVien = new SelectList(db.NhanVien, "ID_NhanVien", "TenNhanVien", sanpham.ID_NhanVien);
            ViewBag.ID_ThuongHieu = new SelectList(db.ThuongHieu, "ID_ThuongHieu", "TenThuongHieu", sanpham.ThuongHieu);
            if (ModelState.IsValid)
            {
                sanpham.ID_NhanVien = (int)Session["ID_Staff"];
                sanpham.NgayCapNhat = DateTime.Now;
                var result = UploadImage(sanpham.DuLieuHinhAnh);
                if (sanpham.DuLieuHinhAnh == null)
                {
                    sanpham.HinhAnhDaiDien = OldImage;
                }
                else
                {
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        return View(sanpham);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        return View(sanpham);
                    }
                    sanpham.HinhAnhDaiDien = result;
                }

                if (sanpham.SoLuong <= 0) sanpham.SoLuong = 0;
                if (sanpham.SoLuong <= 0)
                {
                    // tình trạng : 0 hết hàng 1 còn hàng 2 sắp về
                    sanpham.TinhTrang = 1;
                }
                else
                {
                    sanpham.TinhTrang = 0;
                }
                /*
                if (sanpham.SoLuong < 0)
                {
                    sanpham.TinhTrangHienThi = 0;
                }
                else sanpham.TinhTrangHienThi = 1;*/
                // sanpham.ID_NhanVien = (int)Session["ID_Staff"];
                db.Entry(sanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sanpham);
        }

        // GET: Admin/SanPham/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanpham = db.SanPham.Find(id);
            if (sanpham == null)
            {
                return HttpNotFound();
            }
            return View(sanpham);
        }

        // POST: Admin/SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanpham = db.SanPham.Find(id);
            DeleteImage(sanpham.HinhAnhDaiDien);
            db.SanPham.Remove(sanpham);
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
            string folder = "Images/Products/";
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

        public ActionResult ExportToExcel()
        {
            var product = db.SanPham.ToList();

            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("DoctorsInfo");
            Sheet.Cells["A1"].Value = "ID_SanPham";
            Sheet.Cells["B1"].Value = "Tên Sản Phẩm";
            Sheet.Cells["C1"].Value = "Tên Danh Mục Con";
            Sheet.Cells["D1"].Value = "Tên Thương Hiệu";
            Sheet.Cells["E1"].Value = "Số lượng";
            Sheet.Cells["F1"].Value = "Giá bán";
            Sheet.Cells["G1"].Value = "Giá khuyến mãi";
            Sheet.Cells["H1"].Value = "Tình trạng hiển thị";
            Sheet.Cells["I1"].Value = "Bảo hành";

            int row = 2;
            foreach (var item in product)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ID_SanPham;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.TenSanPham;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.DanhMucSanPhamCon.TenDMSPC;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.ThuongHieu.TenThuongHieu;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.SoLuong;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.GiaBan;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.GiaKhuyenMai;
                if(item.TinhTrangHienThi == 0)
                {
                    Sheet.Cells[string.Format("H{0}", row)].Value = "Ẩn đi";
                }
                else Sheet.Cells[string.Format("H{0}", row)].Value = "Hiện ra";
                if (item.BaoHanh == "0")
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "12 tháng";
                }
                else if (item.BaoHanh == "1")
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "24 tháng";
                }
                else Sheet.Cells[string.Format("I{0}", row)].Value = "36 tháng";
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            fileContents = Ep.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return HttpNotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "danh-sach-san-pham.xlsx"
            );
        }
    }
}
