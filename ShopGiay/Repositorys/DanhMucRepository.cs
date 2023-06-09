﻿using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ShopGiay.Context;
using ShopGiay.Interfaces;
using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShopGiay.Repositorys
{
    public class DanhMucRepository : IDanhMucRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<DanhMucRepository> _logger;
        private readonly string uploadpath = "/Files/Upload/FileDinhKem";

        public DanhMucRepository(DapperContext context, ILogger<DanhMucRepository> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<int> CheckLoign(string TaiKhoan, string MatKhau)
        {
            try
            {
                var procedureName = "TuDH_DM_TaiKhoan_CheckDangNhap";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("TaiKhoan",TaiKhoan, DbType.String, ParameterDirection.Input);
                    parameters.Add("MatKhau", MatKhau, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DM_TaiKhoan_CheckDangNhap", ex);
                throw new ArgumentException("TuDH_DM_TaiKhoan_CheckDangNhap", ex);
            }
        }

        public async Task<int> DoiMatKhau(string TaiKhoan, string MatKhauCu, string MatKhauMoi, int ID_TaiKhoan)
        {
            try
            {
                var procedureName = "TuDH_DM_TaiKhoan_CheckDangNhap";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("TaiKhoan", TaiKhoan, DbType.String, ParameterDirection.Input);
                    parameters.Add("MatKhauCu", MatKhauCu, DbType.String, ParameterDirection.Input);
                    parameters.Add("MatKhauMoi", MatKhauMoi, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DM_TaiKhoan_CheckDangNhap", ex);
                throw new ArgumentException("TuDH_DM_TaiKhoan_CheckDangNhap", ex);
            }
        }

        public async Task<List<DM_TaiKhoanViewModel>> GetList_TaiKhoan()
        {
            try
            {
                var procedureName = "TuDH_DM_TaiKhoan_GetList";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<DM_TaiKhoanViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DM_TaiKhoan_GetList", ex);
                throw new ArgumentException("TuDH_DM_TaiKhoan_GetList", ex);
            }
        }

        public async Task<int> ThemMoiTaiKhoan(DM_TaiKhoanViewModel dM_TaiKhoanViewModel)
        {
            try
            {
                var procedureName = "TuDH_DM_TaiKhoan_Insert_TaiKhoan";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("TaiKhoan", dM_TaiKhoanViewModel.TaiKhoan, DbType.String, ParameterDirection.Input);
                    parameters.Add("MatKhau", dM_TaiKhoanViewModel.MatKhau, DbType.String, ParameterDirection.Input);
                    parameters.Add("Ho", dM_TaiKhoanViewModel.Ho, DbType.String, ParameterDirection.Input);
                    parameters.Add("Ten", dM_TaiKhoanViewModel.Ten, DbType.String, ParameterDirection.Input);
                    parameters.Add("Email", dM_TaiKhoanViewModel.Email, DbType.String, ParameterDirection.Input);
                    parameters.Add("SoDienThoai", dM_TaiKhoanViewModel.SDT, DbType.String, ParameterDirection.Input);
                    parameters.Add("DiaChi", dM_TaiKhoanViewModel.DiaChi, DbType.String, ParameterDirection.Input);
                    parameters.Add("Role", dM_TaiKhoanViewModel.Role, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DM_TaiKhoan_Insert_TaiKhoan", ex);
                throw new ArgumentException("TuDH_DM_TaiKhoan_Insert_TaiKhoan", ex);
            }
        }

        public async Task<int> XoaTaiKhoan(int ID_TaiKhoan)
        {
            try
            {
                var procedureName = "TuDH_DM_TaiKhoan_XoaTaiKhoan";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DM_TaiKhoan_XoaTaiKhoan", ex);
                throw new ArgumentException("TuDH_DM_TaiKhoan_XoaTaiKhoan", ex);
            }
        }
        public async Task<int> ThemNhanHieu(string TenNhanHieu)
        {
            try
            {
                var procedureName = "TuDH_NhanHieu_ThemMoi";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("TenNhanHieu", TenNhanHieu, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_NhanHieu_ThemMoi", ex);
                throw new ArgumentException("TuDH_NhanHieu_ThemMoi", ex);
            }
        }

        public async Task<int> XoaNhanHieu(int ID_NhanHieu)
        {
            try
            {
                var procedureName = "TuDH_NhanHieu_XoaNhanHieu";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_NhanHieu", ID_NhanHieu, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_NhanHieu_XoaNhanHieu", ex);
                throw new ArgumentException("TuDH_NhanHieu_XoaNhanHieu", ex);
            }
        }
        public async Task<List<DM_NhanHieuViewModel>> ListNhanHieu()
        {
            try
            {
                var procedureName = "TuDH_NhanHieu_GetData";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<DM_NhanHieuViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_NhanHieu_GetData", ex);
                throw new ArgumentException("TuDH_NhanHieu_GetData", ex);
            }
        }

        public async Task<int> SuaNhanHieu(int ID_NhanHieu, string TenNhanHieu)
        {
            try
            {
                var procedureName = "TuDH_NhanHieu_SuaTenNhanHieu";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_NhanHieu", ID_NhanHieu, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("TenNhanHieu", TenNhanHieu, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_NhanHieu_SuaTenNhanHieu", ex);
                throw new ArgumentException("TuDH_NhanHieu_SuaTenNhanHieu", ex);
            }
        }

        public async Task<int> ThemDiaChi(int ID_TaiKhoan, string DiaChi)
        {
            try
            {
                var procedureName = "TuDH_DiaChi_ThemDiaChiThanhToan";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("DiaChi", DiaChi, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DiaChi_ThemDiaChiThanhToan", ex);
                throw new ArgumentException("TuDH_DiaChi_ThemDiaChiThanhToan", ex);
            }
        }

        public async Task<int> XoaDiaChi(int ID_DiaChiNhanHang)
        {
            try
            {
                var procedureName = "TuDH_DiaChi_XoaDiaChi";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_DiaChiNhanHang", ID_DiaChiNhanHang, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DiaChi_XoaDiaChi", ex);
                throw new ArgumentException("TuDH_DiaChi_XoaDiaChi", ex);
            }
        }

        public async Task<List<DiaChiNhanHangViewModel>> listDiaChiNhanHang(int ID_TaiKhoan)
        {
            try
            {
                var procedureName = "TuDH_DiaChi_HienThiDiaChi";
                var parameters = new DynamicParameters();
                parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<DiaChiNhanHangViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DiaChi_HienThiDiaChi", ex);
                throw new ArgumentException("TuDH_DiaChi_HienThiDiaChi", ex);
            }
        }

        public async Task<int> ThemDanhGia(int ID_SanPham, int ID_TaiKhoan, string DanhGia)
        {
            try
            {
                var procedureName = "TuDH_DanhGia_ThemDanhGia";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_SanPham", ID_SanPham, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("ID_TaiKhoan", ID_TaiKhoan, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("DanhGia", DanhGia, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DanhGia_ThemDanhGia", ex);
                throw new ArgumentException("TuDH_DanhGia_ThemDanhGia", ex);
            }
        }

        public async Task<int> SuaDanhGia(int ID_DanhGia, string DanhGia)
        {
            try
            {
                var procedureName = "TuDH_DanhGia_SuaDanhGia";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_DanhGia", ID_DanhGia, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("DanhGia", DanhGia, DbType.String, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DanhGia_SuaDanhGia", ex);
                throw new ArgumentException("TuDH_DanhGia_SuaDanhGia", ex);
            }
        }

        public async Task<int> XoaDanhGia(int ID_DanhGia)
        {
            try
            {
                var procedureName = "TuDH_DanhGia_DeleteDanhGia";
                var parameters = new DynamicParameters();
                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_DanhGia", ID_DanhGia, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.ExecuteScalarAsync<int>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DanhGia_DeleteDanhGia", ex);
                throw new ArgumentException("TuDH_DanhGia_DeleteDanhGia", ex);
            }
        }

        public async Task<List<DanhGiaViewModel>> lisDanhGia(int ID_SanPham)
        {
            try
            {
                var procedureName = "TuDH_DanhGia_HienThiDanhGia";
                var parameters = new DynamicParameters();

                using (var connection = _context.CreateConnection())
                {
                    parameters.Add("ID_SanPham", ID_SanPham, DbType.Int32, ParameterDirection.Input);

                    var result = await connection.QueryAsync<DanhGiaViewModel>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 150);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TuDH_DanhGia_HienThiDanhGia", ex);
                throw new ArgumentException("TuDH_DanhGia_HienThiDanhGia", ex);
            }
        }
    }
}
