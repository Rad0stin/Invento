CREATE TABLE users
(
 id int PRIMARY KEY IDENTITY(1,1),
 username VARCHAR(MAX) NULL,
 password VARCHAR(MAX) NULL,
 role VARCHAR(MAX) NULL,
 status VARCHAR(MAX) NULL,
 date DATE NULL
)

SELECT * FROM users

INSERT INTO users(username, password, role, status, date) VALUES('admin123', 'admin123', 'Admin', 'Active', '2024-09-11')

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