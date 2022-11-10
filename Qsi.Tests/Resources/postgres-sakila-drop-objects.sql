-- Drop Views

DROP VIEW IF EXISTS customer_list;
DROP VIEW IF EXISTS film_list;
DROP VIEW IF EXISTS nicer_but_slower_film_list;
DROP VIEW IF EXISTS sales_by_film_category;
DROP VIEW IF EXISTS sales_by_store;
DROP VIEW IF EXISTS staff_list;

-- Drop Tables


DROP TABLE IF EXISTS payment CASCADE;
DROP TABLE IF EXISTS rental CASCADE;
DROP TABLE IF EXISTS inventory CASCADE;
DROP TABLE IF EXISTS film_category CASCADE;
DROP TABLE IF EXISTS film_actor CASCADE;
DROP TABLE IF EXISTS film CASCADE;
DROP TABLE IF EXISTS language CASCADE;
DROP TABLE IF EXISTS customer CASCADE;
DROP TABLE IF EXISTS actor CASCADE;
DROP TABLE IF EXISTS category CASCADE;
DROP TABLE IF EXISTS store CASCADE;
DROP TABLE IF EXISTS address CASCADE;
DROP TABLE IF EXISTS staff CASCADE;
DROP TABLE IF EXISTS city CASCADE;
DROP TABLE IF EXISTS country CASCADE;

--Procedures

DROP FUNCTION IF EXISTS film_in_stock(integer, integer);
DROP FUNCTION IF EXISTS film_not_in_stock(integer, integer);
DROP FUNCTION IF EXISTS get_customer_balance(integer, timestamp without time zone);
DROP FUNCTION IF EXISTS inventory_held_by_customer(integer);
DROP FUNCTION IF EXISTS inventory_in_stock(integer);
DROP FUNCTION IF EXISTS last_day(timestamp without time zone);
DROP FUNCTION IF EXISTS rewards_report(integer, numeric);
DROP FUNCTION IF EXISTS last_updated();
DROP FUNCTION IF EXISTS _group_concat(text, text) CASCADE;

-- DROP SEQUENCES
DROP SEQUENCE IF EXISTS actor_actor_id_seq;
DROP SEQUENCE IF EXISTS address_address_id_seq;
DROP SEQUENCE IF EXISTS category_category_id_seq;
DROP SEQUENCE IF EXISTS city_city_id_seq;
DROP SEQUENCE IF EXISTS country_country_id_seq;
DROP SEQUENCE IF EXISTS customer_customer_id_seq;
DROP SEQUENCE IF EXISTS film_film_id_seq;
DROP SEQUENCE IF EXISTS inventory_inventory_id_seq;
DROP SEQUENCE IF EXISTS language_language_id_seq;
DROP SEQUENCE IF EXISTS payment_payment_id_seq;
DROP SEQUENCE IF EXISTS rental_rental_id_seq;
DROP SEQUENCE IF EXISTS staff_staff_id_seq;
DROP SEQUENCE IF EXISTS store_store_id_seq;

DROP DOMAIN IF EXISTS year;

-- DROP TYPES
DROP TYPE IF EXISTS mpaa_rating;