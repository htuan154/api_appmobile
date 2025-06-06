using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
	public class XuLyYeuCau
	{
		public string Ma_YC { get; set; }
		public string Ma_TK { get; set; }
		public DateTime NgayXuLy { get; set; }

		public string TrangThai_cu { get; set; }
		public string TrangThai_moi { get; set; }

		[ForeignKey("Ma_YC")]
		[JsonIgnore]
		public YeuCau? YeuCau { get; set; }

		[ForeignKey("Ma_TK")]
		[JsonIgnore]
		public TaiKhoan? TaiKhoan { get; set; }
	}

}
