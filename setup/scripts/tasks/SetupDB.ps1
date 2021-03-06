param([string]$databaseScriptsPath, [string]$configurationFilePath, [string]$serviceConfigurationPath)

function UpdateConfigurationSetting($configurationFile, $connectionString, $settingKey)
{
    [xml]$xml = get-content $configurationFile;

	$entry = $xml.configuration.appSettings.add | Where-Object { $_.key -match $settingKey }
	$entry.value = $connectionString 

    $xml.Save($configurationFile);
}

function GetConfigurationValue($entry)
{
	if(-not ($entry)) 
	{ 
		return $null; 
	}
	
	if($entry.StartsWith('{'))
	{
		return $null;
	}
	return $entry;
}

# ------------------------------ 
# Obtaining Configuration Values
# ------------------------------
$configurationFilePath = "$configurationFilePath\Configuration.xml";
[xml]$xml = Get-Content $configurationFilePath;    
$useLocalComputeEmulator = [System.Convert]::ToBoolean($xml.Configuration.UseLocalComputeEmulator.ToLower());

$serverName = GetConfigurationValue($xml.Configuration.Database.ServerName);
$databaseName = GetConfigurationValue($xml.Configuration.Database.DatabaseName);
$username = GetConfigurationValue($xml.Configuration.Database.Username);
$password = GetConfigurationValue($xml.Configuration.Database.Password);
$serverLocation = GetConfigurationValue($xml.Configuration.Database.Location);
$storageAccountName = GetConfigurationValue($xml.Configuration.WindowsAzureStorage.StorageAccountName);
if($storageAccountName -ne $null)
{
	$storageAccountName = $storageAccountName.ToLower();
}
$storageAccountKey = GetConfigurationValue($xml.Configuration.WindowsAzureStorage.StorageAccountKey);
$subscriptionId = GetConfigurationValue($xml.Configuration.subscriptionId);
$thumbprint = GetConfigurationValue($xml.Configuration.managementCertificateThumbprint);

$sqlServerScriptPath = "$DatabaseScriptsPath\SqlServer.Setup.cmd";
$sqlAzureScriptPath = "$DatabaseScriptsPath\SqlAzure.Setup.cmd";
$serviceConfigurationPath = "$serviceConfigurationPath\Web.config";

# Get the management API certificate 
if($thumbprint -ne $null)
{
	$thumbprint = $thumbprint.ToUpper();	
	if ((Test-Path cert:\CurrentUser\MY\$thumbprint) -eq $true){
		$certificate = (Get-Item cert:\CurrentUser\MY\$thumbprint)
	} else {
		$certificate = (Get-Item cert:\LocalMachine\MY\$thumbprint)
	}
}

# ------------------
# Creating database 
# ------------------

Write-Output ""
Write-Output "Creating and populating the database..."

if($useLocalComputeEmulator)
{
    if ($serverName -eq $null)
	{
		$serverName = ".\SQLEXPRESS";
	}
	
	& $sqlServerScriptPath $serverName $databaseName;
    if($LASTEXITCODE -ne 0) { exit $LASTEXITCODE; }
}
else 
{
	Write-Output ""
	Write-Output "Creating the SQL Azure Server..."
	
	if ((Get-PSSnapin | ?{$_.Name -eq "WAPPSCmdlets"}) -eq $null) 
	{
		Add-PSSnapin WAPPSCmdlets	
	} 

	if ($serverName -eq $null)
	{
		$sqlServer = New-SqlAzureServer -AdministratorLogin $username -AdministratorLoginPassword $password -Location $serverLocation -SubscriptionId $subscriptionId -Certificate $certificate

		$sqlServer | New-SqlAzureFirewallRule -RuleName "MicrosoftServices" -StartIpAddress "0.0.0.0" -EndIpAddress "0.0.0.0"

		$sqlServer | New-SqlAzureFirewallRule -RuleName "SetupPC" -UseIpAddressDetection
		
		$serverName = $sqlServer.ServerName + ".database.windows.net"
		
		$username = $username + "@" + $sqlServer.ServerName
	}
	else
	{
		$username = $username + "@" + $serverName.Split('.')[0];
	}

	Write-Output "Server Name: " $serverName
	Write-Output "User Name: " $username
	
    & $sqlAzureScriptPath $serverName $databaseName $username $password;
    if($LASTEXITCODE -ne 0) { exit $LASTEXITCODE; }
}

# -----------------------------
# Updating connection strings 
# -----------------------------

Write-Output ""
Write-Output "Updating connection strings..."

if($useLocalComputeEmulator) 
{
	if($username -eq $null)
	{
		$connectionString = "Data Source=$serverName;Initial Catalog=$databaseName;Integrated Security=True";    
	}
	else
	{
		$connectionString = "Data Source=$serverName;Initial Catalog=$databaseName;User ID=$username;Password=$password";    
	}
}
else
{
    $connectionString = "Server=tcp:$serverName;Database=$databaseName;User ID=$username;Password=$password";
}

$settingKey = "BlobShareDataStoreEntities";
UpdateConfigurationSetting $serviceConfigurationPath $connectionString $settingKey;

if($useLocalComputeEmulator -AND $storageAccountName -eq $null) 
{
    $connectionString = "UseDevelopmentStorage=true";    
}
else 
{
	if($storageAccountName -ne $null -AND $storageAccountKey -ne $null)
	{
		$connectionString = "DefaultEndpointsProtocol=https;AccountName=$storageAccountName;AccountKey=$storageAccountKey";
	}
}
$settingKey = "DataConnectionString";
UpdateConfigurationSetting $serviceConfigurationPath $connectionString $settingKey;
