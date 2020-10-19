﻿--TABLE: APP_USER_ROLE
DROP SEQUENCE APP_USER_SEQ;
DROP TRIGGER APP_USER_PK_TRGR;
DROP TABLE APP_USER;
DROP TABLE APP_USER_ROLE;
------------------------------

CREATE TABLE APP_USER_ROLE
(
   ROLE_ID NUMBER PRIMARY KEY NOT NULL, 
   ROLE_NAME VARCHAR2(20)
);

INSERT INTO APP_USER_ROLE(ROLE_ID, ROLE_NAME) VALUES(1, 'ADMIN');
INSERT INTO APP_USER_ROLE(ROLE_ID, ROLE_NAME) VALUES(2, 'OPERATOR'); 
INSERT INTO APP_USER_ROLE(ROLE_ID, ROLE_NAME) VALUES(3, 'GUEST');
COMMIT;

SELECT * FROM APP_USER_ROLE;

--TABLE: APP_USER
CREATE TABLE APP_USER
(
   ID NUMBER PRIMARY KEY NOT NULL, 
   LOGIN_NAME VARCHAR2(20) NOT NULL, 
   LOGIN_PASSWORD VARCHAR2(30) NOT NULL, 
   DISPLAY_NAME VARCHAR2(30) NOT NULL,
   EMAIL VARCHAR2(80) NOT NULL, 
   STATUS NUMBER DEFAULT 1 NOT NULL,
   ROLE_ID NUMBER NOT NULL, 
   DATE_CREATED DATE NOT NULL, 
   DATE_MODIFIED DATE, 
   
   CONSTRAINT FK_ROLE_ID FOREIGN KEY 
   (ROLE_ID) REFERENCES APP_USER_ROLE(ROLE_ID)
);

--SEQUENCE: APP_USER_SEQ
CREATE SEQUENCE APP_USER_SEQ
START WITH 1
INCREMENT BY 1;

--TRIGGER: APP_USER_PK_TRGR
SET DEFINE OFF;
CREATE OR REPLACE TRIGGER APP_USER_PK_TRGR
BEFORE INSERT ON APP_USER
FOR EACH ROW

BEGIN
      SELECT APP_USER_SEQ.NEXTVAL INTO :NEW.ID
      FROM DUAL;
      
END APP_USER_PK_TRGR;

INSERT INTO APP_USER(LOGIN_NAME, LOGIN_PASSWORD, DISPLAY_NAME, EMAIL, STATUS, ROLE_ID, DATE_CREATED, DATE_MODIFIED)
VALUES('kmubarak', 'p2G5OkFjILs=', 'kashif mubarak', 'kashif.mubarak@qorvo.com', 1, 1, SYSDATE - 60, SYSDATE);
COMMIT; --May2015

SELECT * FROM APP_USER;

--TABLLE: APP_USER_PASSWORD_TRACKING
CREATE TABLE APP_USER_PASSWORD_TRACKING
(
  TRACKING_ID NUMBER PRIMARY KEY NOT NULL,
  USER_ID NUMBER, 
  PASSWORD VARCHAR2(30), 
  SALIENCE NUMBER, 
  
  CONSTRAINT USER_ID_PK FOREIGN KEY
  (USER_ID) REFERENCES APP_USER(ID)
  
);COMMIT;

--SEQUENCE: PASSWORD_TRACKING_SEQ
CREATE SEQUENCE PASSWORD_TRACKING_SEQ
START WITH 1
INCREMENT BY 1;

SET DEFINE OFF;
CREATE OR REPLACE TRIGGER PASSWORD_TRACKING_TRGR
BEFORE INSERT 
ON APP_USER_PASSWORD_TRACKING
FOR EACH ROW

BEGIN
      SELECT PASSWORD_TRACKING_SEQ.NEXTVAL
      INTO :NEW.TRACKING_ID
      FROM DUAL;
      
END PASSWORD_TRACKING_TRGR;

SELECT * FROM APP_USER_PASSWORD_TRACKING;

--------------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE PKG_APP_USERS AS 

