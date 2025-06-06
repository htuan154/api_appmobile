using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLSV_API.Model
{
    public class LoaiYeuCau
    {
        [Key]
        public string Ma_loaiYC { get; set; }
        public string Ten_loaiYC { get; set; }

		[JsonIgnore]
		public ICollection<YeuCau>? YeuCaus { get; set; }
    }

}
