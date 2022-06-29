# Publish Nuget package

For single package

```sh
dotnet nuget push -s http://192.168.0.254:8080/v3/index.json -k TestApiKey --skip-duplicate [package name]
```

For several packages

```sh
setlocal enabledelayedexpansion
For /R %%G IN (*.nupkg) do ( 
    dotnet nuget push -s http://192.168.0.254:8080/v3/index.json -k TestApiKey --skip-duplicate "%%G"
)
```