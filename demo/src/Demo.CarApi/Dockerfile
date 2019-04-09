FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY ./bin/Release/netcoreapp2.2/publish /app
ENV CarApi:UserApiUrl=http://localhost:5002
ENTRYPOINT ["dotnet", "Demo.CarApi.dll"]