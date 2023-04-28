namespace ShopGiay.Models
{
    public class HoaDonViewModel
    {
        public int ID_HoaDon { get; set; } 
        public int ID_TaiKhoan { get; set; }
        public int ID_MaGiamGia { get; set; }   
        public float GiaTri { get; set; }
        public float TongTien { get; set; }
        public float TongTienSauGiamGia { get; set; }
    }
}
