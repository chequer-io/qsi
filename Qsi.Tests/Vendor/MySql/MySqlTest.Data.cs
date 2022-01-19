using NUnit.Framework;

namespace Qsi.Tests.Vendor.MySql;

public partial class MySqlTest
{
    private static readonly TestCaseData[] Print_TestDatas =
    {
        new("INSERT INTO actor (first_name, last_name) VALUES ('MORRIS', 'BABO')")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|          qsi_unit_tests.actor - INSERT          |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| default  | MORRIS     | BABO      | default     |
+-------------------------------------------------+
"
        },

        new("INSERT INTO actor SET first_name = 'MORRIS', last_name = 'BABO'")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|          qsi_unit_tests.actor - INSERT          |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| default  | MORRIS     | BABO      | default     |
+-------------------------------------------------+
"
        },

        new("INSERT INTO actor SELECT 1, 'MORRIS', 'BABO', null")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|          qsi_unit_tests.actor - INSERT          |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| 1        | MORRIS     | BABO      | null        |
+-------------------------------------------------+
"
        },

        new("INSERT INTO actor (actor_id, last_name) SELECT city_id, city FROM city LIMIT 2")
        {
            ExpectedResult = @"
+----------------------------------------------------------+
|              qsi_unit_tests.actor - INSERT               |
+----------+------------+--------------------+-------------+
| actor_id | first_name |     last_name      | last_update |
+----------+------------+--------------------+-------------+
| 1        | default    | A Corua (La Corua) | default     |
| 2        | default    | Abha               | default     |
+----------------------------------------------------------+
"
        },

        new("UPDATE actor SET first_name = 'MORRIS', last_name = 'BABO' WHERE actor_id = 1")
        {
        ExpectedResult = @"
+-------------------------------------------------+
|      qsi_unit_tests.actor - UPDATE_BEFORE       |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unknown  | PENELOPE   | GUINESS   | unknown     |
+-------------------------------------------------+

+-------------------------------------------------+
|       qsi_unit_tests.actor - UPDATE_AFTER       |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unset    | MORRIS     | BABO      | unset       |
+-------------------------------------------------+
"
        },

        new("UPDATE city, actor SET first_name = 'MORRIS', last_name = 'BABO', city_id = 2 WHERE actor_id = 1 LIMIT 1")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|      qsi_unit_tests.actor - UPDATE_BEFORE       |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unknown  | PENELOPE   | GUINESS   | unknown     |
+-------------------------------------------------+

+-------------------------------------------------+
|       qsi_unit_tests.actor - UPDATE_AFTER       |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unset    | MORRIS     | BABO      | unset       |
+-------------------------------------------------+

+----------------------------------------------+
|     qsi_unit_tests.city - UPDATE_BEFORE      |
+---------+---------+------------+-------------+
| city_id |  city   | country_id | last_update |
+---------+---------+------------+-------------+
| 1       | unknown | unknown    | unknown     |
+----------------------------------------------+

+--------------------------------------------+
|     qsi_unit_tests.city - UPDATE_AFTER     |
+---------+-------+------------+-------------+
| city_id | city  | country_id | last_update |
+---------+-------+------------+-------------+
| 2       | unset | unset      | unset       |
+--------------------------------------------+
"
        },

        new("UPDATE film_list SET title = 'EVAN', category = 'CHEQUER' WHERE FID = 1")
        {
            ExpectedResult = @"
+------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
|                                                                                 qsi_unit_tests.film - UPDATE_BEFORE                                                                                  |
+---------+------------------+-------------+--------------+-------------+----------------------+-----------------+-------------+---------+------------------+---------+------------------+-------------+
| film_id |      title       | description | release_year | language_id | original_language_id | rental_duration | rental_rate | length  | replacement_cost | rating  | special_features | last_update |
+---------+------------------+-------------+--------------+-------------+----------------------+-----------------+-------------+---------+------------------+---------+------------------+-------------+
| unknown | ACADEMY DINOSAUR | unknown     | unknown      | unknown     | unknown              | unknown         | unknown     | unknown | unknown          | unknown | unknown          | unknown     |
+------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+

+-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
|                                                                           qsi_unit_tests.film - UPDATE_AFTER                                                                            |
+---------+-------+-------------+--------------+-------------+----------------------+-----------------+-------------+--------+------------------+--------+------------------+-------------+
| film_id | title | description | release_year | language_id | original_language_id | rental_duration | rental_rate | length | replacement_cost | rating | special_features | last_update |
+---------+-------+-------------+--------------+-------------+----------------------+-----------------+-------------+--------+------------------+--------+------------------+-------------+
| unset   | EVAN  | unset       | unset        | unset       | unset                | unset           | unset       | unset  | unset            | unset  | unset            | unset       |
+-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+

+-----------------------------------------+
| qsi_unit_tests.category - UPDATE_BEFORE |
+-------------+-------------+-------------+
| category_id |    name     | last_update |
+-------------+-------------+-------------+
| unknown     | Documentary | unknown     |
+-----------------------------------------+

+----------------------------------------+
| qsi_unit_tests.category - UPDATE_AFTER |
+--------------+----------+--------------+
| category_id  |   name   | last_update  |
+--------------+----------+--------------+
| unset        | CHEQUER  | unset        |
+----------------------------------------+
"
        },

        new("WITH CTE AS (SELECT 'MORRIS' a, 'BABO' b) UPDATE actor SET first_name = (SELECT a FROM CTE) + '!'")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|      qsi_unit_tests.actor - UPDATE_BEFORE       |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unknown  | PENELOPE   | unknown   | unknown     |
