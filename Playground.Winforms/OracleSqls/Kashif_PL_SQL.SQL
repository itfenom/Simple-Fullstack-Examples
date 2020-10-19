SET SERVEROUTPUT ON;

******************************************************
*                    Hello World                     *    
******************************************************

DECLARE 
 V_HELLO_MESSAGE VARCHAR2(25);
 
BEGIN
    V_HELLO_MESSAGE := 'Hello World!';
     DBMS_OUTPUT.PUT_LINE(V_HELLO_MESSAGE);
END;
------------------------------------------------------------------------------------------------------
******************************************************
*      FOR LOOP/BULK COLLECT/PL_SQL TABLE            *    
******************************************************

SELECT * FROM EMPLOYEES WHERE EMPLOYEE_ID = 185;

DECLARE
  TYPE EMP_TABLE_TYPE IS TABLE OF EMPLOYEES%ROWTYPE;
  EMP_TYPE EMP_TABLE_TYPE;
BEGIN
      SELECT * BULK COLLECT INTO EMP_TYPE 
      FROM EMPLOYEES;
      
      IF(EMP_TYPE.COUNT = 0) THEN
         DBMS_OUTPUT.PUT_LINE('No record(s) found');
      ELSE 
         DBMS_OUTPUT.PUT_LINE('Total Record(s) found: ' || TO_CHAR(EMP_TYPE.COUNT));
      END IF;
      
      FOR i IN EMP_TYPE.FIRST .. EMP_TYPE.LAST
         LOOP
              IF(EMP_TYPE(i).EMPLOYEE_ID = 185)THEN
                 DBMS_OUTPUT.PUT_LINE('Employee: ' || EMP_TYPE(i).FIRST_NAME || 
                                      ' earns ' || TO_CHAR(EMP_TYPE(i).SALARY, '$999,000.00') 
                                                || ' /week!');
              END IF;
         END LOOP;
END;
------------------------------------------------------------------------------------------------------
******************************************************
*      CASE STATEMENT WITH PROMPT FROM USER          *    
******************************************************
DECLARE
V_GRADE CHAR(1) := UPPER('&Grade');
V_APPRAISAL VARCHAR2(20);

BEGIN
      V_APPRAISAL := CASE V_GRADE
          WHEN 'A' THEN 'EXCELLENT'
          WHEN 'B' THEN 'VERY GOOD'
          WHEN 'C' THEN 'GOOD'
          ELSE 'NO SUCH GRADE'
      END;
      DBMS_OUTPUT.put_line(V_APPRAISAL);
END;
------------------------------------------------------------------------------------------------------
******************************************************
*               IF-THEN-ELSE-IF                      *    
******************************************************
DECLARE
      grade CHAR(1);
BEGIN
      grade := 'F';
        IF grade = 'A' THEN
            DBMS_OUTPUT.PUT_LINE('Excellent');
        ELSIF grade = 'B' THEN
            DBMS_OUTPUT.PUT_LINE('Very Good');
        ELSIF grade = 'C' THEN
            DBMS_OUTPUT.PUT_LINE('Good');
        ELSIF grade = 'D' THEN
          DBMS_OUTPUT. PUT_LINE('Fair');
        ELSIF grade = 'F' THEN
            DBMS_OUTPUT.PUT_LINE('Poor');
        ELSE
            DBMS_OUTPUT.PUT_LINE('No such grade');
        END IF;
END;
------------------------------------------------------------------------------------------------------
******************************************************
*          case statement - II                       *    
******************************************************
DECLARE
    grade CHAR(1);
BEGIN
    grade := 'B';
    CASE grade
            WHEN 'A' THEN DBMS_OUTPUT.PUT_LINE('Excellent');
            WHEN 'B' THEN DBMS_OUTPUT.PUT_LINE('Very Good');
            WHEN 'C' THEN DBMS_OUTPUT.PUT_LINE('Good');
            WHEN 'D' THEN DBMS_OUTPUT.PUT_LINE('Fair');
            WHEN 'F' THEN DBMS_OUTPUT.PUT_LINE('Poor');
            ELSE DBMS_OUTPUT.PUT_LINE('No such grade');
    END CASE;
END;
------------------------------------------------------------------------------------------------------
******************************************************
*          BASIC LOOP WITH EXIT STATEMENT            *    
******************************************************
DECLARE
      x NUMBER := 0;
BEGIN
      LOOP
          DBMS_OUTPUT.PUT_LINE ('Inside loop: x = ' || TO_CHAR(x));
          x := x + 1;
            IF x > 3 THEN
            	EXIT;
            END IF;
      END LOOP;
-- After EXIT, control resumes here
DBMS_OUTPUT.PUT_LINE(' After loop: x = ' || TO_CHAR(x));
END;
------------------------------------------------------------------------------------------------------
******************************************************
*          BASIC LOOP WITH CONTINUE STATEMENT        *    
******************************************************
DECLARE
    x NUMBER := 0;
BEGIN
      LOOP -- After CONTINUE statement, control resumes here
          DBMS_OUTPUT.PUT_LINE ('Inside loop: x = ' || TO_CHAR(x));
          x := x + 1;
            IF x < 3 THEN
             CONTINUE;
            END IF;
          DBMS_OUTPUT.PUT_LINE
          ('Inside loop, after CONTINUE: x = ' || TO_CHAR(x));
          EXIT WHEN x = 5;
      END LOOP;
      
      DBMS_OUTPUT.PUT_LINE (' After loop: x = ' || TO_CHAR(x));
END;
------------------------------------------------------------------------------------------------------
******************************************************
*      FOR LOOP WITH GOTO STATEMENT/LABEL            *    
******************************************************
DECLARE
      V_MESSAGE VARCHAR2(100);
BEGIN
      FOR i IN 1..20
          LOOP
              IF(i MOD 2 = 0) THEN
                 V_MESSAGE := '';
                 V_MESSAGE := i || ' is EVEN!';
                 GOTO print_now;
              ELSE 
                 V_MESSAGE := '';
                 V_MESSAGE := i || ' is ODD!';
                 GOTO print_now;
              END IF;
                <<print_now>> 
                DBMS_OUTPUT.PUT_LINE(V_MESSAGE);
          END LOOP;
