using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using BCrypt.Net;
using System.Drawing.Drawing2D;

namespace Invento
{
    public partial class Form1 : Form
    {
        public static string username;

        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        public Form1()
        {
            InitializeComponent();

            ApplyModernStyle();
        }

        private Color primaryColor = Color.LightSeaGreen;
        private Color backgroundColor = Color.FromArgb(245, 247, 250);
        private Color textBoxColor = Color.White;
        private Color placeholderColor = Color.FromArgb(160, 160, 160);
        private Color iconColor = Color.FromArgb(100, 88, 255);

        private void ApplyModernStyle()
        {
            // Form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = primaryColor;  

            if (this.Controls.Count > 0 && this.Controls[0] is Panel mainPanel)
            {
                mainPanel.BackColor = Color.White;
                mainPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, mainPanel.Width, mainPanel.Height, 20, 20));

                // Style Labels
                foreach (Control control in mainPanel.Controls)
                {
                    if (control is Label label)
                    {
                        if (label == register_label)
                        {
                            StyleClickableLabel(label);
                        }
                        else if (label == label2)  // "Don't have account?" label
                        {
                            // Apply simple black styling without hover effects
                            label.ForeColor = Color.Black;
                            label.Font = new Font("Segoe UI", 11, FontStyle.Regular);
                            label.BackColor = Color.Transparent;
                            label.Cursor = Cursors.Default;  // Regular cursor instead of hand
                        }
                        else
                        {
                            StyleLabel(label);
                        }
                    }
                }

                // Style TextBoxes
                StyleTextBox(login_username, "Username");
                StyleTextBox(login_password, "Password");

                // Style the Login Button
                StyleLoginButton(login_btn);

                // Style the Show Password CheckBox
                StyleCheckBox(login_showPass);

                // Style Close Button
                StyleCloseButton(close);
            }

            // Add form shadow and rounded corners
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
        }

        private void StyleLabel(Label label)
        {
            label.ForeColor = Color.Black;
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

                // Add shadow effect
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

                // Focus effects
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

        private void StyleLoginButton(Button button)
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

                // Hover effects
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

            // Use default Windows styling
            checkBox.Appearance = Appearance.Normal;
            checkBox.UseVisualStyleBackColor = true;

            // Add hover effect
            checkBox.MouseEnter += (s, e) => checkBox.ForeColor = primaryColor;
            checkBox.MouseLeave += (s, e) => checkBox.ForeColor = Color.FromArgb(80, 80, 80);
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

            // Round corners
            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 15, 15));

            // Hover effects
            closeButton.MouseEnter += (s, e) => {
                closeButton.BackColor = Color.FromArgb(220, 53, 69);
                closeButton.ForeColor = Color.White;
            };
            closeButton.MouseLeave += (s, e) => {
                closeButton.BackColor = Color.FromArgb(240, 242, 245);
                closeButton.ForeColor = Color.FromArgb(100, 100, 100);
            };
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    

    private void close_Click(object sender, EventArgs e)
        {
          Application.Exit();
        }

        private void register_label_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm();
            regForm.Show();

            this.Hide();
        }

        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            login_password.PasswordChar = login_showPass.Checked ? '\0' : '*';
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
        private void login_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(login_username.Text) || string.IsNullOrWhiteSpace(login_password.Text))
            {
                MessageBox.Show("Please enter both username and password",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkConnection())
            {
                try
                {
                    connect.Open();

                    // First get the stored password and status for the user
                    string selectData = "SELECT password, status, role FROM users WHERE username = @usern";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@usern", login_username.Text.Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["password"].ToString();
                                string status = reader["status"].ToString();
                                string role = reader["role"].ToString();

                                // Check if status is Active
                                if (status != "Active")
                                {
                                    MessageBox.Show("Your account is not active. Please contact administrator.",
                                        "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                // Check if the password is already hashed (starts with $2a$ or $2b$)
                                bool passwordMatches;
                                if (storedPassword.StartsWith("$2a$") || storedPassword.StartsWith("$2b$"))
                                {
                                    try
                                    {
                                        passwordMatches = BCrypt.Net.BCrypt.Verify(login_password.Text.Trim(), storedPassword);
                                    }
                                    catch (Exception)
                                    {
                                        // If verification fails, try direct comparison (for legacy passwords)
                                        passwordMatches = (login_password.Text.Trim() == storedPassword);
                                    }
                                }
                                else
                                {
                                    // For non-hashed passwords, do direct comparison
                                    passwordMatches = (login_password.Text.Trim() == storedPassword);

                                    // Optionally, update to hashed password
                                    if (passwordMatches)
                                    {
                                        UpdateToHashedPassword(login_username.Text.Trim(), login_password.Text.Trim());
                                    }
                                }

                                if (passwordMatches)
                                {
                                    username = login_username.Text.Trim();
                                    MessageBox.Show("Login Successfully!",
                                        "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    if (role == "Admin")
                                    {
                                        MainForm mForm = new MainForm();
                                        mForm.Show();
                                        this.Hide();
                                    }
                                    else if (role == "Cashier")
                                    {
                                        CashierMainForm cmForm = new CashierMainForm();
                                        cmForm.Show();
                                        this.Hide();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect username or password!",
                                        "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("User not found!",
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

        private void UpdateToHashedPassword(string username, string plainPassword)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, BCrypt.Net.BCrypt.GenerateSalt(12));

                string updateQuery = "UPDATE users SET password = @pass WHERE username = @usern";
                using (SqlCommand cmd = new SqlCommand(updateQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@pass", hashedPassword);
                    cmd.Parameters.AddWithValue("@usern", username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // Silently fail - we don't want to disturb the user login process
            }
            }
        }
    }
