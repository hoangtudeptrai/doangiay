using System.Collections.Generic;

namespace ShopGiay.Models
{
    public class UpdateSanPhamModels
    {
        public int ID_SanPham { get; set; }
        public int ID_ThongTinSanPham { get; set; }
        public int ID_MoTaSanPham { get; set;}
        public int ID_NhanHieu { get; set; }
        public string TenSanPham { get; set; }
        public string GioiThieuSanPham { get; set; }
        public long GiaSanPham { get; set; }
        public long PhiVanChuyen { get; set; }
        public List<ChiTietSanPham> listChiTietSanPham { get; set; }

        public string ThongTinMoTa { get; set; }

        public string TinhTrangSanPham { get; set; }
        public string LoaiKhoaDay { get; set; }
        public string ChatLieu { get; set; }
        public string XuatXu { get; set; }
        public string ChieuCaoCoGiay { get; set; }
        public string DiaChiGuiHang { get; set; }


    }

    public class ChiTietSanPham
    {
        public int ID_ChiTietSanPham { get; set; }

        public int ID_MauSac { get; set; }
        public int SoLuong { get; set; }
        public int Size { get; set; }

    }
}
