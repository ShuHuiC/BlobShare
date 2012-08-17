@echo off
setlocal
%~d0
cd "%~dp0"

echo F | XCOPY "..\..\..\Configuration.xml.bak" "..\..\..\Configuration.xml" /Y