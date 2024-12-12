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
    public partial class AdminAddProducts : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Radostin\Documents\invento.mdf;Integrated Security=True;Connect Timeout=30;");
        public AdminAddProducts()
        {
            InitializeComponent();
        }

        private void addProducts_addBtn_Click(object sender, EventArgs e)
        {
            if(checkConnection())
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM products WHERE prod_id = @prodID";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@prod_id", addProducts_prodID.Text.Trim());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();

                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show("Product ID: " + addProducts_prodID.Text.Trim() + " already exists",
                                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            string insertData = "INSERT INTO products (prod_id, prod_name, category, price, stock, image_path, status, date_insert)" +
                                "VALUES(@prodID, @prodName, @cat, @price, @stock, @path, @status, @date)";

                            using (SqlCommand insertD = new SqlCommand(insertData, connect))
                            {
                                insertD.Parameters.AddWithValue("@prodID", addProducts_prodID.Text.Trim());
                                insertD.Parameters.AddWithValue("@prodName", addProducts_prodName.Text.Trim());
                                insertD.Parameters.AddWithValue("@cat", addProducts_category.SelectedItem);
                                insertD.Parameters.AddWithValue("@price", addProducts_price.Text.Trim());
                                insertD.Parameters.AddWithValue("@stock", addProducts_stock.Text.Trim());
                                insertD.Parameters.AddWithValue("@path", addProducts_prodID.Text.Trim());
                                insertD.Parameters.AddWithValue("@path", addProducts_status.SelectedItem);

                                DateTime today = DateTime.Today;
                                insertD.Parameters.AddWithValue("@date", addProducts_prodID.Text.Trim());
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

        private void addProducts_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
                string imagePath = "";

                if (dialog.ShowDialog() == DialogResult.Yes)
                {
                    imagePath = dialog.FileName;
                    addProducts_imageView.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
