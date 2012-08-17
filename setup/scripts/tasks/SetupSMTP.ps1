param([string]$configurationFilePath, [string]$serviceConfigurationPath)

function GetConfigurationValue($entry)
{
	if(-not ($entry)) 
	{ 
		return ""; 
	}
	
	if($entry.StartsWith('{'))
	{
		return "";
	}
	return $entry;
}

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

[string]$SMTPhost = GetConfigurationValue($xml.Configuration.SMTP.Host);
[string]$SMTPport = GetConfigurationValue($xml.Configuration.SMTP.Port);
[string]$SMTPuser = GetConfigurationValue($xml.Configuration.SMTP.User);
[string]$SMTPpassword = GetConfigurationValue($xml.Configuration.SMTP.Password);
[string]$SMTPsenderName = GetConfigurationValue($xml.Configuration.SMTP.SenderName);
[string]$SMTPsenderAddress = GetConfigurationValue($xml.Configuration.SMTP.SenderAddress);
  
$serviceConfigurationPath = "$serviceConfigurationPath\Web.config";

# -----------------------------
# Updating SMTP settings
# -----------------------------

Write-Output ""
Write-Output "Updating SMTP settings..."

$settingKey = "SmtpHost";
UpdateConfigurationSetting $serviceConfigurationPath $SMTPhost $settingKey;

$settingKey = "SmtpPort";
UpdateConfigurationSetting $serviceConfigurationPath $SMTPport $settingKey;

$settingKey = "SmtpUser";
UpdateConfigurationSetting $serviceConfigurationPath $SMTPuser $settingKey;

$settingKey = "SmtpPassword";
UpdateConfigurationSetting $serviceConfigurationPath $SMTPpassword $settingKey;

$settingKey = "SmtpSenderName";
UpdateConfigurationSetting $serviceConfigurationPath $SMTPsenderName $settingKey;

$settingKey = "SmtpSenderAddress";
UpdateConfigurationSetting $serviceConfigurationPath $SMTPsenderAddress $settingKey;