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

    public partial class YeuThich
    {
        [Display(Name = "Yêu Thích")]
        public int Id_YeuThich { get; set; }

        [Display(Name = "Khách Hàng")]
        public int Id_KhachHang { get; set; }

        [Display(Name = "Sản Phẩm")]
        public int Id_SanPham { get; set; }
    
        public virtual KhachHang KhachHang { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
