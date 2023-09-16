@echo off
setlocal enabledelayedexpansion

for /d %%i in (*) do ( 
    cd "%%i"
    dotnet pack --configuration release  --include-source --output ..\docs\NugetBuilds\
    cd .. 
)

pause