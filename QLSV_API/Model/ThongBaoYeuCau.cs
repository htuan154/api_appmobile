using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLSV_API.Model
{
	public class ThongBaoYeuCau
	{
		[Key]
		public string Ma_TBYC { get; set; }

		public string Ma_YC { get; set; }

		[ForeignKey("Ma_YC")]
		public virtual YeuCau? YeuCau { get; set; }

		public string Ma_TKSV { get; set; }

		[ForeignKey("Ma_TKSV")]
		public virtual TaiKhoanSinhVien? TaiKhoanSinhVien { get; set; }

		public string NoiDung { get; set; }

		public DateTime NgayThongBao { get; set; }
		public string TrangThai { get; set; }
	}
}
