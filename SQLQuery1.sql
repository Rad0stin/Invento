CREATE TABLE users
(
 id int PRIMARY KEY IDENTITY(1,1),
 username VARCHAR(MAX) NULL,
 password VARCHAR(MAX) NULL,
 role VARCHAR(MAX) NULL,
 status VARCHAR(MAX) NULL,
 date DATE NULL,
 profile_picture NVARCHAR(MAX) NULL
)


CREATE TABLE categories 
(
    id int PRIMARY KEY IDENTITY(1,1),
    category VARCHAR(MAX) NULL, 
    date DATE NULL
);

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
);

CREATE TABLE orders
(
    id int PRIMARY KEY IDENTITY(1,1),
    prod_id VARCHAR(MAX) NULL,
    prod_name VARCHAR(MAX) NULL,
    category VARCHAR(MAX) NULL,
    qty INT NULL,
    orig_price FLOAT NULL,
    total_price FLOAT NULL,
    order_date DATE NULL,
    customer_id INT NULL
);

CREATE TABLE customers
(
    id int PRIMARY KEY IDENTITY(1,1),
    customer_id INT NULL,
    total_price FLOAT NULL,
    amount FLOAT NULL,
    change FLOAT NULL, 
    order_date DATE NULL
);