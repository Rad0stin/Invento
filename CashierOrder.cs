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
            string selectedValue = cashierOrder_category.SelectedItem as string;

            if (selectedValue != null) 
            {
                if (checkConnection())
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                    finally 
                    {

                    }
                }
            }
        }
    }
}
