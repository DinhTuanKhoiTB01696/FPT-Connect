# Bible 13 / TC-297: container chạy non-root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Backend/ ./Backend/
WORKDIR /src/Backend
RUN dotnet restore FptConnect.sln
RUN dotnet publish src/FptConnect.Api/FptConnect.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN adduser --disabled-password --gecos "" appuser
COPY --from=build /app/publish .
USER appuser
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FptConnect.Api.dll"]
