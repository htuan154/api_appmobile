using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
	public class ThongBaoChatYeuCau
	{
		[Key]
		public string Ma_TBCYC { get; set; }

		public string Ma_YC { get; set; }

		[ForeignKey("Ma_YC")]
		public virtual YeuCau? YeuCau { get; set; }

		public string Ma_TK { get; set; }

		[ForeignKey("Ma_TK")]
		public virtual TaiKhoan? TaiKhoan { get; set; }

		public string NoiDung { get; set; }

		public DateTime NgayThongBao { get; set; }
		public string TrangThai { get; set; }
	}
}
