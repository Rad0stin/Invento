using System;
using System.Drawing;
using System.Windows.Forms;

namespace Invento
{
    public partial class Captcha : Form
    {
        private string captchaText;
        private Color primaryColor = Color.LightSeaGreen;

        public bool IsCaptchaValid { get; private set; }

        public Captcha()
        {
            InitializeComponent();
            ApplyModernStyle();
            GenerateCaptcha();
        }

        private void ApplyModernStyle()
        {
            this.BackColor = primaryColor;

            panel1.BackColor = Color.White;
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 20, 20));

            StyleTextBox(txtCaptcha, "Enter Captcha");
            StyleButton(btnVerify);
            StyleRefreshButton(btnRefresh);

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
        }

        private void StyleTextBox(TextBox textBox, string placeholder)
        {
            Panel container = new Panel
            {
                Size = new Size(textBox.Width, textBox.Height + 10),
                Location = new Point(textBox.Location.X, textBox.Location.Y - 5),
                BackColor = Color.FromArgb(240, 242, 245)
            };

            container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 15, 15));

            textBox.Parent = container;
            textBox.Location = new Point(8, 5);
            textBox.Width = container.Width - 16;
            textBox.BackColor = Color.FromArgb(240, 242, 245);
            textBox.ForeColor = Color.FromArgb(80, 80, 80);
            textBox.BorderStyle = BorderStyle.None;
            textBox.Font = new Font("Segoe UI", 11);

            panel1.Controls.Add(container);
            container.BringToFront();
        }

        private void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = primaryColor;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 0;
            button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 15, 15));

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(19, 141, 117);
            button.MouseLeave += (s, e) => button.BackColor = primaryColor;
        }

        private void StyleRefreshButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.FromArgb(240, 242, 245);
            button.ForeColor = Color.FromArgb(80, 80, 80);
            button.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 0;
            button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 15, 15));
        }

        private void GenerateCaptcha()
        {
            Random random = new Random();
            captchaText = "";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            for (int i = 0; i < 6; i++)
            {
                captchaText += chars[random.Next(chars.Length)];
            }

            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            for (int i = 0; i < 50; i++)
            {
                graphics.DrawLine(
                    new Pen(Color.LightGray),
                    random.Next(bitmap.Width),
                    random.Next(bitmap.Height),
                    random.Next(bitmap.Width),
                    random.Next(bitmap.Height)
                );
            }

            Font captchaFont = new Font("Arial", 24, FontStyle.Bold | FontStyle.Italic);
            SizeF textSize = graphics.MeasureString(captchaText, captchaFont);

            float x = (bitmap.Width - textSize.Width) / 2;
            float y = Math.Max(10, (bitmap.Height - textSize.Height) / 2);

            x += random.Next(-10, 10);
            y += random.Next(-5, 5);

            graphics.DrawString(
                captchaText,
                captchaFont,
                new SolidBrush(Color.FromArgb(80, 80, 80)),
                new PointF(x, y)
            );

            pictureBox1.Image = bitmap;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (txtCaptcha.Text.Trim().Equals(captchaText, StringComparison.OrdinalIgnoreCase))
            {
                IsCaptchaValid = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Captcha! Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GenerateCaptcha();
                txtCaptcha.Clear();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
            txtCaptcha.Clear();
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }
}