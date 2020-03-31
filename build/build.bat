@echo off
setlocal enabledelayedexpansion

set SolutionFile=../src/Perplex.ContentBlocks.sln

REM Requires nuget.exe: https://www.nuget.org/downloads
REM Requires vswhere.exe: https://github.com/Microsoft/vswhere/releases
REM Without Visual Studio, requires Build Tools for Visual Studio 2017: https://visualstudio.microsoft.com/downloads/ (under Tools for Visual Studio 2017)

REM Find MSBuild -- https://github.com/Microsoft/vswhere/wiki#examples
for /f "usebackq tokens=*" %%i in (`vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
  set MSBuildExe=%%i
)

if exist "%MSBuildExe%" (
  REM NuGet package restore
  nuget restore %SolutionFile% -Verbosity Quiet

  REM Build solution
  "%MSBuildExe%" %SolutionFile% /verbosity:minimal /property:Configuration=Release

  REM To change build Configuration -> /property:Configuration=<Debug|Release>
)

if not exist "%MSBuildExe%" (
  echo Could not find MSBuild
)
