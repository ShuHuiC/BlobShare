param([string]$configurationFilePath, [string]$serviceConfigurationPath)

function SetupStartupTask($configurationFile, $commandLine)
{
    [xml]$xml = get-content $configurationFile;

	$entry = $xml.ServiceDefinition.WebRole.Startup.Task;
    $appendChild = $false;
	
    if ($entry -eq $null)
    {
        $ns = $xml.DocumentElement.NamespaceURI;
        $entry = $xml.CreateElement("Task", $ns);
		$appendChild = $true;
    }
    
	$entry.SetAttribute("commandLine", $commandLine);
	$entry.SetAttribute("executionContext", "elevated");
	$entry.SetAttribute("taskType", "simple");
	
	if ($appendChild) 
    { 
        $xml.ServiceDefinition.WebRole.Item("Startup").AppendChild($entry);	
    }
	
    $xml.Save($configurationFile);
}  

function RemoveStartupTasks($configurationFile)
{
    [xml]$xml = get-content $configurationFile;
	$xml.ServiceDefinition.WebRole.Item("Startup").RemoveAll();
    $xml.Save($configurationFile);
}

# ------------------------------ 
# Obtaining Configuration Values
# ------------------------------
$configurationFilePath = "$configurationFilePath\Configuration.xml";

[xml]$xml = Get-Content $configurationFilePath;    
$useLocalComputeEmulator = [System.Convert]::ToBoolean($xml.Configuration.UseLocalComputeEmulator.ToLower());

$serviceConfigurationPath = "$serviceConfigurationPath\ServiceDefinition.csdef";

# -----------------------------
# Setting Up Startup Tasks
# -----------------------------

Write-Output ""
Write-Output "Setting Up Startup Tasks..."

if ($useLocalComputeEmulator) {
	RemoveStartupTasks $serviceConfigurationPath
} else {
	SetupStartupTask $serviceConfigurationPath "Startup\SetupWifRuntime.cmd" 
}

if($LASTEXITCODE -ne 0) { exit $LASTEXITCODE; }