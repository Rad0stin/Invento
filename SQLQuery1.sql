CREATE TABLE users
(
 id int PRIMARY KEY IDENTITY(1,1),
 username VARCHAR(MAX) NULL,
 password VARCHAR(MAX) NULL,
 role VARCHAR(MAX) NULL,
 status VARCHAR(MAX) NULL,
 date DATE NULL
)

INSERT INTO users (username, password, role, status) VALUES('rado', 'test1234', 'Cashier', 'Active')

SELECT * FROM users

INSERT INTO users(username, password, role, status, date) VALUES('admin', 'admin123', 'Admin', 'Active', '2024-09-11')

CREATE TABLE categories 
(
   id int PRIMARY KEY IDENTITY(1,1),
   category VARCHAR(MAX) NULL, 
   date DATE NULL
)

SELECT * FROM categories

CREATE TABLE products
(
  id int PRIMARY KEY IDENTITY(1,1),
  prod_id VARCHAR(MAX) NULL, 
  prod_name VARCHAR(MAX) NULL, 
  category VARCHAR(MAX) NULL,
  price FLOAT NULL,
  stock INT NULL,
  image_path VARCHAR(MAX) NULL, 
  status VARCHAR(MAX) NULL,
  date_insert DATE NULL
)

SELECT * FROM products

CREATE TABLE orders
(
 id int PRIMARY KEY IDENTITY(1,1),
 prod_id VARCHAR(MAX) NULL,
 prod_name VARCHAR(MAX) NULL,
 category VARCHAR(MAX) NULL,
 qty INT NULL,
 orig_price FLOAT NULL,
 total_price FLOAT NULL,
 order_date DATE NULL
)

ALTER TABLE orders 
ADD customer_id INT NULL

SELECT * FROM orders

CREATE TABLE customers
(
 id int PRIMARY KEY IDENTITY(1,1),
 customer_id INT NULL,
 total_price FLOAT NULL,
 amount FLOAT NULL,
 change FLOAT NULL, 
 order_date DATE NULL
)

SELECT * FROM customers

EXEC sp_rename 'customers.ustomer_id', 'customer_id', 'COLUMN';

ALTER TABLE customers
DROP COLUMN prod_id

SELECT COUNT(id) FROM customers

UPDATE users 
SET password = '$2a$11$k7R3xZqwzUCVTqLNrhx9yOYXrulrjgkSMLBXE/h9FBw4YGtGpjKPK'
WHERE username = 'admin';

UPDATE users 
SET password = '$2a$11$BXXGgoY8UnJfkUQ3H0ET8.Q/bL9D9WGHBrRxn4kxuUZxgqoVFkXny'
WHERE username = 'rado';
