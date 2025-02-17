using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BCrypt.Net;

namespace Invento
{
    public partial class RegisterForm : Form
    {

        private Color primaryColor = Color.LightSeaGreen;  
        private Color backgroundColor = Color.FromArgb(245, 247, 250); 
        private Color textBoxColor = Color.White;
        private Color placeholderColor = Color.FromArgb(160, 160, 160);
        private Color iconColor = Color.FromArgb(100, 88, 255);

        SqlConnection connect = Database.GetConnection();

        public RegisterForm()
        {
            InitializeComponent();

            ApplyModernStyle();
        }

        private void ApplyModernStyle()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = primaryColor;

            if (close != null) 
            {
                StyleCloseButton(close);
            }

            if (this.Controls.Count > 0 && this.Controls[0] is Panel mainPanel)
            {
                mainPanel.BackColor = Color.White;
                mainPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, mainPanel.Width, mainPanel.Height, 20, 20));

                if (pictureBox1 != null)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Size = new Size(80, 80);
                    pictureBox1.Location = new Point((mainPanel.Width - pictureBox1.Width) / 2, 20);
                }

                StyleIcon(pictureBox2, "username_icon.png", register_username);
                StyleIcon(pictureBox3, "password_icon.png", register_password);
                StyleIcon(pictureBox4, "password_icon.png", register_cPassword);

                foreach (Control control in mainPanel.Controls)
                {
                    if (control is Label label)
                    {
                        StyleLabel(label);
                    }
                }

                StyleTextBox(register_username, "Username");
                StyleTextBox(register_password, "Password");
                StyleTextBox(register_cPassword, "Confirm Password");

                StyleButton(register_btn);

                if (register_showPass != null)
                {
                    StyleCheckBox(register_showPass);
                }

                if (login_label != null)
                {
                    StyleClickableLabel(login_label);

                    if (label2 != null)
                    {
                        login_label.Location = new Point(
                            label2.Right + 5,
                            label2.Top
                        );
                    }
                    else
                    {
                        login_label.Location = new Point(
                            (mainPanel.Width - login_label.Width) / 2,
                            register_btn.Bottom + 20
                        );
                    }
                }
            }
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
        }

        private void StyleLabel(Label label)
        {
            label.ForeColor = Color.FromArgb(80, 80, 80);
            label.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label.BackColor = Color.Transparent;
        }

        private void StyleClickableLabel(Label label)
        {
            label.ForeColor = primaryColor;
            label.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label.Cursor = Cursors.Hand;
            label.BackColor = Color.Transparent;

            label.MouseEnter += (s, e) => label.ForeColor = Color.FromArgb(19, 141, 117);
            label.MouseLeave += (s, e) => label.ForeColor = primaryColor;
        }

        private void StyleIcon(PictureBox pictureBox, string iconName, TextBox associatedTextBox)
        {
            if (pictureBox != null && associatedTextBox != null)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Size = new Size(20, 20);
                pictureBox.BackColor = Color.Transparent;
                pictureBox.Location = new Point(
                    associatedTextBox.Left - pictureBox.Width - 10,
                    associatedTextBox.Top + (associatedTextBox.Height - pictureBox.Height) / 2
                );
            }
        }

        private void StyleTextBox(TextBox textBox, string placeholder)
        {
            if (textBox != null)
            {
                Panel container = new Panel
                {
                    Size = new Size(textBox.Width, textBox.Height + 10),
                    Location = new Point(textBox.Location.X, textBox.Location.Y - 5),
                    BackColor = Color.FromArgb(240, 242, 245)
                };

                Panel parentPanel = textBox.Parent as Panel;
                if (parentPanel != null)
                {
                    parentPanel.Controls.Add(container);
                    container.BringToFront();
                    textBox.Parent = container;
                    textBox.Location = new Point(8, 5);
                    textBox.Width = container.Width - 16;
                }

                container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 15, 15));

                container.Paint += (s, e) =>
                {
                    using (var path = new GraphicsPath())
                    {
                        path.AddRectangle(new Rectangle(0, 0, container.Width, container.Height));
                        using (var brush = new PathGradientBrush(path))
                        {
                            brush.CenterColor = Color.FromArgb(40, 0, 0, 0);
                            brush.SurroundColors = new Color[] { Color.Transparent };
                            e.Graphics.FillPath(brush, path);
                        }
                    }
                };

                textBox.BackColor = Color.FromArgb(240, 242, 245);
                textBox.ForeColor = Color.FromArgb(80, 80, 80);
                textBox.BorderStyle = BorderStyle.None;
                textBox.Font = new Font("Segoe UI", 11);

                textBox.GotFocus += (s, e) =>
                {
                    container.BackColor = Color.FromArgb(235, 237, 240);
                    textBox.BackColor = Color.FromArgb(235, 237, 240);
                };

                textBox.LostFocus += (s, e) =>
                {
                    container.BackColor = Color.FromArgb(240, 242, 245);
                    textBox.BackColor = Color.FromArgb(240, 242, 245);
                };
            }
        }

        private void StyleCloseButton(Button closeButton)
        {
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.BackColor = Color.FromArgb(240, 242, 245);  
            closeButton.ForeColor = Color.FromArgb(100, 100, 100);  
            closeButton.Text = "×"; 
            closeButton.Font = new Font("Arial", 14, FontStyle.Bold);
            closeButton.Size = new Size(30, 30);
            closeButton.Cursor = Cursors.Hand;

            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 15, 15));

            closeButton.MouseEnter += (s, e) => {
                closeButton.BackColor = Color.FromArgb(220, 53, 69); 
                closeButton.ForeColor = Color.White;
            };
            closeButton.MouseLeave += (s, e) => {
                closeButton.BackColor = Color.FromArgb(240, 242, 245);
                closeButton.ForeColor = Color.FromArgb(100, 100, 100);
            };
        }

        private void StyleButton(Button button)
        {
            if (button != null)
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
        }

        private void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.FlatStyle = FlatStyle.Standard;
            checkBox.ForeColor = Color.FromArgb(80, 80, 80);
            checkBox.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            checkBox.BackColor = Color.Transparent;
            checkBox.Cursor = Cursors.Hand;

            checkBox.Appearance = Appearance.Normal;
            checkBox.UseVisualStyleBackColor = true;

            checkBox.MouseEnter += (s, e) => checkBox.ForeColor = Color.FromArgb(19, 141, 117);
            checkBox.MouseLeave += (s, e) => checkBox.ForeColor = Color.FromArgb(80, 80, 80);
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
          int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_label_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();

            this.Hide();
        }

        private bool IsFirstUser(SqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM users";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                int userCount = (int)cmd.ExecuteScalar();
                return userCount == 0;
            }
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(register_username.Text) ||
                string.IsNullOrWhiteSpace(register_password.Text) ||
                string.IsNullOrWhiteSpace(register_cPassword.Text))
            {
                MessageBox.Show("Please fill all empty fields",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (register_password.Text.Length < 8)
            {
                MessageBox.Show("Invalid Password, at least 8 characters",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (register_password.Text.Trim() != register_cPassword.Text.Trim())
            {
                MessageBox.Show("Password does not match",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkConnection())
            {
                try
                {
                    connect.Open();
                    string checkUsername = "SELECT * FROM users WHERE username = @usern";

                    using (SqlCommand cmd = new SqlCommand(checkUsername, connect))
                    {
                        cmd.Parameters.AddWithValue("@usern", register_username.Text.Trim());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show($"{register_username.Text.Trim()} is already taken",
                                "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            connect.Close();
                            return;  
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error checking username. Please contact Admin",
                        "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    connect.Close();  
                }

                using (var captchaForm = new Captcha())
                {
                    if (captchaForm.ShowDialog() != DialogResult.OK)
                    {
                        return; 
                    }
                }

                try
                {
                    connect.Open();
                    bool isFirstUser = IsFirstUser(connect);

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                        register_password.Text.Trim(),
                        BCrypt.Net.BCrypt.GenerateSalt(12)
                    );

                    string insertData = @"INSERT INTO users (username, password, role, status, date) 
                               VALUES (@usern, @pass, @role, @status, @date)";

                    using (SqlCommand insertD = new SqlCommand(insertData, connect))
                    {
                        insertD.Parameters.AddWithValue("@usern", register_username.Text.Trim());
                        insertD.Parameters.AddWithValue("@pass", hashedPassword);
                        insertD.Parameters.AddWithValue("@role", isFirstUser ? "Admin" : "Cashier");
                        insertD.Parameters.AddWithValue("@status", isFirstUser ? "Active" : "Approval");
                        insertD.Parameters.AddWithValue("@date", DateTime.Today);

                        insertD.ExecuteNonQuery();

                        MessageBox.Show(isFirstUser ?
                            "Registered Successfully as Admin!" :
                            "Registered Successfully!",
                            "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Form1 loginform = new Form1();
                        loginform.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Registration failed: " + ex.Message,
                        "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public bool checkConnection()
        {
            if (connect.State == ConnectionState.Closed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void register_showPass_CheckedChanged(object sender, EventArgs e)
        {
            register_password.PasswordChar = register_showPass.Checked ? '\0' : '*';
            register_cPassword.PasswordChar = register_showPass.Checked ? '\0' : '*';
        }
    }
}
