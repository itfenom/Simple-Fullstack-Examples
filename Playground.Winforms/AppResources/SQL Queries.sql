--11/01/2016
SET SERVEROUTPUT ON;
DECLARE
	V_EXCEPTION_MSG VARCHAR2(500) := NULL;
BEGIN
	DBMS_OUTPUT.PUT_LINE('hello!'); 
EXCEPTION
	WHEN OTHERS THEN
		V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
		DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
---------------------------------------------------------------------------------------------------------------------------
DECLARE
       TYPE T_LIST IS TABLE OF NUMBER INDEX BY PLS_INTEGER;
       V_IDS T_LIST;
       V_INDEX NUMBER;
BEGIN
	V_IDS(10700) := 3042;
	V_IDS(10482) := 3043;
	
	V_INDEX := V_IDS.FIRST;
	
	WHILE (V_INDEX IS NOT NULL)
	LOOP
		DBMS_OUTPUT.PUT_LINE(V_IDS(V_INDEX)); 
		V_INDEX := V_IDS.NEXT(V_INDEX); 
	END LOOP;
END;
---------------------------------------------------------------------------------------------------------------------------
SET SERVEROUTPUT ON;
DECLARE
  IN_LOT VARCHAR2(200);
  IN_ROUTE VARCHAR2(200);
  IN_OPERATION NUMBER;
  IN_MOVE_TO_TIME TIMESTAMP;
  IN_WAFER_LIST BAW_MATLAB_HISTORY_PKG.LS_NUMBER;
  OUT_OK_TO_MOVE NUMBER;
  OUT_MESSAGE VARCHAR2(4000);
BEGIN
  IN_LOT := '1920805';
  IN_ROUTE := 'BAWFITRIM';
  IN_OPERATION := 2519;
  IN_MOVE_TO_TIME := CAST('07-AUG-19 08.37.01.000000000 AM' AS TIMESTAMP);
  IN_WAFER_LIST(1) := 1;
  IN_WAFER_LIST(2) := 2;
  IN_WAFER_LIST(3) := 3;
  IN_WAFER_LIST(4) := 4;
  IN_WAFER_LIST(5) := 5;
  IN_WAFER_LIST(6) := 6;
  IN_WAFER_LIST(7) := 7;
  IN_WAFER_LIST(8) := 8;
  IN_WAFER_LIST(9) := 9;
  IN_WAFER_LIST(10) := 10;
  IN_WAFER_LIST(11) := 11;
  IN_WAFER_LIST(12) := 12;
  IN_WAFER_LIST(13) := 13;
  IN_WAFER_LIST(14) := 14;
  IN_WAFER_LIST(15) := 15;
  IN_WAFER_LIST(16) := 16;
  IN_WAFER_LIST(17) := 17;
  IN_WAFER_LIST(18) := 18;
  IN_WAFER_LIST(19) := 19;
  IN_WAFER_LIST(20) := 20;
  IN_WAFER_LIST(21) := 21;
  IN_WAFER_LIST(22) := 22;
  IN_WAFER_LIST(23) := 23;
  IN_WAFER_LIST(24) := 24;
  
  BAW_MATLAB_HISTORY_PKG.PROC_VERIFY_MATLAB_FUNCS(IN_LOT, IN_ROUTE, IN_OPERATION, IN_MOVE_TO_TIME, IN_WAFER_LIST, OUT_OK_TO_MOVE, OUT_MESSAGE);
  DBMS_OUTPUT.PUT_LINE('STATUS: ' || OUT_OK_TO_MOVE || ' Message: ' || OUT_MESSAGE);
END;
---------------------------------------------------------------------------------------------
--!matlab -regserver


--Find Duplicate query	
--*****************************
SELECT XX.* 
FROM 
(SELECT A.ROUTE_ID, A.ROUTE, A.OPER, COUNT(*) COUNT_X
FROM SR_ROUTE_OPER_ORDER A
GROUP BY A.ROUTE_ID, A.ROUTE, A.OPER
ORDER BY A.ROUTE_ID DESC)XX
WHERE XX.COUNT_X > 1;

SELECT 
    A.GROUP_ID,
    A.MFD_KEY,
    COUNT(*)
FROM 
    BAW_FLOW_FUNCTION_ASSIGNMENT A
GROUP BY 
    A.GROUP_ID,
    A.MFD_KEY
HAVING COUNT(*) > 1
ORDER BY 
    A.GROUP_ID DESC;
