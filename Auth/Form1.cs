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

namespace Invento
{
    public partial class Form1 : Form
    {
        public static string username;

        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        public Form1()
        {
            InitializeComponent();
        }

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
