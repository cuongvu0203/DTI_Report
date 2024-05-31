using DTI_Report.Infrastructure.Entities;

namespace DTI_Report.Infrastructure.Data
{
    public interface IDtiRepository
    {
        Task<string> GetMaCapHocByMaTruongAsync(string maSo, int maNamHoc, string maTruong);
        Task<List<DanhGiaThuongXuyen>> GetDanhGiaThuongXuyenAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc);
        Task<List<SoLuongHocLieuSoHoa>> GetSoLuongHocLieuSoHoaAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc);
        Task<IEnumerable<DM_KHOI_TN>> GetDanhMucKhoiAsync(string maCapHoc);
        Task<IEnumerable<DM_MON_HOC_TN>> GetDanhMucMonHocAsync(string maSo, string maCapDonVi, string maTruong, string maKhoi, string maCapHoc);
    }
}
