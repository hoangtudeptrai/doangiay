namespace ShopGiay.Models
{
    public class ThemGioHangViewModel
    {
        public int ID_GioHang { get; set; }

        public int ID_ChiTietSanPham { get; set; }
        public int ID_TaiKhoan { get; set; }
        public int  SoLuong { get; set; }
        public string TenSanPham { get;set; }
        public string TenMauSac { get;set; }
        public int Size { get; set; }
        public string MaMau { get; set; }

    }
}
