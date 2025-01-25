using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Invento
{

    public partial class Settings : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        private Form parentForm;
        private PictureBox currentPicture;
        private TextBox txtUsername;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private Button btnChangePicture;
        private Button btnUpdateUsername;
        private Button btnChangePassword;

        private static readonly Color PrimaryColor = Color.LightSeaGreen;
        private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
        private static readonly Color TextBoxColor = Color.FromArgb(240, 242, 245);
        private static readonly Color TextColor = Color.Black;

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
    int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public Settings()
        {
            InitializeComponent();
            this.BackColor = BackgroundColor;
            SetupCustomControls();
        }

        public void SetParentForm(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form), "Parent form reference cannot be null");
            }
            parentForm = form;
            LoadUserData();
        }

        private void SetupCustomControls()
        {
            // Create a main container panel with padding and scroll
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(30)
            };
            Controls.Add(mainPanel);

            // Profile Picture Section (Moved further to the right)
            int profilePictureX = 50; // Adjust this value to move the profile picture further to the right

            Label lblProfilePicture = new Label
            {
                Text = "Profile Picture",
                Location = new Point(profilePictureX, 0),
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.Black,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            currentPicture = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(profilePictureX, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            // Create rounded container for profile picture
            Panel pictureContainer = new Panel
            {
                Size = new Size(currentPicture.Width, currentPicture.Height),
                Location = currentPicture.Location,
                BackColor = Color.White
            };
            pictureContainer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pictureContainer.Width, pictureContainer.Height, 15, 15));
            mainPanel.Controls.Add(pictureContainer);
            currentPicture.Parent = pictureContainer;
            currentPicture.Location = new Point(0, 0);

            btnChangePicture = new Button
            {
                Text = "Change Picture",
                Location = new Point(profilePictureX, 210),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightSeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnChangePicture.FlatAppearance.BorderSize = 0;
            btnChangePicture.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnChangePicture.Width, btnChangePicture.Height, 15, 15));
            btnChangePicture.Click += BtnChangePicture_Click;

            // Username Section (Middle)
            int middleX = profilePictureX + 200; // Adjust this value to control the spacing between sections

            Label lblUsername = new Label
            {
                Text = "Username",
                Location = new Point(middleX, 0),
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.Black,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            // Username TextBox with container
            Panel usernameContainer = new Panel
            {
                Size = new Size(300, 45),
                Location = new Point(middleX, 40),
                BackColor = Color.FromArgb(240, 242, 245)
            };
            usernameContainer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, usernameContainer.Width, usernameContainer.Height, 15, 15));

            txtUsername = new TextBox
            {
                Location = new Point(8, 12),
                Size = new Size(284, 30),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(240, 242, 245)
            };
            usernameContainer.Controls.Add(txtUsername);

            btnUpdateUsername = new Button
            {
                Text = "Update Username",
                Location = new Point(middleX, 100),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightSeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnUpdateUsername.FlatAppearance.BorderSize = 0;
            btnUpdateUsername.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnUpdateUsername.Width, btnUpdateUsername.Height, 15, 15));
            btnUpdateUsername.Click += BtnUpdateUsername_Click;

            // Password Section (Right Side)
            int rightSideX = middleX + 350; // Adjust this value to control the spacing between sections

            Label lblCurrentPassword = new Label
            {
                Text = "Current Password",
                Location = new Point(rightSideX, 0),
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.Black,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            // Current Password TextBox with container
            Panel currentPasswordContainer = new Panel
            {
                Size = new Size(300, 45),
                Location = new Point(rightSideX, 40),
                BackColor = Color.FromArgb(240, 242, 245)
            };
            currentPasswordContainer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, currentPasswordContainer.Width, currentPasswordContainer.Height, 15, 15));

            txtCurrentPassword = new TextBox
            {
                Location = new Point(8, 12),
                Size = new Size(284, 30),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(240, 242, 245),
                UseSystemPasswordChar = true
            };
            currentPasswordContainer.Controls.Add(txtCurrentPassword);

            Label lblNewPassword = new Label
            {
                Text = "New Password",
                Location = new Point(rightSideX, 100),
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.Black,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            // New Password TextBox with container
            Panel newPasswordContainer = new Panel
            {
                Size = new Size(300, 45),
                Location = new Point(rightSideX, 140),
                BackColor = Color.FromArgb(240, 242, 245)
            };
            newPasswordContainer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, newPasswordContainer.Width, newPasswordContainer.Height, 15, 15));

            txtNewPassword = new TextBox
            {
                Location = new Point(8, 12),
                Size = new Size(284, 30),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(240, 242, 245),
                UseSystemPasswordChar = true
            };
            newPasswordContainer.Controls.Add(txtNewPassword);

            btnChangePassword = new Button
            {
                Text = "Change Password",
                Location = new Point(rightSideX, 200),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightSeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnChangePassword.Width, btnChangePassword.Height, 15, 15));
            btnChangePassword.Click += BtnChangePassword_Click;

            // Add hover effects for buttons
            AddButtonHoverEffects(btnChangePicture);
            AddButtonHoverEffects(btnUpdateUsername);
            AddButtonHoverEffects(btnChangePassword);

            // Add focus effects for textboxes
            AddTextBoxFocusEffects(txtUsername, usernameContainer);
            AddTextBoxFocusEffects(txtCurrentPassword, currentPasswordContainer);
            AddTextBoxFocusEffects(txtNewPassword, newPasswordContainer);

            // Add all controls to the main panel
            mainPanel.Controls.AddRange(new Control[] {
        lblProfilePicture,
        pictureContainer,
        btnChangePicture,
        lblUsername,
        usernameContainer,
        btnUpdateUsername,
        lblCurrentPassword,
        currentPasswordContainer,
        lblNewPassword,
        newPasswordContainer,
        btnChangePassword
    });
        }

        private void AddButtonHoverEffects(Button button)
        {
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(0, 150, 150);
            button.MouseLeave += (s, e) => button.BackColor = Color.LightSeaGreen;
        }

        private void AddTextBoxFocusEffects(TextBox textBox, Panel container)
        {
            textBox.GotFocus += (s, e) => {
                container.BackColor = Color.FromArgb(235, 237, 240);
                textBox.BackColor = Color.FromArgb(235, 237, 240);
            };

            textBox.LostFocus += (s, e) => {
                container.BackColor = Color.FromArgb(240, 242, 245);
                textBox.BackColor = Color.FromArgb(240, 242, 245);
            };
        }

        public void refreshData()
        {
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                connect.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT username, profile_picture FROM users WHERE username = @username", connect))
                {
                    cmd.Parameters.AddWithValue("@username", Form1.username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtUsername.Text = reader["username"].ToString();

                            if (!reader.IsDBNull(reader.GetOrdinal("profile_picture")))
                            {
                                byte[] imageData = (byte[])reader["profile_picture"];
                                using (var ms = new MemoryStream(imageData))
                                {
                                    Image image = Image.FromStream(ms);

                                    // Update Settings profile picture
                                    if (currentPicture.Image != null)
                                    {
                                        currentPicture.Image.Dispose();
                                    }
                                    currentPicture.Image = new Bitmap(image);

                                    // Update parent form profile picture based on form type
                                    UpdateParentFormPicture(new Bitmap(image));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user data: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void BtnChangePicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Select Profile Picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] imageData;
                        using (var stream = new MemoryStream())
                        {
                            using (var image = Image.FromFile(openFileDialog.FileName))
                            {
                                image.Save(stream, image.RawFormat);
                                imageData = stream.ToArray();
                            }
                        }

                        SaveProfilePicture(imageData);

                        using (var ms = new MemoryStream(imageData))
                        {
                            Image image = Image.FromStream(ms);

                            if (currentPicture.Image != null)
                            {
                                currentPicture.Image.Dispose();
                            }
                            currentPicture.Image = new Bitmap(image);

                            if (parentForm == null)
                            {
                                MessageBox.Show("Warning: Parent form reference not set. Profile picture may not update in main window.",
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            try
                            {
                                if (parentForm.InvokeRequired)
                                {
                                    parentForm.Invoke(new Action(() => UpdateParentFormPicture(image)));
                                }
                                else
                                {
                                    UpdateParentFormPicture(image);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error updating parent form picture: {ex.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateParentFormPicture(Image image)
        {
            if (parentForm == null) return;

            PictureBox profilePicture = null;

            // Determine which form we're dealing with and get the appropriate PictureBox
            if (parentForm is MainForm mainForm)
            {
                profilePicture = mainForm.pictureBoxProfile;
            }
            else if (parentForm is CashierMainForm cashierForm)
            {
                profilePicture = cashierForm.pictureBoxProfile;
            }

            if (profilePicture != null)
            {
                if (profilePicture.Image != null)
                {
                    profilePicture.Image.Dispose();
                }
                profilePicture.Image = new Bitmap(image);
                profilePicture.SizeMode = PictureBoxSizeMode.Zoom;
                profilePicture.BackColor = Color.Transparent;
            }
        }

        private void SaveProfilePicture(byte[] imageData)
        {
            try
            {
                connect.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE users SET profile_picture = @picture WHERE username = @username", connect))
                {
                    cmd.Parameters.Add("@picture", SqlDbType.VarBinary, -1).Value = imageData;
                    cmd.Parameters.AddWithValue("@username", Form1.username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving profile picture: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void BtnUpdateUsername_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE users SET username = @username WHERE username = @currentUsername", connect))
                {
                    connect.Open();
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@currentUsername", Form1.username);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Username updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Update username label in parent form
                        if (parentForm is MainForm mainForm && mainForm.user_username != null)
                        {
                            mainForm.user_username.Text = txtUsername.Text.Trim();
                        }
                        else if (parentForm is CashierMainForm cashierForm && cashierForm.user_username != null)
                        {
                            cashierForm.user_username.Text = txtUsername.Text.Trim();
                        }

                        Form1.username = txtUsername.Text.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating username: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please fill in all password fields",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connect.Open();
                using (SqlCommand verifyCmd = new SqlCommand("SELECT password FROM users WHERE username = @username", connect))
                {
                    verifyCmd.Parameters.AddWithValue("@username", Form1.username);
                    string storedPassword = verifyCmd.ExecuteScalar()?.ToString();

                    bool passwordMatches;
                    if (storedPassword.StartsWith("$2a$") || storedPassword.StartsWith("$2b$"))
                    {
                        passwordMatches = BCrypt.Net.BCrypt.Verify(txtCurrentPassword.Text.Trim(), storedPassword);
                    }
                    else
                    {
                        passwordMatches = (txtCurrentPassword.Text.Trim() == storedPassword);
                    }

                    if (!passwordMatches)
                    {
                        MessageBox.Show("Current password is incorrect",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(txtNewPassword.Text.Trim(),
                    BCrypt.Net.BCrypt.GenerateSalt(12));

                using (SqlCommand updateCmd = new SqlCommand("UPDATE users SET password = @password WHERE username = @username", connect))
                {
                    updateCmd.Parameters.AddWithValue("@password", hashedNewPassword);
                    updateCmd.Parameters.AddWithValue("@username", Form1.username);

                    int result = updateCmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Password updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtCurrentPassword.Clear();
                        txtNewPassword.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating password: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }
    }
}