dotnet tool install -g sleet || dotnet tool update -g sleet
sleet push ./NugetBuilds --skip-existing
pause