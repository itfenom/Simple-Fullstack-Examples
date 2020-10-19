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
SELECT distinct A.ASSIGNMENT_ID,
  B.VARIABLE_ID,
  C.LOT,
  D.PRODUCT,
  E.STATE,
  F.ST_RT_SAN,
  G.MFD_KEY,
  A.ML_EXPRESSION
FROM BAWUDV_ASSIGNMENT A,
  BAWUDV_VARIABLE B,
  BAWUDV_GRP_LOT C,
  BAWUDV_GRP_PROD D,
  BAWUDV_GRP_STATE E,
  BAWUDV_GRP_RT_OP F,
  BAWUDV_GRP_FUNC G
WHERE B.NAME         = 'TESTTYPE_ROUTEOPER'
AND C.LOT           IN ('*','1633638')
AND D.PRODUCT       IN ('*', 'EG5520')
AND E.STATE         IN ('*', 'AFTER_PA_TRIM')
AND (F.ASSIGNMENT_ID = A.ASSIGNMENT_ID
AND F.ST_RT_SAN     IS NULL
OR F.ST_RT_SAN      IN
  (SELECT ST_RT_SAN
  FROM BAW_STATE_ROUTE_OPER
  WHERE STATE = 'AFTER_PA_TRIM'
  AND ROUTE  IN ('*', 'BAWPATRIM')
  AND OPER   IN (-1, 2519)
  ))
AND (G.ASSIGNMENT_ID = A.ASSIGNMENT_ID
AND G.MFD_KEY       IS NULL
OR G.MFD_KEY         = 14082)
AND A.VARIABLE_ID    = B.VARIABLE_ID
AND A.ASSIGNMENT_ID  = C.ASSIGNMENT_ID
AND A.ASSIGNMENT_ID  = D.ASSIGNMENT_ID
AND A.ASSIGNMENT_ID  = E.ASSIGNMENT_ID
AND A.ASSIGNMENT_ID  = F.ASSIGNMENT_ID
AND A.ASSIGNMENT_ID  = G.ASSIGNMENT_ID;
  
--------------------------------------------------------------------------------
--all assignments
SELECT A.GROUP_ID, 
       A.DISPLAY_ORDER, A.LOT,
       (C.STATE || '  -  ' || C.SUB_STATE || ' <==> ' || C.ROUTE || '  -  ' || C.OPER || '   SAN ~' || C.ST_RT_SAN) AS STATE,
       B.CALL_LABEL, 
       A.CATEGORY, 
       A.ST_RT_SAN, 
       A.MFD_KEY
FROM BAW_FLOW_FUNCTION_ASSIGNMENT A, 
      BAW_FUNCTION_ORG B, 
      BAW_STATE_ROUTE_OPER C
WHERE A.MFD_KEY = B.MFD_KEY
  AND A.ST_RT_SAN = C.ST_RT_SAN
  AND A.IS_DELETED = 'N' 
  AND B.IS_DELETED = 'N';


--------------------------------------------------------------------------------
/****12/21/2016 Meeting Notes:
Flow functino assignment ID 265
-- For TrimTarget: don't pick any Pending Revision anymore( only pick between BETA or RELEASED)
-- For TrimTemplates: if Released or BETA, pick BETA. Don't pick any pending(same as TrimTarget).
-- Change input type from pending to BETA.
-- Bugzilla 1184: add option to disable matlab crash report.
-- Bugzilla 1205: 
-- Bugzilla 1206: 
****/
--------------------------------------------------------------------------------01/16/2016
SELECT W.NODE_ID
      ,W.FIELD_NAME 
FROM BAW__MATFILES L, 
     BAW__MATFILES S, 
     BAW__MATFILES W
WHERE L.FIELD_NAME = '1634413'
AND S.PARENT_ID = L.NODE_ID
AND S.FIELD_NAME = 'AFTER_GRINDING'
AND S.IS_ROOT = 'Y'
AND W.PARENT_ID = S.NODE_ID
AND W.FIELD_NAME IN ('1', '2', '3', '4', '5', '6', '7', '8',
                   '9', '10', '11', '12', '13', '14', '15',
                   '16', '17', '18', '19', '20', '21', '22', '23', '24');
----------------------------------------------------------------------
--02/07/2017
SELECT DISTINCT (A.STATE
  || '  -  '
  || A.SUB_STATE
  || ' <==> '
  || A.ROUTE
  || '  -  '
  || A.OPER
  || '   SAN ~'
  || A.ST_RT_SAN) AS STATE,
  A.ST_RT_SAN
FROM  BAW_STATE_ROUTE_OPER A
WHERE A.SUB_STATE IS NOT NULL
AND UPPER(A.SUB_STATE) <> '(NULL)'
ORDER BY STATE; 
--03/17/2017:
SELECT DISTINCT BMF.FIELD_NAME,
  BMF.NODE_ID  AS BINARY_ID,
  BMFT.NODE_ID AS TEXT_ID,
  BPD.DESCRIPTION,
  BPD.DESIGNER,
  BPD.CUSTOMER,
  BPD.PROGRAM_MANAGER,
  BPD.PROCESS_TYPE,
  BPD.SHARED_RETICLES_MASKS,
  BPD.CTR_FREQ_GRP,
  BPD.DIE_ID,
  BPD.TRIM_ENGINEER,
  BPD.TRIM_ENG_EMAIL,
  BPD.BACKUP_TRIM_ENGINEER,
  BPD.BACKUP_TRIM_ENG_EMAIL
FROM BAW__MATFILES BMF,
  BAW__MATFILES_TEXT BMFT,
  BAW_VALID_PRODUCTS BVP,
  V_BAW_PRODUCT_INFORMATION BPD
WHERE BMF.PARENT_ID  = 0
AND BMF.FIELD_NAME   = BMFT.FIELD_NAME (+)
AND BMF.FIELD_NAME   = BVP.PRODUCT
AND BMF.FIELD_NAME   = BPD.PRODUCT (+)
AND BMF.NODE_ID NOT IN
  (SELECT NODE_ID FROM BAW_LOT_ACTIVITY WHERE DISPLAY_FLAG = 'N'
  )
ORDER BY BMF.FIELD_NAME;
----------------------------------------------------------------------
SELECT DISTINCT BMF.FIELD_NAME,
  BMF.NODE_ID  AS BINARY_ID,
  BMFT.NODE_ID AS TEXT_ID,
  BPD.DESCRIPTION,
  BPD.DESIGNER,
  BPD.CUSTOMER,
  BPD.PROGRAM_MANAGER,
  BPD.PROCESS_TYPE,
  BPD.SHARED_RETICLES_MASKS,
  BPD.CTR_FREQ_GRP,
  BPD.DIE_ID,
  BPD.TRIM_ENGINEER,
  BPD.TRIM_ENG_EMAIL,
  BPD.BACKUP_TRIM_ENGINEER,
  BPD.BACKUP_TRIM_ENG_EMAIL
FROM BAW__MATFILES BMF 
  LEFT OUTER JOIN  BAW__MATFILES_TEXT BMFT
    ON BMF.FIELD_NAME   = BMFT.FIELD_NAME
  INNER JOIN BAW_VALID_PRODUCTS BVP
    ON BMF.FIELD_NAME   = BVP.PRODUCT
  LEFT OUTER JOIN V_BAW_PRODUCT_INFORMATION BPD
    ON BMF.FIELD_NAME   = BPD.PRODUCT
WHERE BMF.PARENT_ID  = 0
AND BMF.NODE_ID NOT IN
  (SELECT NODE_ID FROM BAW_LOT_ACTIVITY WHERE DISPLAY_FLAG = 'N'
  )
ORDER BY BMF.FIELD_NAME;

--03/24/2017:
SELECT * FROM BAW_FLOW_FUNCTION_ASSIGNMENT;
SELECT * FROM BAW_FLOW_GROUP;
--------------------------------------------------------------------------------
SELECT A.GROUP_ID, 
       A.MFD_KEY, 
       A.LOT, 
       A.ST_RT_SAN, 
       A.CATEGORY,
       A.DISPLAY_ORDER
FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
WHERE A.IS_DELETED = 'N'
      AND A.DELETED_TIMESTAMP IS NULL
      AND EXISTS(
      SELECT * 
      FROM BAW_FLOW_FUNCTION_ASSIGNMENT B
      WHERE A.MFD_KEY = B.MFD_KEY
      AND A.LOT = B.LOT
      AND A.ST_RT_SAN = B.ST_RT_SAN
      AND A.CATEGORY = B.CATEGORY
      AND A.DISPLAY_ORDER = B.DISPLAY_ORDER
      AND A.GROUP_ID <> B.GROUP_ID
      AND B.IS_DELETED = 'N'
      AND B.DELETED_TIMESTAMP IS NULL)
ORDER BY GROUP_ID;
/*5, 120, 125, 132, 976, 1154, 1294, 1593, 3611, 3631, 3795, 3796, 3797*/
--------------------------------------------------------------------------------
--04/14/2017 Fix for lot 1706242 for Charlie:
SELECT * FROM BAW__MATFILES WHERE FIELD_NAME = '1706242'; -- NODE_ID = 86961434
SELECT * FROM BAW__MATFILES WHERE FIELD_NAME = 'MISSING' AND PARENT_ID = 86961434;

SELECT * FROM BAW__MATFILES WHERE PARENT_ID = 89552250;

SELECT * FROM BAW__MATFILES WHERE FIELD_NAME = 'AFTER_TD_TRIM' AND PARENT_ID = 86961434;
--------------------------------------------------------------------------------
SELECT * FROM BAW__MATFILES WHERE PARENT_ID = 89552250; --(89552251, 89564329 HAD PARENT_ID (89552250, 89552250))
SELECT * FROM BAW__MATFILES WHERE PARENT_ID = 89267843;
UPDATE BAW__MATFILES SET PARENT_ID = 89267843
WHERE NODE_ID IN (89552251, 89564329);
-------
UPDATE BAW__MATFILES SET IS_DELETED = 'Y'
WHERE NODE_ID = 89552250;
LOT: 1705141 AFTER_TB_TRIM
--------------------------------------------------------------------------------
SELECT * FROM 
(
SELECT A.GROUP_ID,
  A.MFD_KEY,
  A.LOT,
  A.ST_RT_SAN,
  A.CATEGORY,
  A.DISPLAY_ORDER,
  A.IS_DELETED,
  B.GROUP_ID AS FLOW_GROUP_ID, 
  C.CALL_LABEL, 
  C.IS_DELETED AS FUNC_ORG_IS_DELETED
FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
     LEFT JOIN (SELECT DISTINCT GROUP_ID FROM BAW_FLOW_GROUP) B
         ON A.GROUP_ID = B.GROUP_ID
     INNER JOIN BAW_FUNCTION_ORG C
         ON A.MFD_KEY = C.MFD_KEY
ORDER BY A.GROUP_ID)XX
WHERE XX.FLOW_GROUP_ID IS NULL
--AND XX.IS_DELETED = 'N';

----------------------------------------------------------------------
/*
- Meeting minutes 02/09/2017
- Fix issue with SWR info for a lot and a child lot.
	-  [temp m ] = unique({InVariable2.Lot})
	- unique({InVariable2.Lot})
	- Sample Lot = 1623945 (to be used for debugging)
- Fix issue when copying flow function assingment to choose category based on function selection.
- Lock Mechinism still need to be worked on.
- need to be more smarter when prompting user to choose user rights.
	- When selected the rights on functions, user was not able to modify anything.
	- Need to take a look at assignment with group_id = 4333 with new interface with role of ENGINEER.
- Check if user is able to break the assignment with * in products.
- Need to add category column to the source function.
- Update insert sql for techno_list with correct values from Fabien's Email.
- Get rid of close button in the TrimTemplates' Technology list dialog box.
- Get with David on the stored procedure.
- Add Last_updated column in the BAW_PC table.

Meeting notes (02/23/2017):
- Slowness, may be database or maybe netapp. 
- Need to get Alan Davidson involved as well.
- User don't exit BawDataViewer application on thier remote machines. 
	- They leave it up and running.
- Looking at what we see these days, the issues with slowness started very recently.
- Flow Function assignment needs more thorough testing.
- Oracle connection related codes needs more thorough testing.
- The current BETA 1.5.23.11 may very well be realeased to production early next week.
- Server Pcs are here and testing will begin on those pcs very soon.
- DV_PRODSETUP: need to add codes to set the main flag and email notification in case an exception is thrown.
- Get with David about assigning special rights for SWR lot.
- Check how Eval wafer function works in terms of checking matlab version and setting the paths.

Meeting With Scott: (03/13/2017)
- All Scripts and ERD Diagram are saved on shared drive under Scott Hawkins folder.
- Update to Baw_Function_Definition window to add 3 more fields:
	Run_on_Server/Run_on_Client and Server Type.
	- Save to table BAW_FUNCTION_VERSION
	- Baw_Svr_Server_TYpe table has lookup values of server type.
- Look for TODO: in the sql scripts.
- Is it ok to test in production while we develop?
- Which window to update first?
	- Function Editor.
	- Admin window:
		- Add items as needed.
		- Manual Execution window.
		- Custom batch window can be done at the last.
		- 
- 
03/16/2017: Issue with QueryParam need immediate release.
	- UDV slowness loading/closing/cancelling UDV Editor and saving of course!
	- UDV issue with route/oper select and unselecting items displays * when it should be a list of items.
	- 
------------------------
03/23/2017: Weekly Meeting:
- Need to handle param lock for output parameters.
- What to do whith copy product while I am testing?
incldue in BETA:	
				- Flow function conflicting message. 
				- Improve query on Tree Load.
				- UDV Conflict Message
				- Flow Function Assignment Message.

				
				
04/06/2017: Meeting Notes
- Need a column that indicates stop-taking-jobs for updates.
- 
				
*/
--------------------------------------------------------------------------------
SELECT *
FROM LOG_UDV_ASSIGNMENT_TEST
WHERE 
      DBMS_LOB.SUBSTR(PRODUCT, LENGTH(PRODUCT), 1) LIKE 'EG%';
---------------------------------------------------------------------------------------------------------------------------

/*
server: dfw3camdb21 - dev server
server: dfw0qsap01 - production server
login: camstart_mes
pass: camstart_mes_secure
*/
SELECT distinct "routename"
FROM appiansec."nproutename"@OREO_BAW
WHERE "technologyid" IN
  (SELECT "technologyid"
  FROM APPIANSEC."npfabtechnology"@OREO_BAW
  WHERE "technologyname" = 'BAW')
ORDER BY "routename";
------------------
SELECT DISTINCT 
       A.REWORK_ROUTE,
       A.ROUTE, 
       A.OPER, 
       A.OPER_ATT_ALLOWED, 
       A.FACILITY
FROM PUB.V_ALLOWED_REWORK@OREO_BAW A, 
     (SELECT distinct "routename"
                FROM appiansec."nproutename"@OREO_BAW
                WHERE "technologyid" IN
                  (SELECT "technologyid"
                   FROM APPIANSEC."npfabtechnology"@OREO_BAW
                   WHERE "technologyname" = 'BAW'
                   )
                ORDER BY "routename") B
WHERE A.ROUTE = B."routename"
AND A.REWORK_ROUTE IS NOT NULL
AND A.ROUTE = 'BAWBOALCU';
----------------------
--UPDATED Metrology query for gridView (double-click on treeView tool_id)
SELECT to_number(wafer_id) AS SLOT,
  b.in_date_time,
  c.lot_id,
  d.meas_number,
  d.meas_event_type,
  d.wafer_mass_dv,
  d.wafer_weight_dv,
  d.temperature_dv,
  d.temp_trend_dv,
  d.humidity_dv,
  d.wafer_result,
  d.process_program_selected,
  b.tool_id
FROM metryx_data_hdr a,
  lot_state_history b,
  lot c,
  metryx_wafer_meas d
WHERE upper(c.lot_id) LIKE upper('1710516%')
AND c.lot_pk               = b.lot_fk
AND wafer_id               = '01'
AND a.lot_state_history_fk = b.lot_state_history_pk
AND b.lot_state            = 'AFTER_M1_DEP'
AND d.metryx_data_hdr_fk   = a.metryx_data_hdr_pk
AND b.in_date_time         =
  (SELECT MAX(b.in_date_time)
  FROM metryx_data_hdr a,
    lot_state_history b,
    lot c,
    metryx_wafer_meas d
  WHERE upper(c.lot_id) LIKE upper('1710516%')
  AND c.lot_pk               = b.lot_fk
  AND wafer_id               = '01'
  AND a.lot_state_history_fk = b.lot_state_history_pk
  AND B.TOOL_ID              = 'MX005'
  AND b.lot_state            = 'AFTER_M1_DEP'
  AND d.metryx_data_hdr_fk   = a.metryx_data_hdr_pk
  )
ORDER BY wafer_id,
  d.meas_event_type,
  d.meas_number;
  -----------------------
