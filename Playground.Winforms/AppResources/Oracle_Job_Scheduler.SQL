--Job Scheduler:
BEGIN
DBMS_SCHEDULER.CREATE_JOB (
   job_name             => 'JOB_MONITOR_DATAVIEWER_SERVERS',
   job_type             => 'STORED_PROCEDURE',
   job_action           => 'BAW_SVR_PKG.SEND_HEARTBEAT_NOTIFICATION',
   start_date           =>  TRUNC(SYSDATE)+7/24,
   repeat_interval      => 'FREQ=DAILY; BYDAY=MON,TUE,WED,THU,FRI,SAT,SUN; BYHOUR=0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23;', 
   enabled              =>  TRUE,
   comments             => 'A scheduled job that determines if a BawDataViewer Server is down when Main-Thread-Heartbeat was too old.');
END;
-----------
BEGIN
DBMS_SCHEDULER.CREATE_JOB (
   job_name             => 'JOB_SYNC_ML_VER_ASSIGN_LOCK',
   job_type             => 'STORED_PROCEDURE',
   job_action           => 'BAWVIEWERTEST.PROC_SYNC_MLV_ASSIGNMENT_LOCK',
   start_date           =>  TRUNC(SYSDATE)+7/24,
   repeat_interval      => 'FREQ=DAILY; BYDAY=MON,TUE,WED,THU,FRI,SAT,SUN; BYMINUTE=0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55', 
   enabled              =>  TRUE,
   comments             => 'A scheduled job that syncs assignment locks.');
END;
--------------------------------