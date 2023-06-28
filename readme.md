## Installation
1. Console: dotnet restore
2. Console: dotnet run

## Release
1. dotnet publish --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
2. Docker Push Image