SELECT --SESSION_WAFER_KEY, 
       --TOOL_ID, 
       --LOT_ID, 
       --PARENT_LOT, 
       --ROUTE, 
       --OPERATION, 
       --BAW_LOT_STATE, 
       WAFER_NUM AS SLOT, 
       WAFER_START_DT AS IN_DATE_TIME,
       LOT_ID,
       MEASUREMENT_NUMBER AS MEAS_NUMBER,
       WAFER_MASS_DV,
       WAFER_WEIGHT_DV,
       TEMPERATURE_DV,
       TEMP_TREND_DV,
       HUMIDITY_DV,
       WAFER_RESULT,
       TOOL_ID,
       RECIPE,  
       PRESSURE_DV
FROM SECSHOST.V_SESS_METRYX_LOT_DATA_PIVOT@OREO_BAW
WHERE PARENT_LOT LIKE '1710516%'
AND WAFER_NUM = '1'
AND TOOL_ID = 'MX005'
AND SESSION_KEY IN (SELECT MAXSESSKEY FROM SECSHOST.V_SESS_METRYX_MAX_KEY@OREO_BAW
                    WHERE PARENT_LOT  = '1710516'
                    AND BAW_LOT_STATE = 'AFTER_M1_DEP');
--------------------------------------------------------------------------------
CREATE OR REPLACE PROCEDURE PROC_MERGE_FLOW_FUNC_ASMTS
AS
    TYPE T_LIST IS TABLE OF NUMBER INDEX BY PLS_INTEGER;
    V_GROUP_ID_LIST T_LIST;
    V_OTHER_ASSIGN_GROUP_ID NUMBER;
    V_INDEX                 NUMBER;
    V_CURRENT_GROUP_ID      NUMBER;
    V_COUNTER               NUMBER;
    
    V_MFD_KEY BAW_FLOW_FUNCTION_ASSIGNMENT.MFD_KEY%TYPE;
    V_LOT BAW_FLOW_FUNCTION_ASSIGNMENT.LOT%TYPE;
    V_ST_RT_SAN BAW_FLOW_FUNCTION_ASSIGNMENT.ST_RT_SAN%TYPE;
    V_CATEGORY BAW_FLOW_FUNCTION_ASSIGNMENT.CATEGORY%TYPE;
    V_DISPLAY_ORDER BAW_FLOW_FUNCTION_ASSIGNMENT.DISPLAY_ORDER%TYPE;
    V_PRODUCT_LIST VARCHAR2(4000);
    V_ERR_MSG      VARCHAR2(4000);
    
    /* This procedure performs the following:
    - Delete and log the assignment if there's no PRODUCT associated to it.
    - Delete and lot the assignment if ST_RT_SAN = -1;
    - Merge the assignment if there's existing assignment and delete the other one.
    - What to do if there's assignment with specific lot when:
		- assignment has asterisk (*) for PRODUCT?
		- assignment has no PRODUCT?
		- assignment has more than one PRODUCT?
		
    SELECT * FROM BAW_FLOW_GROUP WHERE GROUP_ID IN (12147, 12563) ORDER BY GROUP_ID DESC, PRODUCT;
    SELECT * FROM BAW_FLOW_FUNCTION_ASSIGNMENT WHERE GROUP_ID IN (12147, 12563);
    */
BEGIN
  BEGIN
    V_INDEX := 1;
    --Getting a list of identical assignments
    FOR IDENTICAL_FLOW_FUNC_ASSGN_REC IN
    (SELECT                            *
    FROM
      (SELECT A.GROUP_ID,
        A.MFD_KEY,
        A.LOT,
        A.ST_RT_SAN,
        A.CATEGORY,
        A.DISPLAY_ORDER
      FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
      WHERE A.IS_DELETED       = 'N'
      AND A.DELETED_TIMESTAMP IS NULL
      AND EXISTS
        (SELECT *
        FROM BAW_FLOW_FUNCTION_ASSIGNMENT B
        WHERE A.MFD_KEY          = B.MFD_KEY
        AND A.LOT                = B.LOT
        AND A.ST_RT_SAN          = B.ST_RT_SAN
        AND A.CATEGORY           = B.CATEGORY
        AND A.DISPLAY_ORDER      = B.DISPLAY_ORDER
        AND A.GROUP_ID          <> B.GROUP_ID
        AND B.IS_DELETED         = 'N'
        AND B.DELETED_TIMESTAMP IS NULL
        )
      ORDER BY GROUP_ID DESC
      )
    WHERE GROUP_ID IN (9217, 6293)
      --(12105 AND 16024 = 100, 11038 AND 12104 = 100, 6444 AND 11037 = 100, 98 AND 6293 = 100
    )
    LOOP
      V_GROUP_ID_LIST(V_INDEX) := IDENTICAL_FLOW_FUNC_ASSGN_REC.GROUP_ID;
      V_INDEX                  := V_INDEX + 1;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('--- BEGIN ---');
    
    IF(V_GROUP_ID_LIST.COUNT > 0)THEN
      FOR i IN 1 .. V_GROUP_ID_LIST.COUNT
      LOOP

        V_COUNTER               := NULL;
        V_OTHER_ASSIGN_GROUP_ID := NULL;
        V_CURRENT_GROUP_ID      := V_GROUP_ID_LIST(i);
        
        SELECT MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER 
        INTO V_MFD_KEY, V_LOT, V_ST_RT_SAN, V_CATEGORY, V_DISPLAY_ORDER
        FROM BAW_FLOW_FUNCTION_ASSIGNMENT
        WHERE GROUP_ID = V_CURRENT_GROUP_ID;
        
        DBMS_OUTPUT.PUT_LINE('--- Processing for Group_ID ' || TO_CHAR(V_CURRENT_GROUP_ID) || ' ---');
        
        SELECT COUNT(*)
        INTO V_COUNTER
        FROM BAW_FLOW_GROUP
        WHERE GROUP_ID = V_CURRENT_GROUP_ID;
        
        --Delete the assignment if there's no PRODUCT associated to it.
        IF(V_COUNTER = 0)THEN
          DBMS_OUTPUT.PUT_LINE('Assignment with Group_ID ' || TO_CHAR(V_CURRENT_GROUP_ID) || ' will be deleted as there were no products found in this assignment!');
          /*TODO: Un-Comment these lines!
          DELETE FROM BAW_FLOW_FUNCTION_ASSIGNMENT WHERE GROUP_ID = V_CURRENT_GROUP_ID;
          DELETE FROM BAW_FLOW_GROUP WHERE GROUP_ID = V_CURRENT_GROUP_ID;
          INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
          VALUES(V_CURRENT_GROUP_ID, 'PROC_MERGE_FLOW_FUNC_ASMTS', 'DB', NULL, V_LOT, V_ST_RT_SAN, V_MFD_KEY, V_CATEGORY, V_DISPLAY_ORDER, 'DELETE', CURRENT_TIMESTAMP, NULL, 'Deleted due to no PRODUCTS.');
          */
          CONTINUE;
        END IF;
        
        --Delete the assignment if there's RT_ST_SAN = -1
        V_COUNTER := NULL;
        SELECT COUNT(*)
        INTO V_COUNTER
        FROM BAW_FLOW_FUNCTION_ASSIGNMENT
        WHERE GROUP_ID = V_CURRENT_GROUP_ID
        AND ST_RT_SAN  = -1;
        
        IF(V_COUNTER   = 1)THEN
          DBMS_OUTPUT.PUT_LINE('Assignment with Group_ID ' || TO_CHAR(V_CURRENT_GROUP_ID) || ' will be deleted as ST_RT_SAN = -1');
          V_PRODUCT_LIST := NULL;
          FOR PROD_REC IN
          ( 
            SELECT PRODUCT FROM BAW_FLOW_GROUP WHERE GROUP_ID = V_CURRENT_GROUP_ID
          )
          LOOP
            IF(LENGTH(V_PRODUCT_LIST) < 3990)THEN
              V_PRODUCT_LIST         := V_PRODUCT_LIST || PROD_REC.PRODUCT || ', ';
            END IF;
          END LOOP;
          
          /*TODO: Un-Comment these lines!
          DELETE FROM BAW_FLOW_FUNCTION_ASSIGNMENT WHERE GROUP_ID = V_CURRENT_GROUP_ID;
          DELETE FROM BAW_FLOW_GROUP WHERE GROUP_ID = V_CURRENT_GROUP_ID;
          INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
          VALUES(V_CURRENT_GROUP_ID, 'PROC_MERGE_FLOW_FUNC_ASMTS', 'DB', V_PRODUCT_LIST, V_LOT, V_ST_RT_SAN, V_MFD_KEY, V_CATEGORY, V_DISPLAY_ORDER, 'DELETE', CURRENT_TIMESTAMP, NULL, 'Deleted due to ST_RT_SAN = -1.');
          */
          CONTINUE;
        END IF;
        
        --Get only assignment with list of products
        V_COUNTER := NULL;
        
        SELECT COUNT(*)
        INTO V_COUNTER
        FROM BAW_FLOW_GROUP
        WHERE GROUP_ID = V_CURRENT_GROUP_ID
        AND PRODUCT   <> '*';
        
        IF(V_COUNTER   > 0)THEN
          --Get other assignments that are identical to assignment with V_CURRENT_GROUP_ID
          V_COUNTER := NULL;
          SELECT X.GROUP_ID
          INTO V_OTHER_ASSIGN_GROUP_ID
          FROM BAW_FLOW_FUNCTION_ASSIGNMENT X,
            (SELECT A.MFD_KEY,
              A.ST_RT_SAN,
              A.LOT,
              A.CATEGORY,
              A.DISPLAY_ORDER
            FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
            WHERE A.GROUP_ID = V_CURRENT_GROUP_ID
            AND A.IS_DELETED = 'N'
            ) XX
          WHERE X.MFD_KEY     = XX.MFD_KEY
          AND X.LOT           = XX.LOT
          AND X.CATEGORY      = XX.CATEGORY
          AND X.DISPLAY_ORDER = XX.DISPLAY_ORDER
          AND X.ST_RT_SAN     = XX.ST_RT_SAN
          AND X.GROUP_ID     <> V_CURRENT_GROUP_ID
          AND X.IS_DELETED    = 'N'
          AND ROWNUM          = 1;
          
          DBMS_OUTPUT.PUT_LINE('Group_IDs: {' || TO_CHAR(V_CURRENT_GROUP_ID) || '} and {' || TO_CHAR(V_OTHER_ASSIGN_GROUP_ID) || '} are identical.');
          
          SELECT COUNT(*) INTO V_COUNTER
          FROM
            ( SELECT DISTINCT YY.GROUP_ID AS OTHER_ASSIGNMENT_GROUP_ID
            FROM
              (SELECT X.GROUP_ID
              FROM BAW_FLOW_FUNCTION_ASSIGNMENT X,
                (SELECT A.MFD_KEY,
                  A.ST_RT_SAN,
                  A.LOT,
                  A.CATEGORY,
                  A.DISPLAY_ORDER
                FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
                WHERE A.GROUP_ID = V_CURRENT_GROUP_ID
                AND A.IS_DELETED = 'N'
                ) XX
              WHERE X.MFD_KEY     = XX.MFD_KEY
              AND X.LOT           = XX.LOT
              AND X.CATEGORY      = XX.CATEGORY
              AND X.DISPLAY_ORDER = XX.DISPLAY_ORDER
              AND X.ST_RT_SAN     = XX.ST_RT_SAN
              AND X.GROUP_ID     <> V_CURRENT_GROUP_ID
              AND X.IS_DELETED    = 'N'
              ) YY,
              BAW_FLOW_GROUP Z
            WHERE Z.GROUP_ID = YY.GROUP_ID
            AND Z.PRODUCT   <> '*'
            AND ROWNUM       = 1
            );
          
          IF(V_COUNTER > 0)THEN
            
            V_OTHER_ASSIGN_GROUP_ID := NULL;
            
            SELECT DISTINCT YY.GROUP_ID
            INTO V_OTHER_ASSIGN_GROUP_ID
            FROM
              (SELECT X.GROUP_ID
              FROM BAW_FLOW_FUNCTION_ASSIGNMENT X,
                (SELECT A.MFD_KEY,
                  A.ST_RT_SAN,
                  A.LOT,
                  A.CATEGORY,
                  A.DISPLAY_ORDER
                FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
                WHERE A.GROUP_ID = V_CURRENT_GROUP_ID
                AND A.IS_DELETED = 'N'
                ) XX
              WHERE X.MFD_KEY     = XX.MFD_KEY
              AND X.LOT           = XX.LOT
              AND X.CATEGORY      = XX.CATEGORY
              AND X.DISPLAY_ORDER = XX.DISPLAY_ORDER
              AND X.ST_RT_SAN     = XX.ST_RT_SAN
              AND X.GROUP_ID     <> V_CURRENT_GROUP_ID
              AND X.IS_DELETED    = 'N'
              ) YY,
              BAW_FLOW_GROUP Z
            WHERE Z.GROUP_ID = YY.GROUP_ID
            AND Z.PRODUCT   <> '*'
            AND ROWNUM       = 1;
            
            /*now, check and see if other assignment contains * for PRODUCT?
              If, so, we need to delete the current assignment and keep the assignmen with asterisk(*) for PRODUCT.
            */
            
            FOR PROD_REC IN
            ( 
              SELECT PRODUCT FROM BAW_FLOW_GROUP WHERE GROUP_ID = V_CURRENT_GROUP_ID
            )
            LOOP
              
              V_COUNTER := NULL;
              
              SELECT COUNT(*)
              INTO V_COUNTER
              FROM BAW_FLOW_GROUP
              WHERE GROUP_ID = V_OTHER_ASSIGN_GROUP_ID
              AND PRODUCT    = PROD_REC.PRODUCT;
              
              IF(V_COUNTER   = 0)THEN
                NULL;
                /*TODO: Un-Comment these lines
                INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
                VALUES(V_OTHER_ASSIGN_GROUP_ID, PROD_REC.PRODUCT);
                INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
                VALUES(V_OTHER_ASSIGN_GROUP_ID, 'PROC_MERGE_FLOW_FUNC_ASMTS', 'DB', PROD_REC.PRODUCT, V_LOT, V_ST_RT_SAN, V_MFD_KEY, V_CATEGORY, V_DISPLAY_ORDER, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Added PRODUCT {' || PROD_REC.PRODUCT || '} from assignment {' || V_CURRENT_GROUP_ID  || '} for merge.');
                */
              END IF;
            END LOOP;
            /*TODO: Un-Comment these lines
            DELETE FROM BAW_FLOW_FUNCTION_ASSIGNMENT WHERE GROUP_ID = V_CURRENT_GROUP_ID;
            DELETE FROM BAW_FLOW_GROUP WHERE GROUP_ID = V_CURRENT_GROUP_ID;
            INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
            VALUES(V_CURRENT_GROUP_ID, 'PROC_MERGE_FLOW_FUNC_ASMTS', 'DB', NULL, V_LOT, V_ST_RT_SAN, V_MFD_KEY, V_CATEGORY, V_DISPLAY_ORDER, 'DELETE', CURRENT_TIMESTAMP, NULL, 'Assignment {' || V_CURRENT_GROUP_ID || '} deleted after merging with assignment {' || V_OTHER_ASSIGN_GROUP_ID || '}.');
            */
            DBMS_OUTPUT.PUT_LINE('Group_ID ' || TO_CHAR(V_CURRENT_GROUP_ID) || ' will get merged with GROUP_ID ' || TO_CHAR(V_OTHER_ASSIGN_GROUP_ID));
            DBMS_OUTPUT.PUT_LINE('Group_ID ' || TO_CHAR(V_CURRENT_GROUP_ID) || ' will be deleted after merging.');
          END IF;
        END IF;
      END LOOP;
    END IF;
    DBMS_OUTPUT.PUT_LINE('--- END ---');
  EXCEPTION
  WHEN OTHERS THEN
    V_ERR_MSG := NULL;
    V_ERR_MSG := 'An Error occured while Merging Flow Function Assignments. - ' || SQLCODE ||' -ERROR- '|| SQLERRM;
    RAISE_APPLICATION_ERROR(-20001, V_ERR_MSG);
    DBMS_OUTPUT.PUT_LINE(V_ERR_MSG);
  END;
END PROC_MERGE_FLOW_FUNC_ASMTS;
---------------------------------------------
DECLARE
       V_NEW_GROUP_ID NUMBER;
       TYPE T_LIST IS TABLE OF NUMBER INDEX BY PLS_INTEGER;
       V_GROUP_IDS T_LIST;
       V_INDEX NUMBER := 1;
BEGIN
      
      FOR MANAUL_ASSIGNMENT_REC IN 
      (
        SELECT GROUP_ID FROM BAW_FLOW_FUNCTION_ASSIGNMENT
        WHERE ST_RT_SAN IN (4018, -1)
      )
      LOOP
           V_GROUP_IDS(V_INDEX) := MANAUL_ASSIGNMENT_REC.GROUP_ID;
           V_INDEX := V_INDEX + 1;
      END LOOP;
      
      FOR i IN 1 .. V_GROUP_IDS.COUNT
      LOOP
          DELETE FROM BAW_FLOW_FUNCTION_ASSIGNMENT
          WHERE GROUP_ID = V_GROUP_IDS(i);
          
          DELETE FROM BAW_FLOW_GROUP
          WHERE GROUP_ID = V_GROUP_IDS(i);
      END LOOP;
      
      --1: Assignments with no PRODUCTs
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 10);
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', NULL, '*', 3334, 4018, 'PROD', 10, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with no PRODUCT for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 10);
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', NULL, '*', 3334, 4018, 'PROD', 10, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with no PRODUCT for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      --2: Assignments with ST_RT_SAN = -1
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', -1, 'PROD', 11);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, '*');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', '*', '*', -1, 4018, 'PROD', 11, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with ST_RT_SAN = -1 for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', -1, 'PROD', 11);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, '*');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', '*', '*', -1, 4018, 'PROD', 11, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with ST_RT_SAN = -1 for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      --3: Assignments with one with only a PRODUCT and other with more than one PRODUCTS.
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 12);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, 'EG0322');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', 'EG0322', '*', 3334, 4018, 'PROD', 12, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with ONE PRODUCT for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 12);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, 'EG0328');
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, 'EG0327');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', 'EG0328, EG0327', '*', 3334, 4018, 'PROD', 12, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with TWO PRODUCTS for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      --4: Assignments with one containing asterisk(*) and other conatining list of products.
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 13);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, '*');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', '*', '*', 3334, 4018, 'PROD', 13, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with asterisk(*) for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 13);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, 'EG9430');
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, 'EG9515');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', 'EG9430, EG9515', '*', 3334, 4018, 'PROD', 13, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with multiple PRODUCTS for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      --5: Assignments with both containing Asterisk(*) for product. - Delete one of them.
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 14);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, '*');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', '*', '*', 3334, 4018, 'PROD', 14, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with asterisk(*) for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
      V_NEW_GROUP_ID := NULL;
      SELECT BAW_FLOW_GROUP_ID_SEQ.NEXTVAL 
      INTO  V_NEW_GROUP_ID
      FROM DUAL;
      
      INSERT INTO BAW_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, MFD_KEY, LOT, ST_RT_SAN, CATEGORY, DISPLAY_ORDER)
      VALUES(V_NEW_GROUP_ID, 3334, '*', 4018, 'PROD', 14);
      
      INSERT INTO BAW_FLOW_GROUP(GROUP_ID, PRODUCT)
      VALUES(V_NEW_GROUP_ID, '*');
      
      INSERT INTO LOG_FLOW_FUNCTION_ASSIGNMENT(GROUP_ID, USERNAME, MACHINE, PRODUCTS, LOT, ST_RT_SAN, MFD_KEY, CATEGORY, DISPLAY_ORDER, ADD_DELETE_MODIFY, TIME_STAMP, DV_VERSION, COMMENTS)
      VALUES(V_NEW_GROUP_ID, 'Anonymous Block', 'DB', '*', '*', 3334, 4018, 'PROD', 14, 'INSERT', CURRENT_TIMESTAMP, NULL, 'Created with asterisk(*) for testing procedure {PROC_MERGE_FLOW_FUNC_ASMTS}');
      
