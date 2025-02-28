using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Invento
{
    public partial class AdminAddProducts : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        public AdminAddProducts()
        {
            InitializeComponent();

            AdminProductsStyle.ApplyProductStyle(this);

            displayCategries();

            displayAllProducts();
        }

        public class AdminProductsStyle
        {
            private static readonly Color PrimaryColor = Color.LightSeaGreen;
            private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
            private static readonly Color TextBoxColor = Color.FromArgb(240, 242, 245);
            private static readonly Color HeaderColor = Color.LightSeaGreen;
            private static readonly Color AlternateRowColor = Color.FromArgb(240, 248, 248);
            private static readonly Color BorderColor = Color.FromArgb(200, 223, 223);
            private static readonly Color TextColor = Color.Black;

            public static void ApplyProductStyle(AdminAddProducts productsControl)
            {
                var dgv = productsControl.dataGridView1;
                dgv.BorderStyle = BorderStyle.None;
                dgv.BackgroundColor = Color.White;
                dgv.EnableHeadersVisualStyles = false;
                dgv.GridColor = BorderColor;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgv.RowHeadersVisible = false;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10.5F);
                dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
                dgv.ColumnHeadersHeight = 45;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                dgv.DefaultCellStyle.BackColor = Color.White;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
                dgv.DefaultCellStyle.ForeColor = TextColor;
                dgv.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 244);
                dgv.DefaultCellStyle.SelectionForeColor = TextColor;

                dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternateRowColor;
                dgv.AlternatingRowsDefaultCellStyle.Font = new Font("Segoe UI", 10F);
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextColor;
                dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 244);
                dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextColor;
                dgv.RowTemplate.Height = 40;

                StyleTextBox(productsControl.addProducts_prodID);
                StyleTextBox(productsControl.addProducts_prodName);
                StyleTextBox(productsControl.addProducts_price);
                StyleTextBox(productsControl.addProducts_stock);

                StyleComboBox(productsControl.addProducts_category);
                StyleComboBox(productsControl.addProducts_status);

                StylePictureBox(productsControl.addProducts_imageView);

                StyleButton(productsControl.addProducts_addBtn, "Add");
                StyleButton(productsControl.addProducts_updateBtn, "Update");
                StyleButton(productsControl.addProducts_removeBtn, "Remove");
                StyleButton(productsControl.addProducts_clearBtn, "Clear");
                StyleButton(productsControl.addProducts_importBtn, "Import");

                dgv.CellMouseEnter += (sender, e) => {
                    if (e.RowIndex >= 0)
                    {
                        var row = dgv.Rows[e.RowIndex];
                        row.DefaultCellStyle.BackColor = Color.FromArgb(230, 244, 244);
                    }
                };

                dgv.CellMouseLeave += (sender, e) => {
                    if (e.RowIndex >= 0)
                    {
                        var row = dgv.Rows[e.RowIndex];
                        row.DefaultCellStyle.BackColor = e.RowIndex % 2 == 0
                            ? Color.White
                            : AlternateRowColor;
                    }
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

                textBox.GotFocus += (s, e) => {
                    container.BackColor = Color.FromArgb(235, 237, 240);
                    textBox.BackColor = Color.FromArgb(235, 237, 240);
                };

                textBox.LostFocus += (s, e) => {
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

            private static void StylePictureBox(PictureBox pictureBox)
            {
                Panel container = new Panel
                {
                    Size = new Size(pictureBox.Width + 10, pictureBox.Height + 10),
                    Location = new Point(pictureBox.Location.X - 5, pictureBox.Location.Y - 5),
                    BackColor = TextBoxColor
                };

                if (pictureBox.Parent is Panel parentPanel)
                {
                    parentPanel.Controls.Add(container);
                    container.BringToFront();
                    pictureBox.Parent = container;
                    pictureBox.Location = new Point(5, 5);
                }

                container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 15, 15));
                pictureBox.BackColor = Color.White;
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
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

                button.MouseEnter += (s, e) => {
                    button.BackColor = Color.FromArgb(0, 150, 150); 
                };

                button.MouseLeave += (s, e) => {
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
            displayCategries();
            displayAllProducts();
        }

        public void displayAllProducts()
        {
            AddProductsData apData = new AddProductsData();
            List<AddProductsData> listData = apData.AllProductsData();

            dataGridView1.DataSource = listData;
        }

        public bool emptyFields()
        {
            if (addProducts_prodID.Text == "" || addProducts_prodName.Text == "" || addProducts_category.SelectedIndex == -1
               || addProducts_price.Text == "" || addProducts_stock.Text == "" || addProducts_status.SelectedIndex == -1
               || addProducts_imageView.Image == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void displayCategries()
        {
            if (checkConnection())
            {
                try
                {
                    connect.Open();

                    addProducts_category.Items.Clear();

                    string selectData = "SELECT * FROM categories";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                addProducts_category.Items.Add(reader["category"].ToString());
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

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            try
            {
                using (Image imageCopy = new Bitmap(image))
                using (MemoryStream ms = new MemoryStream())
                {
                    imageCopy.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Image processing error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null)
                return null;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private void addProducts_addBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (checkConnection())
                {
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT * FROM products WHERE prod_id = @prodID";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@prodID", addProducts_prodID.Text.Trim());

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
                                byte[] imageData = ImageToByteArray(addProducts_imageView.Image);

                                string insertData = "INSERT INTO products (prod_id, prod_name, category, price, stock, product_image, status, date_insert)" +
                                    "VALUES(@prodID, @prodName, @cat, @price, @stock, @image, @status, @date)";

                                using (SqlCommand insertD = new SqlCommand(insertData, connect))
                                {
                                    insertD.Parameters.AddWithValue("@prodID", addProducts_prodID.Text.Trim());
                                    insertD.Parameters.AddWithValue("@prodName", addProducts_prodName.Text.Trim());
                                    insertD.Parameters.AddWithValue("@cat", addProducts_category.SelectedItem);
                                    insertD.Parameters.AddWithValue("@price", addProducts_price.Text.Trim());
                                    insertD.Parameters.AddWithValue("@stock", addProducts_stock.Text.Trim());
                                    insertD.Parameters.AddWithValue("@image", imageData);
                                    insertD.Parameters.AddWithValue("@status", addProducts_status.SelectedItem);

                                    DateTime dateValue;
                                    string formattedDate = DateTime.TryParse(DateTime.Now.ToString(), out dateValue)
                                        ? dateValue.ToString("MM/dd/yyyy HH:mm")
                                        : DateTime.Now.ToString();
                                    insertD.Parameters.AddWithValue("@date", formattedDate);

                                    insertD.ExecuteNonQuery();
                                    clearFields();
                                    displayAllProducts();

                                    MessageBox.Show("Product added successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = dialog.FileName;
                    addProducts_imageView.Image = Image.FromFile(imagePath);
                    addProducts_imageView.ImageLocation = imagePath; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void clearFields()
        {
            addProducts_prodID.Text = "";
            addProducts_prodName.Text = "";
            addProducts_category.SelectedIndex = -1;
            addProducts_price.Text = "";
            addProducts_stock.Text = "";
            addProducts_status.SelectedIndex = -1;
            addProducts_imageView.Image = null;

        }

        private void addProducts_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private int getID = 0;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                getID = (int)row.Cells[0].Value;

                addProducts_prodID.Text = row.Cells[1].Value.ToString();
                addProducts_prodName.Text = row.Cells[2].Value.ToString();
                addProducts_category.Text = row.Cells[3].Value.ToString();
                addProducts_price.Text = row.Cells[4].Value.ToString();
                addProducts_stock.Text = row.Cells[5].Value.ToString();

                string status = row.Cells[6].Value.ToString(); 

                if (status == "Available" || status == "Not Available")
                {
                    addProducts_status.SelectedItem = status;
                }
                else
                {
                    int statusColumnIndex = -1;
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (dataGridView1.Columns[i].Name.ToLower().Contains("status"))
                        {
                            statusColumnIndex = i;
                            break;
                        }
                    }

                    if (statusColumnIndex != -1)
                    {
                        status = row.Cells[statusColumnIndex].Value.ToString();
                        addProducts_status.SelectedItem = status;
                    }
                    else
                    {
                        addProducts_status.SelectedIndex = 0;
                    }
                }

                if (checkConnection())
                {
                    try
                    {
                        connect.Open();
                        string selectImage = "SELECT product_image FROM products WHERE id = @id";

                        using (SqlCommand cmd = new SqlCommand(selectImage, connect))
                        {
                            cmd.Parameters.AddWithValue("@id", getID);

                            byte[] imageData = cmd.ExecuteScalar() as byte[];
                            if (imageData != null && imageData.Length > 0)
                            {
                                addProducts_imageView.Image = ByteArrayToImage(imageData);
                            }
                            else
                            {
                                addProducts_imageView.Image = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void addProducts_updateBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to update Product ID: "
                    + addProducts_prodID.Text.Trim() + "?", "Confirmation Message",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            byte[] imageData = ImageToByteArray(addProducts_imageView.Image);

                            if (imageData == null && addProducts_imageView.Image != null)
                            {
                                connect.Close();
                                return;
                            }

                            string updateData;
                            SqlCommand updateD;

                            if (imageData != null)
                            {
                                updateData = "UPDATE products SET prod_id = @prodID, prod_name = @prodName, category = @cat, " +
                                    "price = @price, stock = @stock, product_image = @image, status = @status, date_insert = @date WHERE id = @id";

                                updateD = new SqlCommand(updateData, connect);
                                updateD.Parameters.AddWithValue("@image", imageData);
                            }
                            else
                            {
                                updateData = "UPDATE products SET prod_id = @prodID, prod_name = @prodName, category = @cat, " +
                                    "price = @price, stock = @stock, status = @status, date_insert = @date WHERE id = @id";

                                updateD = new SqlCommand(updateData, connect);
                            }

                            updateD.Parameters.AddWithValue("@prodID", addProducts_prodID.Text.Trim());
                            updateD.Parameters.AddWithValue("@prodName", addProducts_prodName.Text.Trim());
                            updateD.Parameters.AddWithValue("@cat", addProducts_category.SelectedItem);
                            updateD.Parameters.AddWithValue("@price", addProducts_price.Text.Trim());
                            updateD.Parameters.AddWithValue("@stock", addProducts_stock.Text.Trim());
                            updateD.Parameters.AddWithValue("@status", addProducts_status.SelectedItem);
                            updateD.Parameters.AddWithValue("@id", getID);

                            DateTime dateValue;
                            string formattedDate = DateTime.TryParse(DateTime.Now.ToString(), out dateValue)
                                ? dateValue.ToString("MM/dd/yyyy HH:mm")
                                : DateTime.Now.ToString();
                            updateD.Parameters.AddWithValue("@date", formattedDate);

                            updateD.ExecuteNonQuery();
                            clearFields();
                            displayAllProducts();

                            MessageBox.Show("Updated successfully", "Information Message",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            }
        }

        private void addProducts_removeBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to DELETE Product ID: "
                    + addProducts_prodID.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            string deleteData = "DELETE FROM products WHERE id = @id";

                            using (SqlCommand deleteD = new SqlCommand(deleteData, connect))
                            {
                                deleteD.Parameters.AddWithValue("@id", getID);

                                deleteD.ExecuteNonQuery();
                                clearFields();
                                displayAllProducts();

                                MessageBox.Show("Deleted successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }
    }
}
