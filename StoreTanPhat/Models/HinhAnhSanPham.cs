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

    public partial class HinhAnhSanPham
    {
        [Display(Name = "Hình Ảnh Sản Phẩm")]
        public int ID_HinhAnhSP { get; set; }

        [Display(Name = "Sản Phẩm")]
        public int ID_SanPham { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string HinhAnh { get; set; }

        public List<HttpPostedFileBase> DuLieuHinhAnh { get; set; }

        public virtual SanPham SanPham { get; set; }
    }
}
