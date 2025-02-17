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
    class OrdersData
    {
        SqlConnection connect = Database.GetConnection();
        public int ID { set; get; }
        public string CID { set; get; }
        public string PID { set; get; }
        public string PName { set; get; }
        public string Category { set; get; }
        public string OrigPrice { set; get; }
        public string QTY { set; get; }
        public string TotalPrice { set; get; }

        private static int currentCustomerId = -1; // Store the current session's customer ID

        // Method to set the current customer ID
        public static void SetCurrentCustomerId(int id)
        {
            currentCustomerId = id;
        }

        public List<OrdersData> allOrdersData()
        {
            List<OrdersData> listData = new List<OrdersData>();
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    // Only get orders for the current session's customer ID
                    string selectData = "SELECT * FROM orders WHERE customer_id = @cID";
                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@cID", currentCustomerId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrdersData oData = new OrdersData();
                                oData.ID = (int)reader["id"];
                                oData.CID = reader["customer_id"].ToString();
                                oData.PID = reader["prod_id"].ToString();
                                oData.PName = reader["prod_name"].ToString();
                                oData.Category = reader["category"].ToString();
                                oData.OrigPrice = reader["orig_price"].ToString();
                                oData.QTY = reader["qty"].ToString();
                                oData.TotalPrice = reader["total_price"].ToString();
                                listData.Add(oData);
                            }
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