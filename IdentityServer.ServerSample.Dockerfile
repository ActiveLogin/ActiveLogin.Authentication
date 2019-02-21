# Build ------------------------------------------------------------------------------------------
FROM microsoft/dotnet:2.2-sdk as build

WORKDIR /app
ADD . /app

RUN dotnet restore samples/IdentityServer.ServerSample/IdentityServer.ServerSample.csproj
RUN dotnet publish -c Release samples/IdentityServer.ServerSample/IdentityServer.ServerSample.csproj

# ------------ RUN -----------
FROM microsoft/dotnet:2.2-runtime

WORKDIR /app/
COPY --from=build /app/samples/IdentityServer.ServerSample/bin/Release/netcoreapp2.2/publish/ /app/

EXPOSE 8080
ENTRYPOINT ["dotnet", "app/IdentityServer.ServerSample.dll"]
