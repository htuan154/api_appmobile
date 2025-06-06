using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSV_API.Model
{
    public class LichSuYeuCau
    {
        [Key]
        public string Ma_LSYC { get; set; }

        public string TrangThaiMoi { get; set; }
        public string TrangThaiCu { get; set; }

        public string Ma_YC { get; set; }

        [ForeignKey("Ma_YC")]
        public virtual YeuCau YeuCau { get; set; }
    }
}
