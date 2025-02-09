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
    public partial class AdminAddUser : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=35.233.55.91;Initial Catalog=invento;User ID=sqlserver;Password=Rado1234@;");

        public AdminAddUser()
        {
            InitializeComponent();

            AdminUserStyle.ApplyUserStyle(this);

            displayAllUsersData();

        }

        public class AdminUserStyle
        {
            // Color scheme - matching AdminCategoriesStyle
            private static readonly Color PrimaryColor = Color.LightSeaGreen;
            private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
            private static readonly Color TextBoxColor = Color.FromArgb(240, 242, 245);
            private static readonly Color HeaderColor = Color.LightSeaGreen;
            private static readonly Color AlternateRowColor = Color.FromArgb(240, 248, 248);
            private static readonly Color BorderColor = Color.FromArgb(200, 223, 223);
            private static readonly Color TextColor = Color.Black;

            public static void ApplyUserStyle(AdminAddUser userControl)
            {
                // Style the Users DataGridView
                var dgv = userControl.dataGridView1;
                dgv.BorderStyle = BorderStyle.None;
                dgv.BackgroundColor = Color.White;
                dgv.EnableHeadersVisualStyles = false;
                dgv.GridColor = BorderColor;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgv.RowHeadersVisible = false;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Header styling
                dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10.5F);
                dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
                dgv.ColumnHeadersHeight = 45;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                // Cell styling
                dgv.DefaultCellStyle.BackColor = Color.White;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
                dgv.DefaultCellStyle.ForeColor = TextColor;
                dgv.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 244);
                dgv.DefaultCellStyle.SelectionForeColor = TextColor;

                // Alternating row style
                dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternateRowColor;
                dgv.AlternatingRowsDefaultCellStyle.Font = new Font("Segoe UI", 10F);
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextColor;
                dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 244);
                dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextColor;
                dgv.RowTemplate.Height = 40;

                // Style all TextBoxes
                StyleTextBox(userControl.addUsers_username);
                StyleTextBox(userControl.addUsers_password);

                // Style ComboBoxes
                StyleComboBox(userControl.addUsers_role);
                StyleComboBox(userControl.addUsers_status);

                // Style all buttons
                StyleButton(userControl.addUsers_addBtn, "Add");
                StyleButton(userControl.addUsers_updateBtn, "Update");
                StyleButton(userControl.addUsers_removeBtn, "Remove");
                StyleButton(userControl.addUsers_clearBtn, "Clear");

                // Add hover effect for DataGridView
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

                // TextBox focus effects
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

                // Add hover effect
                button.MouseEnter += (s, e) => {
                    button.BackColor = Color.FromArgb(0, 150, 150); // Darker shade of PrimaryColor
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
            displayAllUsersData();
        }

        public void displayAllUsersData()
        {
            UsersData uData = new UsersData();

            List<UsersData> listData = uData.AllUsersData();

            dataGridView1.DataSource = listData;
        }

        private void addUsers_addBtn_Click(object sender, EventArgs e)
        {
            if (addUsers_username.Text == "" || addUsers_password.Text == "" || addUsers_role.SelectedIndex == -1
                || addUsers_status.SelectedIndex == -1)
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

                        string checkUsername = "SELECT * FROM users WHERE username = @usern";

                        using (SqlCommand cmd = new SqlCommand(checkUsername, connect))
                        {
                            cmd.Parameters.AddWithValue("@usern", addUsers_username.Text.Trim());

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                MessageBox.Show(addUsers_username.Text.Trim()
                                    + " is already used", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else if (addUsers_password.Text.Length < 8)
                            {
                                MessageBox.Show("Password must be at least 8 characters long",
                                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                // Hash the password before storing
                                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                                    addUsers_password.Text.Trim(),
                                    BCrypt.Net.BCrypt.GenerateSalt(12)
                                );

                                string insertData = "INSERT INTO users (username, password, role, status, date) VALUES(@usern, @pass, @role, @status, @date)";
                                using (SqlCommand insertD = new SqlCommand(insertData, connect))
                                {
                                    insertD.Parameters.AddWithValue("@usern", addUsers_username.Text.Trim());
                                    insertD.Parameters.AddWithValue("@pass", hashedPassword);
                                    insertD.Parameters.AddWithValue("@role", addUsers_role.SelectedItem.ToString());
                                    insertD.Parameters.AddWithValue("@status", addUsers_status.SelectedItem.ToString());
                                    insertD.Parameters.AddWithValue("@date", DateTime.Today);

                                    insertD.ExecuteNonQuery();
                                    clearFields();
                                    displayAllUsersData();

                                    MessageBox.Show("Added Successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"SQL Error: {ex.Message}\nError Number: {ex.Number}\nError State: {ex.State}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"General Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            addUsers_username.Text = "";
            addUsers_password.Text = "";
            addUsers_role.SelectedIndex = -1;
            addUsers_status.SelectedIndex = -1;
        }
        private void addUsers_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void addUsers_updateBtn_Click(object sender, EventArgs e)
        {
            if (addUsers_username.Text == "" || addUsers_password.Text == "" || addUsers_role.SelectedIndex == -1
                || addUsers_status.SelectedIndex == -1)
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to update user Id: " + getID + "?", "Confirmation Message"
                   , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();

                            // Check if the password has been changed
                            string currentPassword = "";
                            using (SqlCommand getPass = new SqlCommand("SELECT password FROM users WHERE id = @id", connect))
                            {
                                getPass.Parameters.AddWithValue("@id", getID);
                                currentPassword = getPass.ExecuteScalar()?.ToString() ?? "";
                            }

                            string newPassword = addUsers_password.Text.Trim();
                            bool isPasswordChanged = currentPassword != newPassword;

                            // If password changed, validate length and hash it
                            if (isPasswordChanged)
                            {
                                if (newPassword.Length < 8)
                                {
                                    MessageBox.Show("Password must be at least 8 characters long",
                                        "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                newPassword = BCrypt.Net.BCrypt.HashPassword(
                                    newPassword,
                                    BCrypt.Net.BCrypt.GenerateSalt(12)
                                );
                            }
                            else
                            {
                                // If password wasn't changed, use the existing hash
                                newPassword = currentPassword;
                            }

                            string updateData = "UPDATE users SET username = @usern, password = @pass, role = @role, status = @status WHERE id = @id";

                            using (SqlCommand updateD = new SqlCommand(updateData, connect))
                            {
                                updateD.Parameters.AddWithValue("@usern", addUsers_username.Text.Trim());
                                updateD.Parameters.AddWithValue("@pass", newPassword);
                                updateD.Parameters.AddWithValue("@role", addUsers_role.SelectedItem);
                                updateD.Parameters.AddWithValue("@status", addUsers_status.SelectedItem);
                                updateD.Parameters.AddWithValue("@id", getID);

                                updateD.ExecuteNonQuery();
                                clearFields();
                                displayAllUsersData();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show($"SQL Error: {ex.Message}\nError Number: {ex.Number}\nError State: {ex.State}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"General Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                }
            }
        }

        private int getID = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                getID = (int)row.Cells[0].Value;
                string username = row.Cells[1].Value.ToString();
                string password = row.Cells[2].Value.ToString();
                string role = row.Cells[3].Value.ToString();
                string status = row.Cells[4].Value.ToString();

                addUsers_username.Text = username;
                addUsers_password.Text = password;
                addUsers_role.Text = role;
                addUsers_status.Text = status;
            }
        }

        private void addUsers_removeBtn_Click(object sender, EventArgs e)
        {
            if (addUsers_username.Text == "" || addUsers_password.Text == "" || addUsers_role.SelectedIndex == -1
                || addUsers_status.SelectedIndex == -1)
            {
                MessageBox.Show("Empty Fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to REMOVE user Id: " + getID + "?", "Confirmation Message"
                   , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (checkConnection())
                    {
                        try
                        {
                            connect.Open();


                            string updateData = "DELETE FROM users WHERE id = @id";

                            using (SqlCommand updateD = new SqlCommand(updateData, connect))
                            {
                                updateD.Parameters.AddWithValue("@id", getID);
                                updateD.ExecuteNonQuery();
                                
                                clearFields();
                                displayAllUsersData();

                                MessageBox.Show("Removed succesfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (SqlException ex)
                        {
                            // Handle the specific SQL exception
                            MessageBox.Show($"SQL Error: {ex.Message}\nError Number: {ex.Number}\nError State: {ex.State}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            // Handle any other general exceptions
                            MessageBox.Show($"General Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
