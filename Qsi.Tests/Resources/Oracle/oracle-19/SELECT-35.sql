-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/SELECT.html
SELECT channel_desc, calendar_month_desc, co.country_id,
      TO_CHAR(sum(amount_sold) , '9,999,999,999') SALES$
   FROM sales, customers, times, channels, countries co
   WHERE sales.time_id=times.time_id 
      AND sales.cust_id=customers.cust_id 
      AND sales.channel_id= channels.channel_id 
      AND customers.country_id = co.country_id
      AND channels.channel_desc IN ('Direct Sales', 'Internet') 
      AND times.calendar_month_desc IN ('2000-09', '2000-10')
      AND co.country_iso_code IN ('UK', 'US')
  GROUP BY GROUPING SETS( 
      (channel_desc, calendar_month_desc, co.country_id), 
      (channel_desc, co.country_id), 
      (calendar_month_desc, co.country_id) );