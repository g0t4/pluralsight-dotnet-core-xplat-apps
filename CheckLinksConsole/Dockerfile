FROM microsoft/dotnet:1.0.1-runtime

WORKDIR /app
COPY bin/Release/netcoreapp1.0/publish .

ENTRYPOINT ["dotnet", "CheckLinksConsole.dll"]

