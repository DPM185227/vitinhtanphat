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

    public partial class MaGiamGia
    {

        [Display(Name = "Mã Giảm Giá")]
        public int Id_MaGiamGia { get; set; }

        [Display(Name = "Code")]
        [Required(ErrorMessage ="Vui lòng nhập Code")]
        public string KeyGiamGia { get; set; }

        [Display(Name = "Giá Tiền Gỉảm")]
        [Required(ErrorMessage ="Vui lòng nhập giá tiền giảm")]
        public int TienGiam { get; set; }

        [Display(Name = "Ngày Bắt Đầu")]
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        public System.DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày Hết Hạn")]
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        public System.DateTime NgayKetThuc { get; set; }

        [Display(Name = "Tình Trạng")]
        [Required(ErrorMessage = "Vui lòng chọn tình trạng")]
        public int TinhTrangHienThi { get; set; }
    }
}