TYPE T_USER_NAMES IS TABLE OF VARCHAR2(100) INDEX BY PLS_INTEGER;
TYPE T_IDS IS TABLE OF NUMBER INDEX BY PLS_INTEGER;

 PROCEDURE PROC_GET_ASSOCIATIVE_ARRAYS(
           P_NAMES_OUT OUT T_USER_NAMES, 
           P_IDS_OUT OUT T_IDS);

 PROCEDURE ADD_NEW_APP_USER(
           P_LOGIN_NAME VARCHAR2, 
           P_LOGIN_PASSWORD VARCHAR2, 
           P_DISPLAY_NAME VARCHAR2, 
           P_EMAIL VARCHAR2, 
           P_ROLE_ID NUMBER, 
           P_ID_OUT OUT NUMBER);

 PROCEDURE PROC_PASS_ASSOCIATIVE_ARRAYS(
           P_NAMES_IN T_USER_NAMES,
           P_NUM_IN T_IDS,
           P_MSG_OUT OUT VARCHAR2);

END PKG_APP_USERS;
/


CREATE OR REPLACE PACKAGE BODY PKG_APP_USERS AS


  PROCEDURE PROC_GET_ASSOCIATIVE_ARRAYS(P_NAMES_OUT OUT T_USER_NAMES, 
                                      P_IDS_OUT OUT T_IDS) 
  AS
        V_COUNTER NUMBER;

        CURSOR APP_USER_CURSOR IS
        SELECT "ID", LOGIN_NAME 
        FROM APP_USER 
        ORDER BY "ID";

        ROW_APP_USER_CURSOR APP_USER_CURSOR%ROWTYPE;

  BEGIN
         V_COUNTER := 1;
         OPEN APP_USER_CURSOR;
         LOOP
                 FETCH APP_USER_CURSOR INTO ROW_APP_USER_CURSOR;
                 EXIT WHEN APP_USER_CURSOR%NOTFOUND;
                 P_NAMES_OUT(V_COUNTER) := ROW_APP_USER_CURSOR.LOGIN_NAME;
                 P_IDS_OUT(V_COUNTER) := ROW_APP_USER_CURSOR."ID";
                 V_COUNTER := V_COUNTER + 1;       
         END LOOP;

  END PROC_GET_ASSOCIATIVE_ARRAYS;

  PROCEDURE ADD_NEW_APP_USER(P_LOGIN_NAME VARCHAR2, P_LOGIN_PASSWORD VARCHAR2, 
                           P_DISPLAY_NAME VARCHAR2, P_EMAIL VARCHAR2, P_ROLE_ID NUMBER, 
                           P_ID_OUT OUT NUMBER) 
  AS
        V_USER_ID NUMBER;
  BEGIN
         INSERT INTO APP_USER(LOGIN_NAME, LOGIN_PASSWORD, DISPLAY_NAME, EMAIL, STATUS, 
                              ROLE_ID, DATE_CREATED, DATE_MODIFIED)
                    VALUES(P_LOGIN_NAME, P_LOGIN_PASSWORD, P_DISPLAY_NAME, P_EMAIL,
                           1, P_ROLE_ID, SYSDATE, SYSDATE);

         SELECT APP_USER_SEQ.CURRVAL INTO V_USER_ID FROM DUAL;

         INSERT INTO APP_USER_PASSWORD_TRACKING(USER_ID, PASSWORD, SALIENCE)
                                 VALUES(V_USER_ID, P_LOGIN_PASSWORD, 1);

         P_ID_OUT := V_USER_ID;

  END ADD_NEW_APP_USER;

   PROCEDURE PROC_PASS_ASSOCIATIVE_ARRAYS(
           P_NAMES_IN T_USER_NAMES,
           P_NUM_IN T_IDS,
           P_MSG_OUT OUT VARCHAR2)
   AS
   BEGIN
          P_MSG_OUT := 'Names passed: ';
          FOR i IN 1 .. P_NAMES_IN.COUNT
          LOOP
              P_MSG_OUT := P_MSG_OUT || P_NAMES_IN(i) || ', ';
          END LOOP;

          P_MSG_OUT := P_MSG_OUT || CHR(13) || ' Numbers passed: ' || CHR(13);

          FOR i IN 1 .. P_NUM_IN.COUNT
          LOOP
              P_MSG_OUT := P_MSG_OUT || TO_CHAR(P_NUM_IN(i)) || ', ';
          END LOOP;

   END PROC_PASS_ASSOCIATIVE_ARRAYS;

END PKG_APP_USERS;
/


