-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/CREATE-DATABASE.html
CREATE DATABASE sample
   CONTROLFILE REUSE 
   LOGFILE
      GROUP 1 ('diskx:log1.log', 'disky:log1.log') SIZE 50K, 
      GROUP 2 ('diskx:log2.log', 'disky:log2.log') SIZE 50K 
   MAXLOGFILES 5 
   MAXLOGHISTORY 100 
   MAXDATAFILES 10 
   MAXINSTANCES 2 
   ARCHIVELOG 
   CHARACTER SET AL32UTF8
   NATIONAL CHARACTER SET AL16UTF16
   DATAFILE  
      'disk1:df1.dbf' AUTOEXTEND ON,
      'disk2:df2.dbf' AUTOEXTEND ON NEXT 10M MAXSIZE UNLIMITED
   DEFAULT TEMPORARY TABLESPACE temp_ts
   UNDO TABLESPACE undo_ts 
   SET TIME_ZONE = '+02:00';