using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Invento
{
    public class Database
    {
        private static string dbName = "inventor";
        private static string connectionString;

        public Database()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            string masterConnection = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(masterConnection))
            {
                try
                {
                    conn.Open();

                    string checkDb = $"SELECT database_id FROM sys.databases WHERE Name = '{dbName}'";
                    using (SqlCommand cmd = new SqlCommand(checkDb, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            string createDb = $"CREATE DATABASE {dbName}";
                            using (SqlCommand createCmd = new SqlCommand(createDb, conn))
                            {
                                createCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error initializing database: {ex.Message}");
                }
            }

            connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string createUsersTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'users')
                    CREATE TABLE users (
                        id int PRIMARY KEY IDENTITY(1,1),
                        username VARCHAR(MAX) NULL,
                        password VARCHAR(MAX) NULL,
                        role VARCHAR(MAX) NULL,
                        status VARCHAR(MAX) NULL,
                        date DATETIME NULL,
                        profile_picture varbinary(MAX) NULL
                    )";

                    string createCategoriesTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'categories')
                    CREATE TABLE categories (
                        id int PRIMARY KEY IDENTITY(1,1),
                        category VARCHAR(MAX) NULL,
                        date DATETIME NULL
                    )";

                    string createProductsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'products')
                    CREATE TABLE products (
                        id int PRIMARY KEY IDENTITY(1,1),
                        prod_id VARCHAR(MAX) NULL,
                        prod_name VARCHAR(MAX) NULL,
                        category VARCHAR(MAX) NULL,
                        price FLOAT NULL,
                        stock INT NULL,
                        product_image varbinary(MAX) NULL,
                        status VARCHAR(MAX) NULL,
                        date_insert DATETIME NULL
                    )";

                    string createOrdersTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'orders')
                    CREATE TABLE orders (
                        id int PRIMARY KEY IDENTITY(1,1),
                        prod_id VARCHAR(MAX) NULL,
                        prod_name VARCHAR(MAX) NULL,
                        category VARCHAR(MAX) NULL,
                        qty INT NULL,
                        orig_price FLOAT NULL,
                        total_price FLOAT NULL,
                        order_date DATETIME NULL,
                        customer_id INT NULL
                    )";

                    string createCustomersTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'customers')
                    CREATE TABLE customers (
                        id int PRIMARY KEY IDENTITY(1,1),
                        customer_id INT NULL,
                        total_price FLOAT NULL,
                        amount FLOAT NULL,
                        change FLOAT NULL,
                        order_date DATETIME NULL
                    )";

                    string alterProductsTable = @"
                    IF EXISTS (SELECT * FROM sys.columns WHERE Name = 'image_path' AND Object_ID = Object_ID('products'))
                    BEGIN
                        -- Add new column if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'product_image' AND Object_ID = Object_ID('products'))
                        BEGIN
                            ALTER TABLE products ADD product_image varbinary(MAX) NULL;
                        END
                    END";

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = createUsersTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = createCategoriesTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = createProductsTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = alterProductsTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = createOrdersTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = createCustomersTable;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating tables: {ex.Message}");
                }
            }
        }

        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                new Database();
            }
            return new SqlConnection(connectionString);
        }
    }
}