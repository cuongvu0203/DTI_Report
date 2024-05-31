using DTI_Report.Infrastructure.Entities;

namespace DTI_Report.Infrastructure.Services
{
    public interface IDtiService
    {
        Task<string> GetMaCapHocByTruong(string maSo, int maNamHoc, string maTruong);
        Task<List<DanhGiaThuongXuyen>> DTIDanhGiaThuongXuyenServiceAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc);
        Task<List<SoLuongHocLieuSoHoa>> DTISoLuongHocLieuSoHoaServiceAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc);
        Task<IEnumerable<DM_KHOI_TN>> GetDanhMucKhoiAsync(string maCapHoc);
        Task<IEnumerable<DM_MON_HOC_TN>> GetDanhMucMonHocAsync(string maSo, string maCapDonVi, string maTruong, string maKhoi, string maCapHoc);
    }
}
