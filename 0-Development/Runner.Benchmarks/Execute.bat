@echo off
echo.

::set /p input=Select [0]:Debug, [1]:Local, [2]:Release ...

::if "%input%"=="0" goto DEBUG
::if "%input%"=="1" goto LOCAL
::if "%input%"=="1" goto RELEASE
::echo Selection: %input%
::goto END

:RELEASE
echo EXECUTING IN 'RELEASE' MODE...
echo PLEASE WAIT TO SELECT THE TEST TO RUN:
dotnet run -c release

:::END