END;
------------------------------------------------------------------------------
SELECT *
FROM
  (SELECT A.GROUP_ID,
    A.MFD_KEY,
    A.LOT,
    A.ST_RT_SAN,
    A.CATEGORY,
    A.DISPLAY_ORDER
  FROM BAW_FLOW_FUNCTION_ASSIGNMENT A
  WHERE A.IS_DELETED       = 'N'
  AND A.DELETED_TIMESTAMP IS NULL
  AND EXISTS
    (SELECT *
    FROM BAW_FLOW_FUNCTION_ASSIGNMENT B
    WHERE A.MFD_KEY          = B.MFD_KEY
    AND A.LOT                = B.LOT
    AND A.ST_RT_SAN          = B.ST_RT_SAN
    AND A.CATEGORY           = B.CATEGORY
    AND A.DISPLAY_ORDER      = B.DISPLAY_ORDER
    AND A.GROUP_ID          <> B.GROUP_ID
    AND B.IS_DELETED         = 'N'
    AND B.DELETED_TIMESTAMP IS NULL
    )
  ORDER BY GROUP_ID DESC
  );
------------------------------------------------------------------------------------------

select * 
from
    all_constraints 
where
    r_constraint_name in
    (select       constraint_name
    from
       all_constraints
    where
       table_name='<Table_Name>';
	  
-------------------------------------------------------------------------------------
/*
Meeting with Craig Hall: 07/12/2017:
-- Check for splits?
-- SWR conflicts to perform at the submit.
-- Split points will come later to add ability to provide links to attach documents. 
-- Documents should be able to open from MES application.
-- Be able to copy from existing SWR for more specific changes.
-- What happens to chid lot? does the child lot automatically gets the SWR that is assigned to parent lot?
	-- Rely on Operators following the instructions for splitting and applying approved SWR.
-- Need to segment this SWR for testing.
-- Need to get more people involved for more complex cases related to split points.
	-- Need to get them involved before we actually start implementing.
	-- Only Jerry, Jerry and Brian may not be the only ones whoe can sign off on it.
-- 



- Only initiator could edit an existing SWR.
- Need to get with Appu on Rejecting SWR and set its state to EDITING.
- Validate for 4000 characters for client side validation.
- Where to put common methods like removeLastIndex/FormatSQLString()???
- 


*/


SELECT ROUTE, 
       DISPLAY_SEQUENCE
FROM PUB.WIPPRR
WHERE PROD = (SELECT PROD FROM PUB.WIPLOT WHERE LOT_NUMBER = '1710911')
ORDER BY DISPLAY_SEQUENCE, ROUTE;
--------------------------------------------------------------------------------
SELECT * FROM 
(SELECT A.STEPS,
       TRIM(TO_CHAR(A.MAIN_ROUTE_SEQ,'09')) || '.' || TRIM(TO_CHAR(A.SEQ_ORDER ,'09')) AS STEP_SEQ,
       A.FACILITY,
       A.LOT_NUMBER,
       A.PROD,
       A.SWR_ID,
       A.MAIN_ROUTE,
       A.MAIN_ROUTE_SEQ,
       A.IMPACT_STATUS,
       A.ROUTE,
       A.OPER,
       A.SEQ_ORDER,
       B.TRANS_TYPE,
       B.HOLD_FLAG,
       B.SPLIT_FLAG,
       B.INSTRUCTIONS AS MVIN_MVOUT_INSTRUCTIONS,
       TT.TOOLS_LIST,
       TT.RECIPE_LIST
      FROM V_KM_B A
      LEFT JOIN SR_ROUTE_OPER_INSTRUCTION B
      ON A.ROUTE_ORDER_ID = B.ROUTE_ORDER_ID
      LEFT JOIN (SELECT ROUTE_ORDER_ID, 
                   LISTAGG(T.TOOL || ',') WITHIN GROUP (ORDER BY T.ROUTE_ORDER_ID) AS TOOLS_LIST,
                   DECODE(T.RECIPE, NULL, NULL, LISTAGG(T.TOOL || '|' || T.RECIPE || ',') WITHIN GROUP (ORDER BY T.ROUTE_ORDER_ID)) AS RECIPE_LIST
            FROM SR_TOOL T
            GROUP BY T.ROUTE_ORDER_ID, T.RECIPE) TT
            ON A.ROUTE_ORDER_ID = TT.ROUTE_ORDER_ID
      ORDER BY A.MAIN_ROUTE_SEQ, A.SEQ_ORDER)
      WHERE LOT_NUMBER = '1710911';
-----------------------------------------------------------------------------------
/*
	Before check/in and deploying, needs to take care of the followings:
	
	- Add search criteria with E_SWR in flexigrid.
	- Allow editing based on the following 2 scenarios only:	
		- When initiator and user who is attempting to edit are same. 
	    - SWR is in state other than (Approved/AllApproved/Activated)
	- Updated views to filter data by State (Activated)
	- Trigger on all tables. (still working.... on it)
 	- What happens when an SWR is rejected? 
		- what table needs to be updated before updating SWR's state to "Editing"?
		
    08/22/2017:
	- Add search criteria with E_SWR in flexigrid.
	- Working on placing triggers to keep the history. (working on it)
		- on updates, keep the new values in history table.
	- What happens when an SWR is rejected? 
		- What table needs to be updated before updating SWR's state to "Editing"?
	- Need to take a closer look for the entire SWR-steps before deploying to test.
	- What to keep/remove? {table/accordian?}
	- 
	
	\\dfw0tfs01\Builds\DeployDfwMes.bat TEST DfwMes_2019.01.16.3
	08/25/2017:
	
	
	
	/*
	SELECT * FROM V_SRE_AFFECTED_LOTS 
	WHERE LOT_NUMBER = '1710911'
	ORDER BY SR_ID;
	/*
   SR_ID: paper_SWR(may be more than one as comma separated)
   BAW_ID: Baw SWRs(may be more than one as comma separated)
   SRE_IDs: E-SWR(may be more than one as comma separated)
   
  - Disable checkBox for lot if:
    - SR_IDs, [SRE_IDs and SRE_ROUTES are in conflict with current SWR] or BAW_ID are all not empty.
	
  08/30/2017:
  - Exclude all routes correcponding to selected product from appearing in add new dialog's selected route drop-down
  - for E-SWR, remove Hold/Split point routes selection.
	- Remove validation.
  - Update MVOUT to MVOU.
  
  09/06/2017: E-SWR demo meeting
	- On Tool/Param selection, no need validation on each main route.
	- 
  
  10/11/2017: swr meeting
  -- Need operation description in the select oper/tool details for each route.
-- What if they want to have a recipe for a tool that doesnt exist as yet.?
-- color code that ones that are modified.
-- warning before closing edit detail dialog.
-- what to do with more complex split.
-- provide more visual comparison (rather than just a link, approver still have to go through all operations and it is not very useful)
--

  
*/
---------------------------------------------------------------------------------------------------------

CREATE GLOBAL TEMPORARY TABLE BAW_TEMP_MATLAB_VER_CONTEXT
    (PRODUCT VARCHAR2(20),
     LOT VARCHAR2(20),
     STATE VARCHAR2(50),
     WAFER VARCHAR2(2),
     MFD_KEY NUMBER,
     MATLAB_VERSION_ID NUMBER
     )
  ON COMMIT DELETE ROWS;
  
SELECT * FROM BAW_TEMP_MATLAB_VER_CONTEXT;
---------------------------------------------------------------------------------------------------------
/*
Have David Fuller verify the following exist in the all web.config files

In ConnectionString Section:
        <!-- TQTBAW dev & prod-->
        <add name="TQTBAW.DEV" connectionString="Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = dfw0ora04b)(PORT = 1521)) ) (CONNECT_DATA = (SERVICE_NAME = DBAW) ) );User Id=bawviewertest;password=password;" />
        <add name="TQTBAW.PROD" connectionString="Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = dfw0ora03-scan.tqs.com)(PORT = 1521)) ) (CONNECT_DATA = (SERVICE_NAME = mfgdata.tqs.com) ) );User Id=BAWPRODCREATOR;password=BAWADMINTEX;" />
		
in AppSetting Section:
TEST web.config:       
<!-- perform BawDataViewer MATLAB checks-->
  <add key="Feature.BAW.VerifyMatlab"
	value="true"/>

in ALPHA:       
<!-- perform BawDataViewer MATLAB checks-->
  <add key="Feature.BAW.VerifyMatlab"
	value="false"/>

PRODUCTION web.config:       
<!-- perform BawDataViewer MATLAB checks-->
  <add key="Feature.BAW.VerifyMatlab"
	value="false"/>	
*/
-----------------------------------------------------------------------------------------------
/*
B 8" baw
H 6"
6" applies to baw
default is "T"
Database: pub.cost_tx1_material_view
cost_tx1_material_view
------------------------------------------------------------------------------------------------------
database: pub.v_ws_gaasmw_routing_qorvo (put these two views into source controls for comparison)
database: pub.v_ws_gaasmw_routing_qorvo_4:
line#306 is new "superRouter" to include all cost centers(ex, if there was an swr and lot did something different than the ordanility, SAP has a way to catch it)
pub.v_cost_yrouting_qovo_v4; updated since the last time to fix the bug.
cost_activy
yield
inverse of yield (AUKM)
-1 was to remove one from superRouter.
Randy found some discrepencies between yrouteting in production and V4.
need to fix the problem identified by Randy. 

v2 is in production.
v3 was to fix the Randy's issue (Final visual routes/)
need to update how the Final Visual routes are retrieved (dispatch_areas.p_area)
check email about Randy's concerns about outstanding issues in v3.


***
pub.v_ws_yrouting_input is the view that can give rel pick details for a given route.
***

dblinke to wareouse table is the only dblink in production but not in devtst1.
*/
-----------------------------------------------------------------------------------------------
-- MES <--> DV_CHECK
SELECT * FROM PUB.WIPWAF@OREO_BAW WHERE LAST_LOT = '1815724'; -- GETS WAFER NUMBERS
-----------------------------------------------------------------------------------------------
SELECT TO_TIMESTAMP(A.TRANSACTION_DATE_TIME, 'DD-Mon-YY HH24:MI:SS.FF') AS MVIN_TIME, A.* 
FROM PUB.WIPLTH@OREO_BAW A 
WHERE A.LOT_NUMBER = '1815724' 
AND A.ROUTE_OLD = 'BAWBKWB' 
AND A.OPER_OLD = 2842 
AND A.TRANSACTION = 'MVIN';

-----------------------------------------------------------------------------------------------
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
select CAST(transaction_date_time AS TIMESTAMP) AS TS
from pub.wiplth@oreo_baw
where lot_number = '1920805' 
and route_old = 'BAWFITRIM' 
and oper_old = 2519 
and transaction = 'MVIN' 
and transaction_date_time = 
         (select max(transaction_date_time) 
          from pub.wiplth@oreo_baw 
          where lot_number = '1920805' 
          and route_old = 'BAWFITRIM' 
          and oper_old = 2519
          and transaction = 'MVIN');
-----------------------------------------------------------------------------------------------				
SELECT LISTAGG(WAFER,', ') WITHIN GROUP(ORDER BY WAFER) AS FAILED_WAFERS_LIST
FROM BAW_MATLAB_EXECUTION_STATUS BMES
WHERE BMES.LOT = '1815722'
AND BMES.ROUTE = 'BAWFBTRIM2'
AND BMES.OPER = 2522
AND BMES.MFD_KEY = 4655
AND STATUS = 0
AND WAFER IS NOT NULL;
 -----------------------------------------------------------------------------------------------

/* Powershell Script:

<#
  File Name: Deploy_BawDataViewer_Service.ps1
  
  Description: 
  			  Deploys BawDataViewer Windows Service in 5 steps:	
				a. Stop the service.
				b. Uninstall the service 
				c. Copies new files to target pc
				d. Set Service-Mode to Automatic (Delayed Start)
				e. Re-start service.
				
  Usage: 
  	    1 - Open PowerShell as administrator.
		2 - Run the following command to load the script into memory:
				- <period><space><path to the file Deploy_BawDataViewer_Service.ps1> <ENTER>
				- Example: . C:\Users\km031369\Desktop\DataViewer\Deploy_BawDataViewer_Service.ps1 <ENTER>
			- Run the following command to see the Deploy_BawDataViewerService function:
				- <dir> <space> <function:\deploy*> 
				-Example: dir function:deploy_*
			- Now, run the following command to run the function to deploy Service to remote pc.
				- <FunctionName> <space> <parameterName> <parameterValue:Target ServerName>
				- Example: Deploy_BawDataViewerService -TargetServer DFWMATLAB-1D
		
  Important:
            - Make sure to build the Service project in Release mode for the latest code to deploy. 
  			- Make sure the path in variable "$serviceSourceFolderPath" is pointing to correct folder.
			- Make sure, you are admin on the Target PC.
			- Make sure that the folder with name & path "Users\svc_dfwmatlab\Documents\BawDataViewerService\"
			   exists on target pc and sharing is turned on for both Read/Write access to all users
  
  
  1 - . C:\Users\km031369\Desktop\DataViewer\Deploy_BawDataViewer_Service.ps1
  2 - dir function:deploy_*
  3 - Deploy_BawDataViewerService -TargetServer DFWMATLAB-1D
#> 

Matlab Command:

!matlab -regserver

*/
-----------------------------------------------------------------------------------------------
--Deleting orphaned records from BawDataViewer Matlab Server tables:
SELECT * FROM BAW_SVR_BATCH ORDER BY BATCH_ID;--163
SELECT * FROM BAW_SVR_JOB WHERE BATCH_ID = 163;
SELECT * FROM BAW_SVR_DEPENDENCY WHERE SOURCE_JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = 163);
SELECT * FROM BAW_SVR_PARAMETER WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = 163);
SELECT * FROM BAW_SVR_WILDCARD WHERE BATCH_ID = 163;
SELECT * FROM BAW_SVR_WILDCARD WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = 163);
SELECT * FROM BAW_SVR_WATCHER;
-----------------------------------------------------------------------------------------------
SET SERVEROUTPUT ON;
DECLARE
       V_BATCH_ID NUMBER := 11382;
