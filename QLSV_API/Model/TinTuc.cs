using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
    public class TinTuc
    {
        [Key]
        public string Ma_TT { get; set; }

        public string Ma_TK { get; set; }

		[ForeignKey("Ma_TK")]
		[JsonIgnore]
		public virtual TaiKhoan? TaiKhoan { get; set; }

		public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
