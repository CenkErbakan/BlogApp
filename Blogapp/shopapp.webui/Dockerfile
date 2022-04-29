FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app
 
COPY /. ./
 
ENTRYPOINT ["dotnet", "shopapp.webui.dll"]