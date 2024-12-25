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
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Invento
{
    public partial class CashierOrder : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        public CashierOrder()
        {
            InitializeComponent();

            displayAllAvailableProducts();

            displayAllCategories();
        }

        public void displayAllAvailableProducts() 
        {
            AddProductsData apData = new AddProductsData();
            List<AddProductsData> listdata = apData.allAvailableProducts();

            dataGridView1.DataSource = listdata;
        }

        public bool checkConnection()
        {
            if (connect.State != ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void displayAllCategories() 
        {
            if (checkConnection())
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM categories";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader()) 
                        {
                            cashierOrder_category.Items.Clear();

                            while (reader.Read())
                            {
                                string item = reader.GetString(1);
                                cashierOrder_category.Items.Add(item);
                            }
                        }
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

      
        private void cashierOrder_category_SelectedIndexChanged(object sender, EventArgs e)
        {
            cashierOrder_prodID.SelectedIndex = -1;
            cashierOrder_prodID.Items.Clear();
            cashierOrder_prodName.Text = "";
            cashierOrder_price.Text = "";

            string selectedValue = cashierOrder_category.SelectedItem as string;

            if (selectedValue != null) 
            {
                if (checkConnection())
                {
                    try
                    {
                        connect.Open();

                        string selectData = $"SELECT * FROM products WHERE category = '{selectedValue}' AND status = @status";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@status", "Available");

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string value = reader["prod_id"].ToString();
                                    cashierOrder_prodID.Items.Add(value);
                                }
                            }
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
        }

        private void cashierOrder_prodID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = cashierOrder_prodID.SelectedItem as string;

            if (checkConnection())
            {
                if (selectedValue != null)
                {
                    try
                    {
                        connect.Open();

                        string selectData = $"SELECT * FROM products WHERE prod_ID = '{selectedValue}' AND status = @status";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@status", "Available");

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string prodName = reader["prod_name"].ToString();
                                    float prodPrice = Convert.ToSingle(reader["price"]);

                                    cashierOrder_prodName.Text = prodName;
                                    cashierOrder_price.Text = prodPrice.ToString("0.00");
                                }
                            }
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
        }

        private void cashierOrder_addBtn_Click(object sender, EventArgs e)
        {
            IDGenerator();

            if (cashierOrder_category.SelectedIndex == -1 || cashierOrder_prodID.SelectedIndex == -1
                || cashierOrder_prodName.Text == "" || cashierOrder_price.Text == "" || cashierOrder_qty.Value == 0)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (checkConnection())
                {
                    try
                    {
                        connect.Open();

                        float getprice = 0;
                        string selectOrder = "SELECT * FROM products WHERE prod_id = @prodID";

                        using (SqlCommand getOrder = new SqlCommand(selectOrder, connect))
                        {
                            getOrder.Parameters.AddWithValue("@prodID", cashierOrder_prodID.SelectedItem);

                            using (SqlDataReader reader = getOrder.ExecuteReader()) 
                            {
                                if (reader.Read())
                                {
                                    object rawValue = reader["price"];

                                    if (rawValue != DBNull.Value)
                                    {
                                        getprice = Convert.ToSingle(rawValue);
                                    }
                                }
                            }
                        }

                        string insertData = "INSERT INTO orders (customer_id, prod_id, prod_name, category, qty, orig_price, total_price, order_date)" +
                            "VALUES(@cID, @prodID, @prodName, @cat, @qty, @origPrice, @totalprice, @date)";

                        using (SqlCommand cmd = new SqlCommand(insertData, connect))
                        {
                            cmd.Parameters.AddWithValue("@cID", idGen);
                            cmd.Parameters.AddWithValue("@prodID", cashierOrder_prodID.SelectedItem);
                            cmd.Parameters.AddWithValue("@prodName", cashierOrder_prodName.Text.Trim());
                            cmd.Parameters.AddWithValue("@cat", cashierOrder_category.SelectedItem);
                            cmd.Parameters.AddWithValue("@qty", cashierOrder_qty.Value);
                            cmd.Parameters.AddWithValue("@origPrice", getprice);

                            float totalP = (getprice * (int)cashierOrder_qty.Value);

                            cmd.Parameters.AddWithValue("@totalprice", totalP);

                            DateTime today = DateTime.Today;    
                            cmd.Parameters.AddWithValue("@date", today);

                            cmd.ExecuteNonQuery();
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
        }

        private int idGen;
        public void IDGenerator()
        {
            using (SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;"))
            {
                connect.Open();

                string selectData = "SELECT MAX(customer_id) FROM customers";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        int temp = Convert.ToInt32(result);

                        if (temp == 0)
                        {
                            idGen = 1;
                        }
                        else 
                        {
                            idGen = temp + 1;
                        }
                    }
                    else
                    {
                        idGen = 1;
                    }
                }

            }
        }
    }
}