---------------
WITH DUPLICATE_MFD_KEYS
AS
(
    SELECT A.*,
        COUNT(*) OVER (PARTITION BY GROUP_ID, MFD_KEY) C
    FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
    WHERE IS_DELETED = 'N'
    ORDER BY A.GROUP_ID
)
SELECT * FROM DUPLICATE_MFD_KEYS
WHERE C > 1;

 -----------------------------------------------------------------------------------------------
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
-----------------------------------------------------------------------------------------------
--CTE:
WITH temp (n, fact) AS 
(SELECT 0, 1 FROM DUAL -- Initial Subquery 
 
 UNION ALL 
 
 SELECT n + 1, (n+1) * fact FROM temp -- Recursive Subquery 
 WHERE n < 9) 
 SELECT * FROM temp;
-----------------------------------------------------------------------------------------------
WITH PARAM_TBL (JOB_ID, INOUTPUT, DATATYPE, PARAM_PATH)AS
(
  --INPUTS
  SELECT 1, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/01/data' FROM DUAL
  UNION
  SELECT 2, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/02/data' FROM DUAL
  UNION
  SELECT 3, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/03/data' FROM DUAL
    UNION
  SELECT 4, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/04/data' FROM DUAL
    UNION
  SELECT 5, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/05/data' FROM DUAL
    UNION
  SELECT 6, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE//data' FROM DUAL
    UNION
  SELECT 7, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/01/data' FROM DUAL
    UNION
  SELECT 8, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/02/data' FROM DUAL
    UNION
  SELECT 9, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/03/data' FROM DUAL
    UNION
  SELECT 10, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/04/data' FROM DUAL
    UNION
  SELECT 11, 'INPUT', 'DBStruct', '//EG5520/1633638/AASTATE/05/data' FROM DUAL
  
  UNION
  
  --OUTPUTS
  SELECT 12, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/01/data' FROM DUAL
  UNION
  SELECT 13, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/02/data' FROM DUAL
  UNION
  SELECT 14, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/03/data' FROM DUAL
    UNION
  SELECT 15, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/04/data' FROM DUAL
    UNION
  SELECT 16, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/05/data' FROM DUAL
    UNION
  SELECT 17, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE//data' FROM DUAL
    UNION
  SELECT 18, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/01/data' FROM DUAL
    UNION
  SELECT 19, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/02/data' FROM DUAL
    UNION
  SELECT 20, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/03/data' FROM DUAL
    UNION
  SELECT 21, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/04/data' FROM DUAL
    UNION
  SELECT 22, 'OUTPUT', 'DBStruct', '//EG5520/1633638/AASTATE/05/data' FROM DUAL
)


SELECT JOB_ID,
       INOUTPUT,
       DATATYPE, 
       PARAM_PATH,
       REGEXP_SUBSTR(PARAM_PATH, '[^/]+',1,4) WAFER_VAL,  --4th occurence of slash (/) represents wafer # value!
       REGEXP_SUBSTR(PARAM_PATH, '[^/]+',1,5) WAFER_DBSTRUCT,  --5th occurence of slash (/) represents wafer data value!
       REGEXP_REPLACE(PARAM_PATH, '^(\/\/EG\d{4}\/\d{7}\/.*\/)(\d+|\*?)(\/.*)', '\1\3') AS EXTRACTED_WAFER_VAL,
       REGEXP_REPLACE(PARAM_PATH, '^(\/\/EG\d{4}\/\d{7}\/.*\/)\d+(\/.*)', '\1\2') AS REPLACED_1,
       REGEXP_REPLACE(PARAM_PATH, '^(\/\/EG\d{4}\/\d{7}\/.*\/)\*?(\/.*)', '\1\2') AS REPLACED_2
FROM PARAM_TBL 
WHERE INOUTPUT IN ('OUTPUT', 'OUTPUT');
--AND REGEXP_LIKE(PARAM_PATH, '^\/\/EG\d{4}\/\d{7}\/[^\/]+\/\/')
--AND REGEXP_LIKE(PARAM_PATH, '^\/\/EG\d{4}\/\d{7}\/[^\/]+\/\d+\/');
-----------------------------------------------------------------------------------------------
SELECT * FROM BAW_STATE_ROUTE_OPER
AS OF TIMESTAMP
TO_TIMESTAMP('2018-12-04 09:30:00', 'YYYY-MM-DD HH:MI:SS')
WHERE ST_RT_SAN = 11143;
-----------------------------------------------------------------------------------------------

              select regexp_substr(SWR_REC.PURPOSE,'[^:]+', 1, level) as dat from dual
                connect by regexp_substr(SWR_REC.PURPOSE, '[^:]+', 1, level) is not null
