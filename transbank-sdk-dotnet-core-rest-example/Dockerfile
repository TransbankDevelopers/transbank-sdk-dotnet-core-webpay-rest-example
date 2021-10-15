#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["transbank-sdk-dotnet-core-rest-example/transbank-sdk-dotnet-core-rest-example.csproj", "transbank-sdk-dotnet-core-rest-example/"]
RUN dotnet restore "transbank-sdk-dotnet-core-rest-example/transbank-sdk-dotnet-core-rest-example.csproj"
COPY . .
WORKDIR "/src/transbank-sdk-dotnet-core-rest-example"
RUN dotnet build "transbank-sdk-dotnet-core-rest-example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "transbank-sdk-dotnet-core-rest-example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "transbank-sdk-dotnet-core-rest-example.dll"]