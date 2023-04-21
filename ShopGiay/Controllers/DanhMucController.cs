using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopGiay.Interfaces;
using ShopGiay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ShopGiay.Context;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Helps;
using System.Text.RegularExpressions;
using System.Text;

namespace ShopGiay.Controllers
{
    public class DanhMucController: ControllerBase
    {
        private readonly ILogger<DanhMucController> _logger;
        private readonly IDanhMucRepository _danhMucRepository;


        public DanhMucController(
           ILogger<DanhMucController> logger,
           IDanhMucRepository danhMucRepository
           )
        {
            _logger = logger;
            _danhMucRepository = danhMucRepository;
        }

        [HttpGet("GetList_TaiKhoan")]
        public async Task<IActionResult> GetList_TaiKhoan()
        {
            try
            {
                List<DM_TaiKhoanViewModel> result = await _danhMucRepository.GetList_TaiKhoan();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }

        [HttpGet("CheckDanhNhap")]
        public async Task<IActionResult> CheckLoign(string TaiKhoan, string MatKhau)
        {
            try
            {
                MatKhau = PassWorkHasher.MD5Pass(MatKhau);

                var result = await _danhMucRepository.CheckLoign(TaiKhoan, MatKhau);

                if (result == 0)
                    return Ok(new { flag = false, msg = "Đăng nhập không thành công", value = result });

                return Ok(new { flag = true, msg = "Đăng nhập thành công", value = result });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("ThemMoiTaiKhoan")]
        public async Task<IActionResult> ThemMoiTaiKhoan([FromBody] DM_TaiKhoanViewModel dM_TaiKhoanViewModel)
        {
            try
            {
                string pass = CheckPassWork(dM_TaiKhoanViewModel.MatKhau);
                if (!string.IsNullOrEmpty(pass))
                {
                    return Ok(new { flag = false, msg = pass });
                }

                string email = CheckEmail(dM_TaiKhoanViewModel.Email);
                if (!string.IsNullOrEmpty(email))
                {
                    return Ok(new { flag = false, msg = email });
                }


                dM_TaiKhoanViewModel.MatKhau = PassWorkHasher.MD5Pass(dM_TaiKhoanViewModel.MatKhau);

                var result = await _danhMucRepository.ThemMoiTaiKhoan(dM_TaiKhoanViewModel);
                
                if(result == 0)
                    return Ok(new { flag = false, msg = "Đăng kí không thành công", value = result });

                if (result == -1)
                    return Ok(new { flag = false, msg = "Tên đăng nhập đã tồn tại", value = result });

                return Ok(new { flag = true, msg = "Đăng kí thành công", value = result });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("DoiMatKhau")]
        public async Task<IActionResult> DoiMatKhau(string TaiKhoan, string MatKhauCu, string MatKhauMoi, int ID_TaiKhoan)
        {
            try
            {
                string MatKhauText = MatKhauMoi.ToString();
                MatKhauCu = PassWorkHasher.MD5Pass(MatKhauCu);
                MatKhauMoi = PassWorkHasher.MD5Pass(MatKhauMoi);

                var result = await _danhMucRepository.DoiMatKhau(TaiKhoan, MatKhauCu, MatKhauMoi, ID_TaiKhoan);


                if (result == -1)
                    return Ok(new { flag = false, msg = "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại", value = result });

                if (result == 0)
                    return Ok(new { flag = false, msg = "Không thành công, vui lòng thửu lại sau", value = result });

                string pass = CheckPassWork(MatKhauText);
                if (!string.IsNullOrEmpty(pass))
                {
                    return Ok(new { flag = false, msg = pass });
                }


                return Ok(new { flag = true, msg = "Đổi mật khẩu thàng công", value = result });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("XoaTaiKhoan")]
        public async Task<IActionResult> XoaTaiKhoan(int ID_TaiKhoan)
        {
            try
            {

                var result = await _danhMucRepository.XoaTaiKhoan(ID_TaiKhoan);


                if (result == -1)
                    return Ok(new { flag = false, msg = "Xóa không thành công", value = result });

                return Ok(new { flag = true, msg = "Xóa thàng công", value = result });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        public string CheckPassWork(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
            {
                sb.Append("Mật khẩu phải dài hơn 8 kí tự" + Environment.NewLine);
            }

            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Mật khẩu phải có chữ in hoa và số" + Environment.NewLine);
            }

            if (!Regex.IsMatch(password, "[<,>,@]"))
            {
                sb.Append("Mật khẩu phải có kí tự đặc biệt" + Environment.NewLine);
            }
            return sb.ToString();
        }

        public string CheckEmail (string email)
        {
            StringBuilder sb = new StringBuilder();

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);

            if (!match.Success)
                sb.Append("Email chưua đúng định dạng" + Environment.NewLine);

            return sb.ToString();
        }

        [HttpGet("ListNhanHieu")]
        public async Task<IActionResult> ListNhanHieu()
        {
            try
            {
                List<DM_NhanHieuViewModel> result = await _danhMucRepository.ListNhanHieu();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }

        [HttpPost("XoaNhanHieu")]
        public async Task<IActionResult> XoaNhanHieu(int ID_NhanHieu)
        {
            try
            {
                var result = await _danhMucRepository.XoaNhanHieu(ID_NhanHieu);


                if (result <0)
                    return Ok(new { flag = false, msg = "Xóa không thành công", value = result });

                return Ok(new { flag = true, msg = "Xóa thàng công", value = result });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("SuaNhanHieu")]
        public async Task<IActionResult> SuaNhanHieu(int ID_NhanHieu, string TenNhanHieu)
        {
            try
            {
                var result = await _danhMucRepository.SuaNhanHieu(ID_NhanHieu, TenNhanHieu);


                if (result < 0)
                    return Ok(new { flag = false, msg = "Sửa không thành công", value = result });

                return Ok(new { flag = true, msg = "Sửa thàng công", value = result });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("ThemNhanHieu")]
        public async Task<IActionResult> ThemNhanHieu(string TenNhanHieu)
        {
            try
            {
                var result = await _danhMucRepository.ThemNhanHieu(TenNhanHieu);


                if (result < 0)
                    return Ok(new { flag = false, msg = "Thêm không thành công", value = result });

                return Ok(new { flag = true, msg = "Thêm  thàng công", value = result });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("listDiaChiNhanHang")]
        public async Task<IActionResult> listDiaChiNhanHang(int ID_TaiKhoan)
        {
            try
            {
                List<DiaChiNhanHangViewModel> result = await _danhMucRepository.listDiaChiNhanHang(ID_TaiKhoan);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }
        [HttpPost("ThemDiaChi")]
        public async Task<IActionResult> ThemDiaChi(int ID_TaiKhoan, string DiaChi)
        {
            try
            {
                var result = await _danhMucRepository.ThemDiaChi(ID_TaiKhoan, DiaChi);

                return Ok(new { value = result, flag = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("XoaDiaChi")]
        public async Task<IActionResult> XoaDiaChi(int ID_DiaChiNhanHang)
        {
            try
            {
                var result = await _danhMucRepository.XoaDiaChi(ID_DiaChiNhanHang);

                return Ok(new { value = result, flag = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("lisDanhGia")]
        public async Task<IActionResult> lisDanhGia(int ID_SanPham)
        {
            try
            {
                List<DanhGiaViewModel> result = await _danhMucRepository.lisDanhGia(ID_SanPham);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }

        [HttpPost("ThemDanhGia")]
        public async Task<IActionResult> ThemDanhGia(int ID_SanPham, int ID_TaiKhoan, string DanhGia)
        {
            try
            {
                var result = await _danhMucRepository.ThemDanhGia(ID_SanPham, ID_TaiKhoan, DanhGia);

                return Ok(new { value = result, flag = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("SuaDanhGia")]
        public async Task<IActionResult> SuaDanhGia(int ID_DanhGia, string DanhGia)
        {
            try
            {
                var result = await _danhMucRepository.SuaDanhGia(ID_DanhGia, DanhGia);

                return Ok(new { value = result, flag = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("XoaDanhGia")]
        public async Task<IActionResult> XoaDanhGia(int ID_DanhGia)
        {
            try
            {
                var result = await _danhMucRepository.XoaDanhGia(ID_DanhGia);

                return Ok(new { value = result, flag = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
