//using Playground.Core.AdoNet;
//using Playground.Core.Diagnostics;
//using Playground.Core.Utilities;
//using Playground.MultiThreadedService.Base;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;

//namespace Playground.MultiThreadedService.Threads
//{
//    public enum LastStatusType
//    {
//        DONE,
//        OK,
//        PENDING,
//        ERROR
//    }

//    public enum ServiceOperations
//    {
//        StartService,
//        RestartService,
//        RebootPc,
//        None,
//        Upgrade,
//        NoUpgradeRequired
//    }

//    public class UpgraderThread : BaseTimerThread
//    {
//        protected override async void Run(object source)
//        {
//            if (IsStopping)
//            {
//                Logger.Info($"{Name} is stopping...");
//                return;
//            }

//            Logger.Info($"{Name} started...");

//            try
//            {
//                Logger.Info($"{Name}: Checking for upgrade...");
//                var machine = Environment.MachineName.Trim();

//                //Set the database flag.
//                var useTestDb = DAL.Seraph.ExecuteScalar($@"SELECT USE_DEV_DB FROM BAW_SVR_SERVER WHERE MACHINE = '{machine}'").ToString();
//                ServiceConfig.UseDevDB = (useTestDb == "Y");
//                Core.CoreConfig.UseDevDatabase = () => ServiceConfig.UseDevDB;

//                //This service will only be looking at the production database "BAW_SVR_UPGRADE" table.
//                //Check and see if service needs to be upgraded.
//                var dt = DAL.Seraph.ExecuteQuery($@"
//                SELECT A.SVC_ACCT_ID,
//                       A.SVC_ACCT_P,
//                       A.ALLOW_BETA_RELEASE,
//                       DECODE(A.CURRENT_DV_VERSION, NULL, 'NONE', A.CURRENT_DV_VERSION) CURRENT_DV_VERSION,
//                       B.TARGET_VERSION,
//                       DECODE(B.LAST_STATUS, NULL, ' ', B.LAST_STATUS) LAST_STATUS,
//                       DECODE(B.UPGRADE_STATUS, NULL, 'OK', B.UPGRADE_STATUS) UPGRADE_STATUS,
//                       DECODE(B.STATUS_TIME_STAMP, NULL, TO_TIMESTAMP('01-01-1970', 'MM-DD-YYYY'), B.STATUS_TIME_STAMP) STATUS_TIME_STAMP, 
//                       DECODE(B.RETRY_COUNT_B4_REBOOT, NULL, 5, B.RETRY_COUNT_B4_REBOOT) RETRY_COUNT_B4_REBOOT,
//                       A.INIT_START_TIMESTAMP,
//                       A.INIT_END_TIMESTAMP, 
//                       B.FORCE_REINSTALL 
//                FROM BAW_SVR_SERVER A
//                    INNER JOIN BAW_SVR_UPGRADE B
//                        ON A.MACHINE = B.MACHINE
//                WHERE A.MACHINE = '{machine}'
//                --AND (A.CURRENT_DV_VERSION <> B.TARGET_VERSION OR A.CURRENT_DV_VERSION IS NULL)
//                ");

//                if (dt.Rows.Count == 0)
//                {
//                    Logger.Info($"Machine {machine} not found in the database");
//                    return;
//                }

//                //
//                // upgrade if necessary
//                //

//                SendHeartbeat(machine);

//                var userName = dt.Rows[0]["SVC_ACCT_ID"].ToString();
//                var password = dt.Rows[0]["SVC_ACCT_P"].ToString();
//                var allowBetaRelease = dt.Rows[0]["ALLOW_BETA_RELEASE"].ToString();
//                var previousVersion = dt.Rows[0]["CURRENT_DV_VERSION"].ToString();
//                var targetVersion = dt.Rows[0]["TARGET_VERSION"].ToString();
//                var lastStatus = dt.Rows[0]["LAST_STATUS"].ToString();
//                var upgradeStatus = dt.Rows[0]["UPGRADE_STATUS"].ToString();
//                var statusTimeStamp = Convert.ToDateTime(dt.Rows[0]["STATUS_TIME_STAMP"]);
//                var forceReinstall = dt.Rows[0]["FORCE_REINSTALL"].ToString();
//                ServiceConfig.RetryBeforeReboot = Convert.ToInt32(dt.Rows[0]["RETRY_COUNT_B4_REBOOT"]);
//                DateTime? initStartTimestamp = null;
//                DateTime? initEndTimestamp = null;
//                ServiceOperations serviceOper = ServiceOperations.None;

