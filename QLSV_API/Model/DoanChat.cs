using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
    public class DoanChat
    {
        [Key]
        public string Ma_DC { get; set; }
        public string Ma_YC { get; set; }
        public DateTime NgayTao { get; set; }
        public string MaNguoiGui { get; set; }
        public string NoiDung { get; set; }

        [ForeignKey("Ma_YC")]
		[JsonIgnore]
		public virtual YeuCau? YeuCau { get; set; }
    }

}
