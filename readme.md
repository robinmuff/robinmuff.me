## Installation
### Manual
1. Console: dotnet restore
2. Console: dotnet run
### Scripts
1. Use .sh files in .sh folder

## Release
### Manual
1. dotnet publish --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
2. docker image push robinmuff.azurecr.io/robinmuff.me:1.0.0
### Scripts
1. Use .sh files in .sh folder