BEGIN
      IF(V_BATCH_ID <> 11453 AND V_BATCH_ID <> 11231)THEN
          DELETE FROM BAW_SVR_WILDCARD WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = V_BATCH_ID);
          DELETE FROM BAW_SVR_WILDCARD WHERE BATCH_ID = V_BATCH_ID;
          DELETE FROM BAW_SVR_PARAMETER WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = V_BATCH_ID);
          DELETE FROM BAW_SVR_DEPENDENCY WHERE SOURCE_JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = V_BATCH_ID);
          DELETE FROM BAW_SVR_WATCHER WHERE BATCH_ID = V_BATCH_ID;
          DELETE FROM BAW_SVR_JOB WHERE BATCH_ID = V_BATCH_ID;
          DELETE FROM BAW_SVR_BATCH WHERE BATCH_ID = V_BATCH_ID;
          
          DBMS_OUTPUT.PUT_LINE('Completed AND Committed!'); 
          COMMIT;
      END IF;
END;

DECLARE
       V_BATCH_ID NUMBER := 160;
BEGIN
      DELETE FROM BAW_SVRLOG_WILDCARD WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID);
      DELETE FROM BAW_SVRLOG_WILDCARD WHERE BATCH_ID = V_BATCH_ID;
      DELETE FROM BAW_SVRLOG_PARAMETER WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID);
      DELETE FROM BAW_SVRLOG_DEPENDENCY WHERE SOURCE_JOB_ID IN (SELECT JOB_ID FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID);
      --DELETE FROM BAW_SVRLOG_WATCHER WHERE BATCH_ID = V_BATCH_ID;
      DELETE FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID;
      DELETE FROM BAW_SVRLOG_BATCH WHERE BATCH_ID = V_BATCH_ID;
     
      DBMS_OUTPUT.PUT_LINE('Completed AND Committed!'); 
      COMMIT;
END;

DECLARE
       V_BATCH_ID NUMBER := NULL;
BEGIN
      FOR SRV_LOG_BATCH_ID_REC IN 
      (
        SELECT BATCH_ID FROM BAW_SVRLOG_BATCH WHERE DATE_SUBMITTED < SYSDATE - 10 ORDER BY DATE_SUBMITTED ASC
      )
      LOOP
          V_BATCH_ID := SRV_LOG_BATCH_ID_REC.BATCH_ID;
          DELETE FROM BAW_SVRLOG_WILDCARD WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID);
          DELETE FROM BAW_SVRLOG_WILDCARD WHERE BATCH_ID = V_BATCH_ID;
          DELETE FROM BAW_SVRLOG_PARAMETER WHERE JOB_ID IN (SELECT JOB_ID FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID);
          DELETE FROM BAW_SVRLOG_DEPENDENCY WHERE SOURCE_JOB_ID IN (SELECT JOB_ID FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID);
          --DELETE FROM BAW_SVRLOG_WATCHER WHERE BATCH_ID = V_BATCH_ID;
          DELETE FROM BAW_SVRLOG_JOB WHERE BATCH_ID = V_BATCH_ID;
          DELETE FROM BAW_SVRLOG_BATCH WHERE BATCH_ID = V_BATCH_ID;
      END LOOP;
    
      COMMIT;
      DBMS_OUTPUT.PUT_LINE('Completed AND Committed!'); 
END;
-----------------------------------------------------------------------------------------------
SET SERVEROUTPUT ON;
DECLARE
    V_USER_ID VARCHAR2(30) := ' mking';
    V_COUNTER NUMBER;
BEGIN
     SELECT COUNT(*) INTO V_COUNTER
     FROM MES_OVERRIDE_AUTHORIZATIONS
     WHERE CHECK_NAME = 'ESWR'
     AND LOWER(USER_ID) = LOWER(TRIM(V_USER_ID));
     
     IF(V_COUNTER > 0)THEN
       DBMS_OUTPUT.PUT_LINE('User: {' || LOWER(TRIM(V_USER_ID)) || '} already exist!');
     ELSIF(V_COUNTER = 0)THEN
       
       INSERT INTO MES_OVERRIDE_AUTHORIZATIONS(CHECK_NAME, USER_ID, AUTHORIZED_BY, AUTHORIZED_DATE_TIME)
       VALUES('ESWR', LOWER(TRIM(V_USER_ID)), 'asaigal', SYSDATE);
        COMMIT;
        DBMS_OUTPUT.PUT_LINE('User: {' || LOWER(TRIM(V_USER_ID)) || '} added/transaction committed successfully!');
     END IF;
END;

SELECT * FROM MES_OVERRIDE_AUTHORIZATIONS WHERE CHECK_NAME = 'ESWR' ORDER BY AUTHORIZED_DATE_TIME DESC;
-----------------------------------------------------------------------------------------------
SET SERVEROUTPUT ON;
DECLARE
    V_USER_ID VARCHAR2(30) := 'pg035581';
    V_COUNTER NUMBER;
BEGIN
     SELECT COUNT(*) INTO V_COUNTER
     FROM MES_OVERRIDE_AUTHORIZATIONS
     WHERE CHECK_NAME = 'DV_OVERRIDE'
     AND LOWER(USER_ID) = LOWER(TRIM(V_USER_ID));
     
     IF(V_COUNTER > 0)THEN
       DBMS_OUTPUT.PUT_LINE('User: {' || LOWER(TRIM(V_USER_ID)) || '} already exist!');
     ELSIF(V_COUNTER = 0)THEN
       
       INSERT INTO MES_OVERRIDE_AUTHORIZATIONS(CHECK_NAME, USER_ID, AUTHORIZED_BY, AUTHORIZED_DATE_TIME)
       VALUES('DV_OVERRIDE', LOWER(TRIM(V_USER_ID)), 'wdostalik', SYSDATE);
        COMMIT;
        DBMS_OUTPUT.PUT_LINE('User: {' || LOWER(TRIM(V_USER_ID)) || '} added/transaction committed successfully!');
     END IF;
END;

SELECT * FROM MES_OVERRIDE_AUTHORIZATIONS WHERE CHECK_NAME = 'DV_OVERRIDE' ORDER BY AUTHORIZED_DATE_TIME DESC;
-----------------------------------------------------------------------------------------------
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
-----------------------------------------------------------------------------------------------
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
SELECT A.MFD_KEY,
       A.SERVER_TYPE_ID,
       B.MACHINE,
       B.LAST_HEARTBEAT
FROM 
      BAW_FUNCTION_VERSION A
      LEFT JOIN BAW_SVR_SERVER B
      ON A.SERVER_TYPE_ID = B.SERVER_TYPE_ID
WHERE A.MFD_KEY = 9498 
AND A.MATLAB_VERSION_ID = 4105
AND B.LAST_HEARTBEAT > (SYSDATE - (SELECT TRUNC(MOD(NUMERIC_VALUE, 3600)/60)AS MINUTES FROM BAW_GLOBAL_SETTINGS WHERE SETTING_NAME =  'HEARTBEAT_WAIT_TIME')); -- OR GET A HEARTBEAT THAT WAS SENT 5 MINUTES AGO I.E., (SYSDATE -5/24/60);
--This will also indirectly helps us detect if any server was down. 
-----------------------------------------------------------------------------------------------
SELECT TRUNC(MOD(NUMERIC_VALUE, 3600)/60)AS MINUTES
FROM BAW_GLOBAL_SETTINGS
WHERE SETTING_NAME =  'HEARTBEAT_WAIT_TIME';
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
--Set eSWR status to "Editing"
SELECT * FROM BUSINESS_PROCESS WHERE ID = 135349;
SELECT * FROM BUSINESS_PROCESS_APPROVER WHERE BP_ID = 135349;
SELECT * FROM BUSINESS_PROCESS_HISTORY WHERE BP_ID = 135349 ORDER BY TIMESTAMP;
--------------
UPDATE BUSINESS_PROCESS_HISTORY 
SET IS_DELETED = 'Y'
WHERE STATE = 'Approved'
AND BP_ID = 135349;
----------------------------
INSERT INTO BUSINESS_PROCESS_HISTORY
VALUES(BUSINESS_PROCESS_HIST_SEQ.NEXTVAL, 135349, 'Rejected', 'KM031369', 'IT', 'Rejected by Kashif Mubarak for testing', CURRENT_TIMESTAMP, 'N');
--
INSERT INTO BUSINESS_PROCESS_HISTORY
VALUES(BUSINESS_PROCESS_HIST_SEQ.NEXTVAL, 135349, 'Editing', 'SWR-SYSTEM', 'SWR-SYSTEM', 'Updated state to Editing by the system due to reject. SWR may now be re-submitted.', CURRENT_TIMESTAMP, 'N');
--
UPDATE BUSINESS_PROCESS 
SET STATE = 'Editing'
WHERE ID = 135349;
-----------------------------------------------------------------------------------------------
DECLARE
        OUT_STATUS NUMBER := NULL;
        OUT_MESSAGE VARCHAR2(4000) := NULL;
        OUT_ASSIGNED_VALUE VARCHAR2(4000) := NULL;
        OUT_ASSIGNMENT_ID NUMBER := NULL;
BEGIN
       UDV_RETRIEVE_VAL.GET_UDV_VALUE(
       'TRIMCHECK_FLD4RM' ,
       'EG2832' ,
       '1819726' ,
       'AFTER_FB_TRIM' ,
       '8FBTRIM2' ,
       2523 ,
       'TrimCheck', 
       OUT_STATUS,
       OUT_MESSAGE,
       OUT_ASSIGNED_VALUE,
       OUT_ASSIGNMENT_ID);

       IF(OUT_ASSIGNED_VALUE IS NULL)THEN
          DBMS_OUTPUT.PUT_LINE('Retrieved value was null!');
       END IF;

       DBMS_OUTPUT.PUT_LINE('STATUS: ' || TO_CHAR(OUT_STATUS) || CHR(13) ||
                            'MESSAGE: ' ||     OUT_MESSAGE || CHR(13) ||   
                            'VALUE: ' || OUT_ASSIGNED_VALUE ||  CHR(13) ||
                            'ASSIGNMENT_ID: ' || TO_CHAR(OUT_ASSIGNMENT_ID)); 
END;
-----------------------------------------------------------------------------------------------

/*
--Path to test server:
\\dfw0mfg14\c$\inetpub\wwwroot\DfwMesTestBranch
*/

DECLARE
       --TYPE T_STR_LIST IS TABLE OF VARCHAR2(255) INDEX BY PLS_INTEGER;
       IN_SWR_ID NUMBER := 19405;
       OUT_STATUS NUMBER := NULL;
       OUT_STATUS_MSG VARCHAR2(1000) := NULL;
       OUT_ERRORS E_SWR_PKG.T_STR_LIST;
BEGIN
       --DBMS_OUTPUT.PUT_LINE('hello!');      
       E_SWR_PKG.VALIDATE_SPLIT_MERGE(IN_SWR_ID, OUT_STATUS, OUT_STATUS_MSG, OUT_ERRORS);
       
       IF(OUT_ERRORS.COUNT > 0)THEN
          DBMS_OUTPUT.PUT_LINE(OUT_STATUS_MSG);
          FOR i IN OUT_ERRORS.FIRST .. OUT_ERRORS.LAST
          LOOP
               DBMS_OUTPUT.PUT_LINE(OUT_ERRORS(i));
          END LOOP;
       ELSE
          DBMS_OUTPUT.PUT_LINE(OUT_STATUS_MSG); 
       END IF;
END;
-----------------------------------------------------------------------------------------------
SELECT * FROM BAW_STATE_ROUTE_OPER
AS OF TIMESTAMP
TO_TIMESTAMP('2018-12-04 09:30:00', 'YYYY-MM-DD HH:MI:SS')
WHERE ST_RT_SAN = 11143;
-----------------------------------------------------------------------------------------------
--Press alt + space to bring off screen window to current desktop.
-----------------------------------------------------------------------------------------------
--get MVIN TIME
SELECT CAST(TRANSACTION_DATE_TIME AS TIMESTAMP) AS TRANSACTION_DATE_TIME 
FROM PUB.WIPLTH@OREO_BAW 
WHERE LOT_NUMBER  = '1831213' 
AND ROUTE_OLD = 'BAWPATRIM' 
AND OPER_OLD = 2522
AND TRANSACTION = 'MVIN' 
AND TRANSACTION_DATE_TIME = ( SELECT MAX(TRANSACTION_DATE_TIME) 
                              FROM PUB.WIPLTH@OREO_BAW
                              WHERE LOT_NUMBER = '1831213' 
                              AND ROUTE_OLD = 'BAWPATRIM' 
                              AND OPER_OLD = 2522 
                              AND TRANSACTION = 'MVIN');
-----------------------------------------------------------------------------------------------
DECLARE
       P_BUNDLE_NAME_IN VARCHAR2(30) := 'TRENDCHARTS_BUNDLE';
       P_PROD_IN_OUT VARCHAR2(30) := 'EG5428';
       P_LOT_IN_OUT VARCHAR2(30) := '1814221';
       P_STATE_IN_OUT VARCHAR2(30) := 'AFTER_ZA_TRIM';
       P_ROUTE_IN_OUT VARCHAR2(30) := '8ZAMILL';
       P_OPERATION_IN_OUT NUMBER := 2300;
       P_FUNCTION_IN VARCHAR2(30) := 'TrimCheck';
       P_STATUS_OUT NUMBER := NULL;
       P_MESSAGE_OUT VARCHAR2(400) := NULL; 
       P_VAR_NAMES_OUT BAW_UDV_BUNDLE_PKG.T_LIST; 
       P_ASSIGNMENT_VAL_OUT BAW_UDV_BUNDLE_PKG.T_LIST;  
       P_ASSIGNMENT_OUT BAW_UDV_BUNDLE_PKG.T_LIST; 
       P_VARTYPE_OUT BAW_UDV_BUNDLE_PKG.T_LIST; 
BEGIN
       BAW_UDV_BUNDLE_PKG.GET_UDV_BUNDLE_VALUES(
                P_BUNDLE_NAME_IN,
                P_PROD_IN_OUT,
                P_LOT_IN_OUT,
                P_STATE_IN_OUT,
                P_ROUTE_IN_OUT,
                P_OPERATION_IN_OUT,
                P_FUNCTION_IN,
                P_STATUS_OUT,
                P_MESSAGE_OUT, 
                P_VAR_NAMES_OUT, 
                P_ASSIGNMENT_VAL_OUT, 
                P_ASSIGNMENT_OUT,
                P_VARTYPE_OUT);
                         
         FOR i IN 1 .. P_ASSIGNMENT_OUT.COUNT 
         LOOP
              DBMS_OUTPUT.PUT_LINE(TO_CHAR(P_ASSIGNMENT_OUT(i)) || ' ' ||
                                   TO_CHAR(P_VAR_NAMES_OUT(i)) || ' ' ||
                                   TO_CHAR(P_VARTYPE_OUT(i)) || ' ' ||
                                   TO_CHAR(P_ASSIGNMENT_VAL_OUT(i)));   
         END LOOP;
                 
END;
-----------------------------------------------------------------------------------------------
SET SERVEROUTPUT ON;
DECLARE
       P_BUNDLE_NAME_IN VARCHAR2(30) := 'DFBUNDLE';
       P_PROD_IN_OUT VARCHAR2(30) := 'EG5641';
       P_LOT_IN_OUT VARCHAR2(30) := '1833852';
       P_STATE_IN_OUT VARCHAR2(30) := 'AFTER_PA_PATTERNING';
       P_ROUTE_IN_OUT VARCHAR2(30) := 'BAWPA';
       P_OPERATION_IN_OUT NUMBER := 2519;
       P_FUNCTION_IN VARCHAR2(30) := 'DeviceFitter [ResonatorMap]';
       P_STATUS_OUT NUMBER := NULL;
       P_MESSAGE_OUT VARCHAR2(400) := NULL; 
       
       V_REF_CURSOR BAW_UDV_BUNDLE_PKG.UDVBUNDLE_REF_CURSOR;
       
       V_VARIABLE_NAME BAWUDV_COLLECTION.VARIABLE_NAME%TYPE;
       V_ASSIGNMENT_VALUE BAWUDV_COLLECTION.ASSIGNMENT_VALUE%TYPE;
       V_VARIABLE_TYPE BAWUDV_COLLECTION.VARIABLE_TYPE%TYPE;
       V_ASSIGNMENT_ID BAWUDV_COLLECTION.ASSIGNMENT_ID%TYPE;
       
       V_INDEX_VAL BAWUDV_COLLECTION.INDEX_VAL%TYPE;
       V_SESSION_ID BAWUDV_COLLECTION.SESSION_ID%TYPE;
       V_INSTANCE_ID BAWUDV_COLLECTION.INSTANCE_ID%TYPE;
