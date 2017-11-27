[![Build Status](https://travis-ci.org/jarped/CORESubscriber.svg?branch=master)](https://travis-ci.org/jarped/CORESubscriber)

# CORESubscriber

Lightweight console-subscriber for Geosynchronization written in .NET Core.

## Build

Make sure you have .NET Core installed (for build/publish, not needed to run):

https://www.microsoft.com/net/core


### Publish

See https://github.com/dotnet/docs/blob/master/docs/core/rid-catalog.md#using-rids for RID (bold in examples)

#### Windows 10 64bit
<pre>
git clone https://github.com/jarped/CORESubscriber.git
cd CORESubscriber/CORESubscriber
dotnet publish -c Release --self-contained -r <b>win10-x64</b>
</pre>
#### Ubuntu
<pre>
git clone https://github.com/jarped/CORESubscriber.git
cd CORESubscriber/CORESubscriber
dotnet publish -c Release --self-contained -r <b>ubuntu-x64</b>
</pre>
#### Mac
<pre>
git clone https://github.com/jarped/CORESubscriber.git
cd CORESubscriber/CORESubscriber
dotnet publish -c Release --self-contained -r <b>osx-x64</b>
</pre>

## Usage

### Adding datasets

When adding a provider an xml-file will be created at ${providerSettings}.xml.

```
Coresubscriber.exe add ${providerurl} ${username} ${password} ${providerSettings}.xml
```
### Editing providerSettings

* Open ${providerSettings}.xml in your favourite text-editor (hopefully one that handles xml well)
* Set the "subscribed" element of datasets that you wish to activate to "True"
* Populate the corresponding "wfsClient" element with the link to your WFS-service

### Synchronizing

When synchronizing, the subscriber will download a zip-file containing the changelog. This will be saved and unzipped at ${tempFolder}. If a folder is not given as an argument, the subscriber will attempt to find a temp-folder to write to.

```
Coresubscriber.exe sync ${providerSettings}.xml ${tempFolder}
```
