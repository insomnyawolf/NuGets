# Static Feed Quick Setup Guide

## Create your feed

### Install/Update Sleet

```powershell
dotnet tool install -g sleet || dotnet tool update -g sleet
```

### Create Sleet Config

```powershell
sleet createconfig --local
```

Modify the created config so it fits your needs

```json
{
  "username": "insomnyawolf",
  "useremail": "insomnyawolf@gmail.com",
  "sources": [
    {
      "name": "feed",
      "type": "local",
      "path": "./feed",
      "baseURI": "https://insomnyawolf.github.io/NuGets/feed/"
    }
  ]
}
```

### Initialize Feed

```powershell
sleet init --source feed --with-catalog --with-symbols
```

### Update Feed

```powershell
sleet push ./NugetBuilds --skip-existing
```

## Setup your feed

### Manual way

Add the following url to your package manager

```plaintext
https://insomnyawolf.github.io/NuGets/feed/index.json
```

### Auto Way for sln

Create a file called ``nuget.config`` in the folder where the solution is with the following content.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <config>
        <add key="dependencyVersion" value="Highest" />
    </config>

    <packageRestore>
        <!-- Allow NuGet to download missing packages -->
        <add key="enabled" value="True" />

        <!-- Automatically check for missing packages during build in Visual Studio -->
        <add key="automatic" value="True" />
    </packageRestore>

    <packageSources>
        <add key="insomnyawolf's nuggets" value="https://insomnyawolf.github.io/NuGets/feed/index.json" />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
        
        <!-- this one is for development -->
        <!-- <add key="local packages" value="C:\Users\[Username]\source\repos\insomnyawolf nugets\docs\NugetBuilds" /> -->
    </packageSources>

    <activePackageSource>
        <!-- All non-disabled sources are active -->
        <add key="All" value="(Aggregate source)" />
    </activePackageSource>
</configuration>
```

## Special tanks

* [Marked](https://github.com/markedjs/marked) Markdown to html conversor
* [PrismJS](https://github.com/PrismJS/prism) Syntax Highlighting
