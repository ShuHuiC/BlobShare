@echo off

msiexec /qn /i "%~dp0capicom_dc_sdk.msi"

IF EXIST "%PROGRAMFILES%\Microsoft CAPICOM 2.1.0.2 SDK" (
    SET capicompath="%PROGRAMFILES%\Microsoft CAPICOM 2.1.0.2 SDK\Samples\vbs\cstore.vbs"
    SET cscript=%windir%\system32\cscript.exe
	%windir%\system32\regsvr32.exe /s "%PROGRAMFILES%\Microsoft CAPICOM 2.1.0.2 SDK\Lib\X86\capicom.dll"
)

IF EXIST "%PROGRAMFILES(x86)%\Microsoft CAPICOM 2.1.0.2 SDK" (
    SET capicompath="%PROGRAMFILES(x86)%\Microsoft CAPICOM 2.1.0.2 SDK\Samples\vbs\cstore.vbs"
    SET cscript=%windir%\syswow64\cscript.exe
    ECHO Setting up CAPICOM for 64 bits environment...
    copy /y "%PROGRAMFILES(x86)%\Microsoft CAPICOM 2.1.0.2 SDK\Lib\X86\capicom.dll" %windir%\syswow64
    %windir%\syswow64\regsvr32.exe /s %windir%\syswow64\capicom.dll
)

echo.
echo ========= Installing "CN=BlobShare.cloudapp.net" certificate =========
certutil -addstore "trustedPeople" "%~dp0..\..\resources\BlobShare.cloudapp.net.cer"
%cscript% /nologo %capicompath% import -l LM "%~dp0..\..\resources\BlobShare.cloudapp.net.pfx" "abc!123"

"%~dp0winhttpcertcfg.exe" -g -c LOCAL_MACHINE\My -s BlobShare.cloudapp.net -a "NETWORK SERVICE"

echo.
echo Done

if errorlevel 1 (
  echo ---------------------------------------------------------------------------------------------
  echo   An error occured while installing certificates. Please check error messages above.
  echo ---------------------------------------------------------------------------------------------
  @PAUSE
)
