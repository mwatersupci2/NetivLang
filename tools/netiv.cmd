@echo off
setlocal
set "NETIV_HOME=%~dp0"
set "NETIV_WORKING_DIR=%CD%"
pushd "%NETIV_HOME%" >nul
"%NETIV_HOME%netiv-host.exe" %*
set "NETIV_EXIT=%ERRORLEVEL%"
popd >nul
exit /b %NETIV_EXIT%
