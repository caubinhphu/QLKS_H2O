﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLKS_H2O.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class PHIEU_THUEPHONG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PHIEU_THUEPHONG()
        {
            this.CHITIET_THUEDICHVU = new HashSet<CHITIET_THUEDICHVU>();
            this.CHITIET_THUEPHONG = new HashSet<CHITIET_THUEPHONG>();
        }

        [DisplayName("Số phiếu")]
        [Required(ErrorMessage = "Chưa nhập số phiếu")]
        public string SO_PHIEU { get; set; }
        [DisplayName("Khách thuê")]
        [Required(ErrorMessage = "Chưa nhập mã khách")]
        public string MAKHACH { get; set; }
        [DisplayName("Nhân viên")]
        [Required(ErrorMessage = "Chưa nhập nhân viên")]
        public string MA_NHANVIEN { get; set; }
        [DisplayName("Ngày lập phiếu")]
        [Required(ErrorMessage = "Chưa nhập ngày lập phiếu")]
        public System.DateTime NGAYLAP_PHIEU { get; set; }
        [DisplayName("Ngày đến")]
        [Required(ErrorMessage = "Chưa nhập ngày đến")]
        public System.DateTime NGAYDEN { get; set; }
        [DisplayName("Ngày đi")]
        [Required(ErrorMessage = "Chưa nhập ngày đi")]
        public System.DateTime NGAYDI { get; set; }
        [DisplayName("Đã trả phòng")]
        public Nullable<bool> DATRAPHONG { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIET_THUEDICHVU> CHITIET_THUEDICHVU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIET_THUEPHONG> CHITIET_THUEPHONG { get; set; }
        public virtual KHACH KHACH { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
    }
}
