dotnet publish --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
docker tag robinmuff.me:1.0.0 robinmuff.azurecr.io/robinmuff.me:1.0.0
docker image push robinmuff.azurecr.io/robinmuff.me:1.0.0