//                if (dt.Rows[0]["INIT_START_TIMESTAMP"] != DBNull.Value)
//                {
//                    initStartTimestamp = Convert.ToDateTime(dt.Rows[0]["INIT_START_TIMESTAMP"]);
//                }

//                if (dt.Rows[0]["INIT_END_TIMESTAMP"] != DBNull.Value)
//                {
//                    initEndTimestamp = Convert.ToDateTime(dt.Rows[0]["INIT_END_TIMESTAMP"]);
//                }

//                string updateDirectoryPath;
//                if (allowBetaRelease == "Y")
//                {
//                    updateDirectoryPath = $@"\\dfwdata\public\tooldata\baw\apps\BawDVBETAs AutoUpdate\DVService\{targetVersion}";
//                }
//                else
//                {
//                    updateDirectoryPath = $@"\\dfwdata\public\tooldata\baw\apps\BawDataViewer\DVService\{targetVersion}";
//                }

//                bool didInitializationHang = initStartTimestamp != null && initEndTimestamp == null && DateTime.Now.Subtract(initStartTimestamp.Value).TotalMinutes > 5;
//                bool isFirstRun = ServiceConfig.IsFirstRun;
//                bool needsUpgrade = previousVersion != targetVersion || forceReinstall == "Y";
//                bool isMainServiceStopped = GetMainServiceStatus(machine) == ServiceControllerStatus.Stopped;
//                bool didExceedRetryCount = ServiceConfig.ServiceRestartCount > ServiceConfig.RetryBeforeReboot && ServiceConfig.RetryBeforeReboot > 0;
//                bool didInitializationSucceed = initStartTimestamp != null && initEndTimestamp != null;

//                if (didInitializationSucceed)
//                {
//                    ServiceConfig.ServiceRestartCount = 0;
//                }

//                //
//                if (didExceedRetryCount)
//                {
//                    serviceOper = ServiceOperations.RebootPc;
//                }
//                else if (didInitializationHang)
//                {
//                    ServiceConfig.ServiceRestartCount++;
//                    serviceOper = ServiceOperations.RestartService;
//                }
//                else if (needsUpgrade)
//                {
//                    if (targetVersion.Split('.').Length != 4)
//                    {
//                        UpdateLastStatus(LastStatusType.ERROR, $"Service will not be upgraded as the TARGET_VERSION ({targetVersion}) is not a valid version.", machine, previousVersion, targetVersion);
//                    }
//                    else
//                    {
//                        var okToUpgrade = (allowBetaRelease == "Y" || targetVersion.Split('.')[3] == "0");
//                        var timeDifference = DateTime.Now.Subtract(statusTimeStamp);
//                        var timeToWait = TimeSpan.FromMinutes(3);

//                        // wait for pending upgrade to complete
//                        if (upgradeStatus == LastStatusType.PENDING.ToString())
//                        {
//                            Logger.Info($"Upgrade currently underway...");
//                        }
//                        else if (!okToUpgrade)
//                        {
//                            UpdateLastStatus(LastStatusType.ERROR, $"Service will not be upgraded as the TARGET_VERSION ({targetVersion}) is not allowed to be released on {machine}", machine, previousVersion, targetVersion);
//                        }
//                        else if (true == targetVersion?.Contains(".."))
//                        {
//                            UpdateLastStatus(LastStatusType.ERROR, "targetVersion cannot contain '..'", machine, previousVersion, targetVersion);
//                        }
//                        else if (lastStatus.StartsWith(LastStatusType.ERROR.ToString()) && timeToWait.TotalMinutes > timeDifference.TotalMinutes)
//                        {
//                            // if there was an error during the last upgrade attempt, wait a longer amount of time before retrying
//                            Logger.Info($"{Name} had an error: {lastStatus}... so waiting up to {timeToWait.TotalMinutes} minutes before retrying");
//                        }
//                        else
//                        {
//                            serviceOper = ServiceOperations.Upgrade;
//                        }
//                    }
//                }
//                else if (isFirstRun && isMainServiceStopped)
//                {
//                    serviceOper = ServiceOperations.StartService;
//                }
//                else
//                {
//                    serviceOper = ServiceOperations.NoUpgradeRequired;
//                }

