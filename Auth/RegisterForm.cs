using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace Invento
{
    public partial class RegisterForm : Form
    {

        private Color primaryColor = Color.LightSeaGreen;  // Bright purple
        private Color backgroundColor = Color.FromArgb(245, 247, 250);  // Slightly darker background
        private Color textBoxColor = Color.White;
        private Color placeholderColor = Color.FromArgb(160, 160, 160);
        private Color iconColor = Color.FromArgb(100, 88, 255);

        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        public RegisterForm()
        {
            InitializeComponent();

            ApplyModernStyle();
        }

        private void ApplyModernStyle()
        {
            // Form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = primaryColor;

            // Style close button
            if (close != null) 
            {
                StyleCloseButton(close);
            }

            if (this.Controls.Count > 0 && this.Controls[0] is Panel mainPanel)
            {
                mainPanel.BackColor = Color.White;
                mainPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, mainPanel.Width, mainPanel.Height, 20, 20));

                // Style the avatar
                if (pictureBox1 != null)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Size = new Size(80, 80);
                    pictureBox1.Location = new Point((mainPanel.Width - pictureBox1.Width) / 2, 20);
                }

                // Style input icons
                StyleIcon(pictureBox2, "username_icon.png", register_username);
                StyleIcon(pictureBox3, "password_icon.png", register_password);
                StyleIcon(pictureBox4, "password_icon.png", register_cPassword);

                // Style Labels
                foreach (Control control in mainPanel.Controls)
                {
                    if (control is Label label)
                    {
                        StyleLabel(label);
                    }
                }

                // Style TextBoxes
                StyleTextBox(register_username, "Username");
                StyleTextBox(register_password, "Password");
                StyleTextBox(register_cPassword, "Confirm Password");

                // Style the Register Button
                StyleButton(register_btn);

                // Style the Show Password CheckBox
                if (register_showPass != null)
                {
                    StyleCheckBox(register_showPass);
                }

                // Style Login Label and adjust its position
                if (login_label != null)
                {
                    StyleClickableLabel(login_label);
                    login_label.Location = new Point(
                        (mainPanel.Width - login_label.Width) / 2,
                        register_btn.Bottom + 20
                    );
                }
            }

            // Add form shadow and rounded corners
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

            // Add hover effect
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
            closeButton.BackColor = Color.FromArgb(240, 242, 245);  // Light background
            closeButton.ForeColor = Color.FromArgb(100, 100, 100);  // Gray text
            closeButton.Text = "×";  // Use multiplication sign as close symbol
            closeButton.Font = new Font("Arial", 14, FontStyle.Bold);
            closeButton.Size = new Size(30, 30);
            closeButton.Cursor = Cursors.Hand;

            // Round corners
            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 15, 15));

            // Hover effects
            closeButton.MouseEnter += (s, e) => {
                closeButton.BackColor = Color.FromArgb(220, 53, 69);  // Red on hover
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
            // Simple styling without custom painting
            checkBox.FlatStyle = FlatStyle.Standard;
            checkBox.ForeColor = Color.FromArgb(80, 80, 80);
            checkBox.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            checkBox.BackColor = Color.Transparent;
            checkBox.Cursor = Cursors.Hand;

            // Use default Windows styling
            checkBox.Appearance = Appearance.Normal;
            checkBox.UseVisualStyleBackColor = true;

            // Add hover effect
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
                        }
                        else if (register_password.Text.Length < 8)
                        {
                            MessageBox.Show("Invalid Password, at least 8 characters",
                                "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (register_password.Text.Trim() != register_cPassword.Text.Trim())
                        {
                            MessageBox.Show("Password does not match",
                                "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            try
                            {
                                // Generate a salt and hash the password
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
                                    insertD.Parameters.AddWithValue("@role", "Cashier");
                                    insertD.Parameters.AddWithValue("@status", "Approval");
                                    insertD.Parameters.AddWithValue("@date", DateTime.Today);

                                    insertD.ExecuteNonQuery();

                                    MessageBox.Show("Registered Successfully!",
                                        "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    Form1 loginform = new Form1();
                                    loginform.Show();
                                    this.Hide();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error hashing password: " + ex.Message,
                                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex.Message,
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
