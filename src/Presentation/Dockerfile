# .NET 10 API (non-root)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.sln Directory.Build.props ./
COPY src/ ./src/
RUN dotnet restore FptConnect.sln
RUN dotnet publish src/Presentation/FptConnect.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN adduser --disabled-password --gecos "" appuser
COPY --from=build /app/publish .
USER appuser
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FptConnect.Presentation.dll"]
