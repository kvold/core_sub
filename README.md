[![Build Status](https://api.travis-ci.org/kartverket/CORESubscriber.svg?branch=master)](https://travis-ci.org/kartverket/CORESubscriber)

# CORESubscriber

Lightweight console-subscriber for Geosynchronization written in .NET Core.

## Installation

### Prebuilt binaries

Find releases for Windows and Linux here:

https://github.com/kartverket/CORESubscriber/releases

### Docker
```
docker pull geosynchronization/coresubscriber
```

## Usage

### Adding datasets

When adding a provider an xml-file will be created at ${providerSettings}.xml.

#### Commandline
```
Coresubscriber.exe add ${providerurl} ${username} ${password} ${providerSettings}.xml
```

#### Docker
```
docker run -v ${PWD}:/data geosynchronization/coresubscriber dotnet  CORESubscriber.dll add ${providerurl} ${username} ${password} ${providerSettings}.xml
```

### Editing providerSettings

* Open ${providerSettings}.xml in your favourite text-editor
* Set the "subscribed" element of datasets that you wish to activate to "True"
* Populate the corresponding "wfsClient" element with the link to your WFS-service

### Synchronizing

When synchronizing, the subscriber will download a zip-file containing the changelog. This will be saved and unzipped at ${tempFolder}. If a folder is not given as an argument, the subscriber will attempt to find a temp-folder to write to.

#### Commandline

```
Coresubscriber.exe sync ${providerSettings}.xml ${tempFolder}
```

#### Docker
```
docker run -v ${PWD}:/data geosynchronization/coresubscriber dotnet CORESubscriber.dll sync ${providerSettings}.xml ${tempFolder}
```


## Build

Make sure you have .NET Core installed:

https://www.microsoft.com/net/core


### Publish

See https://github.com/dotnet/docs/blob/master/docs/core/rid-catalog.md#using-rids for RID (bold in examples)

#### Windows 10 64bit
```
git clone https://github.com/kartverket/CORESubscriber.git

cd CORESubscriber/CORESubscriber

dotnet publish -c Release --self-contained -r win10-x64
```

#### Ubuntu
```
git clone https://github.com/kartverket/CORESubscriber.git

cd CORESubscriber/CORESubscriber

dotnet publish -c Release --self-contained -r ubuntu-x64
```

#### Mac
```
git clone https://github.com/kartverket/CORESubscriber.git

cd CORESubscriber/CORESubscriber

dotnet publish -c Release --self-contained -r osx-x64
```