END;
------------------------------------------------------------------------------------------------------
******************************************************
*       ASSOCIATIVE ARRAYS INDEXED BY varchar	     *    
******************************************************
DECLARE
        TYPE POPULATION_TYPE IS TABLE OF NUMBER INDEX BY VARCHAR2(100);
        POPULATION POPULATION_TYPE;
        i VARCHAR2(100);
BEGIN
        POPULATION('DALLAS') := 5000;
        POPULATION('HOUSTON') := 6000;
        POPULATION('SAN ANTONIO') := 2000;
        POPULATION('ARKANSAS') := 9000;
        POPULATION('SANTA FE') := 15000;
        
        i := POPULATION.FIRST;
        
        WHILE i IS NOT NULL
            LOOP
            DBMS_Output.PUT_LINE('Population of ' || i || ' is ' ||POPULATION(i));
            i := POPULATION.NEXT(i);
            END LOOP;
END;

Population of ARKANSAS is 9000
Population of DALLAS is 5000
Population of HOUSTON is 6000
Population of SAN ANTONIO is 2000
Population of SANTA FE is 15000

Notice those city names are indexed by names and not by number as being the values.


------------------------------------------------------------------------------------------------------
*************************************************************
* ARRAYS OF STRING (PRE_DEFINED IN DECLARATION SECTION)     *    
*************************************************************
DECLARE
      TYPE CITY_NAMES IS TABLE OF VARCHAR2(20);
      CITY CITY_NAMES := CITY_NAMES('DALLAS', 'HOUSTON', 'SAN ANTONIO', 'WACO', 'EL PASO');
BEGIN
        DBMS_Output.PUT_LINE('Total records in arry: ' || CITY.COUNT);
        
        FOR i IN CITY.FIRST .. CITY.LAST
            LOOP
                  DBMS_Output.PUT_LINE('City at index: ' || i || ' is ' || TO_CHAR(CITY(i)));
            END LOOP;
END;

Total records in arry: 5
City at index: 1 is DALLAS
City at index: 2 is HOUSTON
City at index: 3 is SAN ANTONIO
City at index: 4 is WACO
City at index: 5 is EL PASO
------------------------------------------------------------------------------------------------------
******************************************************
* Merge Example              *    
******************************************************
DROP TABLE STUDENT;
DROP TABLE STUDENT_N;

CREATE TABLE STUDENT
(
  ID NUMBER PRIMARY KEY NOT NULL,
  NAME VARCHAR2(20) NOT NULL,
  SCORE NUMBER
);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'Thomas', NULL);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'Rose', NULL);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'Ruth', 705);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'William', 650);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'Calderon', 620);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'Fabrizio', 600);

INSERT INTO STUDENT(ID, NAME, SCORE)
VALUES((SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT), 'Jack', 540);

COMMIT;

SELECT * FROM STUDENT;

---- Case: We have another table which contains Student data that needed to be merged with STUDENT table.

CREATE TABLE STUDENT_N
(
  ID NUMBER PRIMARY KEY NOT NULL,
  NAME VARCHAR2(20) NOT NULL,
  SCORE NUMBER
);

INSERT INTO STUDENT_N(ID, NAME, SCORE)
VALUES(7 , 'Jack', 900);

INSERT INTO STUDENT_N(ID, NAME, SCORE)
VALUES(2, 'Rose', 600);

INSERT INTO STUDENT_N(ID, NAME, SCORE)
VALUES(8, 'Spicer', 620);

INSERT INTO STUDENT_N(ID, NAME, SCORE)
VALUES(9, 'Wallace', 600);

INSERT INTO STUDENT_N(ID, NAME, SCORE)
VALUES(10, 'Brock', 600);

INSERT INTO STUDENT_N(ID, NAME, SCORE)
VALUES(11, 'Lizzy', null);

COMMIT;

SELECT * FROM STUDENT_N;

MERGE INTO STUDENT O
USING
    (
      SELECT * FROM STUDENT_N 
    )X
ON (O.ID = X.ID)
WHEN MATCHED THEN
 UPDATE SET O.NAME = X.NAME, O.SCORE = X.SCORE
WHEN NOT MATCHED THEN
INSERT (O.ID, O.NAME, O.SCORE)
VALUES(X.ID, X.NAME, X.SCORE);

SELECT * FROM STUDENT ORDER BY ID;
--------------------------------------------------
--GROUP BY WITH COUNT QUERY:
SELECT ASSIGNMENT_ID, COUNT(*)
FROM BAWUDV_GRP_RT_OP
WHERE ST_RT_SAN IS NOT NULL
AND ST_RT_SAN = 3872
GROUP BY ASSIGNMENT_ID
ORDER BY ASSIGNMENT_ID;
---
SELECT ASSIGNMENT_ID, COUNT(*)
FROM BAWUDV_GRP_RT_OP
WHERE ST_RT_SAN IS NOT NULL
GROUP BY ASSIGNMENT_ID
ORDER BY ASSIGNMENT_ID;
---
SELECT COUNT(*) FROM BAWUDV_GRP_RT_OP WHERE ASSIGNMENT_ID = 221;
---------------------------------------------------------------------------------------------------- 
 Find Duplicate query	
*****************************
SELECT XX.* 
FROM 
(SELECT A.ROUTE_ID, A.ROUTE, A.OPER, COUNT(*) COUNT_X
FROM SR_ROUTE_OPER_ORDER A
GROUP BY A.ROUTE_ID, A.ROUTE, A.OPER
ORDER BY A.ROUTE_ID DESC)XX
WHERE XX.COUNT_X > 1;
-----------------------------------------------------
--cte:
        WITH DEPENDENT_JOBS(SOURCE, DEPENDENT)AS
        (
          SELECT SOURCE_JOB_ID, DEPENDENT_JOB_ID
          FROM BAW_SVRLOG_DEPENDENCY
          WHERE SOURCE_JOB_ID = 771
          
          UNION ALL
          
          SELECT DEPENDENT_JOBS.SOURCE, DEPENDENT_JOB_ID
          FROM BAW_SVRLOG_DEPENDENCY
          INNER JOIN DEPENDENT_JOBS 
          ON BAW_SVRLOG_DEPENDENCY.SOURCE_JOB_ID = DEPENDENT_JOBS.DEPENDENT
        )
        
        SELECT * FROM DEPENDENT_JOBS WHERE DEPENDENT_JOBS.SOURCE = 771;
