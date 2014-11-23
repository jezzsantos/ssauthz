ECHO OFF
CD /d %~dp0

SET config=Debug
SET progfiles=%ProgramFiles%
IF EXIST "%ProgramFiles(x86)%" SET progfiles=%ProgramFiles(x86)%


REM Setup the Visual Studio Build Environment
CALL "%progfiles%\Microsoft Visual Studio 12.0\VC\vcvarsall.bat" x86
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL SetColor.bat 02 "-- Restoring NuGet Packages"
ECHO(
ECHO:

REM Restore all NuGet packages
CALL ..\tools\NuGet\nuget.exe restore ssauthz.sln
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL SetColor.bat 02 "-- Building Solution (Config %config%)"
ECHO(
ECHO:


REM Build solution
msbuild.exe ssauthz.sln /t:Rebuild /p:Configuration=%config% /verbosity:minimal
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL SetColor.bat 0A "---- ssauthz Built Successfully!"
ECHO(
ECHO:

ECHO:
CALL SetColor.bat 02 "-- Unit Testing Solution"
ECHO(
ECHO:

REM Run Unit tests
mstest.exe /testcontainer:Services.UnitTests\bin\%config%\Services.UnitTests.dll /detail:errormessage
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL SetColor.bat 0A "---- ssauthz Unit Tested Successfully!"
ECHO(
ECHO:



ECHO:
CALL SetColor.bat 02 "-- Integration Testing Solution"
ECHO(
ECHO:

REM Run Integration tests
mstest.exe /testcontainer:Services.IntTests\bin\%config%\Services.IntTests.dll /testsettings:ssauthz.testsettings /detail:errormessage
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL SetColor.bat 0A "---- Crib Integration Tested Successfully!"
ECHO(
ECHO:

ECHO:
CALL SetColor.bat 0A "---- ssauthz Built Successfully!"
ECHO(
ECHO:
IF NOT "%silent%"=="q" PAUSE
EXIT /b 0

:error
CD /d %~dp0
ECHO:
ECHO:
CALL SetColor.bat 04 "---- Failed Build and Test! error #%errorlevel% ----"
ECHO(
PAUSE
EXIT /b %errorlevel%