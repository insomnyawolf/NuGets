# Static Feed Quick Setup Guide

## Install/Update Sleet

```powershell
dotnet tool install -g sleet || dotnet tool update -g sleet
```

## Create Sleet Config

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

## Initialize Feed

```powershell
sleet init --source feed --with-catalog --with-symbols
```

## Update Feed

```powershell
sleet push ./NugetBuilds
```