----------------------------------------------------------
--CTE:
WITH temp (n, fact) AS 
(SELECT 0, 1 FROM DUAL -- Initial Subquery 
 
 UNION ALL 
 
 SELECT n + 1, (n+1) * fact FROM temp -- Recursive Subquery 
 WHERE n < 9) 
 SELECT * FROM temp;
 -------------------------------------------------
 --LISTAGG:
SELECT B.ID,
       B.INITIATOR,
       M.TITLE,
       R.ROUTE,
       R.OPER,
       R.TOOLS_LIST,
       R.RECIPE_LIST
FROM BUSINESS_PROCESS B
    INNER JOIN SR_MASTER M
    ON B.ID = M.ID
    INNER JOIN 
        (
          SELECT SWR_NUMBER, 
                 ROUTE, 
                 OPER,
                 LISTAGG(T.TOOLID, ',') WITHIN GROUP (ORDER BY T.TOOLID) AS TOOLS_LIST,
                 LISTAGG(T.TOOLID || '|' || T.RECIPE, ',') WITHIN GROUP (ORDER BY T.TOOLID) AS RECIPE_LIST
          FROM RECIPEDATA.RECIPE_MANAGER T
          GROUP BY T.SWR_NUMBER, ROUTE, OPER
        )R
    ON B.ID = R.SWR_NUMBER
WHERE UPPER(B.STATE) = 'ACTIVATED'
AND M.E_SWR = 'Y'
ORDER BY B.ID DESC;
------------------------------------------------
--PL/SQL table of record collection
DECLARE
    TYPE T_SWR_RECORD IS RECORD(ID NUMBER, INITIATOR VARCHAR2(50), REQUESTOR VARCHAR2(50));   
    TYPE T_SWR_TABLE IS TABLE OF T_SWR_RECORD INDEX BY PLS_INTEGER;
    V_SWR T_SWR_TABLE;
    V_INDEX NUMBER;
BEGIN
    V_INDEX := 1;
    FOR SR_REC IN 
    (
      SELECT A.ID, 
             B.INITIATOR, 
             B.REQUESTOR
      FROM SR_MASTER A 
      INNER JOIN BUSINESS_PROCESS B
      ON A.ID = B.ID
      WHERE A.E_SWR = 'Y' 
      ORDER BY A.ID
    )
    LOOP
      V_SWR(V_INDEX).ID := SR_REC.ID;
      V_SWR(V_INDEX).INITIATOR := SR_REC.INITIATOR;
      V_SWR(V_INDEX).REQUESTOR := SR_REC.REQUESTOR;
      V_INDEX := V_INDEX + 1;
      
      DBMS_OUTPUT.PUT_LINE('ID: ' || TO_CHAR(SR_REC.ID) || ' Initiator: ' || SR_REC.INITIATOR || ' Requestor: ' || SR_REC.REQUESTOR);
    
    END LOOP;    
    DBMS_OUTPUT.PUT_LINE('eSWR count: ' || TO_CHAR(V_SWR.COUNT));
END;
--PL/SQL table of record collection with alternative loop iteration
SET SERVEROUTPUT ON;
DECLARE
    TYPE T_SWR_RECORD IS RECORD(ID NUMBER, INITIATOR VARCHAR2(50), REQUESTOR VARCHAR2(50));   
    TYPE T_SWR_TABLE IS TABLE OF T_SWR_RECORD INDEX BY PLS_INTEGER;
    V_SWR T_SWR_TABLE;
    V_KEY NUMBER;
BEGIN

    V_SWR(1).ID := 101;
    V_SWR(1).INITIATOR := 'test-initiator-101';
    V_SWR(1).REQUESTOR := 'test-requestor-101';
    
    V_SWR(2).ID := 102;
    V_SWR(2).INITIATOR := 'test-initiator-102';
    V_SWR(2).REQUESTOR := 'test-requestor-102';
    
    V_SWR(3).ID := 103;
    V_SWR(3).INITIATOR := 'test-initiator-103';
    V_SWR(3).REQUESTOR := 'test-requestor-103';
    
    V_SWR(4).ID := 104;
    V_SWR(4).INITIATOR := 'test-initiator-104';
    V_SWR(4).REQUESTOR := 'test-requestor-104';
    
    V_SWR(5).ID := 105;
    V_SWR(5).INITIATOR := 'test-initiator-105';
    V_SWR(5).REQUESTOR := 'test-requestor-105';
    
    V_SWR(6).ID := 106;
    V_SWR(6).INITIATOR := 'test-initiator-106';
    V_SWR(6).REQUESTOR := 'test-requestor-106';
    
    DBMS_OUTPUT.PUT_LINE('Count BEFORE deleting record @ position #3: ' || TO_CHAR(V_SWR.COUNT));
    
    --Deleting record at position 3
    V_SWR.DELETE(3);
    
    V_KEY := V_SWR.FIRST;
    LOOP
        EXIT WHEN V_KEY IS NULL;
        DBMS_OUTPUT.PUT_LINE('ID: ' || TO_CHAR(V_SWR(V_KEY).ID) || ' Initiator: ' || V_SWR(V_KEY).INITIATOR || ' Requestor: ' || V_SWR(V_KEY).REQUESTOR);
        V_KEY := V_SWR.NEXT(V_KEY);
    END LOOP;
     
    DBMS_OUTPUT.PUT_LINE('Count AFTER deleting a record @ position # 3: ' || TO_CHAR(V_SWR.COUNT));