//                ServiceConfig.IsFirstRun = false;

//                switch (serviceOper)
//                {
//                    case ServiceOperations.RebootPc:
//                        RebootThisServer(machine, previousVersion, targetVersion);
//                        break;
//                    case ServiceOperations.Upgrade:
//                        await DoUpgrade(updateDirectoryPath, machine, previousVersion, targetVersion, userName, password);
//                        break;
//                    case ServiceOperations.RestartService:
//                        await RestartService(machine, previousVersion, targetVersion, userName, password);
//                        break;
//                    case ServiceOperations.StartService:
//                        await TryStartMainService(machine, previousVersion, targetVersion);
//                        break;
//                    case ServiceOperations.NoUpgradeRequired:
//                        UpdateLastStatus(LastStatusType.OK, "No need to upgrade", machine, previousVersion, targetVersion);
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.Error(ex.Message, ex);
//                try
//                {
//                    UpdateLastStatus(LastStatusType.ERROR, ex.Message, Environment.MachineName, "Not Available", "Not Available");
//                }
//                catch (Exception ex2)
//                {
//                    Logger.Error(ex2.Message, ex2);
//                }
//            }

//            Logger.Info($"{Name} finished.");
//        }

//        private async Task<bool> DoUpgrade(string updateDirectoryPath, string machine, string previousVersion, string targetVersion, string userName, string password)
//        {
//            try
//            {
//                if (!Directory.Exists(updateDirectoryPath))
//                {
//                    UpdateLastStatus(LastStatusType.ERROR, $"Target update directory not found: {updateDirectoryPath} ...... ", machine, previousVersion, targetVersion);
//                    return false;
//                }

//                // 1) stop the old service
//                var serviceResult = await StopService(userName, password, machine, previousVersion, targetVersion);

//                if (serviceResult.isError)
//                {
//                    UpdateLastStatus(LastStatusType.ERROR, serviceResult.message, machine, previousVersion, targetVersion);
//                    return false;
//                }

//                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

//                if (!CopyUpgradeFiles(machine, previousVersion, targetVersion, userName, updateDirectoryPath)) return false;

//                if (!await TryStartMainService(machine, previousVersion, targetVersion)) return false;

//                UpdateLastStatus(LastStatusType.DONE, "Successfully started new service", machine, previousVersion, targetVersion);
//            }
//            catch (Exception ex)
//            {
//                UpdateLastStatus(LastStatusType.ERROR, $"Failed to upgrade service: {ex.Message}", machine, previousVersion, targetVersion);
//                return false;
//            }

//            return true;
//        }

//        private async Task<bool> RestartService(string machine, string previousVersion, string targetVersion, string userName, string password)
//        {
//            try
//            {
//                // 1) stop the old service
//                var serviceResult = await StopService(userName, password, machine, previousVersion, targetVersion);

//                if (serviceResult.isError)
//                {
//                    UpdateLastStatus(LastStatusType.ERROR, serviceResult.message, machine, previousVersion, targetVersion);
//                    return false;
//                }

//                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

//                if (!await TryStartMainService(machine, previousVersion, targetVersion)) return false;

//                UpdateLastStatus(LastStatusType.DONE, "Successfully started new service", machine, previousVersion, targetVersion);
//            }
//            catch (Exception ex)
//            {
//                UpdateLastStatus(LastStatusType.ERROR, $"Failed to upgrade service: {ex.Message}", machine, previousVersion, targetVersion);
//                return false;
//            }

//            return true;
//        }

