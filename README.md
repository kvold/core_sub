# CORESubscriber

Lightweight console-subscriber for Geosynchronization written in .NET Core.

## Build

```
git clone https://github.com/jarped/CORESubscriber.git
cd CORESubscriber/CORESubscriber
dotnet restore
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
Coresubscriber.exe add ${providerurl} ${username} ${password}
```
### Synchronizing
```
Coresubscriber.exe sync ${providerSettings.xml}
```
