using StoreTanPhat.Libary;
using StoreTanPhat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace StoreTanPhat.Controllers
{
    public class HomeController : Controller    
    {
        private ShopTanPhatEntities db = new ShopTanPhatEntities();
        public ActionResult Index()
        {
            if(Session["ID_Customer"] != null)
            {
                int customer = (int)Session["ID_Customer"];
                var list_product = db.SanPham.Where(m=>m.SoLuong > 0).ToList();
                ViewBag.NewProduct = db.SanPham.OrderByDescending(m => m.NgayCapNhat).Where(m=>m.SoLuong > 0).Include(m => m.DanhMucSanPhamCon).Take(5).ToList();
                ViewBag.CateLogy = db.DanhMucSanPham.Where(m=>m.TinhTrangHienThi == 1).ToList();
                ViewBag.ListComment = db.BinhLuan.Where(m=>m.HienThi == 1).ToList();
                // LẤY  DANH SÁCH YÊU THÍCH CỦA KHÁCH HÀNG
                ViewBag.YeuThich = db.YeuThich.Where(m => m.Id_KhachHang == customer).ToList();
                // Lấy Slider
                ViewBag.Slider = db.Slider.Where(m=>m.HienThi == 1).ToList();
                ViewBag.SanPhamLienQuan = db.SanPhamLienQuan.ToList();
                ViewBag.BaiViet = db.BaiViet.Where(m => m.XetDuyet == 1).ToList();
                return View(list_product);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public ActionResult AddYeuThich(int? id)
        {
            if(Session["ID_Customer"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var item = db.YeuThich.Where(m => m.Id_SanPham == id).FirstOrDefault();
                if (item != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                YeuThich yeuthich = new YeuThich();
                yeuthich.Id_KhachHang = (int)Session["ID_Customer"];
                yeuthich.Id_SanPham = (int)id;
                db.YeuThich.Add(yeuthich);
                db.SaveChanges();
                Session["CountYeuThich"] = db.YeuThich.Count();
                ViewBag.AddThanhCong = "Thêm vào thành công";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult ListYeuThich()
        {
            if(Session["ID_Customer"] != null)
            {
                int id_customer = (int)Session["ID_Customer"];
                var yeuthichs = db.YeuThich.Where(m => m.Id_KhachHang == id_customer).Include(m => m.SanPham).ToList();
                return View(yeuthichs);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult DeleteYeuThich(int? id)
        {
            if (Session["ID_Customer"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                int idCustomer = (int)Session["ID_Customer"];
                var item = db.YeuThich.Where(m => m.Id_YeuThich == id).FirstOrDefault();
                db.YeuThich.Remove(item);
                db.SaveChanges();
                Session["CountYeuThich"] = db.YeuThich.Count();
                ViewBag.AddThanhCong = "Thêm vào thành công";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult QuickView(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var product = db.SanPham.Where(m => m.ID_SanPham == id).FirstOrDefault();
            ViewBag.ListImage = db.HinhAnhSanPham.Where(m => m.ID_SanPham == id).ToList();
            ViewBag.ListComment = db.BinhLuan.Where(m => m.ID_SanPham == id).ToList();
            ViewBag.ListCommentRep = db.PhanHoiBinhLuan.ToList();
            // LẤY THEO DANH MỤC SẢN PHẨM và thương hiệu
            ViewBag.ListSanPhamTuongTu = db.SanPham.Where(m => m.ID_ThuongHieu == product.ID_ThuongHieu && m.ID_SanPham != id || m.DanhMucSanPhamCon.ID_DanhMucSanPham == product.DanhMucSanPhamCon.ID_DanhMucSanPham && m.ID_SanPham != id && m.SoLuong > 0).Take(4).ToList();
            return View(product);
        }


        public ActionResult AddToCart(int ?id)
        {
            if(Session["ID_Customer"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var cartitem = db.GioHang.Where(m => m.ID_SanPham == id).FirstOrDefault();
                //var product = db.SanPham.Where(m => m.ID_SanPham == id).FirstOrDefault();
                if (cartitem != null)
                {
                    cartitem.SoLuong++;
                    db.Entry(cartitem).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    GioHang giohang = new GioHang();
                    giohang.ID_KhachHang = (int)Session["ID_Customer"];
                    giohang.ID_SanPham = (int)id;
                    giohang.SoLuong = 1;
                    db.GioHang.Add(giohang);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult Cart()
        {
            int id_customer = Convert.ToInt32(Session["ID_Customer"]);
            var cartitem = db.GioHang.Where(m => m.ID_KhachHang == id_customer).Include(m => m.SanPham).ToList();
            if (cartitem == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.CountSumProduct = db.GioHang.Count(m=>m.ID_KhachHang == id_customer);
            return PartialView(cartitem);
        }


        public ActionResult DeleteToCart(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var cart_product = db.GioHang.Where(m => m.ID_SanPham == id).FirstOrDefault();
            if(cart_product != null)
            {
                db.GioHang.Remove(cart_product);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CheckOut()
        {
            if(Session["ID_Customer"] != null)
            {
                int id_customer = (int)Session["ID_Customer"];
                ViewBag.Cart = db.GioHang.Where(m => m.ID_KhachHang == id_customer).ToList();
                DiaChiKhachHang adrress = db.DiaChiKhachHang.Where(m => m.ID_KhachHang == id_customer && m.TrangThai == 1).FirstOrDefault();
                ViewBag.PhiVanChuyen = db.PhiVanChuyen.Where(m => m.ID_Tinh == adrress.ID_Tinh && m.ID_Huyen == adrress.ID_Huyen && m.ID_Xa == m.ID_Xa).FirstOrDefault();
                ViewBag.Adress = adrress;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CheckOut(string HoTenNguoiNhan,string SoDienThoai,string Adress,string Shipping)
        {
            if(HoTenNguoiNhan == "" || SoDienThoai == "")
            {
                CheckOut();
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin để thanh toán";
                return View();
            }
            else
            {
                DatHang dh = new DatHang();
                dh.HoTenNguoiNhan = HoTenNguoiNhan;
                dh.SoDienThoai = SoDienThoai;
                dh.ID_KhachHang = (int)Session["ID_Customer"];
                dh.PhiShip = Convert.ToInt32(Shipping);
                dh.DiaChiGiaoHang = Adress;
                dh.NgayLap = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                dh.NgayCapNhat = DateTime.Now;
                dh.TinhTrangDonHang = 9;
                db.DatHang.Add(dh);
                db.SaveChanges();
                int id_customer = (int)Session["ID_Customer"];
                var t = db.GioHang.Where(m => m.ID_KhachHang == id_customer).ToList();
                int tongtien = 0;
                foreach(var item in t)
                {
                    DatHangChiTiet h = new DatHangChiTiet();
                    h.ID_DatHang = dh.ID_DatHang;
                    h.ID_SanPham = item.ID_SanPham;
                    h.SoLuong = item.SoLuong;
                    var product = db.SanPham.Where(m => m.ID_SanPham == item.ID_SanPham).FirstOrDefault();
                    product.SoLuong = product.SoLuong - item.SoLuong;
                    h.TongTien = item.SoLuong * item.SanPham.GiaBan;
                    h.DonGia = item.SanPham.GiaBan;
                    tongtien += item.SoLuong * h.DonGia;
                    db.Entry(product).State = EntityState.Modified;
                    db.DatHangChiTiet.Add(h);
                    db.SaveChanges();
                }
                dh.TongTien = tongtien +  Convert.ToInt32(Shipping);
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                foreach(var item in t)
                {
                    DeleteToCart(item.ID_SanPham);
                }
                //Truyền qua nhập mã khuyến mãi
                Session["DonHangId"] = dh.ID_DatHang;
            }
            return RedirectToAction("CheckSale", "Home");
        }

        [HttpPost]
        public ActionResult AddComment(string rating, string ID_SanPham, string NoiDung,HttpPostedFileBase file)
        {
            if(Session["ID_Customer"] != null)
            {
                if (rating != null && NoiDung != "")
                {
                    BinhLuan comment = new BinhLuan();
                    comment.ID_SanPham = Convert.ToInt32(ID_SanPham);
                    comment.ID_KhachHang = (int)Session["ID_Customer"];
                    comment.NoiDung = NoiDung;
                    if (file == null)
                    {
                        comment.HinhAnh = "/Images/Comment/No_Image.png";
                    }
                    else
                    {
                        comment.HinhAnh = UploadImage(file);
                    }
                    comment.HienThi = 0;
                    if (rating != null)
                    {
                        comment.Star = Convert.ToInt32(rating);
                    }
                    comment.NgayBinhLuan = DateTime.Today;
                    db.BinhLuan.Add(comment);
                    db.SaveChanges();
                }
                else
                {
                    Session["ErrorComment"] = "Bình luận không thành công";
                    return Redirect("/Home/QuickView/" + ID_SanPham);
                }
                return Redirect(Url.Action("/QuickView/" + ID_SanPham));
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        [HttpPost]
        public ActionResult AddReply(string ID_BinhLuan, string NoiDung, HttpPostedFileBase file,string ID_SanPham)
        {
            if(Session["ID_Customer"] != null)
            {
                if (NoiDung != "")
                {
                    PhanHoiBinhLuan commentreply = new PhanHoiBinhLuan();
                    commentreply.ID_BinhLuan = Convert.ToInt32(ID_BinhLuan);
                    commentreply.ID_KhachHang = (int)Session["ID_Customer"];
                    commentreply.NoiDung = NoiDung;
                    if (file == null)
                    {
                        commentreply.HinhAnh = "/Images/Comment/No_Image.png";
                    }
                    else
                    {
                        commentreply.HinhAnh = UploadImage(file);
                    }
                    commentreply.NgayBinhLuan = DateTime.Now;
                    commentreply.HienThi = 1;
                    db.PhanHoiBinhLuan.Add(commentreply);
                    db.SaveChanges();
                }
                else
                {
                    return Redirect(Url.Action("/QuickView/" + ID_SanPham));
                }
                return Redirect(Url.Action("/QuickView/" + ID_SanPham));
            }
            else
            {
                return RedirectToAction("Login");
            }

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

        public ActionResult Store()
        {
            var list_product = db.SanPham.Where(m => m.SoLuong > 0 && m.TinhTrangHienThi == 1).ToList();
            ViewBag.Cate = db.DanhMucSanPham.Where(m => m.TinhTrangHienThi == 1).ToList();
            ViewBag.Brand = db.ThuongHieu.Where(m => m.TinhTrangHienThi == 1).ToList();
            return View(list_product);
        }

        public ActionResult LoadCatelogy()
        {
            var t = db.DanhMucSanPham.ToList();
            return PartialView(t);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(KhachHangLogin customer)
        {
            string PassSHA1 = MaHoaSHA1.SHA1(customer.MatKhau);
            var khachhang = db.KhachHang.Where(m => m.TenDangNhap == customer.TenDangNhap && m.MatKhau == PassSHA1).FirstOrDefault();
            if (khachhang == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View(customer);
            }
            else
            {
                if(khachhang.TinhTrang == 0)
                {
                    ViewBag.Error = "Tài khoản chưa được kích hoạt";
                    return View(customer);
                }
                else
                {
                    Session["ID_Customer"] = khachhang.ID_KhachHang;
                    Session["HoTen"] = khachhang.HoTen;
                    Session["HinhAnh"] = khachhang.HinhAnh;
                    Session["Gmail"] = khachhang.Gmail;
                    return RedirectToAction("Index");
                }
            }
        }

        // đăng ký tài khoản
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(KhachHang khachhang)
        {
            if (ModelState.IsValid)
            {
                var list_customer = db.KhachHang.ToList();
                // kiểm tra tên đăng nhập tồn tại
                foreach(var item in list_customer)
                {
                    if(item.TenDangNhap == khachhang.TenDangNhap)
                    {
                        ViewBag.Error_TDN = "Tên đăng nhập tồn tại";
                        return View(khachhang);
                    }
                    if(item.Gmail == khachhang.Gmail)
                    {
                        ViewBag.Error_Gmail = "Gmail này đã được sử dụng";
                        return View(khachhang);
                    }
                }

                if(DateTime.Now.Year - khachhang.NamSinh.Value.Year < 15)
                {
                    ViewBag.ErrorTuoi = "Khách hàng phải trên 15 tuổi";
                    return View(khachhang);
                }
                var result = UploadImageCustomer(khachhang.DuLieuHinhAnh);
                if (khachhang.DuLieuHinhAnh == null)
                {
                    ViewBag.Error = "Vui lòng chọn hình ảnh";
                    return View(khachhang);
                }

                if(khachhang.MatKhau.Length <= 6)
                {
                    ViewBag.ErrorPass = "Vui lòng nhập mật khẩu trên 6 kí tự";
                    return View(khachhang);
                }

                if (result == "ErrorType")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kiểu";
                    return View(khachhang);
                }
                if (result == "ErrorSize")
                {
                    ViewBag.Error = "Vui lòng chọn đúng kích thước";
                    return View(khachhang);
                }
                khachhang.HinhAnh = result;
                string passSHA1 = MaHoaSHA1.SHA1(khachhang.MatKhau);
                khachhang.MatKhau = passSHA1;
                khachhang.TinhTrang = 0;
                db.KhachHang.Add(khachhang);
                db.SaveChanges();
                Session["KhachHangSignUp"] = khachhang.ID_KhachHang;
                Session["Gmail_SignUp"] = khachhang.Gmail;
                return RedirectToAction("SetAddress", "Home");
            }
            return View(khachhang);
        }

        public ActionResult SetAddress()
        {
            if(Session["KhachHangSignUp"] == null)
            {
                return RedirectToAction("Login");
            }
            // lấy tỉnh 
            var tinh = db.Tinh.ToList();
            return View(tinh);
        }

        [HttpPost]
        public ActionResult SetAddress(string MaTinh, string MaHuyen, string MaXa, string DiaChi)
        {
            if (DiaChi != "" || MaHuyen != "" || MaXa != "")
            {
                DiaChiKhachHang diaChiKhach = new DiaChiKhachHang();
                diaChiKhach.ID_Tinh = Convert.ToInt32(MaTinh);
                diaChiKhach.ID_Huyen = Convert.ToInt32(MaHuyen);
                diaChiKhach.ID_Xa = Convert.ToInt32(MaXa);
                diaChiKhach.DiaChi = DiaChi;
                diaChiKhach.ID_KhachHang = Convert.ToInt32(Session["KhachHangSignUp"]);
                diaChiKhach.TrangThai = 1;
                db.DiaChiKhachHang.Add(diaChiKhach);
                db.SaveChanges();
                ActiveAccountSendMail();
                TempData["ActiveAccount"] = "Kích hoạt tài khoản trong gmail để sử dụng";
                return RedirectToAction("Login");
            }
            else
            {
                SetAddress();
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }
        }

        public ActionResult Huyen(int id)
        {
            return Json(db.Huyen.Where(m => m.matp == id && id != 0).Select(s => new {
                maqh = s.maqh,
                matp = s.matp,
                name = s.name,
            }).ToList(),JsonRequestBehavior.AllowGet);
        }

        public ActionResult Xa(int id)
        {
            return Json(db.Xa.Where(m => m.maqh == id && id != 0).Select(t => new {
                xaid = t.xaid,
                maqh = t.maqh,
                name = t.name,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        // upload ảnh khách hành
        public string UploadImageCustomer(HttpPostedFileBase file)
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

        // đăng xuất
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        // LỌC THEO DANH MỤC 
        public ActionResult FitterCatelogy(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var product = db.SanPham.Where(m => m.DanhMucSanPhamCon.DanhMucSanPham.ID_DanhMuc == id && m.SoLuong > 0 && m.TinhTrangHienThi == 1).ToList();
            ViewBag.CateChid = db.DanhMucSanPhamCon.Where(m => m.ID_DanhMucSanPham == id).ToList();
            ViewBag.Brand = db.ThuongHieu.ToList();
            ViewBag.YeuThich = db.YeuThich.ToList();
            ViewBag.Products = product;
            Session["id_DanhMuc"] = id;
            return View(product);
        }

        [HttpPost]
        public ActionResult FitterCateChild(int ?id,string Brand, string cateChild, string minValue ,string maxValue)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.YeuThich = db.YeuThich.ToList();
                int ThuongHieuid = Convert.ToInt32(Brand);
                int DanhMucConid = Convert.ToInt32(cateChild);
                /*
                int start_price = Convert.ToInt32(minValue);
                int end_price = Convert.ToInt32(maxValue);*/
                int id_DanhMuc = (int) Session["id_DanhMuc"];
                ViewBag.Brand = db.ThuongHieu.ToList();
                var list_product = db.SanPham.Where(m => m.DanhMucSanPhamCon.DanhMucSanPham.ID_DanhMuc == id && m.TinhTrangHienThi == 1 && m.SoLuong > 0).ToList();
                ViewBag.Products = list_product;
                if (Brand == null && cateChild == null && maxValue == "" && minValue == "")
                {
                    return Redirect("/Home/FitterCatelogy/" + id);
                }
                else
                {
                    if (Brand != null)
                    {
                        var list_products = list_product.Where(m => m.ID_ThuongHieu == ThuongHieuid && m.SoLuong > 0 && m.TinhTrangHienThi == 1).ToList();
                        return View(list_products);
                    }
                    else if (cateChild != null)
                    {
                        var list_products = list_product.Where(m => m.ID_DanhMucSanPhamCon == DanhMucConid && m.SoLuong > 0 && m.TinhTrangHienThi == 1).ToList();
                        return View(list_products);
                    }
                    else if (cateChild != null && Brand != null)
                    {
                        var list_products = list_product.Where(m => m.ID_ThuongHieu == ThuongHieuid || m.ID_DanhMucSanPhamCon == DanhMucConid && m.SoLuong > 0 && m.TinhTrangHienThi == 1).ToList();
                        return View(list_products);
                    }
                }
            }
            return View();
        }


        public ActionResult OrderOfMe()
        {
            if(Session["ID_Customer"] != null)
            {
                int id_customer = (int)Session["ID_Customer"];
                var order = db.DatHang.Where(m => m.ID_KhachHang == id_customer).ToList();
                ViewBag.DatHangChiTiet = db.DatHangChiTiet.ToList();
                return View(order);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult OrderDetail(int id)
        {
            var order = db.DatHangChiTiet.Where(m => m.ID_DatHang == id).Include(m => m.SanPham).Include(m => m.DatHang).ToList();
            return View(order);
        }


        public ActionResult CancelOrder(int id)
        {
            var order = db.DatHang.Where(m => m.ID_DatHang == id).FirstOrDefault();
            var order_details = db.DatHangChiTiet.Where(m => m.ID_DatHang == id).ToList();
            order.TinhTrangDonHang = 2;
            foreach (var item in order_details)
            {
                // lấy sản phẩm trong đặt hàng chi tiết
                var product = db.SanPham.Where(m => m.ID_SanPham == item.ID_SanPham).FirstOrDefault();
                // cập nhật số lượng trong kho trong bảng Đặt hàng chi tiết
                product.SoLuong += item.SoLuong;
                // update và Lưu
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("OrderOfMe");
        }


        public ActionResult Profiles()
        {
            if (Session["ID_Customer"] != null)
            {
                int id_customer = (int)Session["ID_Customer"];
                var customer = db.KhachHang.Where(m => m.ID_KhachHang == id_customer).FirstOrDefault();
                return View(customer);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profiles(string OldPass, string OldImage, DateTime OldYear, [Bind(Include = "ID_KhachHang,HoTen,SoDienThoai,GioiTinh,NamSinh,Gmail,TenDangNhap,MatKhau,HinhAnh,DuLieuHinhAnh")] KhachHang khachhang)
        {
            if (ModelState.IsValid)
            {
                if (khachhang.DuLieuHinhAnh != null && khachhang.NamSinh == null)
                {
                    var result = UploadImage(khachhang.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    DeleteImage(OldImage);
                    khachhang.HinhAnh = result;
                    khachhang.NamSinh = OldYear;
                }
                else if (khachhang.NamSinh != null && khachhang.DuLieuHinhAnh == null)
                {
                    khachhang.NamSinh = khachhang.NamSinh;
                    khachhang.HinhAnh = OldImage;
                }
                else if (khachhang.NamSinh != null && khachhang.DuLieuHinhAnh != null)
                {
                    var result = UploadImage(khachhang.DuLieuHinhAnh);
                    if (result == "ErrorType")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kiểu";
                        // nếu như người dùng truy cập vào đây và bấm cập nhật thì sẻ mất đi đường dẩn củ
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    if (result == "ErrorSize")
                    {
                        ViewBag.Error = "Vui lòng chọn đúng kích thước";
                        khachhang.HinhAnh = OldImage;
                        return View(khachhang);
                    }
                    DeleteImage(OldImage);
                    khachhang.HinhAnh = result;
                    khachhang.NamSinh = khachhang.NamSinh;
                }
                else
                {
                    khachhang.NamSinh = OldYear;
                    khachhang.HinhAnh = OldImage;
                }
                khachhang.TinhTrang = 1;
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();
                Session["HinhAnh"] = khachhang.HinhAnh;
                return RedirectToAction("DangXuat");
            }
            return View(khachhang);
        }

        public void DeleteImage(string path)
        {
            string oldFilePath = Server.MapPath("~/" + path);
            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
        }

        public ActionResult AdressOfMe()
        {
            if(Session["ID_Customer"] != null)
            {
                int id_cusomer = (int)Session["ID_Customer"];
                var adress = db.DiaChiKhachHang.Where(m => m.ID_KhachHang == id_cusomer).ToList();
                return View(adress);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // tạo mới địa chỉ
        public ActionResult AddAdress()
        {
            if (Session["ID_Customer"] == null)
            {
                return RedirectToAction("Login");
            }
            // lấy tỉnh 
            var tinh = db.Tinh.ToList();
            return View(tinh);
        }

        [HttpPost]
        public ActionResult AddAdress(string MaTinh, string MaHuyen, string MaXa, string DiaChi)
        {
            if (DiaChi != "" || MaHuyen != "" || MaXa != "")
            {
                DiaChiKhachHang diaChiKhach = new DiaChiKhachHang();
                diaChiKhach.ID_Tinh = Convert.ToInt32(MaTinh);
                diaChiKhach.ID_Huyen = Convert.ToInt32(MaHuyen);
                diaChiKhach.ID_Xa = Convert.ToInt32(MaXa);
                diaChiKhach.DiaChi = DiaChi;
                diaChiKhach.ID_KhachHang = Convert.ToInt32(Session["ID_Customer"]);
                diaChiKhach.TrangThai = 0;
                db.DiaChiKhachHang.Add(diaChiKhach);
                db.SaveChanges();
                return RedirectToAction("AdressOfMe");
            }
            else
            {
                SetAddress();
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }
        }

        public ActionResult getHuyen(int id)
        {
            return Json(db.Huyen.Where(m => m.matp == id && id != 0).Select(s => new {
                maqh = s.maqh,
                matp = s.matp,
                name = s.name,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult getXa(int id)
        {
            return Json(db.Xa.Where(m => m.maqh == id && id != 0).Select(t => new {
                xaid = t.xaid,
                maqh = t.maqh,
                name = t.name,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAdress(int id)
        {
            int id_customer = (int)Session["ID_Customer"];
            var item = db.DiaChiKhachHang.Where(m => m.ID_DiaChi == id).FirstOrDefault();
            item.TrangThai = 1;
            db.Entry(item).State = EntityState.Modified;

            // cập nhật lại các địa chỉ bằng kích hoạt
            var items = db.DiaChiKhachHang.Where(m => m.ID_KhachHang == id_customer && m.ID_DiaChi != item.ID_DiaChi).ToList();
            foreach(var diachi in items)
            {
                diachi.TrangThai = 0;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.SaveChanges();

            return RedirectToAction("AdressOfMe");
        }

        public ActionResult DeleteAdress(int id)
        {
            var item = db.DiaChiKhachHang.Where(m => m.ID_DiaChi == id).FirstOrDefault();
            int id_customer = (int)Session["ID_Customer"];
            var checkItem = db.DiaChiKhachHang.Where(m => m.ID_KhachHang == id_customer).ToList();
            if(checkItem.Count() < 2)
            {
                ViewBag.Error = "Phải có ít nhất một địa chỉ";
                AdressOfMe();
                return RedirectToAction("AdressOfMe");
            }
            db.DiaChiKhachHang.Remove(item);
            db.SaveChanges();
            return RedirectToAction("AdressOfMe");
        }
        // check sale
        public ActionResult CheckSale()
        {
            return View();
        }

        [HttpPost] 
        public ActionResult CheckSale(string Key)
        {
            var sale = db.MaGiamGia.Where(m => m.KeyGiamGia == Key).FirstOrDefault();
            if(sale != null)
            {
                if(DateTime.Now >= sale.NgayBatDau && DateTime.Now <= sale.NgayKetThuc && sale.TinhTrangHienThi == 1)
                {
                    int id_dathang = (int)Session["DonHangId"];
                    var order = db.DatHang.Where(m => m.ID_DatHang == id_dathang).FirstOrDefault();
                    if(order.PhiShip > sale.TienGiam)
                    {
                        order.PhiShip -= sale.TienGiam;
                        order.TongTien -= sale.TienGiam;
                    }
                    else if(order.PhiShip <= sale.TienGiam)
                    {
                        order.PhiShip = 0;
                        order.TongTien -= sale.TienGiam;
                    }
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ThankYou");
                }
                else
                {
                    CheckSale();
                    ViewBag.Error = "Mã này đã hết hạn sử dụng";
                    return View();
                }
                
            }
            else
            {
                CheckSale();
                ViewBag.Error = "Mã này không tồn tại";
                return View();
            }
        }


        public ActionResult ThankYou()
        {
            SendMail();
            return View();
        }

        [HttpPost]
        public ActionResult Search(string KeySearch)
        {
            if (KeySearch != null)
            {
                var result = db.SanPham.Where(m => m.TenSanPham.Contains(KeySearch) || m.DanhMucSanPhamCon.TenDMSPC.Contains(KeySearch) || m.ThuongHieu.TenThuongHieu.Contains(KeySearch) && m.SoLuong > 0 && m.TinhTrangHienThi == 1).ToList();
                int ?customer = (int)Session["ID_Customer"];
                if(customer != null)
                {
                    ViewBag.ListComment = db.BinhLuan.ToList();
                    // LẤY  DANH SÁCH YÊU THÍCH CỦA KHÁCH HÀNG
                    ViewBag.YeuThich = db.YeuThich.Where(m => m.Id_KhachHang == customer).ToList();
                    return View(result);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LoadFooter()
        {
            var footer = db.Footer.ToList();
            ViewBag.CateLogy = db.DanhMucSanPham.ToList();
            ViewBag.CateChild = db.DanhMucSanPhamCon.ToList();
            return PartialView(footer);
        }

        public ActionResult ViewFooter (int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var footerView = db.Footer.Where(m => m.ID_Footer == id).FirstOrDefault();
            return View(footerView);
        }


        public ActionResult ViewSales(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            int customer = (int)Session["ID_Customer"];
            var item = db.Slider.Where(m => m.Id_Slider == id).FirstOrDefault();
            ViewBag.products = db.SanPhamLienQuan.Where(m => m.ID_Slider == id).ToList();
            ViewBag.ListComment = db.BinhLuan.ToList();
            ViewBag.YeuThich = db.YeuThich.Where(m => m.Id_KhachHang == customer).ToList();
            return View(item);
        }

        public ActionResult ViewBaiViet(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var baiViet = db.BaiViet.Where(m => m.Id_BaiViet == id).FirstOrDefault();
            baiViet.LuotXem++;
            db.Entry(baiViet).State = EntityState.Modified;
            db.SaveChanges();
            return View(baiViet);
        }

        public ActionResult DoiMatKhau()
        {
            if(Session["ID_Customer"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int customer = (int)Session["ID_Customer"];
            var customer_change = db.KhachHang.Where(m => m.ID_KhachHang == customer).FirstOrDefault();
            return View(customer_change);
        }

        [HttpPost]
        public ActionResult DoiMatKhau(string OldPass, string MatKhauCu, KhachHang khachhang)
        {
            if (khachhang.MatKhau != null && OldPass == MaHoaSHA1.SHA1(MatKhauCu))
            {
                khachhang.MatKhau = MaHoaSHA1.SHA1(khachhang.MatKhau);
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DangXuat", "Home");
            }
            else
            {
                ViewBag.Error = "Mật khẩu sai";
                return View(khachhang);
            }
        }

        // gửi gmail
        public void SendMail()
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("phungtuandatvtt@gmail.com");
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(mail.From.Address, "hbtdvpxajgweuukp");
            smtp.Host = "smtp.gmail.com";
            mail.IsBodyHtml = true;
            string gmail = (string)Session["Gmail"];
            //recipient
            mail.To.Add(new MailAddress(gmail));
            mail.IsBodyHtml = true;
            int id_dathang = (int)Session["DonHangId"];
            string path = "https://localhost:44345/Home/XacNhanDonHang/" + id_dathang;
            var info_donhang = db.DatHang.Where(m => m.ID_DatHang == id_dathang).FirstOrDefault();
            string list_order = "";
            int stt = 1;
            foreach (var item in db.DatHangChiTiet.Where(m => m.ID_DatHang == id_dathang).ToList())
            {
                list_order += "<tr><td>"+stt+"</td><td>" + item.SanPham.TenSanPham + "</td><td>" + item.DonGia.ToString("N0") + "<span>đ</span></td><td>" + item.SoLuong + "</td>" +
                    "<td>" + item.TongTien.ToString("N0") + "<span></span></td></tr>";
                stt++;
            }
            string st = "<p><b style=\"color:red;text-align:center;font-weight:bold;\">VI TÍNH TẤN PHÁT</b><p>"
                + "<p><b style=\"color:red;text-align:center;font-weight:bold;\">ĐỊA CHỈ: Thửa 365,  Xẻo Trôm 1, 2 - Mỹ Phước - TP.Long Xuyên</b><p>"
                + "<p>Họ tên: <b>" + info_donhang.HoTenNguoiNhan + "</b><p>"
                + "<p>Ngày đặt:<b>" + info_donhang.NgayLap.ToShortDateString() + "</b><p>"
                + "<p>Địa chỉ giao hàng: <b>" + info_donhang.DiaChiGiaoHang + "</b><p>"
                + "<p>Phí vận chuyển: <b>" + info_donhang.PhiShip.Value.ToString("N0") + "<span>đ</span></b><p>"
                 + "<p>Số điện thoại: <b>" + info_donhang.SoDienThoai + "</b><p>"
                 + "<p>Tổng tiền: <b>" + info_donhang.TongTien.ToString("N0") + "<span>đ</span></b><p>"
                 + "<a href=" + path + " class=\"btn btn-primary\">Xác nhận đơn hàng</a>"
                 + "<table><tr>"
                 + "</tr><td>STT</td><td>Tên sản phẩm</td><td>Đơn giá</td><td>Số lượng</td><td>Tổng tiền</td></tr>"
                 + string.Join(Environment.NewLine,list_order)
                + "</table>";
            mail.Subject = "XÁC NHẬN ĐƠN HÀNG TỪ VI TÍNH TẤN PHÁT";
            mail.Body = st;
            smtp.Send(mail);
        }

        public ActionResult XacNhanDonHang(int id)
        {
            var order = db.DatHang.Where(m => m.ID_DatHang == id).FirstOrDefault();
            // nếu như đơn hàng đang chờ xác nhận thì cập nhật còn không thì không
            if(order.TinhTrangDonHang == 9)
            {
                order.TinhTrangDonHang = 0;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // active Mail
        public void ActiveAccountSendMail()
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("phungtuandatvtt@gmail.com");
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(mail.From.Address, "hbtdvpxajgweuukp");
            smtp.Host = "smtp.gmail.com";
            mail.IsBodyHtml = true;
            string gmail = (string)Session["Gmail_SignUp"];
            //recipient
            mail.To.Add(new MailAddress(gmail));
            mail.IsBodyHtml = true;
            string path = "https://localhost:44345/Home/ActiveAccount/" + Convert.ToInt32(Session["KhachHangSignUp"]);
            string st = "Xác nhận tài khoản <a href="+path+">tại đây</a>";
            mail.Subject = "XÁC NHẬN KÍCH HOẠT TÀI KHOẢN TỪ VI TÍNH TẤN PHÁT";
            mail.Body = st;
            smtp.Send(mail);
            Session.Remove("Gmail_SignUp");
            Session.Remove("KhachHangSignUp");
        }


        public ActionResult ActiveAccount(int id)
        {
            var customer = db.KhachHang.Where(m => m.ID_KhachHang == id).FirstOrDefault();
            if (customer.TinhTrang == 0)
            {
                customer.TinhTrang = 1;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
            
        }



        public void Send_Forgot_password(string Gmail,int id)
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("phungtuandatvtt@gmail.com");
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(mail.From.Address, "hbtdvpxajgweuukp");
            smtp.Host = "smtp.gmail.com";
            mail.IsBodyHtml = true;
            //recipient
            mail.To.Add(new MailAddress(Gmail));
            mail.IsBodyHtml = true;
            string path = "https://localhost:44345/Home/ChangePassword/" + id;
            string st = "Đổi mật khẩu cho tài khoản <a href=" + path + ">tại đây</a>";
            mail.Subject = "ĐỔI MẬT KHẨU CHO TÀI KHOẢN " ;
            mail.Body = st;
            smtp.Send(mail);
        }


        public ActionResult Forgot_password()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot_password(string TenDangNhap)
        {
            if(TenDangNhap != null)
            {
                var customer = db.KhachHang.Where(m => m.TenDangNhap == TenDangNhap).FirstOrDefault();
                if (customer != null)
                {
                    Send_Forgot_password(customer.Gmail,customer.ID_KhachHang);
                    TempData["ThongBao"] = "Nhận đường dẩn thay đổi mật khẩu ở gmail đã đăng ký";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập này không tồn tại";
                    Forgot_password();
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập của bạn";
                Forgot_password();
                return View();
            }
        }
        // đổi mật khẩu sao xác nhận
        public ActionResult ChangePassword(int ?id)
        {
            if(id == null)
            {
                return RedirectToAction("Login", "Home");
            }
            Session["IDChangePass"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string PassNew, string CheckPass)
        {
            int id = Convert.ToInt32(Session["IDChangePass"]);
            var customer = db.KhachHang.Where(m => m.ID_KhachHang == id).FirstOrDefault();
            if(customer.ID_KhachHang.ToString() != null)
            {
                if(PassNew == "" || CheckPass == "" || PassNew == "" && CheckPass == "")
                {
                    ViewBag.Error = "Vui lòng nhập mật khẩu mới và xác nhận";
                    return View("ChangePassword");
                }
                else if(PassNew != CheckPass)
                {
                    ViewBag.Error = "Vui lòng nhập mật mật khẩu và xác nhận mật khẩu trùng khớp";
                    return View("ChangePassword");
                }
                else
                {
                    customer.MatKhau = MaHoaSHA1.SHA1(PassNew);
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Login", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult AddYeuThichCate(int? id)
        {
            if (Session["ID_Customer"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var item = db.YeuThich.Where(m => m.Id_SanPham == id).FirstOrDefault();
                if (item != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                YeuThich yeuthich = new YeuThich();
                yeuthich.Id_KhachHang = (int)Session["ID_Customer"];
                yeuthich.Id_SanPham = (int)id;
                db.YeuThich.Add(yeuthich);
                db.SaveChanges();
                Session["CountYeuThich"] = db.YeuThich.Count();
                ViewBag.AddThanhCong = "Thêm vào thành công";
                return Redirect("/Home/FitterCatelogy/" + Convert.ToInt32(Session["id_DanhMuc"]));
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult DeleteYeuThichCate(int? id)
        {
            if (Session["ID_Customer"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                int idCustomer = (int)Session["ID_Customer"];
                var item = db.YeuThich.Where(m => m.Id_YeuThich == id).FirstOrDefault();
                db.YeuThich.Remove(item);
                db.SaveChanges();
                Session["CountYeuThich"] = db.YeuThich.Count();
                return Redirect("/Home/FitterCatelogy/" + Convert.ToInt32(Session["id_DanhMuc"]));
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
    }
}