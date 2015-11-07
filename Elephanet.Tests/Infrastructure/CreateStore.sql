DROP DATABASE IF EXISTS elephanet_tests_store;
DROP USER  IF EXISTS elephanet_tests_user;

CREATE USER elephanet_tests_user with PASSWORD 'my super secret password';
CREATE DATABASE elephanet_tests_store;
GRANT ALL PRIVILEGES ON DATABASE elephanet_tests_store to elephanet_tests_user;
