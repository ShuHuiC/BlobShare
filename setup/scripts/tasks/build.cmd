@ECHO off
%~d0
cd "%~dp0"

@REM ----------------------------------------------------------------------------------
@REM Build the Blob Share solution
@REM ----------------------------------------------------------------------------------

set buildType=Release
set verbosity=quiet
set pause=true

:: Check for 64-bit Framework
if exist %WINDIR%\Microsoft.NET\Framework64\v4.0.30319 (
  set msbuild=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe
  goto :run
)
:: Check for 32-bit Framework
if exist %WINDIR%\Microsoft.NET\Framework\v4.0.30319 (
  set msbuild=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
  goto :run
)

:run
call "%msbuild%" %cd%\..\..\..\code\BlobShare.sln /p:Configuration=Release /t:publish /p:TargetProfile=Small

@if errorlevel 1 goto :error
@echo.
@echo Build Complete

@goto :exit

:error
@echo.
@echo An error occured building the Blob Share solution

:exit