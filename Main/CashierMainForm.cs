using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invento
{
    public partial class CashierMainForm : Form
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public CashierMainForm()
        {
            InitializeComponent();

            user_username.Text = Form1.username;

            settings1.SetParentForm(this);

            ApplyModernStyle();
        }

        private void ApplyModernStyle()
        {
            // Form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Round corners for the main form
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

            // Style all navigation buttons
            StyleNavigationButton(dashboard_btn);
            StyleNavigationButton(addProducts_btn);
            StyleNavigationButton(customers_btn);
            StyleNavigationButton(order_btn);
            StyleNavigationButton(settings_btn); // New settings button
            StyleLogoutButton(logout_btn);

            // Style the close button
            if (close != null)
            {
                close.FlatStyle = FlatStyle.Flat;
                close.FlatAppearance.BorderSize = 0;
                close.BackColor = Color.Firebrick;
                close.ForeColor = Color.White;
                close.Text = "×";
                close.Font = new Font("Arial", 14, FontStyle.Bold);
                close.Size = new Size(30, 30);
                close.Cursor = Cursors.Hand;
                close.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, close.Width, close.Height, 15, 15));

                // Hover effects
                close.MouseEnter += (s, e) => {
                    close.BackColor = Color.DarkRed;
                };
                close.MouseLeave += (s, e) => {
                    close.BackColor = Color.Firebrick;
                };
            }
        }

        private void StyleNavigationButton(Button button)
        {
            if (button != null)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = Color.Transparent;
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                button.Cursor = Cursors.Hand;
                button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 15, 15));

                // Hover effects
                button.MouseEnter += (s, e) => {
                    button.BackColor = Color.FromArgb(26, 170, 159);
                    button.ForeColor = Color.White;
                };
                button.MouseLeave += (s, e) => {
                    button.BackColor = Color.Transparent;
                    button.ForeColor = Color.White;
                };

                // Click effect
                button.Click += (s, e) => {
                    foreach (Control ctrl in button.Parent.Controls)
                    {
                        if (ctrl is Button btn && btn != close && btn != logout_btn)
                        {
                            btn.BackColor = Color.Transparent;
                        }
                    }
                    button.BackColor = Color.FromArgb(26, 170, 159);
                };
            }
        }

        private void StyleLogoutButton(Button button)
        {
            if (button != null)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = Color.FromArgb(26, 170, 159);
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                button.Cursor = Cursors.Hand;
                button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 15, 15));

                // Hover effects
                button.MouseEnter += (s, e) => {
                    button.BackColor = Color.FromArgb(22, 145, 136);
                };
                button.MouseLeave += (s, e) => {
                    button.BackColor = Color.FromArgb(26, 170, 159);
                };
            }
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirmation Message", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Form1 loginform = new Form1();
                loginform.Show();
                this.Hide();
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Confirmation Message", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            adminDashboard1.Visible = true;
            adminAddProducts1.Visible = false;
            cashierCustomersForm1.Visible = false;
            cashierOrder1.Visible = false;

            AdminDashboard adform = adminDashboard1 as AdminDashboard;
            if (adform != null)
            {
                adform.refreshData();
            }
        }

        private void addProducts_btn_Click(object sender, EventArgs e)
        {
            adminDashboard1.Visible = false;
            adminAddProducts1.Visible = true;
            cashierCustomersForm1.Visible = false;
            cashierOrder1.Visible = false;

            AdminAddProducts aapform = adminAddProducts1 as AdminAddProducts;
            if (aapform != null)
            {
                aapform.refreshData();
            }
        }

        private void customers_btn_Click(object sender, EventArgs e)
        {
            adminDashboard1.Visible = false;
            adminAddProducts1.Visible = false;
            cashierCustomersForm1.Visible = true;
            cashierOrder1.Visible = false;

            CashierCustomersForm ccfform = cashierCustomersForm1 as CashierCustomersForm;
            if (ccfform != null)
            {
                ccfform.refreshData();
            }
        }

        private void order_btn_Click(object sender, EventArgs e)
        {
            adminDashboard1.Visible = false;
            adminAddProducts1.Visible = false;
            cashierCustomersForm1.Visible = false;
            cashierOrder1.Visible = true;

            CashierOrder coform = cashierOrder1 as CashierOrder;
            if (coform != null)
            {
                coform.refreshData();
            }
        }
        private void settings_btn_Click_1(object sender, EventArgs e)
        {
            adminDashboard1.Visible = false;
            adminAddProducts1.Visible = false;
            cashierCustomersForm1.Visible = false;
            cashierOrder1.Visible = false;
            settings1.Visible = true;

            settings1.refreshData();

            Settings sform = settings1 as Settings;
            if (sform != null)
            {
                sform.refreshData();
            }
        }
    }
}