END;
------------------------------------------------------------------------------
--Trigger:
CREATE OR REPLACE TRIGGER TRGR_SR_LOT_HISTORY
AFTER INSERT OR UPDATE OR DELETE ON SR_LOT
FOR EACH ROW
DECLARE
BEGIN
       IF INSERTING THEN
          INSERT INTO SR_HISTORY_LOT(ID, SR_ID, NAME, ROUTE, OPER, TIME_STAMP, STATE, PRODUCT, X_TYPE, T_STAMP, ATTACHED_BY, DETACHED_BY)
          VALUES(:NEW.ID, :NEW.SR_ID, :NEW.NAME, :NEW.ROUTE, :NEW.OPER, :NEW.TIMESTAMP, :NEW.STATE, :NEW.PRODUCT,'INSERT', CURRENT_TIMESTAMP, :NEW.ATTACHED_BY, :NEW.DETACHED_BY);
       ELSIF DELETING THEN
          INSERT INTO SR_HISTORY_LOT(ID, SR_ID, NAME, ROUTE, OPER, TIME_STAMP, STATE, PRODUCT, X_TYPE, T_STAMP, ATTACHED_BY, DETACHED_BY)
          VALUES(:OLD.ID, :OLD.SR_ID, :OLD.NAME, :OLD.ROUTE, :OLD.OPER, CURRENT_TIMESTAMP, :OLD.STATE, :OLD.PRODUCT,'DELETE', CURRENT_TIMESTAMP, :OLD.ATTACHED_BY, :OLD.DETACHED_BY);
       ELSIF UPDATING THEN
          INSERT INTO SR_HISTORY_LOT(ID, SR_ID, NAME, ROUTE, OPER, TIME_STAMP, STATE, PRODUCT, X_TYPE, T_STAMP, ATTACHED_BY, DETACHED_BY)
          VALUES(:NEW.ID, :NEW.SR_ID, :NEW.NAME, :NEW.ROUTE, :NEW.OPER, :NEW.TIMESTAMP, :NEW.STATE, :OLD.PRODUCT,'UPDATE', CURRENT_TIMESTAMP, :OLD.ATTACHED_BY, :OLD.DETACHED_BY);
       END IF;
END;

grant select,insert,update,delete on SR_HISTORY_LOT to MESUSER;
grant select,insert,update,delete on SR_HISTORY_LOT to MESDATA;
-------------------------------------------------------------------
create or replace TRIGGER "BAWVIEWERTEST"."B4_INSERT_BAW_MATFILES" 
BEFORE INSERT ON BAW__MATFILES 
FOR EACH ROW
DECLARE
    V_COUNTER NUMBER;
    V_DUPLICATE_FIELD_EXCEPTION EXCEPTION;
BEGIN

    BEGIN
        SELECT COUNT(*) INTO V_COUNTER
        FROM BAW__MATFILES 
        WHERE PARENT_ID = :NEW.PARENT_ID
        AND FIELD_NAME = :NEW.FIELD_NAME
        AND IS_DELETED = 'N'
        AND IS_ROOT = 'Y';
        
        IF(V_COUNTER > 0)THEN
            RAISE V_DUPLICATE_FIELD_EXCEPTION; 
        END IF;
    EXCEPTION
        WHEN  V_DUPLICATE_FIELD_EXCEPTION THEN
            RAISE_APPLICATION_ERROR(-20000, 'A row with {' || :NEW.FIELD_NAME || '} already exists in BAW__MATFILES table. Please try again.');
    END;
    
  IF (:NEW.NODE_ID IS NULL)THEN
    SELECT BAWVIEWERTEST.BAW_MATFILE_SEQ.NEXTVAL
    INTO :NEW.NODE_ID FROM DUAL;
  END IF;  
END;
--sequence:
CREATE SEQUENCE SR_ROUTE_OPER_ORDER_SEQ
MINVALUE 100 
MAXVALUE 9999999999999999999999999999 
INCREMENT BY 1 
START WITH 101 
NOCACHE 
NOCYCLE;
-------------------------------------------------------------
--create table with foreign key constraints:
CREATE TABLE SR_ROUTE_OPER_ORDER
(
  ID NUMBER PRIMARY KEY NOT NULL, 
  ROUTE VARCHAR2(30),
  OPER NUMBER, 
  SEQ_ORDER NUMBER, 
  ROUTE_ID NUMBER,
  SPEC_ID VARCHAR2(80),  
  
  CONSTRAINT SR_IMPACTED_ROUTE_FK
  FOREIGN KEY(ROUTE_ID)
  REFERENCES SR_ROUTE(ID)
);
--------------------------------------------------------------
DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       V_COUNTER NUMBER := NULL;
       V_PURPOSE_GROUP VARCHAR2(80); 
       V_PURPOSE_PROJECT VARCHAR2(80); 
       V_PURPOSE_BAND VARCHAR2(80); 
       V_PURPOSE_TYPE VARCHAR2(80); 
       V_PURPOSE_FLOW VARCHAR2(80); 
       V_PURPOSE_DESCRIPTION VARCHAR2(4000);
       V_INDEX NUMBER := 1;
