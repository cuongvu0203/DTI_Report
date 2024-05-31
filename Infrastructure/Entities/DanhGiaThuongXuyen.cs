namespace DTI_Report.Infrastructure.Entities
{
    public class DanhGiaThuongXuyen
    {
        public int Id { get; set; }
        public string? MA_TRUONG { get; set; }
        public string? MA_KHOI { get; set; }
        public string? TEN_KHOI { get; set; }
        public string? MA_MON_HOC_HE_THONG { get; set; }
        public string? TEN_MON_HOC { get; set; }
        public int MOT_LUA_CHON { get; set; }
        public int NHIEU_LUA_CHON { get; set; }
        public int DUNG_SAI { get; set; }
        public int DIEN_VAO_CHO_TRONG { get; set; }
        public int TU_LUAN { get; set; }
        public int DANG_KHAC { get; set; }
        public int TONG { get; set; }
    }
}
