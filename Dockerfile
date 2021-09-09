FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY api-out/ ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "HumanaEdge.Webcore.Example.WebApi.dll"]