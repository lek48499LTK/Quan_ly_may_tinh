using BUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmImport : Form
    {
        private readonly ImportBUS _importBUS = new ImportBUS();
        private readonly List<ImportBUS.ImportItemDto> _importDetails = new List<ImportBUS.ImportItemDto>();
        private decimal _totalAmount = 0;

        public frmImport()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dgvImportDetails.AutoGenerateColumns = false;
            dgvImportDetails.Columns.Clear();

            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaSanPham",
                HeaderText = "Mã SP",
                DataPropertyName = "MaSanPham",
                Width = 80
            });

            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSanPham",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "TenSanPham",
                Width = 250
            });

            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
                Width = 90
            });

            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGiaNhap",
                HeaderText = "Giá nhập",
                DataPropertyName = "DonGiaNhap",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dgvImportDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành tiền",
                DataPropertyName = "ThanhTien",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
        }

        private void frmImport_Load(object sender, EventArgs e)
        {
            try
            {
                LoadSuppliers();
                LoadProducts();
                ResetForm();
                txtStaffName.Text = CurrentUser.HoTen;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSuppliers()
        {
            var suppliers = _importBUS.GetSuppliers();
            cboSupplier.DataSource = suppliers;
            cboSupplier.DisplayMember = "TenNCC";
            cboSupplier.ValueMember = "MaNCC";
            cboSupplier.SelectedIndex = -1;
        }

        private void LoadProducts()
        {
            var products = _importBUS.GetProducts();
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
                var product = _importBUS.GetProductById(productId);
                if (product != null)
                {
                    txtPrice.Text = product.GiaNhap.ToString("N0");
                    CalculateLineTotal();
                }
            }
        }

        private void numQuantity_ValueChanged(object sender, EventArgs e)
        {
            CalculateLineTotal();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateLineTotal();
        }

        private void CalculateLineTotal()
        {
            decimal price;
            if (TryParseDecimal(txtPrice.Text, out price) && numQuantity.Value > 0)
            {
                txtTotal.Text = (price * numQuantity.Value).ToString("N0");
            }
            else
            {
                txtTotal.Text = "0";
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
            if (!TryParseDecimal(txtPrice.Text, out price) || price <= 0)
            {
                MessageBox.Show("Giá nhập không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = _importDetails.FirstOrDefault(x => x.MaSanPham == productId);
            int quantity = (int)numQuantity.Value;

            if (existing != null)
            {
                existing.SoLuong += quantity;
                existing.DonGiaNhap = price;
                existing.ThanhTien = existing.SoLuong * existing.DonGiaNhap;
            }
            else
            {
                _importDetails.Add(new ImportBUS.ImportItemDto
                {
                    MaSanPham = productId,
                    TenSanPham = cboProduct.Text,
                    SoLuong = quantity,
                    DonGiaNhap = price,
                    ThanhTien = quantity * price
                });
            }

            RefreshGrid();
            UpdateTotalAmount();
            ClearInputFields();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvImportDetails.CurrentRow == null)
            {
                return;
            }

            var detail = dgvImportDetails.CurrentRow.DataBoundItem as ImportBUS.ImportItemDto;
            if (detail == null)
            {
                return;
            }

            _importDetails.Remove(detail);
            RefreshGrid();
            UpdateTotalAmount();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int supplierId;
            if (!TryGetSelectedIntValue(cboSupplier, out supplierId))
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = _importBUS.SaveImport(dtpImportDate.Value, supplierId, CurrentUser.MaNguoiDung, _importDetails);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(string.Format("{0}\nMã phiếu nhập: {1}", result.Message, result.Id),
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadProducts();
            ResetForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RefreshGrid()
        {
            dgvImportDetails.DataSource = null;
            dgvImportDetails.DataSource = _importDetails.ToList();
        }

        private void UpdateTotalAmount()
        {
            _totalAmount = _importDetails.Sum(x => x.ThanhTien);
        }

        private void ClearInputFields()
        {
            cboProduct.SelectedIndex = -1;
            numQuantity.Value = 1;
            txtPrice.Clear();
            txtTotal.Clear();
        }

        private void ResetForm()
        {
            _importDetails.Clear();
            _totalAmount = 0;
            RefreshGrid();
            dtpImportDate.Value = DateTime.Now;
            txtInvoiceNumber.Text = "PN" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (cboSupplier.DataSource != null)
            {
                cboSupplier.SelectedIndex = -1;
            }
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
