CREATE USER store_user with PASSWORD 'my super secret password';
CREATE DATABASE store;
GRANT ALL PRIVILEGES ON DATABASE store to store_user;
