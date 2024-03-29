﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StoreTanPhat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            this.BinhLuan = new HashSet<BinhLuan>();
            this.DatHang = new HashSet<DatHang>();
            this.DiaChiKhachHang = new HashSet<DiaChiKhachHang>();
            this.GioHang = new HashSet<GioHang>();
            this.PhanHoiBinhLuan = new HashSet<PhanHoiBinhLuan>();
            this.YeuThich = new HashSet<YeuThich>();
        }

        [Display(Name = "Khách Hàng")]
        public int ID_KhachHang { get; set; }

        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; }

        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Vui lòng nhập đúng định dạng")]
        [Required(ErrorMessage = "You must provide a phone number")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Giới Tính")]
        public int GioiTinh { get; set; }


        [Display(Name = "Năm Sinh")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Vui lòng chọn năm sinh")]
        public System.DateTime ?NamSinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Gmail")]
        [EmailAddress(ErrorMessage = "Địa chỉ Gmail không đúng")]
        public string Gmail { get; set; }

        [Display(Name = "Tên Đăng Nhập")]
        public string TenDangNhap { get; set; }


        [Display(Name = "Mật Khẩu")]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Mật khẩu phải có từ 6 đến 20 ký tự và chứa một ký tự viết hoa, một ký tự viết thường, một chữ số và một ký tự đặc biệt.")]
        public string MatKhau { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string HinhAnh { get; set; }

        [Display(Name = "Tình Trạng")]
        public int TinhTrang { get; set; }

        public HttpPostedFileBase DuLieuHinhAnh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BinhLuan> BinhLuan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatHang> DatHang { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiaChiKhachHang> DiaChiKhachHang { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GioHang> GioHang { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhanHoiBinhLuan> PhanHoiBinhLuan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YeuThich> YeuThich { get; set; }
    }

    public class KhachHangLogin
    {
        [Display(Name = "Tên Đăng Nhập")]
        public string TenDangNhap { get; set; }
        
        [Display(Name ="Mật Khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}
