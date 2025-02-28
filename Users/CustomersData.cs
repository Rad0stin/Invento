using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Invento
{
    class CustomersData
    {
        SqlConnection connect = Database.GetConnection();
        public string CustomerID { set; get; }
        public string TotalPrice { set; get; }
        public string Amount { set; get; }
        public string Change { set; get; }
        public string Date { set; get; }

        public List<CustomersData> allCustomers()
        {
            List<CustomersData> listData = new List<CustomersData>();
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT * FROM customers";
                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            CustomersData cData = new CustomersData();
                            cData.CustomerID = reader["customer_id"].ToString();
                            cData.TotalPrice = reader["total_price"].ToString();
                            cData.Amount = reader["amount"].ToString();
                            cData.Change = reader["change"].ToString();

                            if (DateTime.TryParse(reader["order_date"].ToString(), out DateTime orderDate))
                            {
                                cData.Date = orderDate.ToString("MM/dd/yyyy HH:mm");
                            }
                            else
                            {
                                cData.Date = reader["order_date"].ToString();
                            }

                            listData.Add(cData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listData;
        }

        public List<CustomersData> allTodayCustomers()
        {
            List<CustomersData> listData = new List<CustomersData>();
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    DateTime dateValue;
                    string formattedDate = DateTime.TryParse(DateTime.Now.ToString(), out dateValue)
                        ? dateValue.ToString("MM/dd/yyyy HH:mm")
                        : DateTime.Now.ToString();

                    string selectData = "SELECT * FROM customers WHERE CONVERT(VARCHAR(10), order_date, 101) = CONVERT(VARCHAR(10), @date, 101)";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@date", formattedDate);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            CustomersData cData = new CustomersData();
                            cData.CustomerID = reader["customer_id"].ToString();
                            cData.TotalPrice = reader["total_price"].ToString();
                            cData.Amount = reader["amount"].ToString();
                            cData.Change = reader["change"].ToString();

                            if (DateTime.TryParse(reader["order_date"].ToString(), out DateTime orderDate))
                            {
                                cData.Date = orderDate.ToString("MM/dd/yyyy HH:mm");
                            }
                            else
                            {
                                cData.Date = reader["order_date"].ToString();
                            }

                            listData.Add(cData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listData;
        }
    }
}