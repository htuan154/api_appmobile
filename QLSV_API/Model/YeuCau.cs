using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
    public class YeuCau
    {
        [Key]
        public string Ma_YC { get; set; }
        public string Ma_loaiYC { get; set; }

        [ForeignKey("Ma_loaiYC")]
        [JsonIgnore] // Ngăn serialize/deserialize navigation property
        public virtual LoaiYeuCau? LoaiYeuCau { get; set; }

        public string Ma_TKSV { get; set; }

        [ForeignKey("Ma_TKSV")]
        [JsonIgnore] // Ngăn serialize/deserialize navigation property
        public virtual TaiKhoanSinhVien? TaiKhoanSinhVien { get; set; }

        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }

        [JsonIgnore] // Ngăn serialize/deserialize navigation property
        public virtual ICollection<XuLyYeuCau>? XuLyYeuCaus { get; set; } = new List<XuLyYeuCau>();
    }
}