BEGIN
       FOR SWR_REC IN 
       (
         SELECT * FROM SWRMASTER 
         WHERE PURPOSE LIKE '%:%' 
         --AND SWR_ID >= 104888
         AND PURPOSE_GROUP IS NULL
         AND PURPOSE_PROJECT IS NULL
         AND PURPOSE_BAND IS NULL
         AND PURPOSE_TYPE IS NULL
         AND PURPOSE_FLOW IS NULL
         AND PURPOSE_DESCRIPTION IS NULL
         ORDER BY SWR_ID DESC
       )
       LOOP
            V_COUNTER := NULL;
            V_PURPOSE_GROUP := NULL;
            V_PURPOSE_PROJECT := NULL;
            V_PURPOSE_BAND := NULL;
            V_PURPOSE_TYPE := NULL;
            V_PURPOSE_FLOW := NULL;
            V_PURPOSE_DESCRIPTION := NULL;
            V_INDEX := 1;
            
            FOR PURPOSE_REC IN
            (
              select regexp_substr(SWR_REC.PURPOSE,'[^:]+', 1, level) as dat from dual
                connect by regexp_substr(SWR_REC.PURPOSE, '[^:]+', 1, level) is not null
            )
            LOOP
                 
                 IF(V_INDEX = 1)THEN
                    V_PURPOSE_GROUP := PURPOSE_REC.dat;
                 ELSIF(V_INDEX = 2)THEN 
                    V_PURPOSE_PROJECT := PURPOSE_REC.dat;
                 ELSIF(V_INDEX = 3)THEN 
                    V_PURPOSE_BAND := PURPOSE_REC.dat;
                 ELSIF(V_INDEX = 4)THEN 
                    V_PURPOSE_TYPE := PURPOSE_REC.dat;
                 ELSIF(V_INDEX = 5)THEN 
                    V_PURPOSE_FLOW := PURPOSE_REC.dat;
                 ELSIF(V_INDEX = 6)THEN 
                    V_PURPOSE_DESCRIPTION := PURPOSE_REC.dat;
                 END IF;
                 
                 V_INDEX := V_INDEX + 1;
            END LOOP;
                                            
           IF(V_PURPOSE_GROUP IS NOT NULL
              AND V_PURPOSE_PROJECT IS NOT NULL
              AND V_PURPOSE_BAND IS NOT NULL
              AND V_PURPOSE_TYPE IS NOT NULL
              AND V_PURPOSE_FLOW IS NOT NULL
              AND V_PURPOSE_DESCRIPTION IS NOT NULL)THEN
              
              DBMS_OUTPUT.PUT_LINE('GROUP: ' || V_PURPOSE_GROUP || ' ' ||
                     'PROJECT: ' || V_PURPOSE_PROJECT || ' ' ||
                     'BAND: ' || V_PURPOSE_BAND || ' ' ||
                     'TYPE: ' || V_PURPOSE_TYPE || ' ' ||
                     'FLOW: ' || V_PURPOSE_FLOW || ' ' ||
                     'DESC: ' || V_PURPOSE_DESCRIPTION);
                     
                   SELECT COUNT(*) INTO V_COUNTER 
                   FROM SWRMASTER
                   WHERE SWR_ID = SWR_REC.SWR_ID
                   AND PURPOSE_GROUP IS NULL
                   AND PURPOSE_PROJECT IS NULL
                   AND PURPOSE_BAND IS NULL
                   AND PURPOSE_TYPE IS NULL
                   AND PURPOSE_FLOW IS NULL
                   AND PURPOSE_DESCRIPTION IS NULL
                   AND PURPOSE IS NOT NULL;
                   
                   IF(V_COUNTER = 1)THEN
                      NULL;
                      /*
                      UPDATE SWRMASTER
                      SET PURPOSE = NULL,
                          PURPOSE_GROUP = V_PURPOSE_GROUP,
                          PURPOSE_PROJECT = V_PURPOSE_PROJECT,
                          PURPOSE_BAND = V_PURPOSE_BAND,
                          PURPOSE_TYPE = V_PURPOSE_TYPE,
                          PURPOSE_FLOW = V_PURPOSE_FLOW,
                          PURPOSE_DESCRIPTION = V_PURPOSE_DESCRIPTION
                      WHERE SWR_ID =  SWR_REC.SWR_ID;
                      */
                   END IF;         
           ELSE
               DBMS_OUTPUT.PUT_LINE('Purpose ' || SWR_REC.PURPOSE || ' is not of RnD type for ID: ' || TO_CHAR(SWR_REC.SWR_ID));
           END IF;                     
       END LOOP;
       
       COMMIT;
       
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
      ROLLBACK;
END;
--------------------------------
--CTE EXAMPLE:
with table1(column_a, column_b) as (
  select 1, 2 from dual union all
  select 2, 2 from dual union all
  select 3, null from dual union all
  select null, 4 from dual union all
  select null, null from dual
)
SELECT 
  COLUMN_A, 
  COLUMN_B,
  CASE 
    WHEN COLUMN_A <> COLUMN_B THEN 'Not OK' 
    WHEN column_a is null and column_b is null then 'both NULL'
    WHEN column_a is null then 'A null'
    WHEN column_b is null then 'B null'
ELSE 'OK' END AS Status
FROM Table1;
--------------------------------------------
--REF CUSRSOR
SET SERVEROUTPUT ON;
DECLARE
        V_SUBMIT_BY_CLIENT VARCHAR2(50) := 'BAW DataViewer';
        V_BATCH_ID BAW_SVRLOG_BATCH.BATCH_ID%TYPE;
        V_DATE_SUBMITTED BAW_SVRLOG_BATCH.DATE_SUBMITTED%TYPE;
        V_MACHINE_SUBMITTED BAW_SVRLOG_BATCH.MACHINE_SUBMITTED%TYPE;
        
        CURSOR BAW_SVR_CURSOR(P_SUBMIT_BY_CLIENT VARCHAR2)
        IS
        SELECT XX.*
        FROM(
              SELECT *
              FROM BAW_SVRLOG_BATCH
              WHERE SUBMITTED_BY_CLIENT <> P_SUBMIT_BY_CLIENT
              AND ROWNUM <= 20
            )XX
        ORDER BY XX.BATCH_ID DESC;
        
        ROW_BAW_SVR_CURSOR BAW_SVR_CURSOR%ROWTYPE;
BEGIN
        OPEN BAW_SVR_CURSOR(V_SUBMIT_BY_CLIENT);
        LOOP
             FETCH BAW_SVR_CURSOR INTO ROW_BAW_SVR_CURSOR;
             EXIT WHEN BAW_SVR_CURSOR%NOTFOUND;
             V_BATCH_ID := ROW_BAW_SVR_CURSOR.BATCH_ID;
             V_DATE_SUBMITTED := ROW_BAW_SVR_CURSOR.DATE_SUBMITTED;
             V_MACHINE_SUBMITTED := ROW_BAW_SVR_CURSOR.MACHINE_SUBMITTED;
             
             DBMS_OUTPUT.PUT_LINE('BatchId {' || V_BATCH_ID || '} was submitted by: {' || V_MACHINE_SUBMITTED || '} on :' || V_DATE_SUBMITTED); 
             
        END LOOP;