| unknown  | NICK       | unknown   | unknown     |
| unknown  | ED         | unknown   | unknown     |
| unknown  | JENNIFER   | unknown   | unknown     |
| unknown  | JOHNNY     | unknown   | unknown     |
| unknown  | BETTE      | unknown   | unknown     |
| unknown  | GRACE      | unknown   | unknown     |
| unknown  | MATTHEW    | unknown   | unknown     |
| unknown  | JOE        | unknown   | unknown     |
| unknown  | CHRISTIAN  | unknown   | unknown     |
| unknown  | ZERO       | unknown   | unknown     |
| unknown  | KARL       | unknown   | unknown     |
| unknown  | UMA        | unknown   | unknown     |
| unknown  | VIVIEN     | unknown   | unknown     |
| unknown  | CUBA       | unknown   | unknown     |
| unknown  | FRED       | unknown   | unknown     |
| unknown  | HELEN      | unknown   | unknown     |
| unknown  | DAN        | unknown   | unknown     |
| unknown  | BOB        | unknown   | unknown     |
| unknown  | LUCILLE    | unknown   | unknown     |
+-------------------------------------------------+

+---------------------------------------------------+
|        qsi_unit_tests.actor - UPDATE_AFTER        |
+----------+--------------+-----------+-------------+
| actor_id |  first_name  | last_name | last_update |
+----------+--------------+-----------+-------------+
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
| unset    | MORRIS + '!' | unset     | unset       |
+---------------------------------------------------+
"
        },

        new("DELETE first_name FROM actor_info a WHERE actor_id = 1")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|          qsi_unit_tests.actor - DELETE          |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unset    | PENELOPE   | unset     | unset       |
