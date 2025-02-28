using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Invento
{
    public partial class CashierOrder : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public CashierOrder()
        {
            InitializeComponent();

            OrderDesing.ApplyProductStyle(this);

            displayAllAvailableProducts();

            displayAllCategories();

            displayOrders();

            displayTotalPrice();
        }

        public class OrderDesing
        {
            private static readonly Color PrimaryColor = Color.LightSeaGreen;
            private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
            private static readonly Color TextBoxColor = Color.FromArgb(240, 242, 245);
            private static readonly Color HeaderColor = Color.LightSeaGreen;
            private static readonly Color AlternateRowColor = Color.FromArgb(240, 248, 248);
            private static readonly Color BorderColor = Color.FromArgb(200, 223, 223);
            private static readonly Color TextColor = Color.Black;

            public static void ApplyProductStyle(CashierOrder productsControl)
            {
                StyleTextBox(productsControl.cashierOrder_amount);

                StyleComboBox(productsControl.cashierOrder_category);
                StyleComboBox(productsControl.cashierOrder_prodID);

                StyleButton(productsControl.cashierOrder_addBtn, "Add");
                StyleButton(productsControl.cashierOrder_removeBtn, "Remove");
                StyleButton(productsControl.cashierOrder_clearBtn, "Clear");
                StyleButton(productsControl.cashierOrder_payOrders, "Pay Orders");
                StyleButton(productsControl.cashierOrder_receipt, "Receipt");
                StyleDataGridView(productsControl.dataGridView1);
                StyleDataGridView(productsControl.dataGridView2);
                StyleNumericUpDown(productsControl.cashierOrder_qty);
            }

            private static void StyleDataGridView(DataGridView dgv)
            {
                typeof(DataGridView).InvokeMember(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                    null,
                    dgv,
                    new object[] { true });

                dgv.BackgroundColor = BackgroundColor;
                dgv.BorderStyle = BorderStyle.None;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dgv.EnableHeadersVisualStyles = false;
                dgv.GridColor = BorderColor;
                dgv.RowHeadersVisible = false;

                dgv.ScrollBars = ScrollBars.Both;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; 

                dgv.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10.5f);
                dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
                dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
                dgv.ColumnHeadersHeight = 35;
                dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

                dgv.DefaultCellStyle.BackColor = Color.White;
                dgv.DefaultCellStyle.ForeColor = TextColor;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
                dgv.DefaultCellStyle.Padding = new Padding(5, 2, 5, 2);
                dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternateRowColor;

                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 220, 220);
                dgv.DefaultCellStyle.SelectionForeColor = TextColor;

                dgv.RowTemplate.Height = 30;

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.MinimumWidth = 80; 

                    switch (column.Name.ToLower())
                    {
                        case "id":
                            column.Width = 60;
                            break;
                        case "prodid":
                        case "category":
                        case "status":
                            column.Width = 100;
                            break;
                        case "prodname":
                            column.Width = 150;
                            break;
                        case "imagepath":
                            column.Width = 200;
                            break;
                        case "date":
                            column.Width = 120;
                            break;
                        default:
                            column.Width = 100;
                            break;
                    }
                }

                dgv.AllowUserToResizeRows = false;
                dgv.AllowUserToResizeColumns = true;
                dgv.AllowUserToOrderColumns = true;
                dgv.MultiSelect = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

                dgv.MinimumSize = new Size(200, 0);

                dgv.Resize -= DataGridView_Resize;
                dgv.Resize += DataGridView_Resize;
            }

            private static void DataGridView_Resize(object sender, EventArgs e)
            {
                if (sender is DataGridView dgv)
                {
                    int totalColumnWidth = 0;
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        totalColumnWidth += col.Width;
                    }

                    if (totalColumnWidth > dgv.Width)
                    {
                        dgv.ScrollBars = ScrollBars.Both;
                    }
                }
            }


            private static void StyleNumericUpDown(NumericUpDown numericUpDown)
            {
                Panel container = new Panel
                {
                    Size = new Size(numericUpDown.Width, numericUpDown.Height + 10),
                    Location = new Point(numericUpDown.Location.X, numericUpDown.Location.Y - 5),
                    BackColor = TextBoxColor
                };

                if (numericUpDown.Parent is Panel parentPanel)
                {
                    parentPanel.Controls.Add(container);
                    container.BringToFront();
                    numericUpDown.Parent = container;
                    numericUpDown.Location = new Point(8, 5);
                    numericUpDown.Width = container.Width - 16;
                }

                numericUpDown.BackColor = TextBoxColor;
                numericUpDown.ForeColor = TextColor;
                numericUpDown.Font = new Font("Segoe UI", 12);
                numericUpDown.BorderStyle = BorderStyle.None;

                container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 15, 15));

                numericUpDown.GotFocus += (s, e) =>
                {
                    container.BackColor = Color.FromArgb(235, 237, 240);
                    numericUpDown.BackColor = Color.FromArgb(235, 237, 240);
                };

                numericUpDown.LostFocus += (s, e) =>
                {
                    container.BackColor = TextBoxColor;
                    numericUpDown.BackColor = TextBoxColor;
                };
            }

            private static void StyleTextBox(TextBox textBox)
            {
                Panel container = new Panel
                {
                    Size = new Size(textBox.Width, textBox.Height + 10),
                    Location = new Point(textBox.Location.X, textBox.Location.Y - 5),
                    BackColor = TextBoxColor
                };

                if (textBox.Parent is Panel parentPanel)
                {
                    parentPanel.Controls.Add(container);
                    container.BringToFront();
                    textBox.Parent = container;
                    textBox.Location = new Point(8, 5);
                    textBox.Width = container.Width - 16;
                }

                container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 15, 15));
                textBox.BackColor = TextBoxColor;
                textBox.ForeColor = TextColor;
                textBox.BorderStyle = BorderStyle.None;
                textBox.Font = new Font("Segoe UI", 12);

                textBox.GotFocus += (s, e) =>
                {
                    container.BackColor = Color.FromArgb(235, 237, 240);
                    textBox.BackColor = Color.FromArgb(235, 237, 240);
                };

                textBox.LostFocus += (s, e) =>
                {
                    container.BackColor = TextBoxColor;
                    textBox.BackColor = TextBoxColor;
                };
            }

            private static void StyleComboBox(ComboBox comboBox)
            {
                Panel container = new Panel
                {
                    Size = new Size(comboBox.Width, comboBox.Height + 10),
                    Location = new Point(comboBox.Location.X, comboBox.Location.Y - 5),
                    BackColor = TextBoxColor
                };

                if (comboBox.Parent is Panel parentPanel)
                {
                    parentPanel.Controls.Add(container);
                    container.BringToFront();
                    comboBox.Parent = container;
                    comboBox.Location = new Point(8, 5);
                    comboBox.Width = container.Width - 16;
                }

                container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 15, 15));
                comboBox.BackColor = TextBoxColor;
                comboBox.ForeColor = TextColor;
                comboBox.Font = new Font("Segoe UI", 12);
                comboBox.FlatStyle = FlatStyle.Flat;
            }

            private static void StyleButton(Button button, string type)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.White;
                button.BackColor = PrimaryColor;
                button.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                button.Cursor = Cursors.Hand;
                button.FlatAppearance.BorderSize = 0;
                button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 15, 15));
                button.Padding = new Padding(10, 0, 10, 0);

                button.MouseEnter += (s, e) =>
                {
                    button.BackColor = Color.FromArgb(0, 150, 150); 
                };

                button.MouseLeave += (s, e) =>
                {
                    button.BackColor = PrimaryColor;
                };
            }

            [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
            private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
                int nBottomRect, int nRightRect, int nWidthEllipse, int nHeightEllipse);
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

                        string checkStock = "SELECT stock FROM products WHERE prod_id = @prodID";
                        int currentStock = 0;

                        using (SqlCommand stockCmd = new SqlCommand(checkStock, connect))
                        {
                            stockCmd.Parameters.AddWithValue("@prodID", cashierOrder_prodID.SelectedItem);
                            object result = stockCmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                currentStock = Convert.ToInt32(result);
                            }
                        }

                        if ((int)cashierOrder_qty.Value > currentStock)
                        {
                            MessageBox.Show($"Not enough stock available! Current stock: {currentStock}",
                                "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

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
            cashierOrder_prodName.Text = "";
            cashierOrder_price.Text = "";
            cashierOrder_qty.Value = 0;
            cashierOrder_prodID.AccessibilityObject.Value = "";
            cashierOrder_category.Text = "";
        }

        private int idGen;
        public void IDGenerator()
        {
            using (SqlConnection connect = Database.GetConnection())
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

                    OrdersData.SetCurrentCustomerId(idGen);
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
            cashierOrder_totalPrice.Text = "0.00";
            cashierOrder_amount.Text = "";
            cashierOrder_change.Text = "";
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
                return;
            }

            if (MessageBox.Show("Are you sure you want to pay orders", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (!checkConnection())
            {
                return;
            }

            try
            {
                connect.Open();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["PID"].Value == null) continue;

                    string productId = row.Cells["PID"].Value.ToString();
                    int orderedQuantity = Convert.ToInt32(row.Cells["QTY"].Value);

                    string getStockQuery = "SELECT stock FROM products WHERE prod_id = @prodID";
                    int currentStock = 0;

                    using (SqlCommand getStock = new SqlCommand(getStockQuery, connect))
                    {
                        getStock.Parameters.AddWithValue("@prodID", productId);
                        object result = getStock.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            currentStock = Convert.ToInt32(result);
                        }
                    }

                    string updateStockQuery = "UPDATE products SET stock = @newStock WHERE prod_id = @prodID";
                    using (SqlCommand updateStock = new SqlCommand(updateStockQuery, connect))
                    {
                        int newStock = currentStock - orderedQuantity;
                        if (newStock < 0)
                        {
                            MessageBox.Show($"Insufficient stock for product {productId}!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        updateStock.Parameters.AddWithValue("@newStock", newStock);
                        updateStock.Parameters.AddWithValue("@prodID", productId);
                        updateStock.ExecuteNonQuery();
                    }
                }

                string insertData = @"INSERT INTO customers (customer_id, total_price, amount, change, order_date)
                            VALUES(@cID, @totalPrice, @amount, @change, @date)";

                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                {
                    cmd.Parameters.AddWithValue("@cID", idGen);
                    cmd.Parameters.AddWithValue("@totalPrice", cashierOrder_totalPrice.Text);
                    cmd.Parameters.AddWithValue("@amount", cashierOrder_amount.Text);
                    cmd.Parameters.AddWithValue("@change", cashierOrder_change.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);

                    cmd.ExecuteNonQuery();

                    string clearOrders = "DELETE FROM orders WHERE customer_id = @customerID";
                    using (SqlCommand clearCmd = new SqlCommand(clearOrders, connect))
                    {
                        clearCmd.Parameters.AddWithValue("@customerID", idGen);
                        clearCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Paid successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    displayAllAvailableProducts();
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

        private void cashierOrder_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    float getAmount = Convert.ToSingle(cashierOrder_amount.Text);
                    float getChange = (getAmount - totalPrice);

                    if (getChange < 0)
                    {
                        MessageBox.Show("Insufficient funds. Please enter a higher amount.", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                cashierOrder_totalPrice.Text = "0.00";
                clearFields();

                OrdersData.SetCurrentCustomerId(-1);
                displayOrders();
                displayTotalPrice();
            }
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowIndex = 0;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float y = 0;
            float leftMargin = e.MarginBounds.Left + 40; 
            float rightMargin = e.MarginBounds.Right - 40;
            float width = rightMargin - leftMargin;

            Font logoFont = new Font("Arial", 24, FontStyle.Bold);
            Font titleFont = new Font("Arial", 14, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font normalFont = new Font("Arial", 9);
            Font itemFont = new Font("Courier New", 9);
            Font totalFont = new Font("Arial", 11, FontStyle.Bold);
            Font footerFont = new Font("Arial", 8);

            StringFormat centerAlign = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat rightAlign = new StringFormat() { Alignment = StringAlignment.Far };
            StringFormat leftAlign = new StringFormat() { Alignment = StringAlignment.Near };

            Brush darkGray = new SolidBrush(Color.FromArgb(64, 64, 64));
            Brush mediumGray = new SolidBrush(Color.FromArgb(128, 128, 128));

            y += 20; 
            e.Graphics.DrawString("INVENTO", logoFont, Brushes.Black, leftMargin + (width / 2), y, centerAlign);
            y += logoFont.GetHeight(e.Graphics);

            e.Graphics.DrawString("Inventory Management System", normalFont, mediumGray, leftMargin + (width / 2), y, centerAlign);
            y += normalFont.GetHeight(e.Graphics) + 5;

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

            using (Pen blackPen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawLine(blackPen, leftMargin, y, rightMargin, y);
            }
            y += 10;

            float columnWidth = (width - 20) / 2;

            e.Graphics.DrawString("Receipt #:", headerFont, darkGray, leftMargin, y);
            e.Graphics.DrawString(idGen.ToString(), normalFont, Brushes.Black, leftMargin + 70, y);

            e.Graphics.DrawString("Date:", headerFont, darkGray, leftMargin + columnWidth, y);
            e.Graphics.DrawString(DateTime.Now.ToString("MM/dd/yyyy HH:mm"), normalFont, Brushes.Black, leftMargin + columnWidth + 50, y);

            y += headerFont.GetHeight(e.Graphics) + 15;

            using (Pen grayPen = new Pen(mediumGray, 0.5f))
            {
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
            }
            y += 10;

            using (Pen grayPen = new Pen(Color.Gray, 0.5f))
            {
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
                y += 5;

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

            using (Pen grayPen = new Pen(mediumGray, 0.5f))
            {
                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
            }
            y += 10;

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

            y += 5;
            using (Pen blackPen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawLine(blackPen, leftMargin, y, rightMargin, y);
            }
            y += 15;

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

            y = e.MarginBounds.Bottom - 60;

            using (Pen grayPen = new Pen(mediumGray, 0.5f))
            {
                e.Graphics.DrawString("Thank you for your business!", headerFont, darkGray, leftMargin + (width / 2), y, centerAlign);
                y += headerFont.GetHeight(e.Graphics) + 10;

                e.Graphics.DrawLine(grayPen, leftMargin, y, rightMargin, y);
                y += 10;
            }

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