BEGIN
       BAW_UDV_BUNDLE_PKG.PROC_UDV_BUNDLE_REF_CURSOR(
                P_BUNDLE_NAME_IN,
                P_PROD_IN_OUT,
                P_LOT_IN_OUT,
                P_STATE_IN_OUT,
                P_ROUTE_IN_OUT,
                P_OPERATION_IN_OUT,
                P_FUNCTION_IN,
                P_STATUS_OUT,
                P_MESSAGE_OUT, 
                V_REF_CURSOR);
      
      LOOP
        FETCH V_REF_CURSOR INTO 
            V_INDEX_VAL,
            V_VARIABLE_NAME,
            V_ASSIGNMENT_VALUE,
            V_VARIABLE_TYPE,
            V_ASSIGNMENT_ID,
            V_SESSION_ID,
            V_INSTANCE_ID;
        EXIT WHEN V_REF_CURSOR%NOTFOUND;
        DBMS_OUTPUT.PUT_LINE(TO_CHAR(V_ASSIGNMENT_ID) || ' ' ||
                             TO_CHAR(V_VARIABLE_NAME) || ' ' ||
                             TO_CHAR(V_VARIABLE_TYPE) || ' ' ||
                             TO_CHAR(V_ASSIGNMENT_VALUE)
                             );   
      END LOOP;
      CLOSE V_REF_CURSOR;       
END;
-----------------------------------------------------------------------------------------------
--Find all reference to a procedure in the other objects in the Database:
SELECT * FROM all_source
where UPPER(TEXT) like UPPER('%BOBTEST2%');
-----------------------------------------------------------------------------------------------
DECLARE
       V_LOT VARCHAR2(20) := '1830512';
       V_ROUTE VARCHAR2(30) := 'BAWFITRIM';
       V_OPER NUMBER := 2522;
       V_PREV_OPER NUMBER := NULL;
       V_TIMESTAMP_ORIG TIMESTAMP := NULL;
       V_PREV_OPER_TIMESTAMP TIMESTAMP := NULL;
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
BEGIN
       
       SELECT CAST(TRANSACTION_DATE_TIME AS TIMESTAMP) AS TRANSACTION_DATE_TIME INTO V_TIMESTAMP_ORIG
       FROM PUB.WIPLTH@OREO_BAW 
       WHERE LOT_NUMBER  = V_LOT 
       AND ROUTE_OLD = V_ROUTE
       AND OPER_OLD = V_OPER
       AND TRANSACTION = 'MVIN' 
       AND TRANSACTION_DATE_TIME = 
                (SELECT MAX(TRANSACTION_DATE_TIME) 
                 FROM PUB.WIPLTH@OREO_BAW
                 WHERE LOT_NUMBER = V_LOT 
                 AND ROUTE_OLD = V_ROUTE 
                 AND OPER_OLD = V_OPER 
                 AND TRANSACTION = 'MVIN'
                  );
           
       DBMS_OUTPUT.PUT_LINE('TimeStamp for BAWFITRIM/2522: ' || V_TIMESTAMP_ORIG);    
                  
       SELECT RTO.PREVIOUS_OPER INTO V_PREV_OPER
       FROM PUB.WIPRTO@OREO_BAW RTO 
        LEFT JOIN PUB.WIPOPR@OREO_BAW OPR
            ON OPR.OPER = RTO.OPER
       WHERE UPPER(LONG_DESC) LIKE '%DATA CHECK%' 
       AND ROUTE = V_ROUTE
       AND RTO.OPER = V_OPER;
       
       SELECT CAST(TRANSACTION_DATE_TIME AS TIMESTAMP) AS TRANSACTION_DATE_TIME INTO V_PREV_OPER_TIMESTAMP
       FROM PUB.WIPLTH@OREO_BAW 
       WHERE LOT_NUMBER  = V_LOT 
       AND ROUTE_OLD = V_ROUTE
       AND OPER_OLD = V_PREV_OPER
       AND TRANSACTION = 'MVIN' 
       AND TRANSACTION_DATE_TIME = 
                (SELECT MAX(TRANSACTION_DATE_TIME) 
                 FROM PUB.WIPLTH@OREO_BAW
                 WHERE LOT_NUMBER = V_LOT 
                 AND ROUTE_OLD = V_ROUTE 
                 AND OPER_OLD = V_PREV_OPER 
                 AND TRANSACTION = 'MVIN'
                  );
       
       DBMS_OUTPUT.PUT_LINE('TimeStamp for BAWFITRIM/2350: ' || V_PREV_OPER_TIMESTAMP); 
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG); 
END;
-----------------------------------------------------------------------------------------------
DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       V_COUNTER NUMBER;
       V_GROUP_ID NUMBER := 264;
       V_GRP_SPCL_RIGHTS_ID NUMBER;
       V_INSERT_COUNTER NUMBER := 1;
BEGIN
      
       FOR EG2_PROD_REC IN 
       (
         SELECT DISTINCT FIELD_NAME
         FROM BAW__MATFILES
         WHERE PARENT_ID = 0
         AND IS_DELETED = 'N'
         AND FIELD_NAME LIKE 'EG2%'
       )
       LOOP
            V_COUNTER := NULL;
            V_GRP_SPCL_RIGHTS_ID := NULL;
            SELECT COUNT(*) INTO V_COUNTER
            FROM BAW_USER_GROUP_SPECIAL_RIGHTS
            WHERE GROUP_ID = V_GROUP_ID
            AND SCOPE = 'PRODUCT'
            AND FIELD_NAME = EG2_PROD_REC.FIELD_NAME;
            
            IF(V_COUNTER = 0)THEN
               --BAW_GROUP_DELEGATION_PKG.ADD_SPCL_RIGHTS_TO_GROUP(V_GROUP_ID, 'PRODUCT', EG2_PROD_REC.FIELD_NAME, 5, V_GRP_SPCL_RIGHTS_ID);
               DBMS_OUTPUT.PUT_LINE('Special rights added for Product ' || EG2_PROD_REC.FIELD_NAME || '. ID: ' || TO_CHAR(V_GRP_SPCL_RIGHTS_ID));
               V_INSERT_COUNTER := V_INSERT_COUNTER + 1;
            END IF;
       END LOOP; 
       --COMMIT;
       DBMS_OUTPUT.PUT_LINE('Total Products added: ' ||  TO_CHAR(V_INSERT_COUNTER));
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
END;
-----------------------------------------------------------------------------------------------
create or replace PROCEDURE PROC_TEST_KM
AS
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       V_COUNTER NUMBER := NULL;
       V_PURPOSE_GROUP VARCHAR2(80); 
       V_PURPOSE_PROJECT VARCHAR2(80); 
       V_PURPOSE_BAND VARCHAR2(80); 
       V_PURPOSE_TYPE VARCHAR2(80); 
       V_PURPOSE_FLOW VARCHAR2(80); 
       V_PURPOSE_DESCRIPTION VARCHAR2(4000);
       V_INDEX NUMBER := 1;
       V_SPLIT_VALUE VARCHAR2(4000);
BEGIN
     BEGIN
       FOR SWR_REC IN 
       (
         SELECT * FROM SWRMASTER 
         WHERE PURPOSE LIKE '%:%' 
         --AND SWR_ID >= 153073
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
                 V_COUNTER := NULL;
                 V_SPLIT_VALUE := NULL;
                 
                 IF(V_INDEX = 6)THEN
                     V_PURPOSE_DESCRIPTION := PURPOSE_REC.dat;
                 ELSE
                     V_SPLIT_VALUE := PURPOSE_REC.dat;
                     IF(V_INDEX = 1)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'DEPARTMENT';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_GROUP := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 2)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'PROJECT';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_PROJECT := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 3)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'BAND';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_BAND := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 4)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'TYPE';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_TYPE := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 5)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'FLOW';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_FLOW := UPPER(V_SPLIT_VALUE);
                         END IF;
                     END IF;
                 END IF;       
                 
                 V_INDEX := V_INDEX + 1;
            END LOOP;

           IF(V_PURPOSE_GROUP IS NOT NULL
              AND V_PURPOSE_PROJECT IS NOT NULL
              AND V_PURPOSE_BAND IS NOT NULL
              AND V_PURPOSE_TYPE IS NOT NULL
              AND V_PURPOSE_FLOW IS NOT NULL
              AND V_PURPOSE_DESCRIPTION IS NOT NULL)THEN

             /*
              DBMS_OUTPUT.PUT_LINE('GROUP: ' || V_PURPOSE_GROUP || 
                     ', PROJECT: ' || V_PURPOSE_PROJECT || 
                     ', BAND: ' || V_PURPOSE_BAND || 
                     ', TYPE: ' || V_PURPOSE_TYPE || 
                     ', FLOW: ' || V_PURPOSE_FLOW || 
                     ', DESC: ' || V_PURPOSE_DESCRIPTION ||
                     ', ID: ' || TO_CHAR(SWR_REC.SWR_ID));
              */
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
                      --NULL;
                      --/*
                      UPDATE SWRMASTER
                      SET PURPOSE_GROUP = TRIM(V_PURPOSE_GROUP),
                          PURPOSE_PROJECT = TRIM(V_PURPOSE_PROJECT),
                          PURPOSE_BAND = TRIM(V_PURPOSE_BAND),
                          PURPOSE_TYPE = TRIM(V_PURPOSE_TYPE),
                          PURPOSE_FLOW = TRIM(V_PURPOSE_FLOW),
                          PURPOSE_DESCRIPTION = TRIM(V_PURPOSE_DESCRIPTION)
                      WHERE SWR_ID =  SWR_REC.SWR_ID;
                      --*/
                   END IF;         
           ELSE
               NULL;
               --DBMS_OUTPUT.PUT_LINE('Invalid PURPOSE string for ID: ' || TO_CHAR(SWR_REC.SWR_ID) || ' --> ' || SWR_REC.PURPOSE );
           END IF;                     
       END LOOP;
     EXCEPTION
       	 WHEN OTHERS THEN
         V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
         DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
     END;
END;
--------------------------------------------------------------------------------------------------------------
DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
BEGIN
       --DBMS_OUTPUT.PUT_LINE('hello!');
       FOR SWR_REC IN
       (
         SELECT * FROM SWRMASTER 
         WHERE PURPOSE_GROUP IS NOT NULL
         AND PURPOSE_PROJECT IS NOT NULL
         AND PURPOSE_BAND IS NOT NULL
         AND PURPOSE_TYPE IS NOT NULL
         AND PURPOSE_FLOW IS NOT NULL
         AND PURPOSE_DESCRIPTION IS NOT NULL
         --AND PURPOSE IS NULL
         ORDER BY SWR_ID DESC
       )
       LOOP
            UPDATE SWRMASTER
            SET PURPOSE = SWR_REC.PURPOSE_GROUP || ':' ||
                          SWR_REC.PURPOSE_PROJECT || ':' ||
                          SWR_REC.PURPOSE_BAND || ':' ||
                          SWR_REC.PURPOSE_TYPE || ':' ||
                          SWR_REC.PURPOSE_FLOW || ':' ||
                          SWR_REC.PURPOSE_DESCRIPTION
            WHERE SWR_ID =  SWR_REC.SWR_ID;             
       END LOOP;
       COMMIT;
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
      ROLLBACK;
END;
--------------------------------------------------------------------------------------------------------------
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
       V_SPLIT_VALUE VARCHAR2(4000);
BEGIN
     BEGIN
       FOR SWR_REC IN 
       (
         SELECT * FROM SWRMASTER 
         WHERE PURPOSE LIKE '%:%' 
         AND SWR_ID >= 153073
         AND PURPOSE_GROUP IS NULL
         AND PURPOSE_PROJECT IS NULL
         AND PURPOSE_BAND IS NULL
         AND PURPOSE_TYPE IS NULL
         AND PURPOSE_FLOW IS NULL
         AND PURPOSE_DESCRIPTION IS NULL
         AND ORIGINATION_DATE_TIME > '01-JAN-19'
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
                 V_COUNTER := NULL;
                 V_SPLIT_VALUE := NULL;
                 
                 IF(V_INDEX = 6)THEN
                     V_PURPOSE_DESCRIPTION := PURPOSE_REC.dat;
                 ELSE
                     V_SPLIT_VALUE := PURPOSE_REC.dat;
                     IF(V_INDEX = 1)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'DEPARTMENT';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_GROUP := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 2)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'PROJECT';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_PROJECT := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 3)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'BAND';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_BAND := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 4)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'TYPE';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_TYPE := UPPER(V_SPLIT_VALUE);
                         END IF;
                     ELSIF(V_INDEX = 5)THEN
                         SELECT COUNT(*) INTO V_COUNTER
                         FROM R_AND_D_CATEGORIES
                         WHERE VAL = UPPER(V_SPLIT_VALUE)
                         AND UPPER(CAT) = 'FLOW';
                         IF(V_COUNTER > 0)THEN
                            V_PURPOSE_FLOW := UPPER(V_SPLIT_VALUE);
                         END IF;
                     END IF;
                 END IF;       
                 
                 V_INDEX := V_INDEX + 1;
            END LOOP;

           IF(V_PURPOSE_GROUP IS NOT NULL
              AND V_PURPOSE_PROJECT IS NOT NULL
              AND V_PURPOSE_BAND IS NOT NULL
              AND V_PURPOSE_TYPE IS NOT NULL
              AND V_PURPOSE_FLOW IS NOT NULL
              AND V_PURPOSE_DESCRIPTION IS NOT NULL)THEN

             --/*
              DBMS_OUTPUT.PUT_LINE('GROUP: ' || V_PURPOSE_GROUP || 
                     ', PROJECT: ' || V_PURPOSE_PROJECT || 
                     ', BAND: ' || V_PURPOSE_BAND || 
                     ', TYPE: ' || V_PURPOSE_TYPE || 
                     ', FLOW: ' || V_PURPOSE_FLOW || 
                     ', DESC: ' || V_PURPOSE_DESCRIPTION ||
                     ', ID: ' || TO_CHAR(SWR_REC.SWR_ID));
              --*/
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
                      --NULL;
                      --/*
                      UPDATE SWRMASTER
                      SET PURPOSE_GROUP = TRIM(V_PURPOSE_GROUP),
                          PURPOSE_PROJECT = TRIM(V_PURPOSE_PROJECT),
                          PURPOSE_BAND = TRIM(V_PURPOSE_BAND),
                          PURPOSE_TYPE = TRIM(V_PURPOSE_TYPE),
                          PURPOSE_FLOW = TRIM(V_PURPOSE_FLOW),
                          PURPOSE_DESCRIPTION = TRIM(V_PURPOSE_DESCRIPTION)
                      WHERE SWR_ID =  SWR_REC.SWR_ID;
                      --*/
                   END IF;         
           ELSE
               NULL;
               DBMS_OUTPUT.PUT_LINE('Invalid PURPOSE string for ID: ' || TO_CHAR(SWR_REC.SWR_ID) || ' --> ' || SWR_REC.PURPOSE );
           END IF;                     
       END LOOP;
     EXCEPTION
       	 WHEN OTHERS THEN
         V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
         DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
     END;
