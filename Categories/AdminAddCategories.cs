using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Invento
{
    public partial class AdminAddCategories : UserControl
    {

        SqlConnection connect = Database.GetConnection();
        public AdminAddCategories()
        {
            InitializeComponent();

            displayCategoriesData();

            AdminCategoriesStyle.ApplyCategoriesStyle(this);
        }

        public class AdminCategoriesStyle
        {
            private static readonly Color PrimaryColor = Color.LightSeaGreen;
            private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
            private static readonly Color TextBoxColor = Color.FromArgb(240, 242, 245);
            private static readonly Color HeaderColor = Color.LightSeaGreen;
            private static readonly Color AlternateRowColor = Color.FromArgb(240, 248, 248);
            private static readonly Color BorderColor = Color.FromArgb(200, 223, 223);
            private static readonly Color TextColor = Color.Black;

            public static void ApplyCategoriesStyle(AdminAddCategories categoriesControl)
            {
                var dgv = categoriesControl.dataGridView1;
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

                var textBox = categoriesControl.addCategories_category;
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

                StyleCategoryButton(categoriesControl.addCategories_addBtn, "Add");
                StyleCategoryButton(categoriesControl.addCategories_updateBtn, "Update");
                StyleCategoryButton(categoriesControl.addCategories_removeBtn, "Remove");
                StyleCategoryButton(categoriesControl.addCategories_clearBtn, "Clear");

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

            private static void StyleCategoryButton(Button button, string type)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                button.Cursor = Cursors.Hand;
                button.FlatAppearance.BorderSize = 0;
                button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 15, 15));
                button.Padding = new Padding(10, 0, 10, 0);

            }

            [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
            private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
                int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayCategoriesData();
        }

        public void displayCategoriesData()
        {
            CategoriesData cData = new CategoriesData();
            List<CategoriesData> listdata = cData.AllCategoriesData();

            dataGridView1.DataSource = listdata;
        }

        private void addCategories_addBtn_Click(object sender, EventArgs e)
        {
            if (addCategories_category.Text == "")
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

                        string checkCat = "SELECT * FROM categories WHERE category = @cat";

                        using (SqlCommand cmd = new SqlCommand(checkCat, connect))
                        {
                            cmd.Parameters.AddWithValue("@cat", addCategories_category.Text.Trim());

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                MessageBox.Show("Category: " + addCategories_category.Text.Trim() + " already exists",
                                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                string insertData = "INSERT INTO categories (category, date) VALUES(@cat, @date)";

                                using (SqlCommand insertD = new SqlCommand(insertData, connect))
                                {
                                    insertD.Parameters.AddWithValue("@cat", addCategories_category.Text.Trim());
                                    DateTime dateValue;
                                    string formattedDate = DateTime.TryParse(DateTime.Now.ToString(), out dateValue)
                                        ? dateValue.ToString("MM/dd/yyyy HH:mm")
                                        : DateTime.Now.ToString();

                                    insertD.Parameters.AddWithValue("@date", formattedDate);

                                    insertD.ExecuteNonQuery();
                                    clearFields();
                                    displayCategoriesData();

                                    MessageBox.Show("Added succesfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (connect.State == ConnectionState.Closed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void clearFields()
        {
            addCategories_category.Text = "";
        }
        private void addCategories_clearBtn_Click(object sender, EventArgs e)
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

                addCategories_category.Text = row.Cells[1].Value.ToString();
            }
        }

        private void addCategories_updateBtn_Click(object sender, EventArgs e)
        {
            if (addCategories_category.Text == "")
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to update Cat ID: " + getID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            string updateData = "UPDATE categories SET category = @cat, date = @date WHERE id = @id";

                            using (SqlCommand updateD = new SqlCommand(updateData, connect))
                            {
                                updateD.Parameters.AddWithValue("@cat", addCategories_category.Text.Trim());
                                updateD.Parameters.AddWithValue("@id", getID);

                                DateTime dateValue;
                                string formattedDate = DateTime.TryParse(DateTime.Now.ToString(), out dateValue)
                                    ? dateValue.ToString("MM/dd/yyyy HH:mm")
                                    : DateTime.Now.ToString();

                                updateD.Parameters.AddWithValue("@date", formattedDate);

                                updateD.ExecuteNonQuery();
                                clearFields();
                                displayCategoriesData();

                                MessageBox.Show("Updated succesfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                }
            }
        }

        private void addCategories_removeBtn_Click(object sender, EventArgs e)
        {
            if (addCategories_category.Text == "")
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to Delete Cat ID: " + getID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            string removeData = "DELETE FROM categories WHERE id = @id";

                            using (SqlCommand deleteD = new SqlCommand(removeData, connect))
                            {
                                deleteD.Parameters.AddWithValue("@id", getID);

                                deleteD.ExecuteNonQuery();
                                clearFields();
                                displayCategoriesData();

                                MessageBox.Show("Deleted succesfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
