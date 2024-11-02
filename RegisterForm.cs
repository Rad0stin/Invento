using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Invento
{
    public partial class RegisterForm : Form
    {
        SqlConnection connect =  new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30");
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
            if (register_username.Text == "" || register_password.Text == "" || register_cPassword.Text == "")
            {
                MessageBox.Show("Please fill all empty fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else 
            {
                if (checkConnection())
                {
                    try
                    {
                        connect.Open();

                        string insertData = "Insert into users (username, password, role, status, date)  + " +
                            "VALUEs(@usern, @pass, @role, @status, @date)";

                        using (SqlCommand cmd = new SqlCommand(insertData, connect)) 
                        {
                            cmd.Parameters.AddWithValue("@usern", register_username.Text.Trim());
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    finally 
                    {

                    }
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
    }
}
