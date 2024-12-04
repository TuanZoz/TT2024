namespace WebBanHang.Models
{
    public class SanPhamViewModel
    {
        public string Id { get; set; }
        public string Tensp { get; set; }
        public double Gia { get; set; }
        public string Hinh { get; set; }
        public string ChiTiet { get; set; }
        public string MoTaNgan { get; set; }
        public string Idloai { get; set; }
        public int SoLuongTon { get; set; }
        public int DiemDanhGia { get; set; }
    }
    public class OrderRequest
    {
        public List<Product> Products { get; set; }
        public Order Order { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Order
    {
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
    }

}

