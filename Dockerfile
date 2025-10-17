# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TalentLink/TalentLink.csproj", "TalentLink/"]
RUN dotnet restore "TalentLink/TalentLink.csproj"
COPY . .
RUN dotnet publish "TalentLink/TalentLink.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set ASP.NET Core environment
ENV ASPNETCORE_ENVIRONMENT=Production

# Configure Kestrel to use PORT environment variable
ENTRYPOINT ["dotnet", "TalentLink.dll"]