-----------------------------------------------------------------------------------------------------------------------------------------
----FIND DUPLUCATES in comma separated string.
    --Make sure that there's no duplicates in the UDV_LIST
    V_COUNTER := NULL;
    SELECT COUNT(*) INTO V_COUNTER
    FROM
    (
        WITH CTE AS
        (
            SELECT TRIM(REGEXP_SUBSTR(V_UDV_LIST,'[^,]+', 1, LEVEL)) AS XXX FROM DUAL
            CONNECT BY TRIM(REGEXP_SUBSTR(V_UDV_LIST, '[^,]+', 1, LEVEL)) IS NOT NULL
        )
        SELECT CTE.*, COUNT(*) OVER (PARTITION BY CTE.XXX) C_COUNT
        FROM CTE
    )X
    WHERE X.C_COUNT > 1;
----------------------
---{'fs_%UDVEXT%', 'fp_%UDVEXT%'},
SELECT regexp_substr('{fs_%UDVEXT1%, fp_%UDVEXT2%}', '\%([^%]+)\%', 1,1,null, 1) AS output FROM dual;
SELECT regexp_substr('{fs_%UDVEXT1%, fp_%UDVEXT2%}', '\%([^%]+)\%', 1,2,null, 1) AS output FROM dual;
-----
SELECT REGEXP_SUBSTR('{fs_%UDVEXT1%, fp_%UDVEXT2%}', '\%([^%]+%)', 1, 2) AS OUTPUT FROM DUAL;
---
SELECT TRIM(REGEXP_SUBSTR('{fs_%UDVEXT1%, fp_%UDVEXT2%}','[^%]+', 1, LEVEL)) AS XXX FROM DUAL
CONNECT BY TRIM(REGEXP_SUBSTR('{fs_%UDVEXT1%, fp_%UDVEXT2%}', '[^%]+', 1, LEVEL)) IS NOT NULL;
----
SELECT TRIM(REGEXP_SUBSTR((SELECT ML_EXPRESSION FROM BAWUDV_ASSIGNMENT WHERE ASSIGNMENT_ID = 252),'[^%]+', 1, LEVEL)) AS XXX FROM DUAL
CONNECT BY TRIM(REGEXP_SUBSTR((SELECT ML_EXPRESSION FROM BAWUDV_ASSIGNMENT WHERE ASSIGNMENT_ID = 252), '[^%]+', 1, LEVEL)) IS NOT NULL;
------
SELECT DISTINCT TRIM(REGEXP_SUBSTR('{fs_%UDVEXT1%, fp_%UDVEXT2%}','|%(\w+)%|', 1, LEVEL)) AS VARIABLE_NAME FROM DUAL
CONNECT BY TRIM(REGEXP_SUBSTR('{fs_%UDVEXT1%, fp_%UDVEXT2%}', '|%(\w+)%|', 1, LEVEL)) IS NOT NULL;
------------------------------------------------
                WITH MY_TBL
                AS
                (
                SELECT 'EG5520' AS PRODUCT,
                       'My Function' AS CALL_LABEL, 
                       'EG-M0' || SUBSTR('EG5520', -4) AS PRODUCT_LONG,
                       '1633638' AS LOT,
                       '1633638A'AS CHILD_LOT,
                       'AFTER_M2A_PATTERNING' AS STATE,
                       LPAD('8', 2, '0') AS WAFER,
                       'BAWPATRIM' AS ROUTE,
                       '2523' AS OPER,
                       'ADMIN' AS USER_TYPE,
                       '100' AS USER_RANK,
                       TO_CHAR(TO_NUMBER('08')) AS WAFERNUM, -- Wafer with single digit like 2 instead of 02
                       '123456' AS SWRNUMBER,
                       '<SWR_REQUESTOR>' AS SWRREQUESTOR,
                       '<SWR_OROGINATOR>' AS SWRORIGINATOR,
                       '<SWR_PURPOSE>' AS SWRPURPOSE
                FROM DUAL
                )
                SELECT COLUMN_NAME, COLUMN_VAL
                FROM MY_TBL UNPIVOT
                (
                   COLUMN_VAL FOR COLUMN_NAME IN 
                   (
                    PRODUCT, CALL_LABEL, PRODUCT_LONG, LOT, CHILD_LOT, STATE, WAFER, ROUTE, OPER, USER_TYPE, USER_RANK, 
                    WAFERNUM, SWRNUMBER, SWRREQUESTOR, SWRORIGINATOR, SWRPURPOSE
                   )
                );
------------------------------------------------------------------------------------