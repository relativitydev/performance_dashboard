@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%cd%
SET BUILD_DIR=%~d0%~p0%
SET NANT="%BUILD_DIR%lib\Nant\nant.exe"
SET build.config.settings="%DIR%\settings\UppercuT.config"

%NANT% -logger:NAnt.Core.DefaultLogger -quiet /f:"%BUILD_DIR%build\default.build" -D:build.config.settings=%build.config.settings% %*

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:usage
echo.
echo Usage: build.bat
echo.
goto finish

:errors
PAUSE
EXIT /B %ERRORLEVEL%

:finish
call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
copy "%~dp0\code_drop\RoundhousE\console\rh.exe" "..\..\trunk\SolutionReferences\roundhouse.0.8.6\bin\rh.exe"
corflags /32BIT- "..\..\trunk\SolutionReferences\roundhouse.0.8.6\bin\rh.exe"


