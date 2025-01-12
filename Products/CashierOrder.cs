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
using System.Drawing.Printing;
using System.Drawing.Drawing2D;

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

            displayOrders();

            displayTotalPrice();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayAllAvailableProducts();

            displayAllCategories();
            displayOrders();
            displayTotalPrice();
        }

        public void displayOrders()
        {
            OrdersData oData = new OrdersData();
            List<OrdersData> listData = oData.allOrdersData();

            dataGridView2.DataSource = listData;
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

        private float totalPrice = 0;
        public void displayTotalPrice()
        {
            IDGenerator();

            if (checkConnection())
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT SUM(total_price) FROM orders WHERE customer_id = @cID";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@cID", idGen);

                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value)
                        {
                            totalPrice = Convert.ToSingle(result);

                            cashierOrder_totalPrice.Text = totalPrice.ToString("0.00");
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
            displayOrders();
            displayTotalPrice();
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

        private void cashierOrder_removeBtn_Click(object sender, EventArgs e)
        {
            if (prodID == 0)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to Delete ID: "
                   + prodID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            string deleteData = "DELETE FROM orders WHERE id = @id";

                            using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                            {

                                cmd.Parameters.AddWithValue("@id", prodID);

                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Deleted succesfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            displayOrders();
            displayTotalPrice();
        }

        private int prodID = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

            prodID = (int)row.Cells[0].Value;

        }

        public void clearFields()
        {
            cashierOrder_category.SelectedIndex = -1;
            cashierOrder_prodID.SelectedIndex = -1;
            cashierOrder_prodName.Text = "";
            cashierOrder_price.Text = "";
            cashierOrder_qty.Value = 0;
        }
        private void cashierOrder_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void cashierOrder_payOrders_Click(object sender, EventArgs e)
        {
            IDGenerator();

            if (cashierOrder_amount.Text == "" || dataGridView2.Rows.Count < 0)
            {
                MessageBox.Show("No orders/empty fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to pay orders", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            string insertData = "INSERT INTO customers (customer_id, total_price, amount, change, order_date)" +
                                "VALUES(@cID,  @totalPrice, @amount, @change, @date)";

                            using (SqlCommand cmd = new SqlCommand(insertData, connect))
                            {
                                cmd.Parameters.AddWithValue("@cID", idGen);
                                cmd.Parameters.AddWithValue("@totalPrice", cashierOrder_totalPrice.Text);
                                cmd.Parameters.AddWithValue("@amount", cashierOrder_amount.Text);
                                cmd.Parameters.AddWithValue("@change", cashierOrder_change.Text);

                                DateTime today = DateTime.Now;
                                cmd.Parameters.AddWithValue("@date", today);

                                cmd.ExecuteNonQuery();

                                clearFields();

                                MessageBox.Show("Paid succesfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            displayTotalPrice();
        }

        private void cashierOrder_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    float getAmount = Convert.ToSingle(cashierOrder_amount.Text);
                    float getChange = (getAmount - totalPrice);

                    if (getChange <= -1)
                    {
                        cashierOrder_amount.Text = "";
                        cashierOrder_change.Text = "";
                    }
                    else
                    {
                        cashierOrder_change.Text = getChange.ToString("0.00");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Something went wrong", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cashierOrder_amount.Text = "";
                    cashierOrder_change.Text = "";
                }
            }
        }

        private int rowIndex = 0;
        private void cashierOrder_receipt_Click(object sender, EventArgs e)
        {
            if (cashierOrder_amount.Text == "" || dataGridView2.Rows.Count < 0)
            {
                MessageBox.Show("Please order first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                printDocument1.BeginPrint += new PrintEventHandler(printDocument1_BeginPrint);

                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();

                cashierOrder_amount.Text = "";
                cashierOrder_change.Text = "";
            }
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowIndex = 0;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Initialize variables
            float y = 0;
            int rowIndex = 0;
            float leftMargin = e.MarginBounds.Left + 40; // Increased margin for better appearance
            float rightMargin = e.MarginBounds.Right - 40;
            float width = rightMargin - leftMargin;

            // Define fonts
            Font logoFont = new Font("Arial", 24, FontStyle.Bold);
            Font titleFont = new Font("Arial", 14, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font normalFont = new Font("Arial", 9);
            Font itemFont = new Font("Courier New", 9);
            Font totalFont = new Font("Arial", 11, FontStyle.Bold);
            Font footerFont = new Font("Arial", 8);

            // Alignment
            StringFormat centerAlign = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat rightAlign = new StringFormat() { Alignment = StringAlignment.Far };
            StringFormat leftAlign = new StringFormat() { Alignment = StringAlignment.Near };

            // Colors
            Brush darkGray = new SolidBrush(Color.FromArgb(64, 64, 64));
            Brush mediumGray = new SolidBrush(Color.FromArgb(128, 128, 128));

            // Logo and Header section
            y += 20; // Top padding
            e.Graphics.DrawString("INVENTO", logoFont, Brushes.Black, leftMargin + (width / 2), y, centerAlign);
            y += logoFont.GetHeight(e.Graphics);

            e.Graphics.DrawString("Inventory Management System", normalFont, mediumGray, leftMargin + (width / 2), y, centerAlign);
            y += normalFont.GetHeight(e.Graphics) + 5;

            // Store info
            string[] storeInfo = {
        "123 Main Street, City, State 12345",
        "Tel: (555) 123-4567 | Email: contact@invento.com",
        "www.invento.com"
    };

            foreach (string info in storeInfo)
            {
                e.Graphics.DrawString(info, footerFont, mediumGray, leftMargin + (width / 2), y, centerAlign);
                y += footerFont.GetHeight(e.Graphics);
            }
            y += 10;

            // Separator line
            using (Pen blackPen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawLine(blackPen, leftMargin, y, rightMargin, y);
            }
            y += 10;

            // Receipt details in two columns
            float columnWidth = (width - 20) / 2;

            // Left column
            e.Graphics.DrawString("Receipt #:", headerFont, darkGray, leftMargin, y);
            e.Graphics.DrawString(idGen.ToString(), normalFont, Brushes.Black, leftMargin + 70, y);

            // Right column
            e.Graphics.DrawString("Date:", headerFont, darkGray, leftMargin + columnWidth, y);
            e.Graphics.DrawString(DateTime.Now.ToString("MM/dd/yyyy HH:mm"), normalFont, Brushes.Black, leftMargin + columnWidth + 50, y);

            y += headerFont.GetHeight(e.Graphics) + 15;

            // Separator line before items
            using (Pen grayPen = new Pen(mediumGray, 0.5f))
            {
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
            }
            y += 10;

            // Items header
            using (Pen grayPen = new Pen(Color.Gray, 0.5f))
            {
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
                y += 5;

                // Column headers
                float col1 = leftMargin;
                float col2 = leftMargin + width * 0.4f;
                float col3 = leftMargin + width * 0.6f;
                float col4 = leftMargin + width * 0.8f;

                e.Graphics.DrawString("Item", headerFont, darkGray, col1, y);
                e.Graphics.DrawString("Price", headerFont, darkGray, col2, y, rightAlign);
                e.Graphics.DrawString("Qty", headerFont, darkGray, col3, y, rightAlign);
                e.Graphics.DrawString("Total", headerFont, darkGray, col4, y, rightAlign);

                y += headerFont.GetHeight(e.Graphics) + 5;
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
                y += 5;
            }

            // Separator line after header
            using (Pen grayPen = new Pen(mediumGray, 0.5f))
            {
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
            }
            y += 10;

            // Items
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (y + itemFont.GetHeight(e.Graphics) > e.MarginBounds.Bottom - 150)
                {
                    e.HasMorePages = true;
                    return;
                }

                float col1 = leftMargin;
                float col2 = leftMargin + width * 0.4f;
                float col3 = leftMargin + width * 0.6f;
                float col4 = leftMargin + width * 0.8f;

                // Get correct values from the OrdersData columns
                string itemName = row.Cells["PName"].Value.ToString();
                decimal origPrice = decimal.Parse(row.Cells["OrigPrice"].Value.ToString());
                int qty = int.Parse(row.Cells["QTY"].Value.ToString());
                decimal total = decimal.Parse(row.Cells["TotalPrice"].Value.ToString());

                e.Graphics.DrawString(itemName, itemFont, darkGray, col1, y);
                e.Graphics.DrawString(origPrice.ToString("C"), itemFont, darkGray, col2, y, rightAlign);
                e.Graphics.DrawString(qty.ToString(), itemFont, darkGray, col3, y, rightAlign);
                e.Graphics.DrawString(total.ToString("C"), itemFont, darkGray, col4, y, rightAlign);

                y += itemFont.GetHeight(e.Graphics) + 5;
            }

            // Separator line before totals
            y += 5;
            using (Pen blackPen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawLine(blackPen, leftMargin, y, rightMargin, y);
            }
            y += 15;

            // Totals section with right alignment
            float totalsX = rightMargin - 150;
            float labelsX = rightMargin - 240;

            string[] totalLabels = { "Subtotal:", "Amount Paid:", "Change:" };
            float[] totalValues = {
        Convert.ToSingle(cashierOrder_totalPrice.Text),
        Convert.ToSingle(cashierOrder_amount.Text),
        Convert.ToSingle(cashierOrder_change.Text)
    };

            for (int i = 0; i < totalLabels.Length; i++)
            {
                e.Graphics.DrawString(totalLabels[i], totalFont, darkGray, labelsX, y, rightAlign);
                e.Graphics.DrawString(totalValues[i].ToString("C2"), totalFont, Brushes.Black, rightMargin, y, rightAlign);
                y += totalFont.GetHeight(e.Graphics) + (i == totalLabels.Length - 1 ? 15 : 5);
            }

            // Footer
            y = e.MarginBounds.Bottom - 60;

            // Separator line
            using (Pen grayPen = new Pen(mediumGray, 0.5f))
            {
                e.Graphics.DrawString("Thank you for your business!", headerFont, darkGray, leftMargin + (width / 2), y, centerAlign);
                y += headerFont.GetHeight(e.Graphics) + 10;

                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
                y += 10;
            }

            // Footer text
            string[] footerText = {
        "All prices include applicable taxes",
        "Returns accepted within 30 days with receipt",
        "Follow us on social media @InventoSystem"
    };

            foreach (string text in footerText)
            {
                e.Graphics.DrawString(text, footerFont, mediumGray, leftMargin + (width / 2), y, centerAlign);
                y += footerFont.GetHeight(e.Graphics);
            }
        }
    }
}