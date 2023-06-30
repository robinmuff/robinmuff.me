dotnet publish --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
docker image push robinmuff.azurecr.io/robinmuff.me:1.0.0