+-------------------------------------------------+
"
        },

        new("DELETE first_name, b FROM actor_info a, city b WHERE actor_id = city_id")
        {
            ExpectedResult = @"
+-------------------------------------------------+
|          qsi_unit_tests.actor - DELETE          |
+----------+------------+-----------+-------------+
| actor_id | first_name | last_name | last_update |
+----------+------------+-----------+-------------+
| unset    | PENELOPE   | unset     | unset       |
| unset    | NICK       | unset     | unset       |
| unset    | ED         | unset     | unset       |
| unset    | JENNIFER   | unset     | unset       |
| unset    | JOHNNY     | unset     | unset       |
| unset    | BETTE      | unset     | unset       |
| unset    | GRACE      | unset     | unset       |
| unset    | MATTHEW    | unset     | unset       |
| unset    | JOE        | unset     | unset       |
| unset    | CHRISTIAN  | unset     | unset       |
| unset    | ZERO       | unset     | unset       |
| unset    | KARL       | unset     | unset       |
| unset    | UMA        | unset     | unset       |
| unset    | VIVIEN     | unset     | unset       |
| unset    | CUBA       | unset     | unset       |
| unset    | FRED       | unset     | unset       |
| unset    | HELEN      | unset     | unset       |
| unset    | DAN        | unset     | unset       |
| unset    | BOB        | unset     | unset       |
| unset    | LUCILLE    | unset     | unset       |
+-------------------------------------------------+

+--------------------------------------------------------------------+
|                    qsi_unit_tests.city - DELETE                    |
+---------+-----------------------+------------+---------------------+
| city_id |         city          | country_id |     last_update     |
+---------+-----------------------+------------+---------------------+
| 1       | A Corua (La Corua)    | 87         | 02/15/2006 04:45:25 |
| 2       | Abha                  | 82         | 02/15/2006 04:45:25 |
| 3       | Abu Dhabi             | 101        | 02/15/2006 04:45:25 |
| 4       | Acua                  | 60         | 02/15/2006 04:45:25 |
| 5       | Adana                 | 97         | 02/15/2006 04:45:25 |
| 6       | Addis Abeba           | 31         | 02/15/2006 04:45:25 |
| 7       | Aden                  | 107        | 02/15/2006 04:45:25 |
| 8       | Adoni                 | 44         | 02/15/2006 04:45:25 |
| 9       | Ahmadnagar            | 44         | 02/15/2006 04:45:25 |
| 10      | Akishima              | 50         | 02/15/2006 04:45:25 |
| 11      | Akron                 | 103        | 02/15/2006 04:45:25 |
| 12      | al-Ayn                | 101        | 02/15/2006 04:45:25 |
| 13      | al-Hawiya             | 82         | 02/15/2006 04:45:25 |
| 14      | al-Manama             | 11         | 02/15/2006 04:45:25 |
| 15      | al-Qadarif            | 89         | 02/15/2006 04:45:25 |
| 16      | al-Qatif              | 82         | 02/15/2006 04:45:25 |
| 17      | Alessandria           | 49         | 02/15/2006 04:45:25 |
| 18      | Allappuzha (Alleppey) | 44         | 02/15/2006 04:45:25 |
| 19      | Allende               | 60         | 02/15/2006 04:45:25 |
| 20      | Almirante Brown       | 6          | 02/15/2006 04:45:25 |
+--------------------------------------------------------------------+
"
        },

        new("DELETE title, category FROM film_list WHERE FID = 1")
        {
            ExpectedResult = @"
+----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
|                                                                                    qsi_unit_tests.film - DELETE                                                                                    |
+---------+------------------+-------------+--------------+-------------+----------------------+-----------------+-------------+--------+------------------+--------+------------------+-------------+
| film_id |      title       | description | release_year | language_id | original_language_id | rental_duration | rental_rate | length | replacement_cost | rating | special_features | last_update |
+---------+------------------+-------------+--------------+-------------+----------------------+-----------------+-------------+--------+------------------+--------+------------------+-------------+
| unset   | ACADEMY DINOSAUR | unset       | unset        | unset       | unset                | unset           | unset       | unset  | unset            | unset  | unset            | unset       |
+----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+

+-----------------------------------------+
|    qsi_unit_tests.category - DELETE     |
+-------------+-------------+-------------+
| category_id |    name     | last_update |
+-------------+-------------+-------------+
| unset       | Documentary | unset       |
+-----------------------------------------+
"
        },

        new("WITH CTE AS (SELECT 1) DELETE title, category FROM film_list WHERE FID = 1 AND (SELECT `1` FROM CTE) = 1")
        {
            ExpectedResult = @"
+----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
|                                                                                    qsi_unit_tests.film - DELETE                                                                                    |
+---------+------------------+-------------+--------------+-------------+----------------------+-----------------+-------------+--------+------------------+--------+------------------+-------------+
| film_id |      title       | description | release_year | language_id | original_language_id | rental_duration | rental_rate | length | replacement_cost | rating | special_features | last_update |
+---------+------------------+-------------+--------------+-------------+----------------------+-----------------+-------------+--------+------------------+--------+------------------+-------------+
| unset   | ACADEMY DINOSAUR | unset       | unset        | unset       | unset                | unset           | unset       | unset  | unset            | unset  | unset            | unset       |
+----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+

+-----------------------------------------+
|    qsi_unit_tests.category - DELETE     |
+-------------+-------------+-------------+
| category_id |    name     | last_update |
+-------------+-------------+-------------+
| unset       | Documentary | unset       |
+-----------------------------------------+
"
        },
    };
}
