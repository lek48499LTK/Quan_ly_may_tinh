using BUS;
using System;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmReportRevenue : Form
    {
        private readonly RevenueReportBUS _reportBUS = new RevenueReportBUS();

        public frmReportRevenue()
        {
            InitializeComponent();
        }

        private void frmReportRevenue_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            LoadReport();
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            if (dtpFromDate.Value.Date > dtpToDate.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var summary = _reportBUS.GetRevenueSummary(dtpFromDate.Value, dtpToDate.Value);

                dgvReport.AutoGenerateColumns = true;
                dgvReport.DataSource = null;
                dgvReport.DataSource = summary.RevenueByDate;

                if (dgvReport.Columns["Ngay"] != null)
                {
                    dgvReport.Columns["Ngay"].HeaderText = "Ngày";
                    dgvReport.Columns["Ngay"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                if (dgvReport.Columns["SoLuongHoaDon"] != null)
                {
                    dgvReport.Columns["SoLuongHoaDon"].HeaderText = "Số hóa đơn";
                }
                if (dgvReport.Columns["TongDoanhThu"] != null)
                {
                    dgvReport.Columns["TongDoanhThu"].HeaderText = "Doanh thu";
                    dgvReport.Columns["TongDoanhThu"].DefaultCellStyle.Format = "N0";
                }
                if (dgvReport.Columns["TongLoiNhuan"] != null)
                {
                    dgvReport.Columns["TongLoiNhuan"].HeaderText = "Lợi nhuận";
                    dgvReport.Columns["TongLoiNhuan"].DefaultCellStyle.Format = "N0";
                }
                dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvTopProducts.AutoGenerateColumns = true;
                dgvTopProducts.DataSource = null;
                dgvTopProducts.DataSource = summary.TopProducts;

                if (dgvTopProducts.Columns["MaSanPham"] != null)
                {
                    dgvTopProducts.Columns["MaSanPham"].HeaderText = "Mã SP";
                }
                if (dgvTopProducts.Columns["TenSanPham"] != null)
                {
                    dgvTopProducts.Columns["TenSanPham"].HeaderText = "Tên sản phẩm";
                }
                if (dgvTopProducts.Columns["SoLuongBan"] != null)
                {
                    dgvTopProducts.Columns["SoLuongBan"].HeaderText = "Số lượng bán";
                }
                if (dgvTopProducts.Columns["DoanhThu"] != null)
                {
                    dgvTopProducts.Columns["DoanhThu"].HeaderText = "Doanh thu";
                    dgvTopProducts.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                }
                dgvTopProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                txtTotalRevenue.Text = summary.TotalRevenue.ToString("N0") + " VNĐ";
                txtTotalProfit.Text = summary.TotalProfit.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
