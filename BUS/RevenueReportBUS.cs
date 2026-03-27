using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class RevenueReportBUS
    {
        public class RevenueByDateDto
        {
            public DateTime Ngay { get; set; }
            public int SoLuongHoaDon { get; set; }
            public decimal TongDoanhThu { get; set; }
            public decimal TongLoiNhuan { get; set; }
        }

        public class TopProductDto
        {
            public int MaSanPham { get; set; }
            public string TenSanPham { get; set; }
            public int SoLuongBan { get; set; }
            public decimal DoanhThu { get; set; }
        }

        public class RevenueSummaryDto
        {
            public List<RevenueByDateDto> RevenueByDate { get; set; }
            public List<TopProductDto> TopProducts { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TotalProfit { get; set; }

            public RevenueSummaryDto()
            {
                RevenueByDate = new List<RevenueByDateDto>();
                TopProducts = new List<TopProductDto>();
            }
        }

        public RevenueSummaryDto GetRevenueSummary(DateTime fromDate, DateTime toDate)
        {
            var startDate = fromDate.Date;
            var endDate = toDate.Date.AddDays(1).AddTicks(-1);

            using (var db = new QLMayTinhDB())
            {
                var revenueByDate = (from hd in db.HoaDons
                                     join ct in db.ChiTietHoaDons on hd.MaHoaDon equals ct.MaHoaDon
                                     join sp in db.SanPhams on ct.MaSanPham equals sp.MaSanPham
                                     where hd.NgayLap >= startDate && hd.NgayLap <= endDate
                                     group new { hd, ct, sp } by hd.NgayLap.Year + "-" + hd.NgayLap.Month + "-" + hd.NgayLap.Day into g
                                     select new RevenueByDateDto
                                     {
                                         Ngay = g.Select(x => x.hd.NgayLap).Min(),
                                         SoLuongHoaDon = g.Select(x => x.hd.MaHoaDon).Distinct().Count(),
                                         TongDoanhThu = g.Sum(x => x.ct.ThanhTien),
                                         TongLoiNhuan = g.Sum(x => x.ct.ThanhTien - (x.ct.SoLuong * x.sp.GiaNhap))
                                     })
                                     .ToList()
                                     .OrderBy(x => x.Ngay.Date)
                                     .Select(x => new RevenueByDateDto
                                     {
                                         Ngay = x.Ngay.Date,
                                         SoLuongHoaDon = x.SoLuongHoaDon,
                                         TongDoanhThu = x.TongDoanhThu,
                                         TongLoiNhuan = x.TongLoiNhuan
                                     })
                                     .ToList();

                var topProducts = (from ct in db.ChiTietHoaDons
                                   join hd in db.HoaDons on ct.MaHoaDon equals hd.MaHoaDon
                                   join sp in db.SanPhams on ct.MaSanPham equals sp.MaSanPham
                                   where hd.NgayLap >= startDate && hd.NgayLap <= endDate
                                   group ct by new { ct.MaSanPham, sp.TenSanPham } into g
                                   select new TopProductDto
                                   {
                                       MaSanPham = g.Key.MaSanPham,
                                       TenSanPham = g.Key.TenSanPham,
                                       SoLuongBan = g.Sum(x => x.SoLuong),
                                       DoanhThu = g.Sum(x => x.ThanhTien)
                                   })
                                   .OrderByDescending(x => x.SoLuongBan)
                                   .ThenBy(x => x.TenSanPham)
                                   .Take(10)
                                   .ToList();

                return new RevenueSummaryDto
                {
                    RevenueByDate = revenueByDate,
                    TopProducts = topProducts,
                    TotalRevenue = revenueByDate.Sum(x => x.TongDoanhThu),
                    TotalProfit = revenueByDate.Sum(x => x.TongLoiNhuan)
                };
            }
        }
    }
}
