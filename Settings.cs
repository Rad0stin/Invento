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
        private MainForm mainForm;
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

        public Settings()
        {
            InitializeComponent();
            this.BackColor = BackgroundColor;
            SetupCustomControls();
        }

        public void SetMainForm(MainForm form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form), "MainForm reference cannot be null");
            }
            mainForm = form;
            LoadUserData();
        }

        private void SetupCustomControls()
        {
            // Profile Picture Section
            Label lblProfilePicture = new Label();
            lblProfilePicture.Text = "Profile";
            lblProfilePicture.Location = new Point(20, 20);
            lblProfilePicture.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblProfilePicture.ForeColor = Color.FromArgb(64, 64, 64);

            currentPicture = new PictureBox();
            currentPicture.Size = new Size(100, 100);
            currentPicture.SizeMode = PictureBoxSizeMode.Zoom;  // Changed to Zoom for better aspect ratio
            currentPicture.Location = new Point(20, 50);
            currentPicture.BorderStyle = BorderStyle.None;
            currentPicture.BackColor = Color.White;

            Button btnChangePicture = new Button();
            btnChangePicture.Text = "Change Picture";
            btnChangePicture.Location = new Point(20, 160);
            btnChangePicture.Size = new Size(120, 35);
            btnChangePicture.BackColor = Color.FromArgb(26, 170, 159);
            btnChangePicture.ForeColor = Color.White;
            btnChangePicture.FlatStyle = FlatStyle.Flat;
            btnChangePicture.FlatAppearance.BorderSize = 0;
            btnChangePicture.Font = new Font("Segoe UI", 10);
            btnChangePicture.Click += BtnChangePicture_Click;

            // Username Section
            Label lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Location = new Point(20, 170);
            lblUsername.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            txtUsername = new TextBox();
            txtUsername.Location = new Point(20, 200);
            txtUsername.Size = new Size(200, 30);
            txtUsername.Font = new Font("Segoe UI", 11);

            Button btnUpdateUsername = new Button();
            btnUpdateUsername.Text = "Update Username";
            btnUpdateUsername.Location = new Point(20, 240);
            btnUpdateUsername.Size = new Size(120, 30);
            btnUpdateUsername.Font = new Font("Segoe UI", 10);
            btnUpdateUsername.Click += BtnUpdateUsername_Click;

            // Password Section
            Label lblCurrentPassword = new Label();
            lblCurrentPassword.Text = "Current Password";
            lblCurrentPassword.Location = new Point(20, 290);
            lblCurrentPassword.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            txtCurrentPassword = new TextBox();
            txtCurrentPassword.Location = new Point(20, 320);
            txtCurrentPassword.Size = new Size(200, 30);
            txtCurrentPassword.UseSystemPasswordChar = true;
            txtCurrentPassword.Font = new Font("Segoe UI", 11);

            Label lblNewPassword = new Label();
            lblNewPassword.Text = "New Password";
            lblNewPassword.Location = new Point(20, 360);
            lblNewPassword.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            txtNewPassword = new TextBox();
            txtNewPassword.Location = new Point(20, 390);
            txtNewPassword.Size = new Size(200, 30);
            txtNewPassword.UseSystemPasswordChar = true;
            txtNewPassword.Font = new Font("Segoe UI", 11);

            Button btnChangePassword = new Button();
            btnChangePassword.Text = "Change Password";
            btnChangePassword.Location = new Point(20, 430);
            btnChangePassword.Size = new Size(120, 30);
            btnChangePassword.Font = new Font("Segoe UI", 10);
            btnChangePassword.Click += BtnChangePassword_Click;

            Controls.AddRange(new Control[] {
                lblProfilePicture, currentPicture, btnChangePicture,
                lblUsername, txtUsername, btnUpdateUsername,
                lblCurrentPassword, txtCurrentPassword,
                lblNewPassword, txtNewPassword, btnChangePassword
            });

            // Apply modern styling to buttons
            foreach (Control control in Controls)
            {
                if (control is Button button)
                {
                    StyleButton(button);
                }
            }
        }

        private void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(26, 170, 159);
            button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand;

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(22, 145, 136);
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(26, 170, 159);
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

                                    // Update MainForm profile picture
                                    if (mainForm?.pictureBoxProfile != null)
                                    {
                                        if (mainForm.pictureBoxProfile.Image != null)
                                        {
                                            mainForm.pictureBoxProfile.Image.Dispose();
                                        }
                                        mainForm.pictureBoxProfile.Image = new Bitmap(image);
                                        mainForm.pictureBoxProfile.SizeMode = PictureBoxSizeMode.Zoom;
                                        mainForm.pictureBoxProfile.BackColor = Color.Transparent;
                                    }
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

                        // Save image to database
                        SaveProfilePicture(imageData);

                        // Update both PictureBoxes with the new image
                        using (var ms = new MemoryStream(imageData))
                        {
                            Image image = Image.FromStream(ms);

                            // Update Settings control picture
                            if (currentPicture.Image != null)
                            {
                                currentPicture.Image.Dispose();
                            }
                            currentPicture.Image = new Bitmap(image);

                            // Update MainForm picture - ensure mainForm reference exists
                            if (mainForm == null)
                            {
                                MessageBox.Show("Warning: MainForm reference not set. Profile picture may not update in main window.",
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            try
                            {
                                // Update the main form's picture box
                                if (mainForm.pictureBoxProfile != null)
                                {
                                    // Use Invoke if needed to ensure thread safety
                                    if (mainForm.InvokeRequired)
                                    {
                                        mainForm.Invoke(new Action(() => UpdateMainFormPicture(image)));
                                    }
                                    else
                                    {
                                        UpdateMainFormPicture(image);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error updating main form picture: {ex.Message}",
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

        private void UpdateMainFormPicture(Image image)
        {
            if (mainForm.pictureBoxProfile.Image != null)
            {
                mainForm.pictureBoxProfile.Image.Dispose();
            }
            mainForm.pictureBoxProfile.Image = new Bitmap(image);
            mainForm.pictureBoxProfile.SizeMode = PictureBoxSizeMode.Zoom;
            mainForm.pictureBoxProfile.BackColor = Color.Transparent;
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

                        if (mainForm?.user_username != null)
                        {
                            mainForm.user_username.Text = txtUsername.Text.Trim();
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