﻿using Microsoft.AspNetCore.Http;
using ShopGiay.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ShopGiay.Interfaces
{
    public interface ISanPhamRepository
    {
        public Task<int> ThemSanPham( DM_SanPhamModel obj);
        public Task<List<ImageViewModel>> UploadFile(IFormFileCollection file, int ID_SanPham);
        public Task<bool> LuuThongTinFile(List<ImageViewModel> list);
        public Task<List<TrangChuSanPhamViewModel>> GetList_SanPham();

        public Task<DataSet> ThongTinChiTietSanPham(int ID_SanPham);

        public Task<int> XoaSanPham(int ID_SanPham);

        public Task<bool> UpdateSanPham(UpdateSanPhamModels obj);

        public Task<int> XoaAnh(int ID_SanPham);


        public Task<int> ThemGoHang(int ID_ChiTietSanPham, int ID_TaiKkhoan, int SoLuong);
        public Task<int> XoaGioHang(int ID_GioHang);
        public Task<int> SuaGioHang(int ID_GioHang, int ID_ChiTietSanPham, int SoLuong);
        public Task<List<ThemGioHangViewModel>> listGioHang(int ID_TaiKhoan);



        public Task<int> ThanhToanHoaDon (string ListID_GioHang, int ID_TaiKkhoan, int ID_MaGiamGia);
        public Task<int> XoaHoaDon(int ID_HoaDon);
        public Task<List<HoaDonViewModel>> listHoaDon(int ID_TaiKhoan);
        public Task<List<ThemGioHangViewModel>> listGChiTietHoaDon(int ID_HoaDon);




        public Task<List<MaGiamGiaViewModelcs>> listMaGiamGia();
        public Task<int> ThemMaGiamGia(string MaGiamGia, int GiaTri, int LoaiGiamGia);
        public Task<int> XoaMaGiamGia(int ID_MaGiamGia);


    }
}
