using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using BCrypt.Net;

namespace Invento
{
    public partial class RegisterForm : Form
    {
        SqlConnection connect =  new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        public RegisterForm()
        {
            InitializeComponent();
        }

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