//        private bool CopyUpgradeFiles(string machine, string previousVersion, string targetVersion, string userName, string updateDirectoryPath)
//        {
//            try
//            {
//                UpdateLastStatus(LastStatusType.PENDING, "3: - Copying new files ...", machine, previousVersion, targetVersion);

//                var previousVersionPath = Path.Combine(@"C:\Users", userName, @"Documents\BawDataViewerService_PreviousVersion");
//                var currentVersionPath = Path.Combine(@"C:\Users", userName, @"Documents\BawDataViewerService");

//                // 3) copy the binaries for the new service
//                if (Directory.Exists(previousVersionPath))
//                {
//                    Directory.Delete(previousVersionPath, true);
//                }

//                if (Directory.Exists(currentVersionPath))
//                {
//                    Directory.Move(currentVersionPath, previousVersionPath);
//                }

//                DirectoryCopy(updateDirectoryPath, currentVersionPath, true);
//                UpdateLastStatus(LastStatusType.PENDING, "Successfully copied service binaries", machine, previousVersion, targetVersion);
//            }
//            catch (Exception ex)
//            {
//                UpdateLastStatus(LastStatusType.ERROR, ex.Message, machine, previousVersion, targetVersion);
//                return false;
//            }

//            return true;
//        }

//        private async Task<bool> TryStartMainService(string machine, string previousVersion, string targetVersion)
//        {
//            UpdateLastStatus(LastStatusType.PENDING, "4: - Starting the service ...", machine, previousVersion, targetVersion);

//            ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, machine, "BawDataViewerService");
//            scp.Assert();

//            using (var sc = new ServiceController("BawDataViewerService", machine))
//            {
//                if (sc.Status == ServiceControllerStatus.Stopped)
//                {
//                    try
//                    {
//                        // Tell the service to shut down when it has a break in it's work
//                        sc.Start();
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.Info($"Windows ServiceController failed to start the new service. Waiting for another 3 minutes to see if it starts: {ex.Message}");
//                    }

//                    var maxTimeout = TimeSpan.FromMinutes(3);
//                    var didStart = await WhileConditionIsTrue(() => sc.Status != ServiceControllerStatus.Running, () => sc.Refresh(), maxTimeout).ConfigureAwait(false);
//                    if (!didStart)
//                    {
//                        UpdateLastStatus(LastStatusType.ERROR, $"Failed to start new service", machine, previousVersion, targetVersion);
//                        ServiceConfig.ServiceRestartCount++;
//                        return false;
//                    }

//                    UpdateLastStatus(LastStatusType.OK, "5: - Successfully started BawDataViewer service.", machine, previousVersion, targetVersion);
//                }
//            }

//            return true;
//        }

//        private void RebootThisServer(string machine, string previousVersion, string targetVersion)
//        {
//            Logger.Info("Attempting to reboot");
//            UpdateLastStatus(LastStatusType.ERROR, "Server is being rebooted.", machine, previousVersion, targetVersion);
//            Process.Start("shutdown.exe", "-r -t 0 -d u:4:6");
//        }

//        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
//        {
//            // Get the subdirectories for the specified directory.
//            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

//            if (!dir.Exists)
//            {
//                throw new DirectoryNotFoundException(
//                    "Source directory does not exist or could not be found: "
//                    + sourceDirName);
//            }

//            DirectoryInfo[] dirs = dir.GetDirectories();

//            // If the destination directory doesn't exist, create it.       
//            Directory.CreateDirectory(destDirName);

//            // Get the files in the directory and copy them to the new location.
//            FileInfo[] files = dir.GetFiles();
//            foreach (FileInfo file in files)
//            {
//                var tempPath = Path.Combine(destDirName, file.Name);
//                file.CopyTo(tempPath, false);
//            }

//            // If copying subdirectories, copy them and their contents to new location.
//            if (copySubDirs)
//            {
//                foreach (DirectoryInfo subDir in dirs)
//                {
//                    var tempPath = Path.Combine(destDirName, subDir.Name);
//                    DirectoryCopy(subDir.FullName, tempPath, copySubDirs);
//                }
//            }
//        }

