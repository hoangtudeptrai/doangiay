﻿using ShopGiay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopGiay.Interfaces
{
    public interface IDanhMucRepository
    {
        public Task<List<DM_TaiKhoanViewModel>> GetList_TaiKhoan();
        public Task<int> ThemMoiTaiKhoan(DM_TaiKhoanViewModel dM_TaiKhoanViewModel);
        public Task<int> CheckLoign(string TaiKhoan, string MatKhau);
        public Task<int> DoiMatKhau(string TaiKhoan, string MatKhauCu, string MatKhauMoi, int ID_TaiKhoan);
        public Task<int> XoaTaiKhoan( int ID_TaiKhoan);


        public Task<int> ThemNhanHieu(string TenNhanHieu);
        public Task<int> SuaNhanHieu(int ID_NhanHieu, string TenNhanHieu);
        public Task<int> XoaNhanHieu(int ID_NhanHieu);
        public Task<List<DM_NhanHieuViewModel>> ListNhanHieu();



    }
}
