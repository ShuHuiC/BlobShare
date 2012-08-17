@echo off
setlocal

%~d0
cd "%~dp0"

CALL .\ValidateConfiguration.cmd
CALL .\installPSSnapIn.cmd
CALL .\SetupDB.cmd
CALL .\SetupCertificates.cmd
CALL .\SetupACS.cmd
CALL .\SetupSMTP.cmd
CALL .\SetupAdminSecret.cmd
CALL .\build.cmd
CALL .\Deploy.cmd

pause