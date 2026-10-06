@echo off
cd /d "%~dp0"
echo FreshSpin - clean local build
echo.
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
dotnet restore
if errorlevel 1 pause & exit /b 1
dotnet run
pause