END;
-----------------------------------------------------------------------------------------------------------------------------------------
WITH CURRENT_JOBS 
AS (
    SELECT
        J.JOB_ID,
        J.BATCH_ORDER,
        J.RESTART_COUNTER,
        J.MFD_KEY,
        J.WAFER,
        J.JOB_STATE,
        J.MACHINE_ASSIGNED,
        J.RUN_ON_SERVER,
        J.MATLAB_VERSION_ID_USED,
        J.LOG_KEY,
        J.DATE_ASSIGNED,
        B.BATCH_ID,
        B.PRODUCT,
        B.LOT,
        B.CHILD_LOT,
        B.STATE,
        B.ROUTE,
        B.OPER
    FROM
        BAW_SVR_JOB J
        INNER JOIN BAW_SVR_BATCH B ON J.BATCH_ID = B.BATCH_ID
    WHERE
        J.MFD_KEY IN (10657,6858,11962,6898,9494,10160,8939,9306)
        AND ( J.WAFER IS NULL OR J.WAFER IN ( 7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 ))
        AND B.PRODUCT = :IN_PRODUCT
        AND B.LOT = :IN_LOT
        AND (:IN_CHILD_LOT IS NULL OR :IN_CHILD_LOT = '' OR B.CHILD_LOT = :IN_CHILD_LOT)
        AND B.STATE = :IN_STATE
        AND B.ROUTE = :IN_ROUTE
        AND B.OPER = :IN_OPER
), REQUIRED_FUNC 
   AS (
    SELECT 10657 AS MFD_KEY, 
    'PROD' AS CATEGORY, 
    520 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 6858 AS MFD_KEY, 
    'PROD' AS CATEGORY, 
    400 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 11962 AS MFD_KEY, 
    'PROD' AS CATEGORY, 
    1 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 6898 AS MFD_KEY, 
    'PROD' AS CATEGORY, 
    300 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 9494 AS MFD_KEY, 
    'PROD' AS CATEGORY, 
    203 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 10160 AS MFD_KEY, 
    'PROD' AS CATEGORY, 
    509 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 8939 AS MFD_KEY, 
    'DEV' AS CATEGORY, 
    721 AS DISPLAY_ORDER FROM DUAL 
    UNION ALL 
    SELECT 9306 AS MFD_KEY, 
    'DEV' AS CATEGORY, 
    100000 AS DISPLAY_ORDER FROM DUAL
)
SELECT DISTINCT
    JOBS.*,
    BB.COMMENTS,
    BFO.CALL_LABEL,
    H.RESULT_STATUS,
    H.RESULT_MESSAGE,
    H.RESULT_EXCEPTION,
    COALESCE(BFV.MOVE_OUT_REQUIREMENT, 0) MOVE_OUT_REQUIREMENT,
    RF.DISPLAY_ORDER,
    RF.CATEGORY,
    BFO.CALL_TYPE
FROM
    (
        SELECT
            *
        FROM
            CURRENT_JOBS
        UNION ALL
        SELECT
            J.JOB_ID,
            J.BATCH_ORDER,
            J.RESTART_COUNTER,
            J.MFD_KEY,
            J.WAFER,
            J.JOB_STATE,
            J.MACHINE_ASSIGNED,
            J.RUN_ON_SERVER,
            J.MATLAB_VERSION_ID_USED,
            J.LOG_KEY,
            J.DATE_ASSIGNED,
            B.BATCH_ID,
            B.PRODUCT,
            B.LOT,
            B.CHILD_LOT,
            B.STATE,
            B.ROUTE,
            B.OPER
        FROM
            BAW_SVRLOG_JOB J
            INNER JOIN BAW_SVRLOG_BATCH B ON J.BATCH_ID = B.BATCH_ID
            INNER JOIN (
                SELECT
                    MAX(GREATEST(LB.DATE_SUBMITTED, LJ.DATE_ASSIGNED, LJ.DATE_COMPLETED)) LATEST_DATE,
                    LJ.MFD_KEY,
                    LJ.WAFER,
                    LB.PRODUCT,
                    LB.LOT,
                    LB.STATE,
                    LB.ROUTE,
                    LB.OPER
                FROM
                    BAW_SVRLOG_JOB LJ
                    INNER JOIN BAW_SVRLOG_BATCH LB ON LJ.BATCH_ID = LB.BATCH_ID
                WHERE
                    LJ.MFD_KEY IN (
                        10657,6858,11962,6898,9494,10160,8939,9306
                    )
                    AND ( LJ.WAFER IS NULL OR LJ.WAFER IN ( 7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 ))
                    AND LB.PRODUCT = :IN_PRODUCT
                    AND LB.LOT = :IN_LOT
                    AND (:IN_CHILD_LOT IS NULL OR :IN_CHILD_LOT = '' OR LB.CHILD_LOT = :IN_CHILD_LOT)
                    AND LB.STATE = :IN_STATE
                    AND LB.ROUTE = :IN_ROUTE
                    AND LB.OPER = :IN_OPER
                GROUP BY
                    LJ.MFD_KEY,
                    LJ.WAFER,
                    LB.PRODUCT,
                    LB.LOT,
                    LB.STATE,
                    LB.ROUTE,
                    LB.OPER
            ) LATEST ON LATEST.MFD_KEY = J.MFD_KEY
                        AND ( LATEST.WAFER = J.WAFER OR ( J.WAFER IS NULL AND LATEST.WAFER IS NULL ) )
                        AND LATEST.PRODUCT = B.PRODUCT
                        AND LATEST.LOT = B.LOT
                        AND LATEST.STATE = B.STATE
                        AND LATEST.ROUTE = B.ROUTE
                        AND LATEST.OPER = B.OPER
                        AND ( LATEST.LATEST_DATE = B.DATE_SUBMITTED
                              OR LATEST.LATEST_DATE = J.DATE_ASSIGNED
                              OR LATEST.LATEST_DATE = J.DATE_COMPLETED )
            LEFT OUTER JOIN CURRENT_JOBS CJ ON CJ.MFD_KEY = J.MFD_KEY
                                               AND ( CJ.WAFER = J.WAFER
                                                     OR ( CJ.WAFER IS NULL
                                                          AND J.WAFER IS NULL ) )
                                               AND CJ.PRODUCT = B.PRODUCT
                                               AND CJ.LOT = B.LOT
                                               AND CJ.STATE = B.STATE
                                               AND CJ.ROUTE = B.ROUTE
                                               AND CJ.OPER = B.OPER
        WHERE
            J.MFD_KEY IN (
                10657,6858,11962,6898,9494,10160,8939,9306
            )
            AND ( J.WAFER IS NULL OR J.WAFER IN ( 7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 ))
            AND CJ.JOB_ID IS NULL
            AND B.PRODUCT = :IN_PRODUCT
            AND B.LOT = :IN_LOT
            AND (:IN_CHILD_LOT IS NULL OR :IN_CHILD_LOT = '' OR B.CHILD_LOT = :IN_CHILD_LOT)
            AND B.STATE = :IN_STATE
            AND B.ROUTE = :IN_ROUTE
            AND B.OPER = :IN_OPER
    ) JOBS
    INNER JOIN BAW_FUNCTION_ORG BFO 
        ON JOBS.MFD_KEY = BFO.MFD_KEY
    INNER JOIN REQUIRED_FUNC RF 
        ON RF.MFD_KEY = JOBS.MFD_KEY
    LEFT JOIN BAW_FUNCTION_VERSION BFV 
        ON (BFV.MFD_KEY = JOBS.MFD_KEY AND BFV.MATLAB_VERSION_ID = JOBS.MATLAB_VERSION_ID_USED)
    LEFT JOIN BAW_MATLAB_EXECUTION_HISTORY H 
        ON H.LOG_KEY = JOBS.LOG_KEY
    LEFT JOIN BAW_SVRLOG_BATCH BB 
        ON JOBS.BATCH_ID = BB.BATCH_ID
ORDER BY
   RF.DISPLAY_ORDER,
   TO_NUMBER(JOBS.WAFER);
-----------------------------------------------------------------------------------------------------------------------------------------
SET SERVEROUTPUT ON;
DECLARE
        IN_LOT VARCHAR2(20) := '1625244';
        IN_ROUTE VARCHAR2(50) := 'BAWFBTRIM';
        IN_OPERATION NUMBER := 2522;
        IN_WAFER NUMBER := 3;
        IN_MACHINE_SUBMITTED VARCHAR2(50) := 'DFWKMUBARAK-L';
        IN_SUBMITTED_BY_CLIENT VARCHAR2(50) := 'SqlDeveloper'; -- BAWDATAVIEWER, FASTBAW, ...
        OUT_ERROR_FLAG NUMBER := NULL;
        OUT_ERROR_MESSAGE VARCHAR2(4000) := NULL;
BEGIN
        BAW_SVR_PKG.SUBMIT_ML_FUNC_TO_RUN(IN_LOT, IN_ROUTE, IN_OPERATION, IN_WAFER, IN_MACHINE_SUBMITTED, IN_SUBMITTED_BY_CLIENT, OUT_ERROR_FLAG, OUT_ERROR_MESSAGE);
        
        IF(OUT_ERROR_FLAG = 1)THEN
           DBMS_OUTPUT.PUT_LINE('Error occurred! ' || OUT_ERROR_MESSAGE);
        ELSE
          DBMS_OUTPUT.PUT_LINE('Job submitted successfully for wafer # ' || TO_CHAR(IN_WAFER));
        END IF;
		
		COMMIT;
END;
---------;
SELECT * FROM BAW_SVR_BATCH ORDER BY BATCH_ID DESC;
SELECT * FROM BAW_SVR_JOB WHERE BATCH_ID = 924926;
------------------------------
--get Activated SWRs with Parent/Child SWrs
SELECT A.PARENT_SR_ID, 
       LISTAGG(A.CHILD_SR_ID, ',') WITHIN GROUP(ORDER BY A.CHILD_SR_ID) AS CHILD_SR_IDS
FROM SR_LINK A
GROUP BY A.PARENT_SR_ID
ORDER BY A.PARENT_SR_ID DESC;
--------------
SET SERVEROUTPUT ON;
DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       IN_EG_NUMBER NUMBER := NULL;
       OUT_ERROR_FLAG NUMBER := NULL; 
       OUT_ERROR_TEXT VARCHAR2(4000) := NULL;
       
BEGIN
       --IN_EG_NUMBER := 5458;
       IF(IN_EG_NUMBER IS NOT NULL)THEN
        PRODUCT_SETUP_PKG.DV_PRODSETUP(IN_EG_NUMBER, OUT_ERROR_FLAG, OUT_ERROR_TEXT);   
        DBMS_OUTPUT.PUT_LINE('Msg: ' || OUT_ERROR_TEXT || ' ErrorFlag: ' || TO_CHAR(OUT_ERROR_FLAG)); 
       END IF;
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
-----------------------
/*
private static void ProcessTrimTemplateReleasedRevision(int ttmpMainPk, int indexCounter, string varName, string product, string lot, string state, string route, int oper, string function, string matlabSoftwareVersionDll)
{
	var instructionList = new List<string>();
	var _key = string.Format("TrimTemplate_Released_{0}_{1}", product, lot);
	var _cachedItems = CacheEngine.Instance.GetItem<CachingObject>(_key);

	if (_cachedItems == null || _cachedItems.CachedItems.Count == 0)
	{
		var trimTempMatlabStructure = new TrimTemplateDBStruct(ttmpMainPk, indexCounter, varName, product, lot, state, route, oper, function);
		instructionList = trimTempMatlabStructure.GetTrimTargetDBStruct();

		var _itemToCache = new CachingObject();
		_itemToCache.CachedItems = instructionList;
		CacheEngine.Instance.AddItem(_key, _itemToCache);
	}
	else
	{
		instructionList = _cachedItems.CachedItems;
	}

	if (instructionList.Count > 0)
	{
		MatlabWrapperManager.GetMatlabWrapperForGivenMLVersion(matlabSoftwareVersionDll).ExecuteList(instructionList);
	}
}
*/
--Updated view "V_SR_ROUTE_MODS"
  SELECT M.FACILITY,
     L.NAME AS LOT_NUMBER,
     L.SR_ID AS SWR_ID,
     R.NAME AS MAIN_ROUTE,
     DECODE(R.IMPACT_STATUS, 'M', 'MODIFIED', 'S', 'SKIPPED') AS IMPACT_STATUS,
     RO.ID AS ROUTE_ORDER_ID,
     RO.ROUTE,
     RO.OPER,
     RO.OPER_SHORT_DESC,
     RO.SEQ_ORDER,
     RO.SPEC_ID,
     TT.TOOLS_LIST,
     TT.RECIPE_LIST,
     B.STATE
FROM
    SR_LOT L
 LEFT JOIN SR_ROUTE R
    ON L.SR_ID = R.SR_ID
 LEFT JOIN SR_ROUTE_OPER_ORDER RO
    ON R.ID = RO.ROUTE_ID
 LEFT JOIN 
 (
   SELECT SWR_NUMBER, ROUTE, OPER,
       LISTAGG(T.TOOLID, ',') WITHIN GROUP (ORDER BY T.TOOLID) AS TOOLS_LIST,
       LISTAGG(T.TOOLID || '|' || T.RECIPE, ',') WITHIN GROUP (ORDER BY T.RECIPE) AS RECIPE_LIST
   FROM RECIPEDATA.RECIPE_MANAGER T
   WHERE T.SWR_NUMBER > 0
   GROUP BY T.SWR_NUMBER, ROUTE, OPER
  ) TT
    ON L.SR_ID = TT.SWR_NUMBER
 LEFT JOIN SR_MASTER M
    ON M.ID = R.SR_ID
 LEFT JOIN BUSINESS_PROCESS B
    ON (M.ID = B.ID AND UPPER(B.STATE) = 'ACTIVATED' AND M.ID = B.ID)
WHERE R.IMPACT_STATUS IS NOT NULL
ORDER BY L.SR_ID, R.NAME, RO.SEQ_ORDER, RO.ROUTE;
--------------
/*NCM form:
        DisableHoldTagOwnerDropdown();

        function DisableHoldTagOwnerDropdown() {
            var _status = '@Html.Raw(Model.ncmForms.State)';
            if(_status != null && _status != "" &&  _status.length > 0){
                if(_status.toUpperCase() == "NONE" || _status.toUpperCase() == "NEW" || _status.toUpperCase() == "REJECTED"){
                    $('#Requestor').removeAttr('disabled');
                }
                else{
                    $('#Requestor').attr('disabled', 'disabled');
                }
            }
        };
		
		
		Scrap-report:
#1:
on page load, hide scrap-report drop-down, 
show drop-down on Scap radion button selection.

validate on submit as well.

#2: Take away "Scrap to engineering" radio button.

#3: Drop-down for scrap query should be dynamic based on wafer selected and also search for disposition == "S"

SELECT DISTINCT
     A.SCRAP_ID ,
     A.FACILITY ,
     A.LOT_NUMBER ,
     A.PRODUCT ,
     A.PARENT_LOT ,
     A.ROUTE ,
     A.OPER ,
     A.LOT_TYPE ,
     A.WAFER_TRACE ,
     A.ENTRY_SEQ ,
     A.SCRAP_ERASE_DATE_TIME ,
     A.PROD_DESC ,
     A.PROGRAM ,
     A.LEGACY_ID ,
     A.REMAINING, 
     B.STATUS, 
     B.PROBLEM_ROUTE, 
     B.PROBLEM_OPER, 
     B.CAUSE, 
     C.WAFER_SAN,
     DECODE(C.WAFER_ID, NULL, NULL, TRIM(0 FROM SUBSTR(C.WAFER_ID,12,2)))  AS WAFER_NUMBER
FROM SCRAP_LOT_INFO A
    LEFT JOIN SCRAP_DATA B
        ON A.SCRAP_ID = B.SCRAP_ID 
    LEFT JOIN SCRAP_WAFER_INFO C
        ON B.SCRAP_ID = C.SCRAP_ID
WHERE A.LOT_NUMBER = '1809636';
-------------------
1693: Adding a new product to auto-trig list

Purpose: This SP will add a new product to the list of products which have certain route/oper 
setup for AT. This SP will be executed at product creation as a part of DV_PRODSETUP.

Name: ADD_PRODUCT_2_AT_LIST
Logic:
If the PFD of the new product contains routes/operations which have been already set to AT 
(in BAW_FUNC_AUTO_TRIGGER_RTE_OPER), the product is to be added to the product list 
for each of these route/oper combinations.

DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       IN_PRODUCT VARCHAR2(10) := 'EG5180';
       OUT_ERR_FLAG NUMBER := NULL;
       OUT_ERR_MSG VARCHAR2(4000) := NULL;
       V_IS_AUTO_TRIGGER_ALL_PRODS VARCHAR2(1) := NULL;
       V_COUNTER NUMBER := NULL;
BEGIN
       FOR ROUTE_OPER_REC IN 
       (
         SELECT B.ST_RT_SAN,
                B.STATE, 
                A.ROUTE, 
                B.OPER
         FROM APPIANSEC.V_PFD_ROUTE_APPROVED@TQTGAAS_BAW A
            INNER JOIN BAW_STATE_ROUTE_OPER B
                ON (A.FAB_PROCESS = B.STATE AND A.ROUTE = B.ROUTE)
         WHERE A.PRODUCT_SHORT = IN_PRODUCT
         AND B.IS_AUTO_TRIGGERED = 'Y'
       )
       LOOP
           DBMS_OUTPUT.PUT_LINE('ST_RT_SAN: ' || ROUTE_OPER_REC.ST_RT_SAN || ', ROUTE: ' || ROUTE_OPER_REC.ROUTE || ', OPER: ' || ROUTE_OPER_REC.OPER);
           V_COUNTER := NULL;
           V_IS_AUTO_TRIGGER_ALL_PRODS := NULL;
           
           SELECT AUTO_TRIGGER_ALL_PRODUCTS INTO V_IS_AUTO_TRIGGER_ALL_PRODS
           FROM BAW_STATE_ROUTE_OPER
           WHERE ST_RT_SAN = ROUTE_OPER_REC.ST_RT_SAN;
           
           IF(V_IS_AUTO_TRIGGER_ALL_PRODS = 'N')THEN
               
              SELECT COUNT(*) INTO V_COUNTER
              FROM BAW_FUNC_AUTO_TRIGGER_PRODUCT 
              WHERE ST_RT_SAN = ROUTE_OPER_REC.ST_RT_SAN
              AND PRODUCT = IN_PRODUCT;
              
              IF(V_COUNTER = 0)THEN
                DBMS_OUTPUT.PUT_LINE('Product {' || IN_PRODUCT || '} does not exist in BAW_FUNC_AUTO_TRIGGER_PRODUCT. Therefore, it will be inserted.');
                NULL;
              ELSE
                DBMS_OUTPUT.PUT_LINE('Product {' || IN_PRODUCT || '} already exists in BAW_FUNC_AUTO_TRIGGER_PRODUCT.');
                NULL;
              END IF;
           ELSIF(V_IS_AUTO_TRIGGER_ALL_PRODS = 'Y')THEN
              DBMS_OUTPUT.PUT_LINE(ROUTE_OPER_REC.ROUTE || ' / ' ||ROUTE_OPER_REC.OPER || ' is set for auto-trigger for all Products therefore, {' || IN_PRODUCT || '} will not be added.');
              NULL;
           END IF;
           DBMS_OUTPUT.PUT_LINE('-----------------------------------------------------------');
       END LOOP; 
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      OUT_ERR_FLAG := 1;
      OUT_ERR_MSG := V_EXCEPTION_MSG;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
--------------------------------------------------------------------------------------------------
SELECT * FROM BAW_SVR_BATCH ORDER BY BATCH_ID DESC;
SELECT * FROM BAW_SVR_JOB WHERE BATCH_ID IN (1196207) ORDER BY BATCH_ID DESC;
---
SELECT * 
FROM BAW_SVR_PARAMETER 
WHERE INOUTPUT = 'OUTPUT'
AND JOB_ID IN (SELECT JOB_ID FROM BAW_SVR_JOB WHERE BATCH_ID = 1196207);
------//EG9154/1917907/_Yield_Merge_Data/09/AddParam2YLD_LLS
SELECT * FROM BAW__MATFILES WHERE FIELD_NAME = '1917907';
SELECT * FROM BAW__MATFILES WHERE PARENT_ID = 166525441 ORDER BY FIELD_NAME DESC;
*/


