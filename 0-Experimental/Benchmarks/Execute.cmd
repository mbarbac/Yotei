@echo off
echo.

echo EXECUTING IN 'RELEASE' MODE...
echo PLEASE WAIT TO SELECT THE TEST TO RUN:

del bin
del obj
dotnet run -c release