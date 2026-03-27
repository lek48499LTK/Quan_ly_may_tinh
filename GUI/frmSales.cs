using BUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmSales : Form
    {
        private readonly SalesBUS _salesBUS = new SalesBUS();
        private readonly List<SalesBUS.SaleItemDto> _cart = new List<SalesBUS.SaleItemDto>();
        private decimal _totalAmount = 0;

        public frmSales()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaSanPham",
                HeaderText = "Mã SP",
                DataPropertyName = "MaSanPham",
                Width = 80
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSanPham",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "TenSanPham",
                Width = 230
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
                Width = 80
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGiaBan",
                HeaderText = "Đơn giá",
                DataPropertyName = "DonGiaBan",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GiamGia",
                HeaderText = "Giảm giá",
                DataPropertyName = "GiamGia",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành tiền",
                DataPropertyName = "ThanhTien",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
        }

        private void frmSales_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCustomers();
                LoadProducts();
                ResetForm();
                txtStaffName.Text = CurrentUser.HoTen;
                txtDiscount.Text = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomers()
        {
            var customers = _salesBUS.GetCustomersForSale();
            cboCustomer.DataSource = customers;
            cboCustomer.DisplayMember = "HoTen";
            cboCustomer.ValueMember = "MaKhachHang";
            cboCustomer.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            var products = _salesBUS.GetAvailableProducts();
            cboProduct.DataSource = products;
            cboProduct.DisplayMember = "TenSanPham";
            cboProduct.ValueMember = "MaSanPham";
            cboProduct.SelectedIndex = -1;
        }

        private void cboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            int productId;
            if (TryGetSelectedIntValue(cboProduct, out productId))
            {
                var product = _salesBUS.GetProductById(productId);
                if (product != null)
                {
                    txtUnitPrice.Text = product.GiaBan.ToString("N0");
                    CalculateSubTotal();
                }
            }
        }

        private void numQuantity_ValueChanged(object sender, EventArgs e)
        {
            CalculateSubTotal();
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            CalculateSubTotal();
        }

        private void txtAmountPaid_TextChanged(object sender, EventArgs e)
        {
            CalculateChange();
        }

        private void CalculateSubTotal()
        {
            decimal price;
            decimal discount;
            if (TryParseDecimal(txtUnitPrice.Text, out price) && numQuantity.Value > 0)
            {
                if (!TryParseDecimal(txtDiscount.Text, out discount))
                {
                    discount = 0;
                }

                var subTotal = (price * numQuantity.Value) - discount;
                txtSubTotal.Text = Math.Max(subTotal, 0).ToString("N0");
            }
            else
            {
                txtSubTotal.Text = "0";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int productId;
            if (!TryGetSelectedIntValue(cboProduct, out productId))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price;
            if (!TryParseDecimal(txtUnitPrice.Text, out price) || price <= 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal discount;
            if (!TryParseDecimal(txtDiscount.Text, out discount))
            {
                discount = 0;
            }

            if (discount < 0)
            {
                MessageBox.Show("Giảm giá không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var product = _salesBUS.GetProductById(productId);
            if (product == null)
            {
                MessageBox.Show("Sản phẩm không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int quantity = (int)numQuantity.Value;
            var existing = _cart.FirstOrDefault(x => x.MaSanPham == productId);
            int totalRequestedQuantity = (existing != null ? existing.SoLuong : 0) + quantity;

            if (product.SoLuongTon < totalRequestedQuantity)
            {
                MessageBox.Show(
                    string.Format("Sản phẩm chỉ còn {0} sản phẩm trong kho!", product.SoLuongTon),
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (discount > price * quantity)
            {
                MessageBox.Show("Giảm giá không được lớn hơn thành tiền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (existing != null)
            {
                existing.SoLuong += quantity;
                existing.GiamGia += discount;
                existing.DonGiaBan = price;
                existing.ThanhTien = (existing.DonGiaBan * existing.SoLuong) - existing.GiamGia;
            }
            else
            {
                _cart.Add(new SalesBUS.SaleItemDto
                {
                    MaSanPham = productId,
                    TenSanPham = product.TenSanPham,
                    SoLuong = quantity,
                    DonGiaBan = price,
                    GiamGia = discount,
                    ThanhTien = (price * quantity) - discount
                });
            }

            RefreshGrid();
            UpdateTotalAmount();
            ClearInputFields();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow == null)
            {
                return;
            }

            var detail = dgvCart.CurrentRow.DataBoundItem as SalesBUS.SaleItemDto;
            if (detail == null)
            {
                return;
            }

            _cart.Remove(detail);
            RefreshGrid();
            UpdateTotalAmount();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            decimal amountPaid;
            if (!TryParseDecimal(txtAmountPaid.Text, out amountPaid))
            {
                MessageBox.Show("Vui lòng nhập số tiền khách trả hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int customerId;
            int? selectedCustomerId = null;
            if (TryGetSelectedIntValue(cboCustomer, out customerId) && customerId > 0)
            {
                selectedCustomerId = customerId;
            }

            var result = _salesBUS.SaveSale(dtpSaleDate.Value, selectedCustomerId, CurrentUser.MaNguoiDung, amountPaid, _cart);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(
                string.Format("{0}\nMã hóa đơn: {1}\nTổng tiền: {2:N0} VNĐ\nKhách trả: {3:N0} VNĐ\nTiền thừa: {4:N0} VNĐ",
                    result.Message,
                    result.Id,
                    _totalAmount,
                    amountPaid,
                    amountPaid - _totalAmount),
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadProducts();
            ResetForm();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng in hóa đơn đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RefreshGrid()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = _cart.ToList();
        }

        private void UpdateTotalAmount()
        {
            _totalAmount = _cart.Sum(x => x.ThanhTien);
            txtTotal.Text = _totalAmount.ToString("N0");
            CalculateChange();
        }

        private void CalculateChange()
        {
            decimal amountPaid;
            if (TryParseDecimal(txtAmountPaid.Text, out amountPaid))
            {
                var change = amountPaid - _totalAmount;
                txtChange.Text = (change >= 0 ? change : 0).ToString("N0");
            }
            else
            {
                txtChange.Text = "0";
            }
        }

        private void ClearInputFields()
        {
            cboProduct.SelectedIndex = -1;
            numQuantity.Value = 1;
            txtUnitPrice.Clear();
            txtDiscount.Text = "0";
            txtSubTotal.Clear();
        }

        private void ResetForm()
        {
            _cart.Clear();
            _totalAmount = 0;
            RefreshGrid();
            dtpSaleDate.Value = DateTime.Now;
            txtInvoiceNumber.Text = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (cboCustomer.DataSource != null)
            {
                cboCustomer.SelectedIndex = 0;
            }
            txtAmountPaid.Clear();
            txtTotal.Text = "0";
            txtChange.Text = "0";
            ClearInputFields();
        }

        private static bool TryParseDecimal(string value, out decimal result)
        {
            var normalized = (value ?? string.Empty).Replace(",", string.Empty).Trim();
            return decimal.TryParse(normalized, out result);
        }

        private static bool TryGetSelectedIntValue(ComboBox comboBox, out int value)
        {
            value = 0;
            if (comboBox.SelectedValue == null)
            {
                return false;
            }

            return int.TryParse(comboBox.SelectedValue.ToString(), out value);
        }
    }
}
