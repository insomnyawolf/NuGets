# NuGets

I mostly did it to avoid having to patch each of my libraries on all the projects that share them, they may not be perfect yet but i keep working on them...

I have no template for issues or anything, if you find any errors just open an issue (the more specific you are about the error the easiest for me to fix it), i'll try to check it as soon as i can.

If you find any of this stuff usefull i'ld like to know

## Building the nugets

To build every nuget you have to run the following command on the solution folder

```sh
setlocal enabledelayedexpansion

for /d %i in (*) do ( 
    cd "%i" & 
    dotnet pack --configuration release  --include-source --output ..\Output\  & 
    cd .. 
) 
```

you can also build a single package by executing the following command in it's project folder

```sh
dotnet pack --configuration release  --include-source --output ..\Output\
```

there's 2 flags that can help with the debugging process if something fails which are

```
--include-symbols => debug info
--include-source => debug info + source code
```

I included the second on the previous commands

## Pushing nuget packages into a nuget server

For single package

```sh
dotnet nuget push --source http://127.0.0.1:8080/v3/index.json --api-key TestApiKey --skip-duplicate [package name]
```

remember to change ``[package name]`` with the name of the package that you wanna upload.

For several packages

The following script will try to upload all the nuget packages contained in the directory

```sh
setlocal enabledelayedexpansion
for /R %%G in (*.nupkg) do ( 
    dotnet nuget push --source http://127.0.0.1:8080/v3/index.json --api-key TestApiKey --skip-duplicate "%%G"
)
```

Remember to change the souce and the api key.

## Cleanup

If you wanna copy the repository into anyplace or you just wanna make sure that you are making a clean build you can execute the following commands which will delete all the compilation caches.

```sh
for /F "tokens=*" %G in ('DIR /B /AD /S bin') do RMDIR /S /Q "%G"
for /F "tokens=*" %G in ('DIR /B /AD /S obj') do RMDIR /S /Q "%G"
```

## Static Nuget Server Setup 

Sadly last time i checked wasn't working at least using github as host for the webpage, i'll need to check it at some point

[https://insomnyawolf.github.io/NuGets/#setup-your-feed](https://insomnyawolf.github.io/NuGets/#setup-your-feed)

## Final notes

To whoever is reading this => Have a nice day!!
