using Dapper;
using DTI_Report.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace DTI_Report.Infrastructure.Data
{
    public class DtiRepository : IDtiRepository
    {
        private readonly Connections _connections;

        public DtiRepository(IOptions<Connections> optionConnections)
        {
            _connections = optionConnections.Value;
        }

        public async Task<List<DanhGiaThuongXuyen>> GetDanhGiaThuongXuyenAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc)
        {
            var query = @" SELECT 
                                ch.MA_TRUONG AS MA_TRUONG,
			                    ch.MA_KHOI,
			                    k.TEN_KHOI,
			                    ch.MA_MON_HOC_HE_THONG,
			                    mh.TEN_MON_HOC,
                                COUNT(ch.ID) as MOT_LUA_CHON,
                                0 as NHIEU_LUA_CHON,
                                0 as DUNG_SAI,
                                0 as DIEN_VAO_CHO_TRONG,
                                0 as TU_LUAN,
                                0 as DANG_KHAC,
                                COUNT(ch.ID) AS TONG
                            FROM
                                CAU_HOI ch
		                    LEFT JOIN 
			                    DM_KHOI_TN k on k.MA_KHOI = ch.MA_KHOI AND k.MA_CAP_HOC = ch.MA_CAP_HOC		 	
		                    LEFT JOIN 
			                    DM_MON_HOC_TN mh on ch.MA_MON_HOC_HE_THONG=mh.MA_MON_HOC_HE_THONG 
                                                AND ch.MA_KHOI=mh.MA_KHOI
							                    AND mh.MA_CAP_HOC=ch.MA_CAP_HOC 
                                                AND mh.MA_CAP_DON_VI=ch.MA_CAP_DON_VI
                                                AND mh.MA_SO_GD=ch.MA_SO_GD	
					                            AND mh.MA_TRUONG=ch.MA_TRUONG	
			
                            WHERE 
                                ch.MA_SO_GD=@maSo
                                AND ch.MA_CAP_DON_VI=@maCapDonVi
                                AND ch.IS_DUYET= 1
			                    AND ch.MA_TRUONG=@maTruong ";
            if (!string.IsNullOrEmpty(maKhoi))
            {
                query += $"AND ch.MA_KHOI=@maKhoi ";
            }
            if (!string.IsNullOrEmpty(maMonHoc))
            {
                query += $"AND ch.MA_MON_HOC_HE_THONG=@maMonHoc ";
            }
            query += @" GROUP BY 
                            ch.MA_TRUONG,
			                ch.MA_KHOI,
			                k.TEN_KHOI,
			                ch.MA_MON_HOC_HE_THONG,
			                mh.TEN_MON_HOC;";
            using SqlConnection connection = new(_connections.SqlLMSConnection);
            return (await connection.QueryAsync<DanhGiaThuongXuyen>(query, new { maSo, maCapDonVi, maNamHoc, maTruong, maKhoi, maMonHoc })).AsList();
        }

        public async Task<IEnumerable<DM_KHOI_TN>> GetDanhMucKhoiAsync(string maCapHoc)
        {
            var query = @"SELECT * FROM DM_KHOI_TN K
                            WHERE K.MA_CAP_HOC=@maCapHoc
                            ORDER BY K.THU_TU, K.TEN_KHOI, K.MA_KHOI";
            using SqlConnection connection = new(_connections.SqlLMSConnection);
            return await connection.QueryAsync<DM_KHOI_TN>(query, new { maCapHoc });
        }

        public async Task<IEnumerable<DM_MON_HOC_TN>> GetDanhMucMonHocAsync(string maSo, string maCapDonVi, string maTruong, string maKhoi, string maCapHoc)
        {
            var query = @"SELECT monHoc.* FROM DM_MON_HOC_TN monHoc
                            left join DM_KHOI_TN khoi ON monHoc.MA_KHOI=khoi.MA_KHOI and khoi.MA_CAP_HOC=monHoc.MA_CAP_HOC
                            WHERE
                            monHoc.MA_CAP_DON_VI=@maCapDonVi
                            and monHoc.MA_SO_GD=@maSo
                            and monHoc.MA_TRUONG=@maTruong
                            and monHoc.MA_CAP_HOC=@maCapHoc
                            ";
            if (!string.IsNullOrEmpty(maKhoi))
            {
                query += $" and monHoc.MA_KHOI=@maKhoi";
            }
            query += @" ORDER BY monHoc.MA_KHOI, monHoc.THU_TU;";
            using SqlConnection connection = new(_connections.SqlLMSConnection);
            return await connection.QueryAsync<DM_MON_HOC_TN>(query, new { maSo, maCapDonVi, maTruong, maKhoi, maCapHoc });
        }

        public async Task<string> GetMaCapHocByMaTruongAsync(string maSo, int maNamHoc, string maTruong)
        {
            var query = @"SELECT DS_CAP_HOC FROM TRUONG WHERE MA=@maTruong AND MA_NAM_HOC=@maNamHoc AND MA_SO_GD=@maSo";
            using SqlConnection connection = new(_connections.SqlMinistryConnection);
            var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { maSo, maNamHoc, maTruong });
            return result;
        }

        public async Task<List<SoLuongHocLieuSoHoa>> GetSoLuongHocLieuSoHoaAsync(string maSo, int maNamHoc, string maCapDonVi, string maTruong, string maKhoi, string maMonHoc)
        {
            var query = @" SELECT 
                                A.MA_TRUONG,
	                            A.MA_KHOI,
	                            A.TEN_KHOI,
	                            A.MA_MON_HOC_HE_THONG,
	                            A.TEN_MON_HOC,
                                COALESCE(B.TAI_LIEU, 0) + COALESCE(C.TAI_LIEU, 0) + COALESCE(A.TAI_LIEU, 0) AS TAI_LIEU,
                                COALESCE(A.VIDEO, 0) AS VIDEO,	
                                COALESCE(A.AM_THANH, 0) AS AM_THANH,
                                COALESCE(B.HINH_ANH, 0) + COALESCE(C.HINH_ANH, 0) + COALESCE(A.HINH_ANH, 0) AS HINH_ANH,
                                COALESCE(D.CAU_HOI,0) AS CAU_HOI,
                                COALESCE(A.NHUNG, 0) AS NHUNG,
                                COALESCE(A.DANG_KHAC, 0) AS DANG_KHAC,
	                            COALESCE(SUM(COALESCE(B.TAI_LIEU, 0) + COALESCE(C.TAI_LIEU, 0) + COALESCE(A.TAI_LIEU, 0) +
                                    COALESCE(A.VIDEO, 0) + COALESCE(A.AM_THANH, 0) + 
                                    COALESCE(B.HINH_ANH, 0) + COALESCE(C.HINH_ANH, 0) + COALESCE(A.HINH_ANH, 0) +
                                    COALESCE(D.CAU_HOI,0) + COALESCE(A.NHUNG, 0) +  COALESCE(A.DANG_KHAC, 0)),0) AS TONG
                            FROM (
                             SELECT
                                    bg.MA_TRUONG AS MA_TRUONG,
		                            bg.MA_KHOI,
		                            k.TEN_KHOI,
		                            bg.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC,
                                    COUNT(DISTINCT CASE WHEN bgnd.MA_LOAI_NOI_DUNG = '04' AND bgnd.KIEU_FILE IN ('.docx', '.doc', '.xls', '.xlsx', '.ppt', '.pptx', '.pdf') THEN bgnd.ID END) AS TAI_LIEU,
                                    COUNT(DISTINCT CASE WHEN bgnd.MA_LOAI_NOI_DUNG = '02' THEN bgnd.ID END) AS VIDEO,	
                                    COUNT(DISTINCT CASE WHEN bgnd.MA_LOAI_NOI_DUNG = '03' THEN bgnd.ID END) AS AM_THANH,
                                    COUNT(DISTINCT CASE WHEN bgnd.MA_LOAI_NOI_DUNG = '04' AND bgnd.KIEU_FILE IN ('.jpeg', '.jpg', '.gif', '.png', '.bmp') THEN bgnd.ID END) AS HINH_ANH,
                                    COUNT(DISTINCT CASE WHEN bgnd.MA_LOAI_NOI_DUNG IN ('05', '06') THEN bgnd.ID END) AS NHUNG,
                                    COUNT(DISTINCT CASE WHEN bgnd.MA_LOAI_NOI_DUNG = '01' THEN bgnd.ID END) AS DANG_KHAC
      
                                FROM
                                    LMS_BAI_GIANG bg
                                LEFT JOIN 
                                    LMS_BAI_GIANG_NOI_DUNG bgnd ON bgnd.ID_BAI_GIANG = bg.ID
	                            LEFT JOIN 
		                            DM_KHOI_TN k on k.MA_KHOI = bg.MA_KHOI AND k.MA_CAP_HOC = bg.MA_CAP_HOC		 	
	                            LEFT JOIN 
		                            DM_MON_HOC_TN mh on bg.MA_MON_HOC_HE_THONG=mh.MA_MON_HOC_HE_THONG 
                                                        AND bg.MA_KHOI=mh.MA_KHOI
							                            AND mh.MA_CAP_HOC=bg.MA_CAP_HOC 
                                                        AND mh.MA_CAP_DON_VI=bg.MA_CAP_DON_VI
                                                        AND mh.MA_SO_GD=bg.MA_SO_GD	
					                                    AND mh.MA_TRUONG=bg.MA_TRUONG	
                                WHERE 
                                    bg.MA_SO_GD = @maSo
                                    AND bg.MA_CAP_DON_VI =@maCapDonVi
                                    AND bg.MA_NAM_HOC = @maNamHoc
                                GROUP BY 
                                    bg.MA_TRUONG,
		                            bg.MA_KHOI,
		                            k.TEN_KHOI,
		                            bg.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC
  
                            ) A
                            LEFT JOIN (
                                SELECT
                                    kt.MA_TRUONG AS MA_TRUONG,
		                            kt.MA_KHOI,
		                            k.TEN_KHOI,
		                            kt.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC,
                                    COUNT(DISTINCT CASE WHEN ktf.KIEU_FILE IN ('.docx', '.doc', '.xls', '.xlsx', '.ppt', '.pptx', '.pdf') THEN ktf.ID END) AS TAI_LIEU,
                                    COUNT(DISTINCT CASE WHEN ktf.KIEU_FILE IN ('.jpeg', '.jpg', '.gif', '.png', '.bmp') THEN ktf.ID END) AS HINH_ANH
                                FROM 
                                    KY_THI kt
                                LEFT JOIN 
                                    KY_THI_FILE ktf ON ktf.ID_KY_THI = kt.ID
	                            LEFT JOIN 
		                            DM_KHOI_TN k on k.MA_KHOI = kt.MA_KHOI AND k.MA_CAP_HOC = kt.MA_CAP_HOC		 	
	                            LEFT JOIN 
		                            DM_MON_HOC_TN mh on kt.MA_MON_HOC_HE_THONG=mh.MA_MON_HOC_HE_THONG 
                                                        AND kt.MA_KHOI=mh.MA_KHOI
							                            AND mh.MA_CAP_HOC=kt.MA_CAP_HOC 
                                                        AND mh.MA_CAP_DON_VI=kt.MA_CAP_DON_VI
                                                        AND mh.MA_SO_GD=kt.MA_SO_GD	
					                                    AND mh.MA_TRUONG=kt.MA_TRUONG	
                                WHERE
                                    kt.MA_SO_GD = @maSo
                                    AND kt.MA_TRUONG IS NOT NULL
                                    AND kt.MA_NAM_HOC = @maNamHoc
                                GROUP BY 
                                     kt.MA_TRUONG,
		                            kt.MA_KHOI,
		                            k.TEN_KHOI,
		                            kt.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC
                            ) B ON A.MA_TRUONG = B.MA_TRUONG AND A.MA_KHOI = B.MA_KHOI AND A.MA_MON_HOC_HE_THONG = B.MA_MON_HOC_HE_THONG
                            LEFT JOIN (
                                SELECT
                                    bt.MA_TRUONG AS MA_TRUONG,
		                            bt.MA_KHOI,
		                            k.TEN_KHOI,
		                            bt.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC,
                                    COUNT(DISTINCT CASE WHEN btf.KIEU_FILE IN ('.docx', '.doc', '.xls', '.xlsx', '.ppt', '.pptx', '.pdf') THEN btf.ID END) AS TAI_LIEU,
                                    COUNT(DISTINCT CASE WHEN btf.KIEU_FILE IN ('.jpeg', '.jpg', '.gif', '.png', '.bmp') THEN btf.ID END) AS HINH_ANH
                                FROM 
                                    BAI_TAP bt
                                LEFT JOIN 
                                    BAI_TAP_FILE btf ON btf.ID_BAI_TAP = bt.ID
	                            LEFT JOIN 
		                            DM_KHOI_TN k on k.MA_KHOI = bt.MA_KHOI AND k.MA_CAP_HOC = bt.MA_CAP_HOC		 	
	                            LEFT JOIN 
		                            DM_MON_HOC_TN mh on bt.MA_MON_HOC_HE_THONG=mh.MA_MON_HOC_HE_THONG 
                                                        AND bt.MA_KHOI=mh.MA_KHOI
							                            AND mh.MA_CAP_HOC=bt.MA_CAP_HOC 
                                                        AND mh.MA_CAP_DON_VI=bt.MA_CAP_DON_VI
                                                        AND mh.MA_SO_GD=bt.MA_SO_GD	
					                                    AND mh.MA_TRUONG=bt.MA_TRUONG	
                                WHERE
                                    bt.MA_SO_GD = @maSo
                                    AND bt.MA_TRUONG IS NOT NULL
                                    AND bt.MA_NAM_HOC = @maNamHoc
                                GROUP BY 
                                    bt.MA_TRUONG,
		                            bt.MA_KHOI,
		                            k.TEN_KHOI,
		                            bt.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC
                            ) C ON A.MA_TRUONG = C.MA_TRUONG  AND A.MA_KHOI = C.MA_KHOI AND A.MA_MON_HOC_HE_THONG = C.MA_MON_HOC_HE_THONG
                            LEFT JOIN (
                                 SELECT 
                                    ch.MA_TRUONG AS MA_TRUONG,
		                            ch.MA_KHOI,
		                            k.TEN_KHOI,
		                            ch.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC,
                                    COUNT(ch.ID) AS CAU_HOI
  
                                FROM
                                    CAU_HOI ch
	                            LEFT JOIN 
		                            DM_KHOI_TN k on k.MA_KHOI = ch.MA_KHOI AND k.MA_CAP_HOC = ch.MA_CAP_HOC		 	
	                            LEFT JOIN 
		                            DM_MON_HOC_TN mh on ch.MA_MON_HOC_HE_THONG=mh.MA_MON_HOC_HE_THONG 
                                                        AND ch.MA_KHOI=mh.MA_KHOI
							                            AND mh.MA_CAP_HOC=ch.MA_CAP_HOC 
                                                        AND mh.MA_CAP_DON_VI=ch.MA_CAP_DON_VI
                                                        AND mh.MA_SO_GD=ch.MA_SO_GD	
					                                    AND mh.MA_TRUONG=ch.MA_TRUONG	
                                WHERE 
                                    ch.MA_SO_GD = @maSo
                                    AND ch.MA_CAP_DON_VI =@maCapDonVi
                                    AND ch.IS_DUYET = 1
                                GROUP BY 
                                    ch.MA_TRUONG,
		                            ch.MA_KHOI,
		                            k.TEN_KHOI,
		                            ch.MA_MON_HOC_HE_THONG,
		                            mh.TEN_MON_HOC
                            ) D ON A.MA_TRUONG = D.MA_TRUONG AND A.MA_KHOI = D.MA_KHOI AND A.MA_MON_HOC_HE_THONG = D.MA_MON_HOC_HE_THONG
	
                            WHERE A.MA_TRUONG = @maTruong ";
            if (!string.IsNullOrEmpty(maKhoi))
            {
                query += $"AND A.MA_KHOI=@maKhoi ";
            }
            if (!string.IsNullOrEmpty(maMonHoc))
            {
                query += $"AND A.MA_MON_HOC_HE_THONG=@maMonHoc ";
            }
            query += @" GROUP BY
                            A.MA_TRUONG,
                            A.MA_KHOI,
                            A.TEN_KHOI,
                            A.MA_MON_HOC_HE_THONG,
                            A.TEN_MON_HOC,
                            D.CAU_HOI,
                            COALESCE(B.TAI_LIEU, 0) + COALESCE(C.TAI_LIEU, 0) + COALESCE(A.TAI_LIEU, 0),
                            COALESCE(A.VIDEO, 0),
                            COALESCE(A.AM_THANH, 0),
                            COALESCE(B.HINH_ANH, 0) + COALESCE(C.HINH_ANH, 0) + COALESCE(A.HINH_ANH, 0),
                            COALESCE(A.NHUNG, 0),
                            COALESCE(A.DANG_KHAC, 0);";
            using SqlConnection connection = new(_connections.SqlLMSConnection);
            return (await connection.QueryAsync<SoLuongHocLieuSoHoa>(query, new { maSo, maCapDonVi, maNamHoc, maTruong, maKhoi, maMonHoc })).AsList();

        }
    }
}
