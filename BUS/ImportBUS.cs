using DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BUS
{
    public class ImportBUS
    {
        public class ImportItemDto
        {
            public int MaSanPham { get; set; }
            public string TenSanPham { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGiaNhap { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public int? Id { get; set; }
        }

        public List<NhaCungCap> GetSuppliers()
        {
            using (var db = new QLMayTinhDB())
            {
                return db.NhaCungCaps.OrderBy(x => x.TenNCC).ToList();
            }
        }

        public List<SanPham> GetProducts()
        {
            using (var db = new QLMayTinhDB())
            {
                return db.SanPhams.OrderBy(x => x.TenSanPham).ToList();
            }
        }

        public SanPham GetProductById(int productId)
        {
            using (var db = new QLMayTinhDB())
            {
                return db.SanPhams.FirstOrDefault(x => x.MaSanPham == productId);
            }
        }

        public OperationResult SaveImport(DateTime ngayNhap, int maNcc, int maNguoiDung, List<ImportItemDto> items)
        {
            if (maNguoiDung <= 0)
            {
                return new OperationResult { Success = false, Message = "Người dùng đăng nhập không hợp lệ!" };
            }

            if (maNcc <= 0)
            {
                return new OperationResult { Success = false, Message = "Vui lòng chọn nhà cung cấp!" };
            }

            if (items == null || items.Count == 0)
            {
                return new OperationResult { Success = false, Message = "Chưa có sản phẩm nào trong phiếu nhập!" };
            }

            foreach (var item in items)
            {
                if (item.SoLuong <= 0)
                {
                    return new OperationResult { Success = false, Message = "Số lượng nhập phải lớn hơn 0!" };
                }

                if (item.DonGiaNhap <= 0)
                {
                    return new OperationResult { Success = false, Message = "Đơn giá nhập phải lớn hơn 0!" };
                }
            }

            using (var db = new QLMayTinhDB())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    decimal tongTien = items.Sum(x => x.ThanhTien);

                    var phieuNhap = new PhieuNhap
                    {
                        NgayNhap = ngayNhap,
                        MaNCC = maNcc,
                        MaNguoiDung = maNguoiDung,
                        TongTien = tongTien
                    };

                    db.PhieuNhaps.Add(phieuNhap);
                    db.SaveChanges();

                    foreach (var item in items)
                    {
                        var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);
                        if (sanPham == null)
                        {
                            transaction.Rollback();
                            return new OperationResult
                            {
                                Success = false,
                                Message = "Có sản phẩm không tồn tại trong hệ thống!"
                            };
                        }

                        db.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                        {
                            MaPhieuNhap = phieuNhap.MaPhieuNhap,
                            MaSanPham = item.MaSanPham,
                            SoLuong = item.SoLuong,
                            DonGiaNhap = item.DonGiaNhap,
                            ThanhTien = item.ThanhTien
                        });

                        sanPham.SoLuongTon += item.SoLuong;
                        sanPham.GiaNhap = item.DonGiaNhap;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return new OperationResult
                    {
                        Success = true,
                        Message = "Nhập hàng thành công!",
                        Id = phieuNhap.MaPhieuNhap
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Lỗi khi lưu phiếu nhập: " + ex.Message
                    };
                }
            }
        }
    }
}
