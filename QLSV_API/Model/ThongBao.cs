using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QLSV_API.Model
{
    public class ThongBao
    {
        // Sử dụng Composite Key (khóa chính hợp nhất)
        [Key]
        [Column(Order = 0)]
        public string Ma_TT { get; set; }

        [Key]
        [Column(Order = 1)]
        public string Ma_TKSV { get; set; }

        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }

        // Chỉ cần khai báo mối quan hệ với TinTuc và TaiKhoanSinhVien 
        [ForeignKey("Ma_TT")] // Chỉ ra khóa ngoại đối với TinTuc
		[JsonIgnore]
		[ValidateNever]
		public virtual TinTuc TinTuc { get; set; }

        [ForeignKey("Ma_TKSV")] // Chỉ ra khóa ngoại đối với TaiKhoanSinhVien
		[JsonIgnore]
		[ValidateNever]
		public virtual TaiKhoanSinhVien TaiKhoanSinhVien { get; set; }
    }
}
