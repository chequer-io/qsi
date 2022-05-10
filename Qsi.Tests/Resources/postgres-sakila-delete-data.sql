-- Delete data
ALTER TABLE IF EXISTS store DROP CONSTRAINT store_manager_staff_id_fkey;
ALTER TABLE IF EXISTS staff DROP CONSTRAINT staff_address_id_fkey;
ALTER TABLE IF EXISTS staff DROP CONSTRAINT staff_store_id_fkey;

-- DELETE FROM payment;
-- DELETE FROM rental ;
-- DELETE FROM customer ;
-- DELETE FROM film_category ;
-- DELETE FROM film_actor ;
-- DELETE FROM inventory ;
-- DELETE FROM film ;
-- DELETE FROM category ;
-- DELETE FROM staff ;
-- DELETE FROM store ;
-- DELETE FROM actor ;
-- DELETE FROM address ;
-- DELETE FROM city ;
-- DELETE FROM country ;
-- DELETE FROM language ;

ALTER TABLE IF EXISTS staff ADD CONSTRAINT staff_address_id_fkey FOREIGN KEY (address_id) REFERENCES address (address_id) ON UPDATE CASCADE;
ALTER TABLE IF EXISTS staff ADD CONSTRAINT staff_store_id_fkey FOREIGN KEY (store_id) REFERENCES store (store_id) ON UPDATE CASCADE;
ALTER TABLE IF EXISTS store ADD CONSTRAINT store_manager_staff_id_fkey FOREIGN KEY (manager_staff_id) REFERENCES staff(staff_id) ON UPDATE CASCADE ON DELETE RESTRICT;
