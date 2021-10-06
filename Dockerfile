FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY api-out/ ./

EXPOSE 80
ENTRYPOINT ["dotnet", "HumanaEdge.Webcore.Example.WebApi.dll"]