/*
//1918604/InkSpecLot
//EG9583/1918604/AFTER_GRINDING/13/InterpRoutineFlag
//EG9583/1918604/AFTER_GRINDING/13/CheckLog
//EG9583/1918604/AFTER_GRINDING/13/YieldMap
//EG9583/1918604/AFTER_GRINDING/InkSpec
//EG9583/1918604/AFTER_GRINDING/13/CSV_filter_spreadsheet
//EG9583/1918604/CheckLog
//EG9583/1918604/AFTER_GRINDING/13/FilterMap
*/

/*
Re-sharper Tips:
- Refactoring:
	- renaming a method
- Navigating through class hierarchy.
- Static Analysis
- 
---------
K+T ==> finds the class or method name (method name SumNumber ==> search for SN)
Ctrl + U + L ==> run all unit tests.
Shit + Delete ==> Deletes a line.
Ctrl + U U ==> run last test.
Alt + Enter ==> move to a different file.
*/
SELECT B.ST_RT_SAN,
       A.ROUTE, 
       A.OPER, 
       B.PRODUCTS
FROM BAW_STATE_ROUTE_OPER A
    INNER JOIN 
        (
          SELECT X.ST_RT_SAN, 
                 LISTAGG(X.PRODUCT, ',') WITHIN GROUP (ORDER BY X.PRODUCT) AS PRODUCTS
          FROM BAW_FUNC_AUTO_TRIGGER_PRODUCT X
          GROUP BY X.ST_RT_SAN
        )B
ON A.ST_RT_SAN = B.ST_RT_SAN
WHERE A.IS_AUTO_TRIGGERED = 'Y';
---------------
DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       IN_PRODUCT VARCHAR2(10) := 'EG0328';
       IN_ROUTE VARCHAR2(30) := '8UCAP';
       IN_OPER NUMBER := 2368;
       OUT_STATUS NUMBER := NULL;
       OUT_MSG VARCHAR2(4000) := NULL;
BEGIN
       PROC_RUN_FUNCTIONS_MANUALLY(IN_PRODUCT, IN_ROUTE, IN_OPER, OUT_STATUS, OUT_MSG);
       DBMS_OUTPUT.PUT_LINE('Status: ' || OUT_STATUS || ' Msg: ' || OUT_MSG);
       DBMS_OUTPUT.PUT_LINE('-------------------------------------');
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
-------------------
/*
Open Visual studio command prompt in admin mode
browse to the 2017 directory and run the following command to remove ODT.
vsixinstaller /a /u:"Oracle.VsDevTools.15.0"

if Visual Studio gives error on About panel: 
remove the duplicate Oracle.ManagedDataAccess assemblies from the global assembly cache. 
Open Visual studio command prompt in admin mode
gacutil /l   ===> to get list of all assemblies.
gacutil /u <nameOfTheAssemblyToBeRemoved>===> to uninstall an assembly.
Visual Studio 2017:
Un-install all ODAC
Un-install all ODT.
Remove all references from the path (environment variables)

----- 
download ODAC for Oracle 12C Release 3 which supports EntityFramework 6.0 and also includes ODT for Visual Studio.
while installing ODAC, make sure to check option to configure odp.net for machine-level configuration.
---
Start Visual Studio 2017
create new blank solution.
Add new data project within blank solutions of type class library.
Right-click on the project and select Manage NuGet packages.
search for Oracle Managed 
install Oracle.Managed.DataAccess.EntityFramework
choose version:  12.2.1100
it will also install its following dependencies:
- Oracle.Managed.DataAccess v12.2.1100
- EntityFramework v6.0.0
----
Add new Item to the project.
Select ADO.net Entity Data Model
select tables.
configure mapping.
done.
*/
------------------------------------------------------------------------------------
SELECT * FROM MATCREATOR.LOT_MATERIAL;
SELECT LPAD('1', 2, 0) TEXT FROM DUAL;
SELECT SUBSTR('1234567890123',1,9) AS TEXT FROM DUAL;
--Find PFD or a a lot:
SELECT DISTINCT X.*
FROM
(
SELECT FACILITY, 
       LOT_NUMBER, 
       PROD, 
       ROUTE,
       MAIN_ROUTE_DISP_ORDER AS DISPLAY_ORDER
FROM 
    V_SR_ROUTE_FLOW_MAIN 
WHERE LOT_NUMBER = '1916833'
)X
ORDER BY X.DISPLAY_ORDER;

--Get route/Oper from pub.wiplot.
SELECT ROUTE, OPER FROM PUB.WIPLOT WHERE LOT_NUMBER = '1921105' AND ROWNUM = 1 AND WS_ERASE_DATE_TIME IS NULL;
---
CREATE OR REPLACE VIEW V_MRA_NCM_FORM
AS
SELECT 
/*
 This view was created to accomodate editing of an NCM form. 
 It is an extention to an existing View "V_MATERIAL_REVIEW_AREA_RECORDS"
 with extra conditions in the join clause when joining with PUB.WIPLOT. 
*/
        LOT_NUMBER,
        LISTAGG(WAFER_NUMBER, ', ') WITHIN GROUP(ORDER BY WAFER_NUMBER) AS WAFER_NUMBER,
        LISTAGG(SAPWAF_SAN, ', ') WITHIN GROUP(ORDER BY WAFER_NUMBER) AS SAP_WAFER_SAN,
        NCM_ID,
        NCM_REQUESTOR,
        NCM_INITIATOR,
        STATE,
        PROBLEM,
        DISPOSITION,
        INSTRUCTION,
        RATIONALE,
        SWRID,
        SCRAP,
        EIGHTD,
        PROD, 
        OPER,
        CASE WHEN ROUTE IS NULL THEN 'MRA' ELSE ROUTE END AS ROUTE,
        LOCATION,
        PENDING,
        MATERIAL,
        CASE 
            WHEN STATE = 'New'  THEN 'New MRA Hold'  
            WHEN STATE = 'Initiated'   THEN 'Ready for Signatures'  
            WHEN STATE = 'Approved'    THEN 'In Signature Process'  
            WHEN STATE = 'AllApproved' AND MATERIAL LIKE 'G%' THEN  'GaAs/GaN' || ' Lot Ready for Cabinet Removal'
            WHEN STATE = 'AllApproved' AND MATERIAL NOT LIKE 'G%' THEN  MATERIAL || ' Lot Ready for Cabinet Removal'  
            WHEN STATE = 'Activated' THEN 'MRA Completed' 
			WHEN STATE = 'Editing'	THEN 'Rejected' 
			ELSE STATE || ' state is not defined' 
        END AS PRETTY_STATE
FROM
(
    SELECT DISTINCT 
           E.WS_LOT AS LOT_NUMBER,
           E.WS_WAFER_NO AS WAFER_NUMBER,
           E.SAPWAF_SAN ,
           A.ID AS NCM_ID, 
           NVL(A.REQUESTOR, 'NONE') AS NCM_REQUESTOR,
           NVL(A.INITIATOR, 'NONE') AS NCM_INITIATOR,
           NVL(A.STATE, 'NONE') AS STATE,
           C.PROBLEM AS PROBLEM,
           C.DISPOSITION AS DISPOSITION,
           C.INSTRUCTION AS INSTRUCTION,
           C.RATIONALE AS RATIONALE,
           C.SWRID AS SWRID,
           C.SCRAP AS SCRAP,
           C.EIGHTD AS EIGHTD,
           F.PROD, 
           F.OPER,
           F.ROUTE,
           F.INTRANSIT_OWNER_FAC as LOCATION,
           B.APPROVALS_PENDING AS PENDING,
           J.TECHNOLOGY_TYPE AS MATERIAL
    FROM 
        MESDATA.BUSINESS_PROCESS A
            LEFT JOIN MESDATA.V_BUSINESS_PROCESS_PENDING B
                ON A.ID = B.ID
            LEFT JOIN MESDATA.MRA_DISPOSITION C
                ON A.ID = C.ID
            LEFT JOIN MESDATA.MRA_WAFER D
                ON C.ID = D.ID
            LEFT JOIN MATCREATOR.LOT_MATERIAL E
                ON D.SAPWAF_SAN = E.SAPWAF_SAN
            LEFT JOIN PUB.WIPLOT F
                ON (
                     (E.WS_LOT = F.LOT_NUMBER AND LENGTH(F.LOT_NUMBER) < 9)
                      OR
                      (SUBSTR(F.LOT_NUMBER,1,9) = (E.WS_LOT || LPAD(E.WS_WAFER_NO, 2, 0)) AND LENGTH(F.LOT_NUMBER) >= 9)
                      AND F.FACILITY <> 'GAASIN'
                   )
            LEFT JOIN APPIANSEC.V_ALL_PRODUCTS J
                ON F.PROD = J.PRODUCT_FULL
    WHERE A.APPLICATION_NAME LIKE 'MaterialReviewArea%'
    --AND F.LOT_NUMBER = '1904435'
)
GROUP BY
        LOT_NUMBER,
        NCM_ID,
        NCM_REQUESTOR,
        NCM_INITIATOR,
        STATE,
        PROBLEM,
        DISPOSITION,
        INSTRUCTION,
        RATIONALE,
        SWRID,
        SCRAP,
        EIGHTD,
        PROD, 
        OPER,
        ROUTE,
        LOCATION,    
        PENDING,
        MATERIAL;

GRANT SELECT ON V_MRA_NCM_FORM TO MESDATA;
GRANT SELECT ON V_MRA_NCM_FORM TO MESUSER;
------------------------
SELECT * 
FROM 
    BUSINESS_PROCESS_RULE
WHERE APPLICATION_NAME = 'SpecialRequest'
AND PROCESS_NAME IN ('SWR', 'SWR BLANKET', 'STR BLANKET')
AND ROLE = 'TRB Chairman'; 
--Keith.Salzman@Qorvo.com
-----------------------
 SELECT 
/*
 This view was created to accomodate editing of an NCM form. 
 It is an extention to an existing View "V_MATERIAL_REVIEW_AREA_RECORDS"
 with extra conditions in the join clause when joining with PUB.WIPLOT. 
*/
        LOT_NUMBER,
        LISTAGG(WAFER_NUMBER, ', ') WITHIN GROUP(ORDER BY WAFER_NUMBER) AS WAFER_NUMBER,
        LISTAGG(SAPWAF_SAN, ', ') WITHIN GROUP(ORDER BY WAFER_NUMBER) AS SAP_WAFER_SAN,
        NCM_ID,
        NCM_REQUESTOR,
        NCM_INITIATOR,
        STATE,
        PROBLEM,
        DISPOSITION,
        INSTRUCTION,
        RATIONALE,
        SWRID,
        SCRAP,
        EIGHTD,
        PROD, 
        OPER,
        CASE WHEN ROUTE IS NULL THEN 'MRA' ELSE ROUTE END AS ROUTE,
        LOCATION,
        PENDING,
        MATERIAL,
        CASE 
            WHEN STATE = 'New'  THEN 'New MRA Hold'  
            WHEN STATE = 'Initiated'   THEN 'Ready for Signatures'  
            WHEN STATE = 'Approved'    THEN 'In Signature Process'  
            WHEN STATE = 'AllApproved' AND MATERIAL LIKE 'G%' THEN  'GaAs/GaN' || ' Lot Ready for Cabinet Removal'
            WHEN STATE = 'AllApproved' AND MATERIAL NOT LIKE 'G%' THEN  MATERIAL || ' Lot Ready for Cabinet Removal'  
            WHEN STATE = 'Activated' THEN 'MRA Completed' 
			WHEN STATE = 'Editing'	THEN 'Rejected' 
			ELSE STATE || ' state is not defined' 
        END AS PRETTY_STATE
FROM
(
    SELECT DISTINCT 
           E.WS_LOT AS LOT_NUMBER,
           E.WS_WAFER_NO AS WAFER_NUMBER,
           E.SAPWAF_SAN ,
           A.ID AS NCM_ID, 
           NVL(A.REQUESTOR, 'NONE') AS NCM_REQUESTOR,
           NVL(A.INITIATOR, 'NONE') AS NCM_INITIATOR,
           NVL(A.STATE, 'NONE') AS STATE,
           C.PROBLEM AS PROBLEM,
           C.DISPOSITION AS DISPOSITION,
           C.INSTRUCTION AS INSTRUCTION,
           C.RATIONALE AS RATIONALE,
           C.SWRID AS SWRID,
           C.SCRAP AS SCRAP,
           C.EIGHTD AS EIGHTD,
           F.PROD, 
           F.OPER,
           F.ROUTE,
           F.INTRANSIT_OWNER_FAC as LOCATION,
           B.APPROVALS_PENDING AS PENDING,
           J.TECHNOLOGY_TYPE AS MATERIAL
    FROM 
        MESDATA.BUSINESS_PROCESS A
            LEFT JOIN MESDATA.V_BUSINESS_PROCESS_PENDING B
                ON A.ID = B.ID
            LEFT JOIN MESDATA.MRA_DISPOSITION C
                ON A.ID = C.ID
            LEFT JOIN MESDATA.MRA_WAFER D
                ON C.ID = D.ID
            LEFT JOIN MATCREATOR.LOT_MATERIAL E
                ON D.SAPWAF_SAN = E.SAPWAF_SAN
            LEFT JOIN PUB.WIPLOT F
                ON (
                     (E.WS_LOT = F.LOT_NUMBER AND LENGTH(F.LOT_NUMBER) < 9)
                      OR
                      (SUBSTR(F.LOT_NUMBER,1,9) = (E.WS_LOT || LPAD(E.WS_WAFER_NO, 2, 0)) AND LENGTH(F.LOT_NUMBER) >= 9)
                      AND F.FACILITY <> 'GAASIN'
                      AND F.DELETED = 'N'
                   )
            LEFT JOIN APPIANSEC.V_ALL_PRODUCTS J
                ON F.PROD = J.PRODUCT_FULL
    WHERE A.APPLICATION_NAME LIKE 'MaterialReviewArea%'
)
GROUP BY
        LOT_NUMBER,
        NCM_ID,
        NCM_REQUESTOR,
        NCM_INITIATOR,
        STATE,
        PROBLEM,
        DISPOSITION,
        INSTRUCTION,
        RATIONALE,
        SWRID,
        SCRAP,
        EIGHTD,
        PROD, 
        OPER,
        ROUTE,
        LOCATION,
        PENDING,
        MATERIAL;
---------
SELECT
    WL.LOT_NUMBER AS LOT,
    LM.IMPLANT_WAFER_NO AS WAFER,
    NVL(WL.ROUTE, 'MRA') AS ROUTE,
    WL.OPER AS OPERATION
