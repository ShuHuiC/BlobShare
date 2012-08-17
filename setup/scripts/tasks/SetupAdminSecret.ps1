param([string]$configurationFilePath, [string]$serviceConfigurationPath)

function UpdateConfigurationSetting($configurationFile, $value, $settingKey)
{
    [xml]$xml = get-content $configurationFile;

	$entry = $xml.configuration.appSettings.add | Where-Object { $_.key -match $settingKey }
	$entry.value = $value 

    $xml.Save($configurationFile);
}

# ------------------------------ 
# Obtaining Configuration Values
# ------------------------------
$configurationFilePath = "$configurationFilePath\Configuration.xml";
[xml]$xml = Get-Content $configurationFilePath;    
$adminSecret = $xml.Configuration.BootstrapAdministratorSecret;

$serviceConfigurationPath = "$serviceConfigurationPath\Web.config";

# -----------------------------
# Updating Admin Secret Setting
# -----------------------------

Write-Output ""
Write-Output "Updating Admin Secret setting..."

$settingKey = "BootstrapAdministratorSecret";
UpdateConfigurationSetting $serviceConfigurationPath $adminSecret $settingKey;