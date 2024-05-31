using DTI_Report.Infrastructure.Entities;
using DTI_Report.Infrastructure.Helper;
using DTI_Report.Infrastructure.Services;
using DTI_Report.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace DTI_Report.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;
        private readonly IDtiService _dtiService;
        public string maSo = string.Empty;
        public int maNamHoc = 0;
        public string maCapDonVi = string.Empty;
        public HomeController(ILogger<HomeController> logger, IConfiguration config, IDtiService dtiService)
        {
            _logger = logger;
            _config = config;
            _dtiService = dtiService;
            maSo = _config["CauHinhSo:MaSo"];
            maNamHoc = int.Parse(_config["CauHinhSo:MaNamHoc"]);
            maCapDonVi = _config["CauHinhSo:MaCapDonVi"];
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("kiemtradanhgiathuongxuyen")]
        public async Task<IActionResult> KiemTraDanhGiaThuongXuyen(string token)
        {
            if (token == null || token.Trim() == "")
            {
                return Redirect("/");
            }
            var model = new List<DanhGiaThuongXuyen>();
            var jsonString = CoreUtility.DecodeJwt(token, _config["Jwt:Key"]);
            var payload = JsonConvert.DeserializeObject<dynamic>(jsonString);
            string ma_truong = string.Empty;
            if (jsonString.Contains("\"error\""))
            {
                string error = payload?.error;
                ViewBag.Status = false;
                ViewBag.ThongBao = error;
                return View(model);
            }
            else
            {
                long exp = payload?.exp;
                DateTime expirationDate = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                if (expirationDate < DateTime.Now)
                {
                    ViewBag.Status = false;
                    ViewBag.ThongBao = "Token hết hạn";
                    return View(model);
                }
                if (payload?.iss != "DTI")
                {
                    ViewBag.Status = false;
                    ViewBag.ThongBao = "ISS sai";
                    return View(model);
                }
                ma_truong = payload?.schoolCode;
            }
            var getDmCapHoc = await _dtiService.GetMaCapHocByTruong(maSo, maNamHoc, ma_truong);
            var dmKhoiTask = _dtiService.GetDanhMucKhoiAsync(getDmCapHoc);
            var KhoiSelectList = new SelectList(await dmKhoiTask, "MA_KHOI", "TEN_KHOI").ToList();
            KhoiSelectList.Insert(0, new SelectListItem { Text = "Chọn khối lớp", Value = "" });
            ViewBag.Khois = KhoiSelectList;

            var dmMonTask = _dtiService.GetDanhMucMonHocAsync(maSo, maCapDonVi, ma_truong, "", getDmCapHoc);
            var MonSelectList = new SelectList(await dmMonTask, "MA_MON_HOC_HE_THONG", "TEN_MON_HOC").ToList();
            MonSelectList.Insert(0, new SelectListItem { Text = "Chọn môn học", Value = "" });
            ViewBag.Mons = MonSelectList;
            ViewBag.MaTruong = ma_truong;
            ViewBag.MaCapHoc = getDmCapHoc;
            ViewBag.Status = true;
            model = await _dtiService.DTIDanhGiaThuongXuyenServiceAsync(maSo, maNamHoc, maCapDonVi, ma_truong, "", "");
            return View(model);
        }
        [HttpGet("soluonghoclieusohoa")]
        public async Task<IActionResult> SoLuongHocLieuSoHoa(string token)
        {
            if (token == null || token.Trim() == "")
            {
                return Redirect("/");
            }
            var model = new List<SoLuongHocLieuSoHoa>();
            var jsonString = CoreUtility.DecodeJwt(token, _config["Jwt:Key"]);
            var payload = JsonConvert.DeserializeObject<dynamic>(jsonString);
            string ma_truong = string.Empty;
            if (jsonString.Contains("\"error\""))
            {
                string error = payload?.error;
                ViewBag.Status = false;
                ViewBag.ThongBao = error;
                return View(model);
            }
            else
            {
                long exp = payload?.exp;
                DateTime expirationDate = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                if (expirationDate < DateTime.Now)
                {
                    ViewBag.Status = false;
                    ViewBag.ThongBao = "Token hết hạn";
                    return View(model);
                }
                if (payload?.iss != "DTI")
                {
                    ViewBag.Status = false;
                    ViewBag.ThongBao = "ISS sai";
                    return View(model);
                }
                ma_truong = payload?.schoolCode;
            }
            var getDmCapHoc = await _dtiService.GetMaCapHocByTruong(maSo, maNamHoc, ma_truong);
            var dmKhoiTask = _dtiService.GetDanhMucKhoiAsync(getDmCapHoc);
            var KhoiSelectList = new SelectList(await dmKhoiTask, "MA_KHOI", "TEN_KHOI").ToList();
            KhoiSelectList.Insert(0, new SelectListItem { Text = "Chọn khối lớp", Value = "" });
            ViewBag.Khois = KhoiSelectList;

            var dmMonTask = _dtiService.GetDanhMucMonHocAsync(maSo, maCapDonVi, ma_truong, "", getDmCapHoc);
            var MonSelectList = new SelectList(await dmMonTask, "MA_MON_HOC_HE_THONG", "TEN_MON_HOC").ToList();
            MonSelectList.Insert(0, new SelectListItem { Text = "Chọn môn học", Value = "" });       
            ViewBag.Mons = MonSelectList;
            ViewBag.MaTruong = ma_truong;
            ViewBag.MaCapHoc = getDmCapHoc;
            ViewBag.Status = true;
            model = await _dtiService.DTISoLuongHocLieuSoHoaServiceAsync(maSo, maNamHoc, maCapDonVi, ma_truong, "", "");
            return View(model);
        }

        [HttpGet("/danh-sach-mon-hoc")]
        public async Task<IActionResult> GetDanhSachMonHocAsync(string maTruong, string maKhoi, string capHoc)
        {
            var monHocs = await _dtiService.GetDanhMucMonHocAsync(maSo, maCapDonVi, maTruong, maKhoi, capHoc);
            return Json(monHocs.Select(q =>
            new
            {
                Value = q.MA_MON_HOC_HE_THONG,
                Text = q.TEN_MON_HOC,
            }));
        }

        #region Fillter
        [HttpGet]
        public async Task<ActionResult> FillterKhoi(string maTruong, string maKhoi, string monHoc)
        {
            var model = await _dtiService.DTISoLuongHocLieuSoHoaServiceAsync(maSo, maNamHoc, maCapDonVi, maTruong, maKhoi, monHoc);
            return PartialView("Partial/DTI/_HocLieuSoHoaListContent", model);
        }
        [HttpGet]
        public async Task<ActionResult> FillterKhoiDGTX(string maTruong, string maKhoi, string monHoc)
        {
            var model = await _dtiService.DTIDanhGiaThuongXuyenServiceAsync(maSo, maNamHoc, maCapDonVi, maTruong, maKhoi, monHoc);
            return PartialView("Partial/DTI/_DanhGiaThuongXuyenListContent", model);
        }
        #endregion 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
