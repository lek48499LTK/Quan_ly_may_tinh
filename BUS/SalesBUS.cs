using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class SalesBUS
    {
        public class SaleItemDto
        {
            public int MaSanPham { get; set; }
            public string TenSanPham { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGiaBan { get; set; }
            public decimal GiamGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class CustomerOptionDto
        {
            public int MaKhachHang { get; set; }
            public string HoTen { get; set; }
        }

        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public int? Id { get; set; }
        }

        public List<CustomerOptionDto> GetCustomersForSale()
        {
            using (var db = new QLMayTinhDB())
            {
                var result = new List<CustomerOptionDto>
                {
                    new CustomerOptionDto { MaKhachHang = 0, HoTen = "Khách lẻ" }
                };

                result.AddRange(db.KhachHangs
                    .OrderBy(x => x.HoTen)
                    .Select(x => new CustomerOptionDto
                    {
                        MaKhachHang = x.MaKhachHang,
                        HoTen = x.HoTen
                    })
                    .ToList());

                return result;
            }
        }

        public List<SanPham> GetAvailableProducts()
        {
            using (var db = new QLMayTinhDB())
            {
                return db.SanPhams
                    .Where(x => x.SoLuongTon > 0)
                    .OrderBy(x => x.TenSanPham)
                    .ToList();
            }
        }

        public SanPham GetProductById(int productId)
        {
            using (var db = new QLMayTinhDB())
            {
                return db.SanPhams.FirstOrDefault(x => x.MaSanPham == productId);
            }
        }

        public OperationResult SaveSale(DateTime ngayLap, int? maKhachHang, int maNguoiDung, decimal amountPaid, List<SaleItemDto> items)
        {
            if (maNguoiDung <= 0)
            {
                return new OperationResult { Success = false, Message = "Người dùng đăng nhập không hợp lệ!" };
            }

            if (items == null || items.Count == 0)
            {
                return new OperationResult { Success = false, Message = "Chưa có sản phẩm nào trong giỏ hàng!" };
            }

            foreach (var item in items)
            {
                if (item.SoLuong <= 0)
                {
                    return new OperationResult { Success = false, Message = "Số lượng bán phải lớn hơn 0!" };
                }

                if (item.DonGiaBan <= 0)
                {
                    return new OperationResult { Success = false, Message = "Đơn giá bán phải lớn hơn 0!" };
                }

                if (item.ThanhTien < 0)
                {
                    return new OperationResult { Success = false, Message = "Thành tiền không hợp lệ!" };
                }
            }

            decimal tongTien = items.Sum(x => x.ThanhTien);
            decimal tongGiamGia = items.Sum(x => x.GiamGia);

            if (amountPaid < tongTien)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = string.Format("Số tiền khách trả không đủ!\nTổng tiền: {0:N0} VNĐ", tongTien)
                };
            }

            using (var db = new QLMayTinhDB())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in items)
                    {
                        var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);
                        if (sanPham == null)
                        {
                            transaction.Rollback();
                            return new OperationResult { Success = false, Message = "Có sản phẩm không tồn tại trong hệ thống!" };
                        }

                        if (sanPham.SoLuongTon < item.SoLuong)
                        {
                            transaction.Rollback();
                            return new OperationResult
                            {
                                Success = false,
                                Message = string.Format("Sản phẩm '{0}' chỉ còn {1} sản phẩm trong kho!", sanPham.TenSanPham, sanPham.SoLuongTon)
                            };
                        }
                    }

                    var hoaDon = new HoaDon
                    {
                        NgayLap = ngayLap,
                        MaKhachHang = maKhachHang,
                        MaNguoiDung = maNguoiDung,
                        TongTien = tongTien,
                        GiamGia = tongGiamGia,
                        ThanhToan = tongTien
                    };

                    db.HoaDons.Add(hoaDon);
                    db.SaveChanges();

                    foreach (var item in items)
                    {
                        db.ChiTietHoaDons.Add(new ChiTietHoaDon
                        {
                            MaHoaDon = hoaDon.MaHoaDon,
                            MaSanPham = item.MaSanPham,
                            SoLuong = item.SoLuong,
                            DonGiaBan = item.DonGiaBan,
                            ThanhTien = item.ThanhTien
                        });

                        var sanPham = db.SanPhams.First(x => x.MaSanPham == item.MaSanPham);
                        sanPham.SoLuongTon -= item.SoLuong;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return new OperationResult
                    {
                        Success = true,
                        Message = "Thanh toán thành công!",
                        Id = hoaDon.MaHoaDon
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Lỗi khi thanh toán: " + ex.Message
                    };
                }
            }
        }
    }
}
