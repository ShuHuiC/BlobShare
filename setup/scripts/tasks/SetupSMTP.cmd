@echo off
setlocal
%~d0
cd "%~dp0"

IF EXIST %WINDIR%\SysWow64 (
	SET powerShellDir=%WINDIR%\SysWow64\windowspowershell\v1.0
) ELSE (
	SET powerShellDir=%WINDIR%\system32\windowspowershell\v1.0
)

%powerShellDir%\powershell.exe -NonInteractive -Command "Set-ExecutionPolicy unrestricted"
%powerShellDir%\powershell.exe -NonInteractive -command "%~dp0SetupSMTP ..\..\.. ..\..\..\code\BlobShare.Web"

if errorlevel 1 (
  echo -------------------------------------------------------------------------------
  echo   SMTP Configuration failed. Please check error messages above.
  echo -------------------------------------------------------------------------------
  @PAUSE
)