END;
-------------------------------------------------------------------------------------------------------
--find duplicates
with dup_counts as (
  select f.*, count(*) over (partition by lot, route, oper) ct
  from   DATAVIEWER_FUNCTION_OVERRIDE f
)
  select *
  from   dup_counts
  where  ct > 1
 ---CONNECT BY EAMPLE: (CHANGE THE VALUE OF 3 IN FIRST ROW TO SEE DIFFERENT RESULTS.
SELECT LEVEL AS element,
       REGEXP_SUBSTR( '//EG9583/1918604/AFTER_GRINDING/13/CSV_filter_spreadsheet' ,'(.*?)(/)', 3, LEVEL, NULL, 1 ) AS element_value 
FROM  DUAL
CONNECT BY LEVEL <= regexp_count('//EG9583/1918604/AFTER_GRINDING/13/CSV_filter_spreadsheet', '/') + 1;
-------------------------------------------------------------------------------------------------------
    IF NOT REGEXP_LIKE(V_PRODUCT, '^EG\d\d\d\d$')
        OR NOT (V_LOT IS NULL OR REGEXP_LIKE(V_LOT, '^\d{7}$'))
        OR NOT (V_WAFER IS NULL OR REGEXP_LIKE(V_WAFER, '^\d{1,2}$'))
    THEN
        INSERT INTO LOG_ANYTHING
        (
            LOG_MSG
        )
        VALUES
        (
            'Weird inputs in FN_CREATE_PARENT_NODE_ID.  Product: ''' || V_PRODUCT || ''', Lot: ''' || V_LOT || ''', State: ''' || V_STATE || ''', Wafer: ''' || V_WAFER || ''', Parameter: ''' || V_PARAMETER || '''.'
        );
    END IF;
---------------------------------------------------------
--Creating Java class:
CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED MyJavaUtilityClass
/*CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED MyJavaUtilityClass*/
AS 
import java.sql.*;  
import java.io.*;  
import java.util.List;  
import java.util.ArrayList;  
import oracle.sql.*;

public class MyJavaUtilityClass
{
  public static String GetHelloWorldMessage()
  {
    String retVal = "";
    try
    {
        retVal = "Hello world! From: Java class method 'GetHelloWorldMessage()'."; 
    }
    catch(Exception ex)
    {
      System.err.println(ex.getMessage());
    }
    return retVal;
  }
  
  public static boolean IsNumeric(String inputVal) 
  {
    try 
    {
      //int i = Integer.parseInt(inputVal);
	  double d = Double.parseDouble(inputVal);
    } 
    catch (Exception ex) 
    {
        System.err.println(ex.getMessage());
        return false;
    }
    return true;
  }

  public static String FormatName(String firstName, String lastName) 
  {
    String retVal = "";
    try 
    {
      retVal =  lastName + ", " + firstName;
    } 
    catch (Exception ex) 
    {
      retVal = ex.getMessage();
    }
    return retVal;
  }
   
  public static void ThrowExceptionMethod(int inputId, String inputValue) throws SQLException
  {
    throw new SQLException("Intentionally throwing an exception.");
  }
  
}/*end class MyJavaUtilityClass*/
---------
--Defining Java function inside a package:

FUNCTION GET_JAVA_MESSAGE RETURN VARCHAR2;

FUNCTION IS_NUMERIC(IN_INPUT_VAL VARCHAR2) RETURN BOOLEAN;

FUNCTION GET_FULL_NAME(IN_FIRST_NAME VARCHAR2, IN_LAST_NAME VARCHAR2) RETURN VARCHAR2;

PROCEDURE EXCEPTION_HANDLING(IN_ID NUMBER, IN_VALUE VARCHAR2);

 FUNCTION GET_JAVA_MESSAGE
 RETURN VARCHAR2
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.GetHelloWorldMessage() return java.lang.String';
  
 FUNCTION IS_NUMERIC(IN_INPUT_VAL VARCHAR2)
 RETURN BOOLEAN
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.IsNumeric(java.lang.String) return java.lang.Boolean';
 
 FUNCTION GET_FULL_NAME(IN_FIRST_NAME VARCHAR2, IN_LAST_NAME VARCHAR2) 
 RETURN VARCHAR2
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.FormatName(java.lang.String, java.lang.String) return java.lang.String';
 
  PROCEDURE EXCEPTION_HANDLING(IN_ID NUMBER, IN_VALUE VARCHAR2) 
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.ThrowExceptionMethod(int, java.lang.String)';
--- calling from anonymous block:
SET SERVEROUTPUT ON;
DECLARE
       V_EXCEPTION_MSG VARCHAR2(4000) := NULL;
       V_IS_NUMERIC BOOLEAN := NULL;
       V_VALUE VARCHAR2(1000) := '145.35';
       V_HELLO_WORLD_MSG VARCHAR2(200) := NULL;
       V_FORMATTED_NAME VARCHAR2(1000) := NULL;
       
BEGIN
       V_IS_NUMERIC := TEST_PKG.IS_NUMERIC(V_VALUE);
       V_HELLO_WORLD_MSG := TEST_PKG.GET_JAVA_MESSAGE();
       V_FORMATTED_NAME := TEST_PKG.GET_FULL_NAME('Kashif', 'Mubarak');
       TEST_PKG.EXCEPTION_HANDLING(1234, 'blaBla');

       DBMS_OUTPUT.PUT_LINE(V_HELLO_WORLD_MSG);
       
       DBMS_OUTPUT.PUT_LINE(V_FORMATTED_NAME);
       
       DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
       
       IF(V_IS_NUMERIC = TRUE)THEN
          DBMS_OUTPUT.PUT_LINE(V_VALUE || ' is numeric.'); 
       ELSE
          DBMS_OUTPUT.PUT_LINE(V_VALUE || ' is NOT numeric.');          
       END IF;
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
-----------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE Customers 
(
    CustNo NUMBER(3) NOT NULL,
    CustName VARCHAR2(30) NOT NULL,
    Street VARCHAR2(20) NOT NULL,
    City VARCHAR2(20) NOT NULL,
    State CHAR(2) NOT NULL,
    Zip VARCHAR2(10) NOT NULL,
    Phone VARCHAR2(12),
    PRIMARY KEY (CustNo)
);
--------------------------------------------------
CREATE TABLE Orders 
(
    PONo NUMBER(5),
    Custno NUMBER(3) REFERENCES Customers,
    OrderDate DATE,
    ShipDate DATE,
    ToStreet VARCHAR2(20),
    ToCity VARCHAR2(20),
    ToState CHAR(2),
    ToZip VARCHAR2(10),
    PRIMARY KEY (PONo)
);
--------------------------------------------------
CREATE TABLE StockItems 
(
    StockNo NUMBER(4) PRIMARY KEY,
    Description VARCHAR2(20),
    Price NUMBER(6,2)
);
--------------------------------------------------
CREATE TABLE LineItems 
(
    LineNo NUMBER(2),
    PONo NUMBER(5) REFERENCES Orders,
    StockNo NUMBER(4) REFERENCES StockItems,
    Quantity NUMBER(2),
    Discount NUMBER(4,2),
    PRIMARY KEY (LineNo, PONo)
);
--------------------------------------------------
CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED POManager
/*CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED POManager*/
AS 
import java.sql.*;
import java.io.*;
import oracle.jdbc.*;

public class POManager
{
public static void addCustomer (int custNo, String custName, String street,
   String city, String state, String zipCode, String phoneNo) throws SQLException
  {
    String sql = "INSERT INTO Customers VALUES (?,?,?,?,?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, custNo);
      pstmt.setString(2, custName);
      pstmt.setString(3, street);
      pstmt.setString(4, city);
      pstmt.setString(5, state);
      pstmt.setString(6, zipCode);
      pstmt.setString(7, phoneNo);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e) 
    {
      System.err.println(e.getMessage());
    }
  }

  public static void addStockItem (int stockNo, String description, float price)
                                                               throws SQLException
  {
    String sql = "INSERT INTO StockItems VALUES (?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, stockNo);
      pstmt.setString(2, description);
      pstmt.setFloat(3, price);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void enterOrder (int orderNo, int custNo, String orderDate,
   String shipDate, String toStreet, String toCity, String toState,
    String toZipCode) throws SQLException 
  {
    String sql = "INSERT INTO Orders VALUES (?,?,?,?,?,?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, orderNo);
      pstmt.setInt(2, custNo);
      pstmt.setString(3, orderDate);
      pstmt.setString(4, shipDate);
      pstmt.setString(5, toStreet);
      pstmt.setString(6, toCity);
      pstmt.setString(7, toState);
      pstmt.setString(8, toZipCode);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void addLineItem (int lineNo, int orderNo, int stockNo,
   int quantity, float discount) throws SQLException
  {
    String sql = "INSERT INTO LineItems VALUES (?,?,?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, lineNo);
      pstmt.setInt(2, orderNo);
      pstmt.setInt(3, stockNo);
      pstmt.setInt(4, quantity);
      pstmt.setFloat(5, discount);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void totalOrders () throws SQLException 
  {
    String sql = "SELECT O.PONo, ROUND(SUM(S.Price * L.Quantity)) AS TOTAL " +
     "FROM Orders O, LineItems L, StockItems S " +
     "WHERE O.PONo = L.PONo AND L.StockNo = S.StockNo " +
     "GROUP BY O.PONo";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      ResultSet rset = pstmt.executeQuery();
      printResults(rset);
      rset.close();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  static void printResults (ResultSet rset) throws SQLException
  {
    String buffer = "";
    try
    {
      ResultSetMetaData meta = rset.getMetaData();
      int cols = meta.getColumnCount(), rows = 0;
      for (int i = 1; i <= cols; i++)
      {
        int size = meta.getPrecision(i);
        String label = meta.getColumnLabel(i);
        if (label.length() > size)
          size = label.length();
        while (label.length() < size)
          label += " ";
        buffer = buffer + label + " ";
      }
      buffer = buffer + "\n";
      while (rset.next())
      {
        rows++;
        for (int i = 1; i <= cols; i++)
        {
          int size = meta.getPrecision(i);
          String label = meta.getColumnLabel(i);
          String value = rset.getString(i);
          if (label.length() > size) 
            size = label.length();
          while (value.length() < size)
            value += " ";
          buffer = buffer + value + " ";
        }
        buffer = buffer + "\n";
      }
      if (rows == 0)
        buffer = "No data found!\n";
      System.out.println(buffer);
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void checkStockItem (int stockNo) throws SQLException
  {
    String sql = "SELECT O.PONo, O.CustNo, L.StockNo, " + 
     "L.LineNo, L.Quantity, L.Discount " +
     "FROM Orders O, LineItems L " +
     "WHERE O.PONo = L.PONo AND L.StockNo = ?";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, stockNo);
      ResultSet rset = pstmt.executeQuery();
      printResults(rset);
      rset.close();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void changeQuantity (int newQty, int orderNo, int stockNo)
                                                               throws SQLException
  {
    String sql = "UPDATE LineItems SET Quantity = ? " +
     "WHERE PONo = ? AND StockNo = ?";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, newQty);
      pstmt.setInt(2, orderNo);
      pstmt.setInt(3, stockNo);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void deleteOrder (int orderNo) throws SQLException
  {
    String sql = "DELETE FROM LineItems WHERE PONo = ?";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, orderNo);
      pstmt.executeUpdate();
      sql = "DELETE FROM Orders WHERE PONo = ?";
      pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, orderNo);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }
}/*end class POManager*/
----------------------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE po_mgr 
AS
    PROCEDURE add_customer (cust_no NUMBER, cust_name VARCHAR2,
    street VARCHAR2, city VARCHAR2, state CHAR, zip_code VARCHAR2,
    phone_no VARCHAR2);
    PROCEDURE add_stock_item (stock_no NUMBER, description VARCHAR2,
    price NUMBER);
    PROCEDURE enter_order (order_no NUMBER, cust_no NUMBER,
    order_date VARCHAR2, ship_date VARCHAR2, to_street VARCHAR2,
    to_city VARCHAR2, to_state CHAR, to_zip_code VARCHAR2);
    PROCEDURE add_line_item (line_no NUMBER, order_no NUMBER,
    stock_no NUMBER, quantity NUMBER, discount NUMBER);
    PROCEDURE total_orders;
    PROCEDURE check_stock_item (stock_no NUMBER);
    PROCEDURE change_quantity (new_qty NUMBER, order_no NUMBER,
    stock_no NUMBER);
    PROCEDURE delete_order (order_no NUMBER);
END po_mgr;

/

CREATE OR REPLACE PACKAGE BODY po_mgr 
AS
    PROCEDURE add_customer (cust_no NUMBER, cust_name VARCHAR2,
    street VARCHAR2, city VARCHAR2, state CHAR, zip_code VARCHAR2,
    phone_no VARCHAR2) AS LANGUAGE JAVA
    NAME 'POManager.addCustomer(int, java.lang.String,
    java.lang.String, java.lang.String, java.lang.String,
    java.lang.String, java.lang.String)';
    
    PROCEDURE add_stock_item (stock_no NUMBER, description VARCHAR2,
    price NUMBER) AS LANGUAGE JAVA
    NAME 'POManager.addStockItem(int, java.lang.String, float)';
    
    PROCEDURE enter_order (order_no NUMBER, cust_no NUMBER,
    order_date VARCHAR2, ship_date VARCHAR2, to_street VARCHAR2,
    to_city VARCHAR2, to_state CHAR, to_zip_code VARCHAR2)
    AS LANGUAGE JAVA
    NAME 'POManager.enterOrder(int, int, java.lang.String,
    java.lang.String, java.lang.String, java.lang.String,
    java.lang.String, java.lang.String)';
    
    PROCEDURE add_line_item (line_no NUMBER, order_no NUMBER,
    stock_no NUMBER, quantity NUMBER, discount NUMBER)
    AS LANGUAGE JAVA
    NAME 'POManager.addLineItem(int, int, int, int, float)';
    
    PROCEDURE total_orders
    AS LANGUAGE JAVA
    NAME 'POManager.totalOrders()';
    
    PROCEDURE check_stock_item (stock_no NUMBER)
    AS LANGUAGE JAVA
    NAME 'POManager.checkStockItem(int)';
    
    PROCEDURE change_quantity (new_qty NUMBER, order_no NUMBER,
    stock_no NUMBER) AS LANGUAGE JAVA
    NAME 'POManager.changeQuantity(int, int, int)';
    
    PROCEDURE delete_order (order_no NUMBER)
    AS LANGUAGE JAVA
    NAME 'POManager.deleteOrder(int)';
END po_mgr;

/
--------------------------------------------------------------------------------------------------------------------------------
BEGIN
  po_mgr.add_stock_item(2010, 'camshaft', 245.00);
  po_mgr.add_stock_item(2011, 'connecting rod', 122.50);
  po_mgr.add_stock_item(2012, 'crankshaft', 388.25);
  po_mgr.add_stock_item(2013, 'cylinder head', 201.75);
  po_mgr.add_stock_item(2014, 'cylinder sleeve', 73.50);
  po_mgr.add_stock_item(2015, 'engine bearning', 43.85);
  po_mgr.add_stock_item(2016, 'flywheel', 155.00);
  po_mgr.add_stock_item(2017, 'freeze plug', 17.95);
  po_mgr.add_stock_item(2018, 'head gasket', 36.75);
  po_mgr.add_stock_item(2019, 'lifter', 96.25);
  po_mgr.add_stock_item(2020, 'oil pump', 207.95);
  po_mgr.add_stock_item(2021, 'piston', 137.75);
  po_mgr.add_stock_item(2022, 'piston ring', 21.35);
  po_mgr.add_stock_item(2023, 'pushrod', 110.00);
  po_mgr.add_stock_item(2024, 'rocker arm', 186.50);
  po_mgr.add_stock_item(2025, 'valve', 68.50);
  po_mgr.add_stock_item(2026, 'valve spring', 13.25);
  po_mgr.add_stock_item(2027, 'water pump', 144.50);
  COMMIT;
END;
-----------------------------------------------------------
BEGIN
  po_mgr.add_customer(101, 'A-1 Automotive', '4490 Stevens Blvd',
    'San Jose', 'CA', '95129', '408-555-1212');
  po_mgr.add_customer(102, 'AutoQuest', '2032 America Ave',
    'Hayward', 'CA', '94545', '510-555-1212');
  po_mgr.add_customer(103, 'Bell Auto Supply', '305 Cheyenne Ave',
    'Richardson', 'TX', '75080', '972-555-1212');
  po_mgr.add_customer(104, 'CarTech Auto Parts', '910 LBJ Freeway',
    'Dallas', 'TX', '75234', '214-555-1212');
  COMMIT;
END;
-----------------------------------------------------------
BEGIN
  po_mgr.enter_order(30501, 103, '14-SEP-1998', '21-SEP-1998',
    '305 Cheyenne Ave', 'Richardson', 'TX', '75080');
  po_mgr.add_line_item(01, 30501, 2011, 5, 0.02);
  po_mgr.add_line_item(02, 30501, 2018, 25, 0.10);
  po_mgr.add_line_item(03, 30501, 2026, 10, 0.05);

  po_mgr.enter_order(30502, 102, '15-SEP-1998', '22-SEP-1998',
    '2032 America Ave', 'Hayward', 'CA', '94545');
  po_mgr.add_line_item(01, 30502, 2013, 1, 0.00);
  po_mgr.add_line_item(02, 30502, 2014, 1, 0.00);

  po_mgr.enter_order(30503, 104, '15-SEP-1998', '23-SEP-1998',
    '910 LBJ Freeway', 'Dallas', 'TX', '75234');
  po_mgr.add_line_item(01, 30503, 2020, 5, 0.02);
  po_mgr.add_line_item(02, 30503, 2027, 5, 0.02);
  po_mgr.add_line_item(03, 30503, 2021, 15, 0.05);
  po_mgr.add_line_item(04, 30503, 2022, 15, 0.05);

  po_mgr.enter_order(30504, 101, '16-SEP-1998', '23-SEP-1998',
    '4490 Stevens Blvd', 'San Jose', 'CA', '95129');
  po_mgr.add_line_item(01, 30504, 2025, 20, 0.10);
  po_mgr.add_line_item(02, 30504, 2026, 20, 0.10);
  COMMIT;
END;
-----------------------------------------------------------
SELECT * FROM CUSTOMERS;
SELECT * FROM Orders;
SELECT * FROM StockItems;
SELECT * FROM LineItems;
-----------------------------------------------------------
SET SERVEROUTPUT ON;
CALL dbms_java.set_output(2000);
CALL po_mgr.total_orders();
----------------------------------------------------------------------------------------------------------------------------------------------------------