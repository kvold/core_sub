[![Build Status](https://travis-ci.org/jarped/CORESubscriber.svg?branch=master)](https://travis-ci.org/jarped/CORESubscriber)

# CORESubscriber

Lightweight console-subscriber for Geosynchronization written in .NET Core.

## Build

Make sure you have .NET Core installed:

https://www.microsoft.com/net/core

### Clone:

```
git clone https://github.com/jarped/CORESubscriber.git
cd CORESubscriber/CORESubscriber
```

### Publish

See https://github.com/dotnet/docs/blob/master/docs/core/rid-catalog.md#using-rids for RID

#### Windows 10 64bit
```
dotnet publish -r win10-x64
```
#### Ubuntu
```
dotnet publish -r ubuntu-x64
```
### Mac
```
dotnet publish -r osx-x64
```

## Usage

### Adding datasets
```
Coresubscriber.exe add ${providerurl} ${username} ${password} ${providerSettings}.xml
```
### Editing providerSettings

* Open ${providerSettings}.xml in your favourite text-editor (hopefully one that handles xml well)
* Set the "subscribed" element of datasets that you wish to activate to "True"
* Populate the corresponding "wfsClient" element with the link to your WFS-service

### Synchronizing
```
Coresubscriber.exe sync ${providerSettings}.xml ${tempFolder}
```
