namespace GUI
{
    partial class frmChangePassword
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMatKhauCu = new System.Windows.Forms.TextBox();
            this.txtMatKhauMoi = new System.Windows.Forms.TextBox();
            this.txtXacNhan = new System.Windows.Forms.TextBox();
            this.groupbox = new System.Windows.Forms.GroupBox();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.btnXacNhan = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.groupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(126, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mật khẩu cũ:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(126, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Mật khẩu mới:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(126, 302);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Xác nhận mật khẩu mới:";
            // 
            // txtMatKhauCu
            // 
            this.txtMatKhauCu.Location = new System.Drawing.Point(267, 60);
            this.txtMatKhauCu.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMatKhauCu.Name = "txtMatKhauCu";
            this.txtMatKhauCu.Size = new System.Drawing.Size(247, 26);
            this.txtMatKhauCu.TabIndex = 1;
            // 
            // txtMatKhauMoi
            // 
            this.txtMatKhauMoi.Location = new System.Drawing.Point(267, 149);
            this.txtMatKhauMoi.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMatKhauMoi.Name = "txtMatKhauMoi";
            this.txtMatKhauMoi.Size = new System.Drawing.Size(247, 26);
            this.txtMatKhauMoi.TabIndex = 1;
            // 
            // txtXacNhan
            // 
            this.txtXacNhan.Location = new System.Drawing.Point(267, 254);
            this.txtXacNhan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtXacNhan.Name = "txtXacNhan";
            this.txtXacNhan.Size = new System.Drawing.Size(247, 26);
            this.txtXacNhan.TabIndex = 1;
            // 
            // groupbox
            // 
            this.groupbox.Controls.Add(this.txtXacNhan);
            this.groupbox.Controls.Add(this.txtMatKhauCu);
            this.groupbox.Controls.Add(this.txtMatKhauMoi);
            this.groupbox.Controls.Add(this.chkShowPassword);
            this.groupbox.Location = new System.Drawing.Point(96, 42);
            this.groupbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupbox.Name = "groupbox";
            this.groupbox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupbox.Size = new System.Drawing.Size(699, 356);
            this.groupbox.TabIndex = 2;
            this.groupbox.TabStop = false;
            this.groupbox.Text = "Thay đổi mật khẩu";
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Location = new System.Drawing.Point(534, 60);
            this.chkShowPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(138, 24);
            this.chkShowPassword.TabIndex = 5;
            this.chkShowPassword.Text = "Hiện mật khẩu";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            // 
            // btnXacNhan
            // 
            this.btnXacNhan.Location = new System.Drawing.Point(141, 442);
            this.btnXacNhan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnXacNhan.Name = "btnXacNhan";
            this.btnXacNhan.Size = new System.Drawing.Size(114, 42);
            this.btnXacNhan.TabIndex = 3;
            this.btnXacNhan.Text = "Xác nhận";
            this.btnXacNhan.UseVisualStyleBackColor = true;
            this.btnXacNhan.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Location = new System.Drawing.Point(387, 442);
            this.btnThoat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(95, 42);
            this.btnThoat.TabIndex = 4;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            // 
            // frmChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 562);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnXacNhan);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupbox);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmChangePassword";
            this.Text = "frmChangePassword";
            this.groupbox.ResumeLayout(false);
            this.groupbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMatKhauCu;
        private System.Windows.Forms.TextBox txtMatKhauMoi;
        private System.Windows.Forms.TextBox txtXacNhan;
        private System.Windows.Forms.GroupBox groupbox;
        private System.Windows.Forms.Button btnXacNhan;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.CheckBox chkShowPassword;
    }
}