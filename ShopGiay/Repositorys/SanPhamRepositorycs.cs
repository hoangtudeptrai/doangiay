using Dapper;
using Microsoft.Extensions.Logging;
using ShopGiay.Context;
using ShopGiay.Interfaces;
using ShopGiay.Models;
using System.Data;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace ShopGiay.Repositorys
{
    public class SanPhamRepositorycs : ISanPhamRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<SanPhamRepositorycs> _logger;
        private readonly IHostingEnvironment _env;

        private readonly string uploadpath = "/Files/Upload/FileDinhKem";

        public SanPhamRepositorycs(DapperContext context, ILogger<SanPhamRepositorycs> logger, IHostingEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        public async Task<int> ThemSanPham(DM_SanPhamModel obj)
        {
            try
            {
                int ID_Sanpham = 0;
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        #region Thêm bản ghi bảng DM_SanPham
                        string procedureName = "TuDH_SanPham_ThemMoiSanPham";
                        var parameters = new DynamicParameters();
                        parameters.Add("ThongTinMoTa", obj.ThongTinMoTa, DbType.String, ParameterDirection.Input);

                        parameters.Add("TinhTrangSanPham", obj.TinhTrangSanPham, DbType.String, ParameterDirection.Input);
                        parameters.Add("LoaiKhoaDay", obj.LoaiKhoaDay, DbType.String, ParameterDirection.Input);
                        parameters.Add("ChatLieu", obj.ChatLieu, DbType.String, ParameterDirection.Input);
                        parameters.Add("XuatXu", obj.XuatXu, DbType.String, ParameterDirection.Input);
                        parameters.Add("ChieuCaoCoGiay", obj.ChieuCaoCoGiay, DbType.String, ParameterDirection.Input);
                        parameters.Add("DiaChiGuiHang", obj.DiaChiGuiHang, DbType.String, ParameterDirection.Input);

                        parameters.Add("ID_NhanHieu", obj.ID_NhanHieu, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("TenSanPham", obj.TenSanPham, DbType.String, ParameterDirection.Input);
                        parameters.Add("GioiThieuSanPham", obj.GioiThieuSanPham, DbType.String, ParameterDirection.Input);
                        parameters.Add("GiaSanPham", obj.GiaSanPham, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("PhiVanChuyen", obj.PhiVanChuyen, DbType.Int32, ParameterDirection.Input);
                        int result = await connection.ExecuteScalarAsync<int>
                                        (procedureName, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);

                        ID_Sanpham = result;
                        #endregion

                        #region Thêm bản ghi bảng DM_ChitietSanPham
                        if (result > 0)
                        {
                            foreach (var item in obj.listChiTietSanPham)
                            {
                                string procedureName_chiTiet = "TUDH_SanPham_Insert_DM_ChiTietSanPham";
                                var parameters_chiTiet = new DynamicParameters();
                                parameters_chiTiet.Add("ID_SanPham", result, DbType.Int32, ParameterDirection.Input);
                                parameters_chiTiet.Add("ID_MauSac", item.ID_MauSac, DbType.Int32, ParameterDirection.Input);
                                parameters_chiTiet.Add("Size", item.Size, DbType.Int32, ParameterDirection.Input);
                                parameters_chiTiet.Add("SoLuong", item.SoLuong, DbType.Int32, ParameterDirection.Input);

                                long them = await connection.ExecuteScalarAsync<long>
                                    (procedureName_chiTiet, parameters_chiTiet, commandType: CommandType.StoredProcedure, transaction: transaction);
                            }
                        }
                        #endregion
                        transaction.Commit();
                    }

                    return ID_Sanpham;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi Save: " + ex.Message);
                throw new ArgumentException("TUDH_SanPham_Insert_DM_ChiTietSanPham", ex);
            }
        }

        public async Task<List<ImageViewModel>> UploadFile(IFormFileCollection file, int ID_SanPham)
        {
            try
            {
                List<ImageViewModel> list = new List<ImageViewModel>();

                foreach (var file_upload in file)
                {
                    string fileName = file_upload.FileName;
                    string ticks = DateTime.Now.Ticks.ToString();
                    fileName = fileName.Trim();
                    fileName = fileName.Replace(" ", "_");
                    string extention = fileName.Substring(fileName.LastIndexOf('.') + 1);
                    fileName = fileName.Substring(0, fileName.LastIndexOf('.')).Replace('.', '_').ToLower();
                    string path_date = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/";
                    string folder_upload = _env.ContentRootPath + uploadpath + "/" + path_date;

                    if (!Directory.Exists(folder_upload))
                        Directory.CreateDirectory(folder_upload);

                    string path_file_upload = folder_upload + @"\" + fileName + ticks + "." + extention;
                    using (var fileStream = new FileStream(path_file_upload, FileMode.Create))
                    {
                        await file_upload.CopyToAsync(fileStream);
                    };

                    ImageViewModel attachment = new ImageViewModel();
                    attachment.URL = "HoSo_FileDinhKem/" + path_date + fileName + ticks + "." + extention;
                    attachment.ID_SanPham = ID_SanPham;
                    list.Add(attachment);
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError("UploadFile hồ sơ fail " + ex.Message);
                throw new ArgumentException("UploadFile", ex);
            }
        }
        public async Task<bool> LuuThongTinFile(List<ImageViewModel> list)
        {
            try
            {
                var procedureName = "TuDH_SanPham_ThemAnh";
                int count = 0;
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (ImageViewModel obj in list)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ID_SanPham", obj.ID_SanPham, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("URL", obj.URL, DbType.String, ParameterDirection.Input);

                            long result = await connection.ExecuteScalarAsync<long>
                                (procedureName, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            if (result > 0)
                                count++;
                        }
                        transaction.Commit();
                    }
                    if (count == 0)
                        return false;
                    else
                        return count == list.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_SanPham_ThemAnh " + ex.Message);
                throw new ArgumentException("TuDH_SanPham_ThemAnh", ex);
            }
        }

        public async Task<List<TrangChuSanPhamViewModel>> GetList_SanPham()
        {
            try
            {
                var procedureName = "TuDH_SanPham_GetData_SanPham_TrangChu";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<TrangChuSanPhamViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_SanPham_GetData_SanPham_TrangChu", ex);
                throw new ArgumentException("TuDH_SanPham_GetData_SanPham_TrangChu", ex);
            }
        }

        public async Task<DataSet> ThongTinChiTietSanPham(int ID_SanPham)
        {
            try
            {
                var parameters = new DynamicParameters();
                var procedureName = "TuDH_SanPham_ThongTinChiTietSanPham";
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_SanPham", ID_SanPham, DbType.Int32, ParameterDirection.Input);
                    var reader = await connection.ExecuteReaderAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                    DataSet ds = new DataSet();
                    ds = ConvertDataReaderToDataSet(reader);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_SanPham_ThongTinChiTietSanPham" + ex.Message);
                throw new ArgumentException("TuDH_SanPham_ThongTinChiTietSanPham", ex);
            }
        }
        public DataSet ConvertDataReaderToDataSet(IDataReader data)
        {
            DataSet ds = new DataSet();
            int i = 0;
            while (!data.IsClosed)
            {
                ds.Tables.Add("Table" + (i + 1));
                ds.EnforceConstraints = false;
                ds.Tables[i].Load(data);
                i++;
            }
            return ds;
        }

        public async Task<int> XoaSanPham(int ID_SanPham)
        {
            try
            {
                var procedureName = "TuDH_SanPham_Delete_SanPham";
                var parameters = new DynamicParameters();
                parameters.Add("ID_SanPham", ID_SanPham, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_SanPham_Delete_SanPham", ex);
                throw new ArgumentException("TuDH_SanPham_Delete_SanPham", ex);
            }
        }

        public async Task<bool> UpdateSanPham(UpdateSanPhamModels obj)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        #region UPDATE_SanPham
                        string procedureName = "TuDH_SanPham_UpdateThongTinSanPham";
                        var parameters = new DynamicParameters();

                        parameters.Add("ID_SanPham", obj.ID_SanPham, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("ID_ThongTinSanPham", obj.ID_ThongTinSanPham, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("ID_MoTaSanPham", obj.ID_MoTaSanPham, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("ID_NhanHieu", obj.ID_NhanHieu, DbType.Int32, ParameterDirection.Input);

                        parameters.Add("TenSanPham", obj.TenSanPham, DbType.String, ParameterDirection.Input);
                        parameters.Add("GioiThieuSanPham", obj.GioiThieuSanPham, DbType.String, ParameterDirection.Input);
                        parameters.Add("Gia", obj.GiaSanPham, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("PhiVanChuyen", obj.PhiVanChuyen, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("ThongTinMoTa", obj.ThongTinMoTa, DbType.String, ParameterDirection.Input);

                        parameters.Add("TinhTrangSanPham", obj.TinhTrangSanPham, DbType.String, ParameterDirection.Input);
                        parameters.Add("LoaiKhoaDay", obj.LoaiKhoaDay, DbType.String, ParameterDirection.Input);
                        parameters.Add("ChatLieu", obj.ChatLieu, DbType.String, ParameterDirection.Input);
                        parameters.Add("XuatXu", obj.XuatXu, DbType.String, ParameterDirection.Input);
                        parameters.Add("ChieuCaoCoGiay", obj.ChieuCaoCoGiay, DbType.String, ParameterDirection.Input);
                        parameters.Add("DiaChiGuiHang", obj.DiaChiGuiHang, DbType.String, ParameterDirection.Input);

                        int result = await connection.ExecuteScalarAsync<int>
                                        (procedureName, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);

                        #endregion

                        #region UPDATE_ChiTietSanPham
                         if (result > 0)
                        {
                            foreach (var item in obj.listChiTietSanPham)
                            {
                                string procedureName_chiTiet = "TuDH_SanPham_UpdateThongTinChiTietSanPham";
                                var parameters_chiTiet = new DynamicParameters();
                                parameters_chiTiet.Add("ID_ChiTietSanPham", item.ID_ChiTietSanPham, DbType.Int32, ParameterDirection.Input);
                                parameters_chiTiet.Add("ID_MauSac", item.ID_MauSac, DbType.Int32, ParameterDirection.Input);
                                parameters_chiTiet.Add("Size", item.Size, DbType.Int32, ParameterDirection.Input);
                                parameters_chiTiet.Add("SoLuong", item.SoLuong, DbType.Int32, ParameterDirection.Input);

                                long them = await connection.ExecuteScalarAsync<long>
                                    (procedureName_chiTiet, parameters_chiTiet, commandType: CommandType.StoredProcedure, transaction: transaction);
                            }
                        }
                        #endregion
                        transaction.Commit();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi Save: " + ex.Message);
                throw new ArgumentException("TUDH_SanPham_Insert_DM_ChiTietSanPham", ex);
            }
        }

        public async Task<int> XoaAnh(int ID_SanPham)
        {
            try
            {
                var procedureName = "TuDH_SanPham_XoaAnh";
                var parameters = new DynamicParameters();
                parameters.Add("ID_SanPham", ID_SanPham, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_SanPham_XoaAnh", ex);
                throw new ArgumentException("TuDH_SanPham_XoaAnh", ex);
            }
        }

        public async Task<int> ThemGoHang(int ID_ChiTietSanPham, int ID_TaiKkhoan, int SoLuong)
        {
            try
            {
                var procedureName = "TuDH_GioHang_ThemSanPham";
                var parameters = new DynamicParameters();
                parameters.Add("ID_ChiTietSanPham", ID_ChiTietSanPham, DbType.Int32, ParameterDirection.Input);
                parameters.Add("ID_TaiKhoan", ID_TaiKkhoan, DbType.Int32, ParameterDirection.Input);
                parameters.Add("SoLuong", SoLuong, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_GioHang_ThemSanPham", ex);
                throw new ArgumentException("TuDH_GioHang_ThemSanPham", ex);
            }
        }

        public async Task<int> XoaGioHang(int ID_GioHang)
        {
            try
            {
                var procedureName = "TuDH_GioHang_DeleteGioHang";
                var parameters = new DynamicParameters();
                parameters.Add("ID_GioHang", ID_GioHang, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_GioHang_DeleteGioHang", ex);
                throw new ArgumentException("TuDH_GioHang_DeleteGioHang", ex);
            }
        }

        public async Task<int> SuaGioHang(int ID_GioHang, int ID_ChiTietSanPham, int SoLuong)
        {
            try
            {
                var procedureName = "TuDH_GioHang_SuaDanhSachGioHang";
                var parameters = new DynamicParameters();
                parameters.Add("ID_GioHang", ID_GioHang, DbType.Int32, ParameterDirection.Input);
                parameters.Add("ID_ChiTietSanPham", ID_ChiTietSanPham, DbType.Int32, ParameterDirection.Input);
                parameters.Add("SoLuong", SoLuong, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_GioHang_SuaDanhSachGioHang", ex);
                throw new ArgumentException("TuDH_GioHang_SuaDanhSachGioHang", ex);
            }
        }

        public async Task<List<ThemGioHangViewModel>> listGioHang(int ID_TaiKhoan)
        {
            try
            {
                var procedureName = "TuDH_GioHang_GetGioHang";
                var parameters = new DynamicParameters();
                parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<ThemGioHangViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_GioHang_GetGioHang", ex);
                throw new ArgumentException("TuDH_GioHang_GetGioHang", ex);
            }
        }

        public async Task<int> ThanhToanHoaDon(string ListID_GioHang, int ID_TaiKkhoan, int ID_MaGiamGia)
        {
            try
            {
                var procedureName = "TuDH_HoaDon_ThanhToanHoaDon";
                var parameters = new DynamicParameters();
                parameters.Add("ListID_GioHang", ListID_GioHang, DbType.String, ParameterDirection.Input);
                parameters.Add("ID_TaiKhoan", ID_TaiKkhoan, DbType.Int32, ParameterDirection.Input);
                parameters.Add("ID_MaGiamGia", ID_MaGiamGia, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_HoaDon_ThanhToanHoaDon", ex);
                throw new ArgumentException("TuDH_HoaDon_ThanhToanHoaDon", ex);
            }
        }

        public async Task<int> XoaHoaDon(int ID_HoaDon)
        {
            try
            {
                var procedureName = "TuDH_HoaDon_HuyHoaDon";
                var parameters = new DynamicParameters();
                parameters.Add("ID_HoaDon", ID_HoaDon, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_HoaDon_HuyHoaDon", ex);
                throw new ArgumentException("TuDH_HoaDon_HuyHoaDon", ex);
            }
        }

        public async Task<List<HoaDonViewModel>> listHoaDon(int ID_TaiKhoan)
        {
            try
            {
                var procedureName = "TuDH_HoaDon_DanhSachHoaDon";
                var parameters = new DynamicParameters();
                parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<HoaDonViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_HoaDon_DanhSachHoaDon", ex);
                throw new ArgumentException("TuDH_HoaDon_DanhSachHoaDon", ex);
            }
        }

        public async Task<List<ThemGioHangViewModel>> listGChiTietHoaDon(int ID_HoaDon)
        {
            try
            {
                var procedureName = "TuDH_HoaDon_ChiTietThanhToan";
                var parameters = new DynamicParameters();
                parameters.Add("ID_HoaDon", ID_HoaDon, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<ThemGioHangViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_HoaDon_ChiTietThanhToan", ex);
                throw new ArgumentException("TuDH_HoaDon_ChiTietThanhToan", ex);
            }
        }

        public async Task<List<MaGiamGiaViewModelcs>> listMaGiamGia()
        {
            try
            {
                var procedureName = "TuDH_MaGiamGia_GetData";
                var parameters = new DynamicParameters();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<MaGiamGiaViewModelcs>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_MaGiamGia_GetData", ex);
                throw new ArgumentException("TuDH_MaGiamGia_GetData", ex);
            }
        }

        public async Task<int> ThemMaGiamGia(string MaGiamGia, int GiaTri, int LoaiGiamGia)
        {
            try
            {
                var procedureName = "TuDH_MaGiamGia_ThemMaGiamGia";
                var parameters = new DynamicParameters();
                parameters.Add("GiaTri", GiaTri, DbType.Int32, ParameterDirection.Input);
                parameters.Add("LoaiGiamGia", LoaiGiamGia, DbType.Int32, ParameterDirection.Input);
                parameters.Add("MaGiamGia", MaGiamGia, DbType.String, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_MaGiamGia_ThemMaGiamGia", ex);
                throw new ArgumentException("TuDH_MaGiamGia_ThemMaGiamGia", ex);
            }
        }

        public async Task<int> XoaMaGiamGia(int ID_MaGiamGia)
        {
            try
            {
                var procedureName = "TuDH_MaGiamGia_XoaMa";
                var parameters = new DynamicParameters();
                parameters.Add("ID_MaGiamGia", ID_MaGiamGia, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_MaGiamGia_XoaMa", ex);
                throw new ArgumentException("TuDH_MaGiamGia_XoaMa", ex);
            }
        }
    }
}