//        private void SendHeartbeat(string machine)
//        {
//            //Update the heartbeat
//            DAL.Seraph.ExecuteNonQuery($@"
//                    UPDATE BAW_SVR_UPGRADE 
//                    SET SERVICE_UPGRADER_HEARTBEAT = SYSTIMESTAMP,
//                        FORCE_REINSTALL = 'N'
//                    WHERE MACHINE = '{machine}'");
//        }

//        private async Task<(bool isError, string message)> StopService(string userName, string password, string machine, string previousVersion, string targetVersion)
//        {
//            UpdateLastStatus(LastStatusType.PENDING, "1: - Stopping service ...", machine, previousVersion, targetVersion);

//            try
//            {
//                if (ServiceController.GetServices(machine)
//                    .Any(svc => svc.ServiceName == "BawDataViewerService"))
//                {
//                    ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, machine, "BawDataViewerService");
//                    scp.Assert();

//                    using (var sc = new ServiceController("BawDataViewerService", machine))
//                    {
//                        if (sc.Status == ServiceControllerStatus.Stopped)
//                        {
//                            // if the service is already stopped, just let the user know and
//                            // don't do anything else
//                            return (isError: false, message: $"Stopped service on {machine}");
//                        }

//                        if (sc.Status == ServiceControllerStatus.Running)
//                        {
//                            // Tell the service to shut down when it has a break in it's work
//                            sc.ExecuteCommand((int)ServiceCommands.ShutDownWhenPossible);
//                        }

//                        if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StopPending)
//                        {
//                            // wait while the service is stopping
//                            var maxTimeout = TimeSpan.FromHours(2);
//                            var didStop = await WhileConditionIsTrue(() => sc.Status != ServiceControllerStatus.Stopped, () => sc.Refresh(), maxTimeout).ConfigureAwait(false);
//                            if (!didStop)
//                            {
//                                return (isError: true, message: $"Failed to stop service on {machine} after waiting for {maxTimeout.TotalHours} hours");
//                            }
//                        }
//                    }

//                    return (isError: false, message: $"Stopped service on {machine}");
//                }

//                return (isError: false, message: $"The service could not be found on {machine}");
//            }
//            catch (Exception ex)
//            {
//                Logger.Error(ex.Message, ex);
//                return (isError: true,
//                    message: $"An error occurred while stopping service on {machine}");
//            }
//        }

//        private ServiceControllerStatus GetMainServiceStatus(string machine)
//        {
//            ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, machine, "BawDataViewerService");
//            scp.Assert();

//            using (var sc = new ServiceController("BawDataViewerService", machine))
//            {
//                return sc.Status;
//            }
//        }

//        private void UpdateLastStatus(LastStatusType type, string statusMessage, string machine, string previousVersion, string targetVersion)
//        {
//            Logger.Info($"{type}: {statusMessage} on machine: {machine} (old: {previousVersion} / new: {targetVersion})");

//            DAL.Seraph.ExecuteNonQuery($@"
//                                        UPDATE BAW_SVR_UPGRADE
//                                        SET LAST_STATUS = '{HelperTools.FormatSqlString(statusMessage)} (old: {HelperTools.FormatSqlString(previousVersion)} / new: {HelperTools.FormatSqlString(targetVersion)})',
//                                        UPGRADE_STATUS = '{type}',
//                                        STATUS_TIME_STAMP = SYSTIMESTAMP,
//                                        SERVICE_UPGRADER_HEARTBEAT = SYSTIMESTAMP
//                                        WHERE MACHINE = '{HelperTools.FormatSqlString(machine)}'");
//        }

//        async Task<bool> WhileConditionIsTrue(Func<bool> condition, Action iterationOperation, TimeSpan maxTimeout)
//        {
//            var sw = Stopwatch.StartNew();

//            iterationOperation?.Invoke();
//            while (condition())
//            {
//                if (maxTimeout.CompareTo(sw.Elapsed) < 0)
//                {
//                    return false;
//                }

//                await Task.Delay(TimeSpan.FromMilliseconds(250)).ConfigureAwait(false);
//                // break if timeout longer than 10 minutes

//                iterationOperation?.Invoke();
//            }

//            return true;
//        }
//    }
//}
