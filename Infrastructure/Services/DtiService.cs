using DTI_Report.Infrastructure.Data;
using DTI_Report.Infrastructure.Entities;

namespace DTI_Report.Infrastructure.Services
{
    public class DtiService : IDtiService
    {
        private readonly IDtiRepository _dtiRepository;
        public DtiService(IDtiRepository dtiRepository)
        {
            _dtiRepository = dtiRepository;
        }
        public Task<List<DanhGiaThuongXuyen>> DTIDanhGiaThuongXuyenServiceAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc)
        {
            return _dtiRepository.GetDanhGiaThuongXuyenAsync(maSo, maNamHoc, maCapDonVi, maTruong, maKhoi, maMonHoc);
        }

        public Task<List<SoLuongHocLieuSoHoa>> DTISoLuongHocLieuSoHoaServiceAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc)
        {
            return _dtiRepository.GetSoLuongHocLieuSoHoaAsync(maSo, maNamHoc, maCapDonVi, maTruong, maKhoi, maMonHoc);
        }

        public Task<IEnumerable<DM_KHOI_TN>> GetDanhMucKhoiAsync(string maCapHoc)
        {
            return _dtiRepository.GetDanhMucKhoiAsync(maCapHoc);
        }

        public Task<IEnumerable<DM_MON_HOC_TN>> GetDanhMucMonHocAsync(string maSo, string maCapDonVi, string maTruong, string maKhoi, string maCapHoc)
        {
            return _dtiRepository.GetDanhMucMonHocAsync(maSo, maCapDonVi, maTruong, maKhoi, maCapHoc);
        }

        public Task<string> GetMaCapHocByTruong(string maSo, int maNamHoc, string maTruong)
        {
            return _dtiRepository.GetMaCapHocByMaTruongAsync(maSo, maNamHoc, maTruong);
        }
    }
}
