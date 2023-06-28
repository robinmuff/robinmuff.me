#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./robinmuff.me.csproj", "robinmuff.me/"]
RUN dotnet restore "robinmuff.me/robinmuff.me.csproj"
WORKDIR "src/robinmuff.me"

COPY . .
COPY static /app/publish/static/

RUN dotnet build "robinmuff.me.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "robinmuff.me.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "robinmuff.me.dll"]