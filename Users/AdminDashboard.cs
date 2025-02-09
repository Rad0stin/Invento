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
using System.Runtime.Remoting.Contexts;
using System.Drawing.Drawing2D;

namespace Invento
{
    public partial class AdminDashboard : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=35.233.55.91;Initial Catalog=invento;User ID=sqlserver;Password=Rado1234@;");
        public AdminDashboard()
        {
            InitializeComponent();

            displayTodaysCustomers();

            displayAllUsers();

            displayAllCustomers();

            displayTodaysIncome();

            displayTotalIncome();

            ApplyProfessionalGridStyle();
        }

        private void ApplyProfessionalGridStyle()
        {
            // Professional color scheme
            Color primaryColor = Color.LightSeaGreen;
            Color headerColor = Color.LightSeaGreen;  // Changed to LightSeaGreen as requested
            Color alternateRowColor = Color.FromArgb(240, 248, 248);  // Very light teal tint
            Color borderColor = Color.FromArgb(200, 223, 223);  // Subtle grid lines

            // Main DataGridView properties
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.GridColor = borderColor;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Header styling
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10.5F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dataGridView1.ColumnHeadersHeight = 45;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Cell styling
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dataGridView1.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;  // Remove selection color effect
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromArgb(64, 64, 64);  // Keep text color consistent

            // Alternating row style
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = alternateRowColor;
            dataGridView1.AlternatingRowsDefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionBackColor = alternateRowColor;  // Keep consistent with row color
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(64, 64, 64);  // Keep text color consistent

            // Row height
            dataGridView1.RowTemplate.Height = 40;

            // Add hover effect only
            dataGridView1.CellMouseEnter += new DataGridViewCellEventHandler(DataGridView1_CellMouseEnter);
            dataGridView1.CellMouseLeave += new DataGridViewCellEventHandler(DataGridView1_CellMouseLeave);

            // Optional: Add smooth scrolling
            dataGridView1.ScrollBars = ScrollBars.Both;

            // Adjust column headers if needed
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        // Hover effect handlers
        private void DataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                row.DefaultCellStyle.BackColor = Color.FromArgb(230, 244, 244);  // Light hover effect
            }
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                row.DefaultCellStyle.BackColor = e.RowIndex % 2 == 0
                    ? Color.White
                    : Color.FromArgb(240, 248, 248);  // Reset to original color
            }
        }

        // Add this to your form to create a custom border effect
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Create subtle shadow effect around the DataGridView
            Rectangle rect = dataGridView1.Bounds;
            rect.Inflate(3, 3);

            using (Graphics g = e.Graphics)
            {
                // Draw a subtle shadow
                for (int i = 0; i < 3; i++)
                {
                    using (Pen shadowPen = new Pen(Color.FromArgb(10, 0, 0, 0)))
                    {
                        g.DrawRectangle(shadowPen, rect);
                        rect.Inflate(-1, -1);
                    }
                }
            }
        }


        public void refreshData() 
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;   
            }
            displayTodaysCustomers();
            displayAllUsers();
            displayAllCustomers();
            displayTodaysIncome();
            displayTotalIncome();
        }

        public void displayTodaysCustomers()
        {
            CustomersData cData = new CustomersData();

            List<CustomersData> listData = cData.allTodayCustomers();

            dataGridView1.DataSource = listData;
        }
        public void displayAllUsers() 
        {
            if (checkConnection())
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT COUNT(id) FROM users WHERE status = @status";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@status", "Active");

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read()) 
                        {
                            int count = Convert.ToInt32(reader[0]);
                            dashboard_AU.Text = count.ToString();
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally 
                {
                    connect.Close();
                }
            }
        }

        public void displayAllCustomers() 
        {
            if (checkConnection())
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT COUNT(id) FROM customers";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int count = Convert.ToInt32(reader[0]);
                            dashboard_AC.Text = count.ToString();
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public void displayTodaysIncome()
        {
            if (checkConnection())
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT SUM(total_price) FROM customers WHERE order_date = @date";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        DateTime today = DateTime.Today;
                        string getToday = today.ToString("yyyy-MM-dd");
                        cmd.Parameters.AddWithValue("@date", getToday);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            object value = reader[0];
                            if (value != DBNull.Value)
                            {
                                decimal count = Convert.ToDecimal(value);
                                dashboard_TI.Text = "$" + count.ToString("0.00");
                            }
                            else
                            {
                                dashboard_TI.Text = "$0.00";
                            }
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex, "Error Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public void displayTotalIncome()
        {
            if (checkConnection())
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT SUM(total_price) FROM customers";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            object value = reader[0];
                            if (value != DBNull.Value)
                            {
                                decimal count = Convert.ToDecimal(value); 
                                dashboard_totalIncome.Text = "$" + count.ToString("0.00");
                            }
                            else
                            {
                                dashboard_totalIncome.Text = "$0.00"; 
                            }
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex, "Error Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    }
}
