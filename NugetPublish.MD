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

publish new version of every package contained in the main directory

```sh

set "command=dotnet pack --configuration release  --include-source --output ..\Output\"

for /d %i in (*) do ( cd "%i" & %command%  & cd .. ) 
```

publish compiles the dll
```sh
dotnet publish --configuration release --output ..\Output\ --os any --self-contained false
```

dotnet pack creates the nuget package

--include-symbols => debug info
--include-source => debug info + source code

```sh
dotnet pack --configuration release  --include-source --output ..\Output\
```

