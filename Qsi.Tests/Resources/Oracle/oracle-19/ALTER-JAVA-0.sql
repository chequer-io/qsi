-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/ALTER-JAVA.html
ALTER JAVA CLASS "Agent"
   RESOLVER (("/usr/bin/bfile_dir/*" pm)(* public))
   RESOLVE;