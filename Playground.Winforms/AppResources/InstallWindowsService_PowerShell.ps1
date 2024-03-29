<#
    http://www.geoffhudik.com/tech/2012/3/22/install-a-windows-service-remotely-with-powershell.html
    Powershell script to install windows service.
#>

function Get-Service(
    [string]$serviceName = $(throw "serviceName is required"), 
    [string]$targetServer = $(throw "targetServer is required"))
{
    $service = Get-WmiObject -Namespace "root\cimv2" -Class "Win32_Service" `
        -ComputerName $targetServer -Filter "Name='$serviceName'" -Impersonation 3    
    return $service
}


function Uninstall-Service(
    [string]$serviceName = $(throw "serviceName is required"), 
    [string]$targetServer = $(throw "targetServer is required"))
{
    $service = Get-Service $serviceName $targetServer
     
    if (!($service))
    { 
        Write-Warning "Failed to find service $serviceName on $targetServer. Nothing to uninstall."
        return
    }
     
    "Found service $serviceName on $targetServer; checking status"
             
    if ($service.Started)
    {
        "Stopping service $serviceName on $targetServer"
        #could also use Set-Service, net stop, SC, psservice, psexec etc.
        $result = $service.StopService()
        Test-ServiceResult -operation "Stop service $serviceName on $targetServer" -result $result
    }
     
    "Attempting to uninstall service $serviceName on $targetServer"
    $result = $service.Delete()
    Test-ServiceResult -operation "Delete service $serviceName on $targetServer" -result $result   
}


function Test-ServiceResult(
    [string]$operation = $(throw "operation is required"), 
    [object]$result = $(throw "result is required"), 
    [switch]$continueOnError = $false)
{
    $retVal = -1
    if ($result.GetType().Name -eq "UInt32") { $retVal = $result } else {$retVal = $result.ReturnValue}
         
    if ($retVal -eq 0) {return}
     
    $errorcode = 'Success,Not Supported,Access Denied,Dependent Services Running,Invalid Service Control'
    $errorcode += ',Service Cannot Accept Control, Service Not Active, Service Request Timeout'
    $errorcode += ',Unknown Failure, Path Not Found, Service Already Running, Service Database Locked'
    $errorcode += ',Service Dependency Deleted, Service Dependency Failure, Service Disabled'
    $errorcode += ',Service Logon Failure, Service Marked for Deletion, Service No Thread'
    $errorcode += ',Status Circular Dependency, Status Duplicate Name, Status Invalid Name'
    $errorcode += ',Status Invalid Parameter, Status Invalid Service Account, Status Service Exists'
    $errorcode += ',Service Already Paused'
    $desc = $errorcode.Split(',')[$retVal]
     
    $msg = ("{0} failed with code {1}:{2}" -f $operation, $retVal, $desc)
     
    if (!$continueOnError) { Write-Error $msg } else { Write-Warning $msg }        
}

function Install-Service(
    [string]$serviceName = $(throw "serviceName is required"), 
    [string]$targetServer = $(throw "targetServer is required"),
    [string]$displayName = $(throw "displayName is required"),
    [string]$physicalPath = $(throw "physicalPath is required"),
    [string]$userName = $(throw "userName is required"),
    [string]$password = "",
    [string]$startMode = "Automatic",
    [string]$description = "",
    [bool]$interactWithDesktop = $false
)
{
    # can't use installutil; only for installing services locally
    #[wmiclass]"Win32_Service" | Get-Member -memberType Method | format-list -property:*    
    #[wmiclass]"Win32_Service"::Create( ... )        
         
    # todo: cleanup this section 
    $serviceType = 16          # OwnProcess
    $serviceErrorControl = 1   # UserNotified
    $loadOrderGroup = $null
    $loadOrderGroupDepend = $null
    $dependencies = $null
    
    # description?
    $params = `
        $serviceName, `
        $displayName, `
        $physicalPath, `
        $serviceType, `
        $serviceErrorControl, `
        $startMode, `
        $interactWithDesktop, `
        $userName, `
        $password, `
        $loadOrderGroup, `
        $loadOrderGroupDepend, `
        $dependencies `
         
    $scope = new-object System.Management.ManagementScope("\\$targetServer\root\cimv2", `
        (new-object System.Management.ConnectionOptions))
    "Connecting to $targetServer"
    $scope.Connect()
    $mgt = new-object System.Management.ManagementClass($scope, `
        (new-object System.Management.ManagementPath("Win32_Service")), `
        (new-object System.Management.ObjectGetOptions))
     
    $op = "service $serviceName ($physicalPath) on $targetServer"    
    "Installing $op"
    $result = $mgt.InvokeMethod("Create", $params)    
    Test-ServiceResult -operation "Install $op" -result $result
    "Installed $op"
     
    "Setting $serviceName description to '$description'"
    Set-Service -ComputerName $targetServer -Name $serviceName -Description $description
    "Service install complete"
}


function Start-Service(
    [string]$serviceName = $(throw "serviceName is required"), 
    [string]$targetServer = $(throw "targetServer is required"))
{
    "Getting service $serviceName on server $targetServer"
    $service = Get-Service $serviceName $targetServer
    if (!($service.Started))
    {
        "Starting service $serviceName on server $targetServer"
        $result = $service.StartService()
        Test-ServiceResult -operation "Starting service $serviceName on $targetServer" -result $result   
    }
}

function Publish-Service
{
    Set-Activity "Deploying Windows Service"
    $serviceName = "SeraphService"
     
    Write-Log "Stopping, uninstalling service $serviceName on $_targetServer"
    Uninstall-Service $serviceName $_targetServer
 
    "Pausing to avoid potential temporary access denied"
    Start-Sleep -s 5 # Yeah I know, don't beat me up over this
     
    Remove-RootFilesInDir $_targetServicesDir
     
    Copy-Files "$_packagePath\Services\**" $_targetServicesDir -recurse
     
    Install-Service `
    -ServiceName $serviceName `
    -TargetServer $_targetServer `
    -DisplayName "MyApp Data Service" `
    -PhysicalPath "D:\Apps\MyApp\Services\MyApp.DataService.exe" `
    -Username "NT AUTHORITY\NetworkService" `
    -Description "Provides remote TCP/IP communication between the MyApp client application and the database tier."
         
    Start-Service $serviceName $_targetServer
}


