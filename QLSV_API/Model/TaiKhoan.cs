using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSV_API.Model
{
	public class TaiKhoan
	{
		[Key]
		public string Ma_TK { get; set; }

		public string TenDangNhap { get; set; }
		public string MatKhau { get; set; }

		public string Ma_NV { get; set; }

		// Cho phép NhanVien null
		[ForeignKey("Ma_NV")]
		public virtual NhanVien? NhanVien { get; set; }

		public string Ma_Loai { get; set; }

		// Cho phép LoaiTaiKhoan null
		[ForeignKey("Ma_Loai")]
		public virtual LoaiTaiKhoan? LoaiTaiKhoan { get; set; }
	}
}
