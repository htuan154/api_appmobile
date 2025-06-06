using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
	public class TaiKhoanSinhVien
	{
		[Key]
		public string Ma_TKSV { get; set; }
		public string TenDangNhap { get; set; }
		public string MatKhau { get; set; }
		public string Ma_SV { get; set; }

		[ForeignKey("Ma_SV")]
		[JsonIgnore] // Ngăn serialize/deserialize navigation property
		public virtual SinhVien? SinhVien { get; set; }

		[JsonIgnore] // Ngăn serialize/deserialize navigation property
		public virtual ICollection<YeuCau>? YeuCaus { get; set; } = new List<YeuCau>();
	}
}