FROM
    MESDATA.MRA_WAFER MW
    INNER JOIN MATCREATOR.LOT_MATERIAL LM
        ON MW.SAPWAF_SAN = LM.SAPWAF_SAN
    LEFT OUTER JOIN PUB.WIPWAF WW
        ON RPAD(LM.WS_LOT, 11, ' ') || RPAD(LPAD(LM.IMPLANT_WAFER_NO, 2, '0'), 4, '0') = WW.WAFER_ID
        AND WW.DELETED = 'N'
    LEFT OUTER JOIN PUB.WIPLOT WL
        ON NVL(WW.LAST_LOT, LM.WS_LOT || LPAD(LM.IMPLANT_WAFER_NO, 2, '0')) = WL.LOT_NUMBER
        AND WL.DELETED = 'N'
WHERE
    MW.ID = 204233;
---209011
--------------------------------------------------------
SELECT DISTINCT
    WL.LOT_NUMBER AS LOT,
    LM.IMPLANT_WAFER_NO AS WAFER,
    LM.SAPWAF_SAN AS SAP_WAFER_SAN,
    NVL(WL.ROUTE, 'MRA') AS ROUTE,
    WL.OPER,
    WL.PROD AS PRODUCT,
    WL.INTRANSIT_OWNER_FAC AS LOCATION,
    WL.DELETED,
    MD.PROBLEM,
    MD.DISPOSITION,
    MD.INSTRUCTION,
    MD.RATIONALE,
    MD.SWRID,
    MD.SCRAP,
    MD.EIGHTD AS EIGHTD,
    BP.ID AS NCM_ID, 
    NVL(BP.REQUESTOR, 'NONE') AS NCM_REQUESTOR,
    NVL(BP.INITIATOR, 'NONE') AS NCM_INITIATOR,
    NVL(BP.STATE, 'NONE') AS STATE,
    CASE 
        WHEN BP.STATE = 'New'  THEN 'New MRA Hold'  
        WHEN BP.STATE = 'Initiated'   THEN 'Ready for Signatures'  
        WHEN BP.STATE = 'Approved'    THEN 'In Signature Process'  
        WHEN BP.STATE = 'AllApproved' AND TECHNOLOGY_TYPE LIKE 'G%' THEN  'GaAs/GaN' || ' Lot Ready for Cabinet Removal'
        WHEN BP.STATE = 'AllApproved' AND TECHNOLOGY_TYPE NOT LIKE 'G%' THEN  TECHNOLOGY_TYPE || ' Lot Ready for Cabinet Removal'  
        WHEN BP.STATE = 'Activated' THEN 'MRA Completed' 
        WHEN BP.STATE = 'Editing'	THEN 'Rejected' 
        ELSE BP.STATE || ' state is not defined' 
    END AS PRETTY_STATE,
    BPP.APPROVALS_PENDING AS PENDING,
    J.TECHNOLOGY_TYPE AS MATERIAL
FROM
    MESDATA.MRA_WAFER MW
    INNER JOIN MATCREATOR.LOT_MATERIAL LM
        ON MW.SAPWAF_SAN = LM.SAPWAF_SAN
    LEFT OUTER JOIN PUB.WIPWAF WW
        ON RPAD(LM.WS_LOT, 11, ' ') || RPAD(LPAD(LM.IMPLANT_WAFER_NO, 2, '0'), 4, '0') = WW.WAFER_ID
        AND WW.DELETED = 'N'
    LEFT OUTER JOIN PUB.WIPLOT WL
        ON NVL(WW.LAST_LOT, LM.WS_LOT || LPAD(LM.IMPLANT_WAFER_NO, 2, '0')) = WL.LOT_NUMBER
        AND WL.DELETED = 'N'
    LEFT JOIN MESDATA.MRA_DISPOSITION MD
        ON MW.ID = MD.ID
    LEFT JOIN MESDATA.BUSINESS_PROCESS BP
        ON BP.ID = MD.ID
    LEFT JOIN MESDATA.V_BUSINESS_PROCESS_PENDING BPP
        ON BP.ID = BPP.ID
    LEFT JOIN APPIANSEC.V_ALL_PRODUCTS J
        ON WL.PROD = J.PRODUCT_FULL
WHERE
    MW.ID = 211067
AND BP.APPLICATION_NAME LIKE 'MaterialReviewArea%'
ORDER BY TO_NUMBER(WAFER);
----------------------------
DECLARE
       V_EXCEPTION_MSG VARCHAR2(500) := NULL;
       V_SQL VARCHAR2(1000) := NULL;
       V_PUB_COUNTER NUMBER;
       V_LOT VARCHAR2(20) := '1910140';
       V_WAFER VARCHAR2(5) := '2';
       V_LOT_OUT VARCHAR2(20);
BEGIN
                   /*
        --V_SQL := 'SELECT COUNT(*) FROM PUB.WIPLOT WHERE WAFER_TRACE = ''Y'' AND LOT_NUMBER = ''' || V_LOT || LPAD(WAFER_REC_WAFER_NUM, 2, '0') || '''';
        --RPAD(LM.WS_LOT, 11, ' ') || RPAD(LPAD(LM.IMPLANT_WAFER_NO, 2, '0'), 4, '0')
        --V_SQL := 'SELECT COUNT(*) INTO ';
                    V_SQL := 'SELECT COUNT(*) FROM PUB.WIPLOT WHERE WAFER_TRACE = ''Y'' AND LOT_NUMBER = ''' 
                     || V_LOT || LPAD(WAFER_REC_WAFER_NUM, 2, '0') 
                     || ''' AND OPER = 8007 AND DELETED = ''N''';
                            EXECUTE IMMEDIATE V_SQL INTO V_PUB_COUNTER;
                     */
    SELECT LOT_NUMBER
    INTO V_LOT_OUT
    FROM PUB.WIPLOT 
    WHERE LOT_NUMBER LIKE V_LOT || LPAD(V_WAFER, 2, '0') || '%'
    AND WAFER_TRACE = 'Y';
    
    DBMS_OUTPUT.PUT_LINE('From WIPLOT we got: ' || V_LOT_OUT); 
       
    SELECT LAST_LOT
    INTO V_LOT_OUT
    FROM PUB.WIPWAF WW
    WHERE WW.WAFER_ID = RPAD(V_LOT, 11, ' ') || RPAD(LPAD(V_WAFER, 2, '0'), 4, '0');
    
    DBMS_OUTPUT.PUT_LINE('From WIPWAF we got: ' || V_LOT_OUT);
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
-----------------
/*
SELECT A.*,
       B.ROLE_ID,
       C.ROLE_NAME, 
       C.RANK
FROM
    BAW_MENU_ITEMS A
    LEFT JOIN BAW_ROLE_MENU_ITEMS B
        ON A.MENU_ID = B.MENU_ID
    LEFT JOIN BAW_USER_ROLE C
        ON B.ROLE_ID = C.ROLE_ID
ORDER BY A.MENU_ID;

private void manageAutoTriggerRouteOperToolStripMenuItem_Click(object sender, EventArgs e)
{
	if (sender is ToolStripMenuItem mi)
	{
		if (!CanAccessMenuItem(mi.Text))
		{
			MessageBox.Show(@"You are not allowed to access this MENU Item.", @"BAW DataViewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}
	}

	var view = new AutoTriggerRouteOper.AutoTriggerRouteOperView();
	var viewModel = new AutoTriggerRouteOper.AutoTriggerRouteOperViewModel();

	view.DataContext = viewModel;
	view.Closing += viewModel.OnWindowClosing;

	ElementHost.EnableModelessKeyboardInterop(view);
	view.Topmost = true;
	WindowManager.Add(view);
	view.Show();
	view.Topmost = false;
}

private bool CanAccessMenuItem(string menuItemName)
{
	var userId = UserInfo.ActiveUser.userID;
	var usersRank = UserInfo.ActiveUser.rank;

	if (usersRank == 100) return true;

	var menuItemRows = _menuItemsDt.Select("MENU_ITEM = '" + menuItemName + "' AND RANK >= " + usersRank);
	if (menuItemRows.Length > 0) return true;

	//Check user's special rights on this menuItem

	if (!UserInfo.ActiveUser.useSpecialRights) return false;

	var specialRights = UserInfo.GetUsersSpecialRights(userId);
	var usersMenuItemSpecialRights = specialRights.FirstOrDefault(x => x.Scope == "MENU_ITEM" && x.FieldName == menuItemName);

	return (usersMenuItemSpecialRights != null && usersMenuItemSpecialRights.ElevatedRank >= usersRank);
}
*/

WITH CTE
AS
(
	SELECT GROUP_ID,
	COUNT(*) AS TOTAL_PRODUCTS
	FROM BAW_FLOW_GROUP
	GROUP BY GROUP_ID
)
SELECT CTE.GROUP_ID,
	   CASE
		   WHEN CTE.TOTAL_PRODUCTS = 1 THEN
			(SELECT DECODE(PRODUCT, '*', '*', '1') FROM BAW_FLOW_GROUP WHERE GROUP_ID = CTE.GROUP_ID)
	   ELSE
			TO_CHAR(CTE.TOTAL_PRODUCTS)
	   END AS PRODUCTS
FROM CTE
ORDER BY CTE.GROUP_ID;
----------------------------------------------------------------------------------------------------------
SELECT B.QUERY_PARAM_TABLE_FK,
       LISTAGG(B.COLUMN_NAME, ', ') WITHIN GROUP(ORDER BY B.COLUMN_NAME) AS SELECTED_COLUMNS
FROM QUERY_PARAM_SELECT B
WHERE B.QUERY_PARAM_TABLE_FK IN (1740, 1818, 2058, 2083, 2119, 2138, 2139, 2158,
                                2178, 2198, 2199, 2218, 2239, 2278, 2339, 2378, 2398,
                                2438, 2478, 2498, 2578, 2598, 2618, 2679, 2719, 2738, 
                                2739, 2740, 2758, 2798)
GROUP BY B.QUERY_PARAM_TABLE_FK
ORDER BY B.QUERY_PARAM_TABLE_FK;
-----------------------------------
SELECT X.*
FROM
(
    SELECT D.CALL_LABEL,
           C.MATLAB_VERSION_ID,
           E.MATLAB_VERSION_NAME,
           A.*,
           B.SELECTED_COLUMNS
    FROM QUERY_PARAM_TABLE A
        LEFT JOIN 
            (SELECT QUERY_PARAM_TABLE_FK,
                   LISTAGG(COLUMN_NAME, ', ') WITHIN GROUP(ORDER BY COLUMN_NAME) AS SELECTED_COLUMNS
            FROM QUERY_PARAM_SELECT 
            GROUP BY QUERY_PARAM_TABLE_FK) B
        ON A.QUERY_PARAM_TABLE_PK = B.QUERY_PARAM_TABLE_FK
        INNER JOIN BAW_FUNCTION_DEFINITION C
            ON A.BAW_FUNCTION_DEFINITION_FK = TO_CHAR(C.DATASET)
        INNER JOIN BAW_FUNCTION_ORG D
            ON C.MFD_KEY = D.MFD_KEY
        INNER JOIN BAW_MATLAB_VERSION E
            ON C.MATLAB_VERSION_ID = E.MATLAB_VERSION_ID
    WHERE A.SELECT_ALL = 'N'
    AND C.DATATYPE = 'QUERY_PARAM'
    AND D.IS_DELETED = 'N'
    ORDER BY A.QUERY_PARAM_TABLE_PK
)X
WHERE X.SELECTED_COLUMNS IS NULL;
---------
------------------------
SELECT A.*,
       C.SELECTED_COLUMNS
FROM QUERY_PARAM_TEMPLATE_TABLE A
    INNER JOIN  QUERY_PARAM_TEMPLATE B
        ON A.QUERY_PARAM_TEMPLATE_FK = B.QUERY_PARAM_TEMPLATE_PK
    LEFT JOIN 
    (
     SELECT QUERY_PARAM_TEMPLATE_TABLE_FK, 
            LISTAGG(COLUMN_NAME, ',') WITHIN GROUP(ORDER BY COLUMN_NAME) SELECTED_COLUMNS
     FROM QUERY_PARAM_TEMPLATE_SELECT 
     GROUP BY QUERY_PARAM_TEMPLATE_TABLE_FK
    )C
    ON A.QUERY_PARAM_TEMPLATE_TABLE_PK = C.QUERY_PARAM_TEMPLATE_TABLE_FK
WHERE A.SELECT_ALL = 'N'
ORDER BY A.QUERY_PARAM_TEMPLATE_TABLE_PK ;
-----------------------------------
DECLARE
	V_EXCEPTION_MSG VARCHAR2(500) := NULL;
    
    IN_VARIABLE_NAME VARCHAR2(50) := 'UDV_1';
    IN_PRODUCT VARCHAR2(50) := 'EG5520';
    IN_LOT VARCHAR2(50) := '1633638';
    IN_STATE VARCHAR2(50) := 'AFTER_SU8_CURING';
    IN_ROUTE VARCHAR2(50) := 'BAWSU8';
    IN_OPER NUMBER := 2522;
    IN_FUNCTION VARCHAR2(50) := 'Test_StringWithDoubleQuote';
    OUT_STATUS NUMBER := NULL;
    OUT_MESSAGE VARCHAR2(4000) := NULL;
    OUT_ASSIGNED_VALUE VARCHAR2(500) := NULL;
    OUT_ASSIGNMENT_ID NUMBER := NULL;
    
BEGIN
       UDV_RETRIEVE_VAL.GET_UDV_VALUE(IN_VARIABLE_NAME, IN_PRODUCT, IN_LOT, IN_STATE, IN_ROUTE, IN_OPER, IN_FUNCTION, OUT_STATUS, OUT_MESSAGE, OUT_ASSIGNED_VALUE, OUT_ASSIGNMENT_ID);
        DBMS_OUTPUT.PUT_LINE('Status: ' || OUT_STATUS || CHR(13) ||
                             'Msg: ' || OUT_STATUS || CHR(13) ||
                             'Value: ' || OUT_ASSIGNED_VALUE || CHR(13) ||
                             'Assignment Id: ' || OUT_ASSIGNMENT_ID || CHR(13)); 
EXCEPTION
	WHEN OTHERS THEN
		V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
		DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;

DECLARE
	V_EXCEPTION_MSG VARCHAR2(500) := NULL;
    
    IN_VARIABLE_NAME VARCHAR2(50) := 'TESTTYPE';
    IN_PRODUCT VARCHAR2(50) := 'EG9515';
    IN_LOT VARCHAR2(50) := '1022502';
    IN_STATE VARCHAR2(50) := 'AFTER_M2A_PATTERNING';
    IN_ROUTE VARCHAR2(50) := 'BAWTW1';
    IN_OPER NUMBER := 2522;
    IN_FUNCTION VARCHAR2(50) := 'Test_UDVinUDV';
    OUT_STATUS NUMBER := NULL;
    OUT_MESSAGE VARCHAR2(4000) := NULL;
    OUT_ASSIGNED_VALUE VARCHAR2(500) := NULL;
    OUT_ASSIGNMENT_ID NUMBER := NULL;
    
BEGIN
       UDV_RETRIEVE_VAL.GET_UDV_VALUE(IN_VARIABLE_NAME, IN_PRODUCT, IN_LOT, IN_STATE, IN_ROUTE, IN_OPER, IN_FUNCTION, OUT_STATUS, OUT_MESSAGE, OUT_ASSIGNED_VALUE, OUT_ASSIGNMENT_ID);
        DBMS_OUTPUT.PUT_LINE('Status: ' || OUT_STATUS || CHR(13) ||
                             'Msg: ' || OUT_STATUS || CHR(13) ||
                             'Value: ' || OUT_ASSIGNED_VALUE || CHR(13) ||
                             'Assignment Id: ' || OUT_ASSIGNMENT_ID || CHR(13)); 
EXCEPTION
	WHEN OTHERS THEN
		V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
		DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
------------------------------------------
    --Throw error if any of these assignments do not exist
    WITH V (ID)
    AS
    (
      SELECT 11087 ID FROM DUAL UNION ALL
      SELECT 11088 ID FROM DUAL UNION ALL
      SELECT 11137 ID FROM DUAL UNION ALL
      SELECT 11129 ID FROM DUAL UNION ALL
      SELECT 11086 ID FROM DUAL
    )
    SELECT LISTAGG(V.ID, ',') WITHIN GROUP(ORDER BY V.ID) INTO V_NON_EXISTING_ASSIGNMENTS
    FROM V
    LEFT JOIN BAWUDV_ASSIGNMENT I 
        ON I.ASSIGNMENT_ID = V.ID
    WHERE I.ASSIGNMENT_ID IS NULL;
    
    IF(V_NON_EXISTING_ASSIGNMENTS  IS NOT NULL)THEN
        DBMS_OUTPUT.PUT_LINE('These assignment(s) do not exist: (' || V_NON_EXISTING_ASSIGNMENTS || ')');
        RETURN;
    END IF;
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