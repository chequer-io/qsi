-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/NANVL.html
SELECT bin_float, NANVL(bin_float,0)
  FROM float_point_demo;