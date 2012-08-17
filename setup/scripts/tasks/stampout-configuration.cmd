@echo off
setlocal
%~d0
cd "%~dp0"

echo F | XCOPY "..\..\..\Configuration.xml" "..\..\..\Configuration.xml.bak" /Y
echo F | XCOPY "..\..\..\Configuration-Development.xml" "..\..\..\Configuration.xml" /Y